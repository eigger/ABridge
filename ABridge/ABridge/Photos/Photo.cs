// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ABridge.Photos;

namespace ABridge.Photos
{
    /// <summary>
    ///     This class describes a single photo - its location, the image and
    ///     the metadata extracted from the image.
    /// </summary>
    public class Photo
    {
        private readonly Uri _source;

        public Photo(string path)
        {
            Source = path;
            _source = new Uri(path);
            //Image = BitmapFrame.Create(_source);
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                Image = GetBitmapSource(fileStream);
            }
            Metadata = new PhotoMetadata(path);
        }

        public string Source { get; }
        //public BitmapFrame Image { get; set; }
        public WriteableBitmap Image { get; set; }
        public PhotoMetadata Metadata { get; }

        public override string ToString() => _source.ToString();

        private WriteableBitmap GetBitmapSource(Stream stream)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

            bitmapImage.StreamSource = stream;

            bitmapImage.EndInit();

            bitmapImage.Freeze();

            BitmapSource bitmapSource = new FormatConvertedBitmap(bitmapImage, PixelFormats.Pbgra32, null, 0);

            WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);

            int width = writeableBitmap.PixelWidth;

            int height = writeableBitmap.PixelHeight;
            int[] pixelArray = new int[width * height];
            int stride = writeableBitmap.PixelWidth * (writeableBitmap.Format.BitsPerPixel / 8);
            writeableBitmap.CopyPixels(pixelArray, stride, 0);
            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixelArray, stride, 0);
            bitmapImage = null;
            return writeableBitmap;

        }
    }
}