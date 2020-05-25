using Windows.UI.Xaml.Controls;
using LibraryNET.Helper;
using LibraryNET.Model;

namespace LibraryNET.Controller
{
    public class ContentDialogController
    {
        /// <summary>
        /// Creates the book content dialog.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns>the content dialog</returns>
        public ContentDialog CreateBookContentDialog(Book b)
        {
            ContentDialog cd = new ContentDialog
            {
                Title = $"{b.Title} ({b.PublisherDate})",
                Content = CreateScrollViewer(b),
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = "Add to collection",
                CloseButtonText = "Close"
            };
            return cd;
        }
        /// <summary>
        /// Creates the content dialog message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>the content dialog</returns>
        public ContentDialog CreateContentDialogMessage(string message)
        {
            ContentDialog cd = new ContentDialog
            {
                Title = $"{message}",
                CloseButtonText = "Got it!",
                DefaultButton = ContentDialogButton.Primary
            };
            return cd;
        }
        /// <summary>
        /// Creates the error content dialog.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>the content dialog</returns>
        public ContentDialog CreateErrorContentDialog(string text)
        {
            ContentDialog cd = new ContentDialog
            {
                Title = "Something wrong!",
                Content = text,
                CloseButtonText = "Got it!",
                DefaultButton = ContentDialogButton.Close
            };
            return cd;
        }
        /// <summary>
        /// Creates the scroll viewer.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <returns>the content dialog</returns>
        private ScrollViewer CreateScrollViewer(Book b)
        {
            TextBlock CreateTextBlock()
            {
                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = Windows.UI.Xaml.TextWrapping.WrapWholeWords;
                textBlock.Text =
                    $"Author:\n{new InputValidator().StringValidator(b.Author)}\n\n" +
                    $"Publisher:\n{new InputValidator().StringValidator(b.Publisher)}" +
                    $"\n\nPages: {new InputValidator().StringValidator(b.PageCount.ToString())}" +
                    $"\n\nDescription:\n{new InputValidator().StringValidator(b.Description)}";

                return textBlock;
            }
            ScrollViewer scroller = new ScrollViewer();
            scroller.Height = 300;
            scroller.Width = 500;
            scroller.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            scroller.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
            scroller.VerticalScrollMode = ScrollMode.Auto;
            scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroller.Content = CreateTextBlock();
            return scroller;
        }
    }
}
