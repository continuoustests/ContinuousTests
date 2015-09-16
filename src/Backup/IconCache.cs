using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoTest.VS.RiskClassifier
{
    public static class IconCache
    {
        private static readonly Dictionary<string, BitmapSource> Cache = new Dictionary<string, BitmapSource>();

        public static Image GetImage(string name)
        {
            BitmapSource item;
            if(Cache.TryGetValue(name, out item))
            {
                return BuildImage(item);
            }
            using (var stream = typeof(IconCache).Assembly.GetManifestResourceStream("AutoTest.VS.RiskClassifier.Resources." + name))
            {
                if (stream != null)
                {
                    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None,
                                                       BitmapCacheOption.None);
                    BitmapSource source = decoder.Frames[0];
                    
                    Cache.Add(name, source);
                    return BuildImage(source);
                }
            }
            return null;
        }

        private static Image BuildImage(BitmapSource source)
        {
            var image = new Image();
            image.Height = 16;
            image.Width = 16;
            image.Source = source;
            image.Stretch = Stretch.Fill;
            return image;
        }
    }
}