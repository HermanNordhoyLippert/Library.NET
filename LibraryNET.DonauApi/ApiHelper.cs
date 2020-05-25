using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LibraryNET.Model;
using System.Net.Http;
using System.Linq;
using System.Net;

namespace LibraryNET.DonauApi
{
    public class ApiHelper
    {
        /// <summary>
        /// Gets or sets the API client.
        /// </summary>
        /// <value>
        /// The API client.
        /// </value>
        public static HttpClient ApiClient { get; set; }
        /// <summary>
        /// The URL
        /// </summary>
        public const string url = @"http://localhost:60161";
        /// <summary>
        /// Initializes the client.
        /// </summary>
        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        // GET        
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns></returns>
        public static async Task<List<User>> GetUser()
        {
            try
            {
                using (HttpResponseMessage r = await ApiHelper.ApiClient.GetAsync(url + "/api/Users"))
                {
                    if (r.IsSuccessStatusCode)
                    {
                        var root = await r.Content.ReadAsAsync<List<User>>();
                        // Crypto attempt
                        //foreach (var item in root)
                        //{
                        //    var newUser = SimpleCryptographic.DecryptUser(item);
                        //    root.Add(newUser);
                        //    root.Remove(item);
                        //};
                       return root;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (System.Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        public static async Task<List<Book>> GetCollection(User user)
        {
            async Task<List<Book>> GetBooks()
            {
                using (HttpResponseMessage r = await ApiHelper.ApiClient.GetAsync(url + "/api/Books"))
                {
                    if (r.IsSuccessStatusCode)
                    {
                        var root = await r.Content.ReadAsAsync<List<Book>>();
                        return root;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            //user = SimpleCryptographic.EncryptUser(user);
            using (HttpResponseMessage r = await ApiHelper.ApiClient.GetAsync(url + "/api/Collections"))
            {
                if (r.IsSuccessStatusCode)
                {
                    var root = await r.Content.ReadAsAsync<List<Collection>>();
                    var booksInDatabase = GetBooks();
                    var usersCollection = new List<Book>();

                    foreach (var book in await booksInDatabase)
                    {
                        foreach (var item in root)
                        {
                            if (book.BookId == item.bookId && item.userId == user.Id)
                            {
                                usersCollection.Add(book);
                            }
                        }
                    }
                    return usersCollection;
                }
                else
                {
                    return null;
                }
            }
        }
        // POST        
        /// <summary>
        /// Posts the user.
        /// </summary>
        /// <param name="user">The user.</param>
        public static async Task PostUser(User user)
        {
            //user = SimpleCryptographic.EncryptUser(user);
            try
            {
                await ApiClient.PostAsJsonAsync(url + "/api/Users", user);
            }
            catch (WebException e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Posts the book.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        public static async Task<bool> PostBook(User user, Book book)
        {
            // Inner methodes
            async Task<List<Book>> GetBooks()
            {
                using (HttpResponseMessage r = await ApiHelper.ApiClient.GetAsync(url + "/api/Books"))
                {
                    if (r.IsSuccessStatusCode)
                    {
                        var root = await r.Content.ReadAsAsync<List<Book>>();
                        return root;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            // Methode
            //user = SimpleCryptographic.EncryptUser(user);
            await PostCollectionAsync(user, book);
            var listOfBooksInDatabase = await GetBooks();
            if (!listOfBooksInDatabase.Any(x => x.BookId == book.BookId))
            {
                try
                {
                    await ApiClient.PostAsJsonAsync(url + "/api/Books", book);
                    return true;
                }
                catch (WebException e)
                {
                    //throw e;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Posts the collection asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="book">The book.</param>
        /// <returns></returns>
        private async static Task<bool> PostCollectionAsync(User user, Book book)
        {
            //user = SimpleCryptographic.EncryptUser(user);
            Collection collection = new Collection();
            collection.bookId = book.BookId;
            collection.userId = user.Id;
            var listOfCollections = await GetCollection(user);
            if (!listOfCollections.Any(x => x.BookId == book.BookId))
            {
                try
                {
                    await ApiClient.PostAsJsonAsync(url + "/api/Collections", collection);
                    return true;
                }
                catch (WebException e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        // DELETE        
        /// <summary>
        /// Deletes a collection.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="bookId">The book identifier.</param>
        /// <returns></returns>
        public static async Task<bool> DeleteACollection(User user, string bookId)
        {
            using (HttpResponseMessage getR = await ApiHelper.ApiClient.GetAsync(url + "/api/Collections"))
            {
                if (getR.IsSuccessStatusCode)
                {
                    var listOfCollections = await getR.Content.ReadAsAsync<List<Collection>>();
                    foreach (var item in listOfCollections)
                    {
                        if (item.userId.Equals(user.Id) && item.bookId.Equals(bookId))
                        {
                            HttpResponseMessage deleteR = await ApiClient.DeleteAsync(url + $"/api/Collections/{item.Id}");
                            if (deleteR.IsSuccessStatusCode)
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }
    }
}
