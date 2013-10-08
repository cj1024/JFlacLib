using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CJToolkit.Util
{
    public static class BitmapExtention
    {
        /// <summary>
        /// 去单色色阶算法枚举
        /// </summary>
        public enum MonochromeAlgorithmType
        {
            Max,
            Average,
            WeightedAverage,
            AdjustedMax
        }

        #region MagicNumber

        /// <summary>
        /// 精细度低于阀值的需要对像素颜色做特殊处理
        /// </summary>
        private const int FinesseSpecialHandleValve = 4;

        #region 加权平均算法常量

        private const double RWeight = 0.7;
        private const double GWeight = 0.2;
        private const double BWeight = 0.1;

        #endregion

        #endregion
        
        #region 辅助函数

        /// <summary>
        /// 根据设定的颜色确定颜色是否需要被替换
        /// </summary>
        /// <param name="sColor">需要检测的颜色</param>
        /// <param name="dColor">对照的颜色</param>
        /// <param name="valve">阀值</param>
        /// <returns></returns>
        private static bool NeedKeepCurrentColor(int sColor, int dColor, int valve = 64)
        {
            var sc = sColor.GetColorByte();
            var dc = dColor.GetColorByte();
            return Math.Abs(sc[1] - dc[1]) < valve && Math.Abs(sc[2] - dc[2]) < valve && Math.Abs(sc[3] - dc[3]) < valve;
        }

        /// <summary>
        /// 根据颜色列表检测是否需要替换颜色
        /// </summary>
        /// <param name="sColor">需要检测的颜色</param>
        /// <param name="dColors">颜色列表</param>
        /// <param name="valve">阀值</param>
        /// <returns></returns>
        private static bool NeedKeepCurrentColor(int sColor, IEnumerable<int> dColors, int valve = 64)
        {
            return dColors.Any(dColor => NeedKeepCurrentColor(sColor, dColor, valve));
        }

        /// <summary>
        /// 按算法计算平均像素值
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="algorithmType"></param>
        /// <returns></returns>
        private static int GetCalculatedPixelValue(int r, int g, int b, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max)
        {
            switch (algorithmType)
            {
                case MonochromeAlgorithmType.Max:
                    return Math.Max(r, Math.Max(g, b));
                case MonochromeAlgorithmType.Average:
                    return (r + g + b)/3;
                case MonochromeAlgorithmType.WeightedAverage:
                    return (int) (r*RWeight + g*GWeight + b*BWeight);
                default:
                    return Math.Max(r, Math.Max(g, b));
            }
        }

        /// <summary>
        /// 创建斜条纹背景图
        /// </summary>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <param name="stripWidth">条纹宽度</param>
        /// <param name="blankWidth">留白宽度</param>
        /// <param name="background">背景色</param>
        /// <param name="foreground">条纹色</param>
        /// <returns></returns>
        private static WriteableBitmap CreateStripBackground(int width, int height, int stripWidth, int blankWidth, Color background, Color foreground)
        {
            var fore = foreground.GetColorInteger();
            var back = background.GetColorInteger();
            var result = BitmapFactory.New(width, height);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if ((x + y) % (stripWidth + blankWidth) > blankWidth)
                    {
                        result.Pixels[y * width + x] = fore;
                    }
                    else
                    {
                        result.Pixels[y * width + x] = back;
                    }
                }
            }
            return result;
        }

        private static void FixSaturation(ref WriteableBitmap sourceBitmap)
        {
            byte maxR = 0, maxG = 0, maxB = 0;
            foreach (var bytes in sourceBitmap.Pixels.Select(pixel => pixel.GetColorByte()))
            {
                if (bytes[1] > maxR)
                    maxR = bytes[1];
                if (bytes[2] > maxG)
                    maxG = bytes[2];
                if (bytes[3] > maxB)
                    maxB = bytes[3];
            }
            double rateR = 255d/maxR, rateG = 255d/maxG, rateB = 255d/maxB;
            for (var i = 0; i < sourceBitmap.Pixels.Length; i++)
            {
                var pixel = sourceBitmap.Pixels[i].GetColorByte();
                pixel[1] = (byte)(int)(pixel[1] * rateR);
                pixel[2] = (byte)(int)(pixel[2] * rateG);
                pixel[3] = (byte)(int)(pixel[3] * rateB);
                sourceBitmap.Pixels[i] = pixel.GetColorInteger();
            }
        }

        private static int GetMergedColor(int color,double rate)
        {
            if (rate > 1) rate = 1;
            else if (rate < 0) rate = 0;
            var c = color.GetColorByte();
            c[0] = 0xff;
            c[1] = (byte)(int)(c[1] * rate);
            c[2] = (byte)(int)(c[2] * rate);
            c[3] = (byte)(int)(c[3] * rate);
            return c.GetColorInteger();
        }

        #endregion
        
        /// <summary>
        /// 转化图片至黑白
        /// </summary>
        /// <param name="sourceBitmap">原图片</param>
        /// <param name="finesseLevel">精细度</param>
        /// <param name="algorithmType">采用的平均算法类型</param>
        /// <returns></returns>
        public static WriteableBitmap ToMonochrome(this WriteableBitmap sourceBitmap, byte finesseLevel = 0xff, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max)
        {
            var rate = 256/(finesseLevel + 1);
            var stage = 255/finesseLevel;
            var result = new WriteableBitmap(sourceBitmap);
            //调整饱和度
            if(algorithmType==MonochromeAlgorithmType.AdjustedMax)
            {
                FixSaturation(ref result);
            }
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                var color = result.Pixels[i].GetColorByte();
                //取目标颜色值中RGB综合项
                var pixelValue = GetCalculatedPixelValue(color[1], color[2], color[3], algorithmType);
                //精细度的调整
                var rst = (pixelValue > 255 ? 255 : (pixelValue < 0 ? 0 : pixelValue)) / rate * stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    rst = rst + rate > 255 ? 255 : rst;
                color[1] = (byte) rst;
                color[2] = (byte) rst;
                color[3] = (byte) rst;
                result.Pixels[i] = color.GetColorInteger();
            }
            return result;
        }

        /// <summary>
        /// 转化图片至单色
        /// </summary>
        /// <param name="sourceBitmap">原图片</param>
        /// <param name="fillColor">填充用的颜色</param>
        /// <param name="finesseLevel">精细度</param>
        /// <param name="algorithmType">采用的平均算法类型</param>
        /// <returns></returns>
        public static WriteableBitmap ToMonochrome(this WriteableBitmap sourceBitmap, Color fillColor, byte finesseLevel = 0xff, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max)
        {
            //如果是白色，直接调用别的函数
            if (fillColor.R == fillColor.G && fillColor.G == fillColor.B && fillColor.B == 255)
                return sourceBitmap.ToMonochrome(finesseLevel);
            var rate = 256/(finesseLevel + 1);
            var stage = 255/finesseLevel;
            var result = new WriteableBitmap(sourceBitmap);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                var color = result.Pixels[i].GetColorByte();
                var fColor = fillColor.GetColorByte();
                //取目标颜色值中RGB综合项
                var pixelValue = GetCalculatedPixelValue(color[1], color[2], color[3], algorithmType);
                //得出饱和度
                var tmp = (pixelValue > 255 ? 255 : (pixelValue < 0 ? 0 : pixelValue))/255d;
                //RGB计算饱和度
                var r = fColor[1]*tmp;
                var g = fColor[2]*tmp;
                var b = fColor[3]*tmp;
                //调整精细度
                r = (r > 255 ? 255 : (r < 0 ? 0 : r))/rate*stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    r = r + rate > 255 ? 255 : r;
                g = (g > 255 ? 255 : (g < 0 ? 0 : g))/rate*stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    g = g + rate > 255 ? 255 : g;
                b = (b > 255 ? 255 : (b < 0 ? 0 : b))/rate*stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    b = b + rate > 255 ? 255 : b;
                color[1] = (byte) r;
                color[2] = (byte) g;
                color[3] = (byte) b;
                result.Pixels[i] = color.GetColorInteger();
            }
            return result;
        }

        /// <summary>
        /// 转至古铜色经典样式
        /// </summary>
        /// <param name="sourceBitmap">原图片</param>
        /// <param name="finesseLevel">精细度</param>
        /// <param name="algorithmType">采用的平均算法类型</param>
        /// <returns></returns>
        public static WriteableBitmap ToClassic(this WriteableBitmap sourceBitmap, byte finesseLevel = 0xff, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max)
        {
            var result = new WriteableBitmap(sourceBitmap);
            return result.ToMonochrome(Color.FromArgb(0xFF, 0xF8, 0xB3, 0x53),finesseLevel,algorithmType);
        }

        /// <summary>
        /// 转化至反色
        /// </summary>
        /// <param name="sourceBitmap">原图片</param>
        /// <param name="finesseLevel">精细度</param>
        /// <param name="algorithmType">采用的平均算法类型</param>
        /// <returns></returns>
        public static WriteableBitmap ToAntiColor(this WriteableBitmap sourceBitmap, byte finesseLevel = 0xff, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max)
        {
            var rate = 256 / (finesseLevel + 1);
            var stage = 255 / finesseLevel;
            var result = new WriteableBitmap(sourceBitmap);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                var color = result.Pixels[i].GetColorByte();
                //RGB取反
                var r = 255 - color[1];
                var g = 255 - color[2];
                var b = 255 - color[3];
                //调整精细度
                r = (r > 255 ? 255 : (r < 0 ? 0 : r)) / rate * stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    r = r + rate > 255 ? 255 : r;
                g = (g > 255 ? 255 : (g < 0 ? 0 : g)) / rate * stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    g = g + rate > 255 ? 255 : g;
                b = (b > 255 ? 255 : (b < 0 ? 0 : b)) / rate * stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    b = b + rate > 255 ? 255 : b;
                color[1] = (byte)r;
                color[2] = (byte)g;
                color[3] = (byte)b;
                result.Pixels[i] = color.GetColorInteger();
            }
            return result;
        }

        /// <summary>
        /// 添加颜色遮罩
        /// </summary>
        /// <param name="sourceBitmap">原图片</param>
        /// <param name="finesseLevel">精细度</param>
        /// <returns></returns>
        [Obsolete("Method未完成", true)]
        public static WriteableBitmap AddColorLayer(this WriteableBitmap sourceBitmap, byte finesseLevel = 0xff)
        {
            var rate = 256 / (finesseLevel + 1);
            var result = new WriteableBitmap(sourceBitmap);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                var color = result.Pixels[i].GetColorByte();
                //RGB取反
                var r = 255 - color[1];
                var g = 255 - color[2];
                var b = 255 - color[3];
                //调整精细度
                r = (r > 255 ? 255 : (r < 0 ? 0 : r)) / rate * rate;
                if (finesseLevel < FinesseSpecialHandleValve)
                    r = r + rate > 255 ? 255 : r;
                g = (g > 255 ? 255 : (g < 0 ? 0 : g)) / rate * rate;
                if (finesseLevel < FinesseSpecialHandleValve)
                    g = g + rate > 255 ? 255 : g;
                b = (b > 255 ? 255 : (b < 0 ? 0 : b)) / rate * rate;
                if (finesseLevel < FinesseSpecialHandleValve)
                    b = b + rate > 255 ? 255 : b;
                //上色
                color[1] = (byte)r;
                color[2] = (byte)g;
                color[3] = (byte)b;
                result.Pixels[i] = color.GetColorInteger();
            }
            return result;
        }

        [Obsolete("Method未完成", false)]
        public static WriteableBitmap ToCommicStyle(this WriteableBitmap sourceBitmap,int brushWidth = 5,int bandWidth = 5)
        {

            #region 需要替换的颜色常量

            const byte color1 = 0xAA;

            #endregion

            #region 笔刷常量

            var iTransparent = Colors.Transparent.GetColorInteger();

            #endregion

            //使用4色阶黑白图
            var result = sourceBitmap.ToMonochrome(3);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                if(result.Pixels[i].GetColorByte()[1]==color1)
                {
                    result.Pixels[i] = iTransparent;
                }
            }
            var background = CreateStripBackground(result.PixelWidth, result.PixelHeight, brushWidth, bandWidth,
                                                   Colors.White, Color.FromArgb(0xFF, 0x7F, 0x7F, 0x7F));
            return result.AddBackground(background);
        }

        /// <summary>
        /// 保留强调色，其余颜色取灰度
        /// </summary>
        /// <param name="sourceBitmap">原图</param>
        /// <param name="strengthenColors">强调色列表</param>
        /// <param name="valve">阀值</param>
        /// <param name="finesseLevel">精细度（仅对变为灰度部分有效）</param>
        /// <param name="algorithmType">采用的平均算法类型</param>
        /// <returns></returns>
        public static WriteableBitmap ToStrengthenColor(this WriteableBitmap sourceBitmap, IEnumerable<Color> strengthenColors, int valve = 64, byte finesseLevel = 0xff,MonochromeAlgorithmType algorithmType=MonochromeAlgorithmType.Max)
        {
            var sColors = (from strengthenColor in strengthenColors
                          select strengthenColor.GetColorInteger()).ToList();
            var rate = 256/(finesseLevel + 1);
            var stage = 255 / finesseLevel;
            var result = new WriteableBitmap(sourceBitmap);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                //对保留原色的像素跳过
                if(NeedKeepCurrentColor(result.Pixels[i],sColors,valve))
                    continue;
                var color = result.Pixels[i].GetColorByte();
                //取目标颜色值中RGB最大项
                var pixelValue = GetCalculatedPixelValue(color[1], color[2], color[3], algorithmType);
                //精细度的调整
                var rst = (pixelValue > 255 ? 255 : (pixelValue < 0 ? 0 : pixelValue))/rate*stage;
                if (finesseLevel < FinesseSpecialHandleValve)
                    rst = rst + rate > 255 ? 255 : rst;
                color[1] = (byte) rst;
                color[2] = (byte) rst;
                color[3] = (byte) rst;
                result.Pixels[i] = color.GetColorInteger();
            }
            return result;
        }

        /// <summary>
        /// 在透明图像出添加背景
        /// </summary>
        /// <param name="front"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        public static WriteableBitmap AddBackground(this WriteableBitmap front,WriteableBitmap back)
        {
            //如果尺寸不符，则修正尺寸
            if (front.PixelWidth != back.PixelWidth || front.PixelHeight != back.PixelHeight)
                back=back.Resize(front.PixelWidth, front.PixelHeight, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
            var result = new WriteableBitmap(front);
            for(var i=0;i<result.Pixels.Length;i++)
            {
                var pixel = result.Pixels[i].GetColorByte();
                //半透明一下则用背景填充
                if (pixel[0] < 0x7F)
                    result.Pixels[i] = back.Pixels[i];
            }
            return result;
        }
        
        [Obsolete("有内存使用量问题",false)]
        public static WriteableBitmap FixSaturation(this WriteableBitmap sourceBitmap)
        {
            FixSaturation(ref sourceBitmap);
            return sourceBitmap;
        }
        
        /// <summary>
        /// 添加相框
        /// </summary>
        /// <param name="back">主图</param>
        /// <param name="front">相框图</param>
        /// <returns></returns>
        [Obsolete("只支持黑白相框，黑白图片",false)]
        public static WriteableBitmap AddFrame(this WriteableBitmap back,WriteableBitmap front)
        {
            //如果尺寸不符，则修正尺寸
            if (front.PixelWidth != back.PixelWidth || front.PixelHeight != back.PixelHeight)
                front = front.Resize(back.PixelWidth, back.PixelHeight, WriteableBitmapExtensions.Interpolation.NearestNeighbor);
            front = front.ToMonochrome(31,MonochromeAlgorithmType.Average);
            var result = new WriteableBitmap(back);
            for (var i = 0; i < front.Pixels.Length; i++)
            {
                var pixel = front.Pixels[i].GetColorByte();
                var rate = GetCalculatedPixelValue(pixel[1], pixel[2], pixel[3],MonochromeAlgorithmType.Average)/255d;
                result.Pixels[i] = GetMergedColor(result.Pixels[i],rate);
            }
            return result;
        }

        /// <summary>
        /// 保留红色
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="algorithmType"></param>
        /// <param name="valve"></param>
        /// <returns></returns>
        public static WriteableBitmap KeepRedOnly(this WriteableBitmap sourceBitmap, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max, int valve = 0x3F)
        {
            var result = new WriteableBitmap(sourceBitmap);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                var color = result.Pixels[i].GetColorByte();
                //取目标颜色值中RGB综合项
                var pixelValue = GetCalculatedPixelValue(color[1], color[2], color[3], algorithmType);
                //RGB
                var r = color[1];
                var g = color[2];
                var b = color[3];
                //判断是否替换上色
                if (r < g + b || (g > valve || b > valve))
                {
                    color[1] = (byte) pixelValue;
                    color[2] = (byte) pixelValue;
                    color[3] = (byte) pixelValue;
                    result.Pixels[i] = color.GetColorInteger();
                }
            }
            return result;
        }

        /// <summary>
        /// 保留绿色
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="algorithmType"></param>
        /// <param name="valve"></param>
        /// <returns></returns>
        public static WriteableBitmap KeepGreenOnly(this WriteableBitmap sourceBitmap, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max, int valve = 0x3F)
        {
            var result = new WriteableBitmap(sourceBitmap);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                var color = result.Pixels[i].GetColorByte();
                //取目标颜色值中RGB综合项
                var pixelValue = GetCalculatedPixelValue(color[1], color[2], color[3], algorithmType);
                //RGB
                var r = color[1];
                var g = color[2];
                var b = color[3];
                //判断是否替换上色
                if (g < r + b || (r > valve || b > valve) && !(g > 0xAF))
                {
                    color[1] = (byte)pixelValue;
                    color[2] = (byte)pixelValue;
                    color[3] = (byte)pixelValue;
                    result.Pixels[i] = color.GetColorInteger();
                }
            }
            return result;
        }

        /// <summary>
        /// 保留蓝色
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="algorithmType"></param>
        /// <param name="valve"></param>
        /// <returns></returns>
        public static WriteableBitmap KeepBlueOnly(this WriteableBitmap sourceBitmap, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max, int valve = 0x3F)
        {
            var result = new WriteableBitmap(sourceBitmap);
            for (var i = 0; i < result.Pixels.Length; i++)
            {
                var color = result.Pixels[i].GetColorByte();
                //取目标颜色值中RGB综合项
                var pixelValue = GetCalculatedPixelValue(color[1], color[2], color[3], algorithmType);
                //RGB
                var r = color[1];
                var g = color[2];
                var b = color[3];
                //判断是否替换上色
                if ((b < r + g || (r > valve || g > valve)) && !(b > 0xAF))
                {
                    color[1] = (byte)pixelValue;
                    color[2] = (byte)pixelValue;
                    color[3] = (byte)pixelValue;
                    result.Pixels[i] = color.GetColorInteger();
                }
            }
            return result;
        }
        
        /// <summary>
        /// 保留黄色
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <param name="algorithmType"></param>
        /// <param name="valve"></param>
        /// <returns></returns>
        public static WriteableBitmap KeepYellowOnly(this WriteableBitmap sourceBitmap, MonochromeAlgorithmType algorithmType = MonochromeAlgorithmType.Max, int valve = 0x3F)
        {
            var result = new WriteableBitmap(sourceBitmap);
            return result.ToStrengthenColor(new[]
                                                {
                                                    Color.FromArgb(0xFF, 0xFF, 0xCF, 0x1F),
                                                    Color.FromArgb(0xFF, 0xBF, 0x8F, 0x1F)
                                                },algorithmType:algorithmType);
        }

        /// <summary>
        /// 锐化
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        public static WriteableBitmap Sharper(this WriteableBitmap sourceBitmap)
        {
            var result = new WriteableBitmap(sourceBitmap);
            //拉普拉斯模板
            int[] laplacian = { -1, -1, -1, -1, 9, -1, -1, -1, -1 };
            for (var x = 1; x < result.PixelWidth - 1; x++)
            {
                for (var y = 1; y < result.PixelHeight - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    var index = 0;
                    for (var col = -1; col <= 1; col++)
                        for (var row = -1; row <= 1; row++)
                        {
                            var pixel = result.Pixels[(x + row) + (y + col)*result.PixelWidth].GetColorByte();
                            r += pixel[1]*laplacian[index];
                            g += pixel[2]*laplacian[index];
                            b += pixel[3]*laplacian[index];
                            index++;
                        }
                    //处理颜色值溢出                             
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    var newColor = new byte[] {0xFF, (byte) r, (byte) g, (byte) b}.GetColorInteger();
                    result.Pixels[x - 1 + (y - 1)*result.PixelWidth] = newColor;
                }
            }
            return result;
        }

        /// <summary>
        /// 柔化
        /// </summary>
        /// <param name="sourceBitmap"></param>
        /// <returns></returns>
        public static WriteableBitmap Softer(this WriteableBitmap sourceBitmap)
        {
            var result = new WriteableBitmap(sourceBitmap);
            //高斯模板                     
            int[] gauss = {1, 2, 1, 2, 4, 2, 1, 2, 1};
            for (var x = 1; x < result.PixelWidth - 1; x++)
            {
                for (var y = 1; y < result.PixelHeight - 1; y++)
                {
                    int r = 0, g = 0, b = 0;
                    var index = 0;
                    for (var col = -1; col <= 1; col++)
                        for (var row = -1; row <= 1; row++)
                        {
                            var pixel = result.Pixels[(x + row) + (y + col)*result.PixelWidth].GetColorByte();

                            r += pixel[1]*gauss[index];
                            g += pixel[2]*gauss[index];
                            b += pixel[3]*gauss[index];
                            index++;
                        }
                    r /= 16;
                    g /= 16;
                    b /= 16;
                    //处理颜色值溢出                             
                    r = r > 255 ? 255 : r;
                    r = r < 0 ? 0 : r;
                    g = g > 255 ? 255 : g;
                    g = g < 0 ? 0 : g;
                    b = b > 255 ? 255 : b;
                    b = b < 0 ? 0 : b;
                    var newColor = new byte[] {0xFF, (byte) r, (byte) g, (byte) b}.GetColorInteger();
                    result.Pixels[x - 1 + (y - 1)*result.PixelWidth] = newColor;
                }
            }
            return result;
        }

    }

    [Obsolete("不知道怎么用",false)]
    [AttributeUsage(AttributeTargets.All)]
    public class CompletionAttribute:Attribute
    {
        public enum DegreeOfCompletion
        {
            /// <summary>
            /// 完全不能使用
            /// </summary>
            TotallyNot,
            /// <summary>
            /// 开发中
            /// </summary>
            OnProgress,
            /// <summary>
            /// 差不多完成，可以使用
            /// </summary>
            Almost,
            /// <summary>
            /// 完成
            /// </summary>
            Completed
        }

        protected DegreeOfCompletion Completion { get; set; }
        private string Author { get; set; }
        private string Comment { get; set; }
        private string Date { get; set; }

        public CompletionAttribute(DegreeOfCompletion degreeOfCompletion=DegreeOfCompletion.OnProgress,string author="",string comment="",string date="")
        {
            Completion = degreeOfCompletion;
            Author = author;
            Comment = comment;
            Date = date;
        }

    }

}
