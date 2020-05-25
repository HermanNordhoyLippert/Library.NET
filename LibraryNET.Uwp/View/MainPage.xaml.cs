using LibraryNET.Controller;
using LibraryNET.DonauApi;
using LibraryNET.Helper;
using LibraryNET.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LibraryNET
{
    public sealed partial class MainPage : Page
    {
        /// <summary>Gets or sets the users collection.</summary>
        /// <value>The users collection.</value>
        private ObservableCollection<Book> usersCollection { get; set; } = new ObservableCollection<Book>();
        
        /// <summary>Gets or sets the search result collection.</summary>
        /// <value>The search result collection.</value>
        private ObservableCollection<Book> searchResultCollection { get; set; } = new ObservableCollection<Book>();
       
        /// <summary>Initializes a new instance of the <see cref="MainPage" /> class.</summary>
        public MainPage()
        {
            this.InitializeComponent();
            MyPivot.Items.Clear(); 
            IsGridViewVisible(false);
        }
        
        
        /* METHODES */
       
        /// <summary>Adapts the new logged in user.</summary>
        /// <param name="user">The user.</param>
        private void AdaptNewLoggedInUser(User user)
        {
            MyTextBlock_LoggedInUser.Visibility = Visibility.Visible;
            if (user is User)
            {
                MyTextBlock_LoggedInUser.Text = user.Username;
                MyTextBlock_ListView_Collection_Header.Text = $"Welcome {user.Username}";
                MyButton_Logout_Flyout.Visibility = Visibility.Visible;
                MyPivot.Items.Clear();
                MyPivot.Items.Add(MyPivotItem_Explore);
                MyPivot.Items.Add(MyPivotItem_Account);
                MyPivot.SelectedIndex = 0;
                UpdateCollection();
            }
            else if (user == null)
            {
                MyTextBlock_LoggedInUser.Text = $"Guest mode";
                MyButton_Logout_Flyout.Visibility = Visibility.Collapsed;
                MyPivot.Items.Clear();
                MyPivot.Items.Add(MyPivotItem_Login);
                MyPivot.Items.Add(MyPivotItem_Explore);
                MyPivot.SelectedIndex = 1;
            }
            else
            {
                MyTextBlock_Login_ErrorMessage.Text = "saadf";
            }
        }
        /// <summary>Updates the collection.</summary>
        private async void UpdateCollection()
        {
            usersCollection.Clear();
            var listFromApi = await DonauApi.ApiHelper.GetCollection(LoggedInUser.CurrentLoggedInUser);
            foreach (var item in listFromApi)
            {
                usersCollection.Add(item);
            }
        }
        
        /// <summary>Fills the grid view.</summary>
        /// <param name="list">The list.</param>
        /// <returns>true if list is populated, false if not </returns>
        private async Task<bool> FillGridView(List<JObject> list)
        {
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    Book b = await CreateBookObjectFromApiObject.CreateBookFromGoogleApiLightWeight(item["selfLink"].ToString());
                    searchResultCollection.Add(b);
                };
                return true;
            }
            else
            {
                return false;
            }
        }
       
        /// <summary>Collecteds data needed for a search. then sends that data to the api and the results is populated in gridview with FillGridView method
        /// pluss it does alot of frontend stuff at the same time
        /// </summary>
        private async Task SearchAsync()
        {
            IsGridViewVisible(false);
            string GetComboBoxItemContent(string searchTerm)
            {
                if (searchTerm.Equals("All"))
                {
                    return "";
                }
                if (searchTerm.Equals("Title"))
                {
                    return "intitle";
                }
                if (searchTerm.Equals("Authors"))
                {
                    return "inauthor";
                }
                if (searchTerm.Equals("Publisher"))
                {
                    return "inpublisher";
                }
                else
                {
                    return searchTerm;
                }
            }
            searchResultCollection.Clear();
            IsLoading(true);
            MyTextBlock_Background_Message.Visibility = Visibility.Collapsed;

            if (MyTextBox_SeachWord.Text.Length > 0)
            {
                ComboBoxItem searchTerm = (ComboBoxItem)MyComboBox_SearchTerms.SelectedItem;
                ComboBoxItem orderBy = (ComboBoxItem)MyComboBox_Sorting.SelectedItem;
                var searchResults = await GoogleBookApiController.Search(10, MyTextBox_SeachWord.Text, GetComboBoxItemContent(searchTerm.Content.ToString()), orderBy.Content.ToString(), 0);

                if (await FillGridView(searchResults))
                {
                    MyGridView.Header = $"Results: {MyGridView.Items.Count}/{await GoogleBookApiController.SearchInfo(MyTextBox_SeachWord.Text)}";
                    MyGridView.SelectedIndex = 0;
                    IsGridViewVisible(true);
                    IsLoading(false);
                }
                else
                {
                    MyGridView.Header = "No results found...";
                }
            }
            else
            {
                MyTextBox_SeachWord.PlaceholderText = "Empty field...";
            }      
        }
       
        /// <summary>
        /// Displays the book information.
        /// </summary>
        /// <param name="url">The URL.</param>
        private async void DisplayBookInfo(string url)
        {
            MyGridView.Opacity = 0.5;
            IsLoading(true);
            try
            {
                Book book = await CreateBookObjectFromApiObject.CreateBookFromGoogleApi(url);
                var contentDialog = new ContentDialogController().CreateBookContentDialog(book);
                if (LoggedInUser.CurrentLoggedInUser == null)
                {
                    contentDialog.IsPrimaryButtonEnabled = false;
                }


                ContentDialogResult result = await contentDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await DonauApi.ApiHelper.PostBook(LoggedInUser.CurrentLoggedInUser, book);
                    UpdateCollection();
                }
            }
            catch (Exception e)
            {
                new ContentDialogController().CreateErrorContentDialog(e.Message);
            }
            IsLoading(false);
            MyGridView.Opacity = 1;
        }
       
        /// <summary>
        /// Logins the attempt.
        /// </summary>
        private async void LoginAttempt()
        {
            // Clear error at each atempt
            MyTextBlock_Login_ErrorMessage.Text = "";
            searchResultCollection.Clear();
            usersCollection.Clear();
            MyTextBox_SeachWord.Text = "";
            MyGridView.Header = "";

            IsProgressRingLoginAndLogoutVisible(true);
                if (await new LoginController().LoginRequestAsync(MyTextBox_Username.Text, MyTextBox_Password.Password) == true)
            {
                AdaptNewLoggedInUser(LoggedInUser.CurrentLoggedInUser);
            }
            else
            {
                MyTextBlock_Login_ErrorMessage.Text = "Invalid information";
            }
            IsProgressRingLoginAndLogoutVisible(false);
        }
       
        
        /* TRIGGER METHODES */
       
        /// <summary>
        /// Determines whether [is progress ring login and logout visible] [the specified trigger].
        /// </summary>
        /// <param name="trigger">if set to <c>true</c> [trigger].</param>
        private void IsProgressRingLoginAndLogoutVisible(bool trigger)
        {
            if (trigger == true)
            {
                myProgressRing_LoginLogout.IsActive = true;
            }
            else
            {
                myProgressRing_LoginLogout.IsActive = false;
            }
        }
        
        /// <summary>
        /// Determines whether [is grid view visible] [the specified trigger].
        /// </summary>
        /// <param name="trigger">if set to <c>true</c> [trigger].</param>
        private void IsGridViewVisible(bool trigger)
        {
            if (trigger == true)
            {
                MyTextBlock_Background_Message.Visibility = Visibility.Collapsed;
                MyGridView.Visibility = Visibility.Visible;
            }
            else
            {
                MyGridView.Visibility = Visibility.Collapsed;
                MyTextBlock_Background_Message.Visibility = Visibility.Visible;
            }
        }
       
        /// <summary>
        /// Determines whether the specified trigger is loading.
        /// </summary>
        /// <param name="trigger">if set to <c>true</c> [trigger].</param>
        private void IsLoading(bool trigger = false)
        {
            LoadingControl.IsLoading = trigger;
        }
        
        
        /* XAML METHODES */
    
            /// <summary>
        /// Handles the Loaded event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (InternettChecker.CheckForInternetConnection())
            {
                MyTextBlock_LoggedInUser.Visibility = Visibility.Collapsed;
                MyPivot.Items.Add(MyPivotItem_Login);
                DonauApi.ApiHelper.InitializeClient();
                GoogleBookApiController.InitializeClient();
            }
            else
            {
                MyTextBlock_LoggedInUser.Visibility = Visibility.Collapsed;
                ContentDialogResult result = await new ContentDialogController().CreateErrorContentDialog("Application does not have a internet connection!\n\nClick the button to close the application\nand connect to the internet and try again").ShowAsync();
                if (result == ContentDialogResult.None)
                {
                    Application.Current.Exit();
                }
            }
        }
    
        /// <summary>
        /// Handles the Click event of the MyButton_Search control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private async void MyButton_Search_Click(object sender, RoutedEventArgs e)
        {
            await SearchAsync();
        }
     
        /// <summary>
        /// Handles the KeyDown event of the MyTextBox_SeachWord control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Input.KeyRoutedEventArgs" /> instance containing the event data.</param>
        private async void MyTextBox_SeachWord_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                await SearchAsync();
            }
        }
    
        /// <summary>
        /// Handles the Click event of the MyButton_Login control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void MyButton_Login_Click(object sender, RoutedEventArgs e)
        {
            LoginAttempt();
        }
    
        /// <summary>
        /// Handles the Click event of the MyButton_Logout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void MyButton_Logout_Click(object sender, RoutedEventArgs e)
        {
            LoggedInUser.CurrentLoggedInUser = null;
            MyPivot.Items.Clear();
            MyPivot.Items.Add(MyPivotItem_Login);
            usersCollection.Clear();
            searchResultCollection.Clear();
            MyTextBox_SeachWord.Text = "";
            MyTextBlock_LoggedInUser.Text = "";
            MyTextBox_Username.Text = "";
            MyTextBox_Password.Password = "";
            MyGridView.Header = "";
            MyListView_Collection.Header = "";
        }
       
        /// <summary>
        /// Handles the Click event of the MyButton_Guest control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void MyButton_Guest_Click(object sender, RoutedEventArgs e)
        {
            searchResultCollection.Clear();
            usersCollection.Clear();
            MyTextBox_SeachWord.Text = "";
            LoggedInUser.CurrentLoggedInUser = null;
            AdaptNewLoggedInUser(null);
        }
        
        /// <summary>
        /// Handles the ItemClick event of the MyGridView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Controls.ItemClickEventArgs" /> instance containing the event data.</param>
        private void MyGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // to identify the gridviewitem I am storing the books unique url in the 'image' tag to access it here.
            // display the books information that i get from the url. no information is stored in the gridview because of long load time and so on
            DisplayBookInfo($"https://www.googleapis.com/books/v1/volumes/{MyGridView.SelectedItem}");
        }
        
        /// <summary>
        /// Handles the Click event of the MyButton_Register control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void MyButton_Register_Click(object sender, RoutedEventArgs e)
        {
            /* Because the deadline is soon here I had to rush this functionality and focus on other apspects of the application  */
            MyTextBox_Username.Header = "Registration mode";
            MyTextBox_Username.PlaceholderText = "What is your username?";
            MyTextBox_Password.Header = "Registration mode";
            MyTextBox_Password.PlaceholderText = "What is your password?";
            MyButton_Register_Submit.Visibility = Visibility.Visible;
            MyButton_Register.Visibility = Visibility.Collapsed;
            MyButton_Login.Visibility = Visibility.Collapsed;
            MyButton_Logout.Visibility = Visibility.Collapsed;
        }
        
        /// <summary>
        /// Handles the Click event of the MyButton_Register_Submit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private async void MyButton_Register_Submit_Click(object sender, RoutedEventArgs e)
        {
            if (await new RegController().RegisteredUser(MyTextBox_Username.Text, MyTextBox_Password.Password))
            {
                MyButton_Register_Submit.Visibility = Visibility.Collapsed;
                MyButton_Register.Visibility = Visibility.Visible;
                MyButton_Login.Visibility = Visibility.Visible;
                MyButton_Logout.Visibility = Visibility.Visible;
                var cd = new ContentDialogController().CreateContentDialogMessage("Success!").ShowAsync();
                MyTextBox_Username.Header = "";
                MyTextBox_Username.Text = "";
                MyTextBox_Username.PlaceholderText = "Username:";
                MyTextBox_Password.Header = "";
                MyTextBox_Password.Password = "";
                MyTextBox_Password.PlaceholderText = "Password:";
            }
            else
            {
                var cd = new ContentDialogController().CreateContentDialogMessage("Something went wrong or user already registered!").ShowAsync();
            }
        }
        
        /// <summary>
        /// Handles the DeleteClick event of the MyButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private async void MyButton_DeleteClick(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            if (await ApiHelper.DeleteACollection(LoggedInUser.CurrentLoggedInUser, btn.Tag.ToString()))
            {
                var cd = new ContentDialogController().CreateContentDialogMessage("Success!").ShowAsync();
                UpdateCollection();
            }
            else
            {
                MyListView_Collection.Header = "failed";
            }
        }
        
        /// <summary>
        /// Handles the KeyDown event of the MyTextBox_Password control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.Input.KeyRoutedEventArgs" /> instance containing the event data.</param>
        private void MyTextBox_Password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginAttempt();
            }
        }
        
        /// <summary>
        /// Handles the Click event of the MyButton_Refresh_Collection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private void MyButton_Refresh_Collection_Click(object sender, RoutedEventArgs e)
        {
            UpdateCollection();
        }
    }
}
