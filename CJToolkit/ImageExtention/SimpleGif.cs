using System.Collections.Generic;
using System.Windows.Media;

namespace CJToolkit.ImageExtention
{
    public class SimpleGif:FakeGifBase
    {
        public IEnumerable<ImageSource> ImageList
        {
            get { return (IEnumerable<ImageSource>)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
    }
}
