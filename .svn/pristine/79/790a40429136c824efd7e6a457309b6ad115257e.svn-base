using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Devices;
using Microsoft.Phone.Controls;

namespace CJToolkit.Util
{
    public class ScreenCaptureUtil
    {
        public static Boolean ServiceOn { get; private set; }

        /// <summary>
        /// 默认存储文件的路径定义
        /// </summary>
        private const String DefaultPath = "Shared/ShellContent/Capture";

        /// <summary>
        /// 对指定的FrameworkElement截图并存放到独立存储空间
        /// </summary>
        /// <param name="obj">要截图的对象（需要是在屏幕上显示的空间）</param>
        /// <param name="width">截图的宽度，默认为-1，小于0会自动计算目标宽度</param>
        /// <param name="height">截图的高度，默认为-1，小于0会自动计算目标高度</param>
        /// <param name="path">存储的位置，有默认值</param>
        /// <param name="fileName">存储的文件名，默认会以时间存储</param>
        public static void Capture(FrameworkElement obj, double width = -1, double height = -1, String path = DefaultPath,String fileName="")
        {
            try
            {

                int w = Convert.ToInt32(width < 0 ? obj.ActualWidth : width),
                    h = Convert.ToInt32(height < 0 ? obj.ActualHeight : height);
                string p = string.IsNullOrEmpty(path) ? DefaultPath : path;
                string fn = string.IsNullOrEmpty(fileName) ? DateTime.Now.ToString("yyyyMMddhhmmss") : fileName;
                if (!fn.EndsWith(".JPG", StringComparison.OrdinalIgnoreCase) && !fn.EndsWith(".JPEG", StringComparison.OrdinalIgnoreCase))
                {
                    fn = fn + ".jpg";
                }
                var writeableBitmap = new WriteableBitmap(w, h);
                writeableBitmap.Render(obj, null);
                writeableBitmap.Invalidate();

                if (!IsolatedStorageFile.GetUserStoreForApplication().DirectoryExists(
                        string.Format("{0}/", p)))
                {
                    IsolatedStorageFile.GetUserStoreForApplication().CreateDirectory(
                        p);
                }
                var stream =
                    new IsolatedStorageFileStream(string.Format("{0}/{1}", p, fn),
                        FileMode.Create,
                        IsolatedStorageFile.GetUserStoreForApplication());
                writeableBitmap.SaveJpeg(stream, w, h, 0, 100);
                stream.Close();
                stream.Dispose();
            }
            catch (Exception exception)
            {
                NotifyMessage.NotifyMessage.Show(exception.StackTrace);
#if DEBUG
                throw;
#endif
            }
        }

        /// <summary>
        /// 开始拍照键截图服务
        /// </summary>
        public static void StartService()
        {
            if(ServiceOn) return;
            //The CameraButtonsEvent won't fire without the CaptureSource.Start()
            var cs = new CaptureSource();
            cs.Start();
            ServiceOn = false;
            CameraButtons.ShutterKeyPressed += CameraButtonsShutterKeyPressed;
        }

        /// <summary>
        /// 关闭拍照键截图的服务
        /// </summary>
        public static void StopService()
        {
            if(!ServiceOn) return;
            CameraButtons.ShutterKeyPressed -= CameraButtonsShutterKeyPressed;
            ServiceOn = false;
        }

        /// <summary>
        /// 拍照键的监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CameraButtonsShutterKeyPressed(object sender, EventArgs e)
        {
            ServiceOn = true;
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
                Capture(phoneApplicationFrame.Content as PhoneApplicationPage);
        }
    }
}
