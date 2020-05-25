using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls;
using System;

namespace LibraryNET.Controller
{
    public class GridViewItemController
    {
        // Code not in use. a Old solution i had for the gridview

        /// <summary>
        /// Creates the book cover.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public GridViewItem CreateBookCover(string url)
        {
            GridViewItem gridViewItem = new GridViewItem();
            gridViewItem.Content = CreateImage(url);
            return gridViewItem;
        }

        /// <summary>
        /// Creates the bit map.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public BitmapImage CreateBitMap(string url)
        {
            BitmapImage bit = new BitmapImage(new Uri(url, UriKind.Absolute));
            bit.DecodePixelType = DecodePixelType.Physical;
            return bit;
        }

        /// <summary>
        /// Creates the image.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public Image CreateImage(string url)
        {
            BitmapImage CreateBitImage()
            {
                BitmapImage bit = new BitmapImage(new Uri(url, UriKind.Absolute));
                bit.DecodePixelType = DecodePixelType.Physical;
                return bit;
            }
            Image image = new Image();
            image.Width = 175;
            image.Source = CreateBitImage();
            return image;
        }
    }
}
