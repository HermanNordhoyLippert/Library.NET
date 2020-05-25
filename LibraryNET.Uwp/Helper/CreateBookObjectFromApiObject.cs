using System.Threading.Tasks;
using LibraryNET.Controller;
using Newtonsoft.Json.Linq;
using LibraryNET.Model;
using System.Linq;
using System;

namespace LibraryNET.Helper
{
    public class CreateBookObjectFromApiObject
    {
        /// <summary>
        /// Creates book object from google API.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>returns true if successful and false if not</returns>
        public async static Task<Book> CreateBookFromGoogleApi(string url)
        {
            JObject jObject = await GoogleBookApiController.GetSpesificBook(url);
            Book b = new Book();
            //b.BookId = jObject["id"].Value<string>();
            b.BookId = jObject.Value<string>("id");
            b.Title = jObject["volumeInfo"].Value<string>("title");
            b.PublisherDate = jObject["volumeInfo"].Value<string>("publishedDate");
            if (jObject["volumeInfo"].SelectTokens("authors").Count() > 0)
            {
                foreach (var item in jObject["volumeInfo"].SelectTokens("authors").FirstOrDefault())
                {
                    b.Author += item.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Trim();
                }
            }
            else
            {
                b.Author = "No Author found";
            }
            b.Publisher = jObject["volumeInfo"].Value<string>("publisher");
            b.PageCount = Int32.Parse(jObject["volumeInfo"].Value<string>("pageCount"));
            b.Description = jObject["volumeInfo"].Value<string>("description");
            b.imageUrl = jObject["volumeInfo"].SelectTokens("imageLinks").First().Value<string>("thumbnail");
            return b;
        }

        /// <summary>
        /// Creates book object from google API light weight.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>returns true if successful and false if not</returns>
        public async static Task<Book> CreateBookFromGoogleApiLightWeight(string url)
        {
            JObject jObject = await GoogleBookApiController.GetSpesificBook(url);
            Book b = new Book();
            b.BookId = jObject.Value<string>("id");
            b.Title = jObject["volumeInfo"].Value<string>("title");
            b.imageUrl = jObject["volumeInfo"].SelectTokens("imageLinks").FirstOrDefault().Value<string>("thumbnail");
            return b;
        }
    }
}
