using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;

namespace LibraryNET.Controller
{
    public class GoogleBookApiController
    {
        /// <summary>
        /// Gets or sets the API client.
        /// </summary>
        /// <value>
        /// The API client.
        /// </value>
        private static HttpClient ApiClient { get; set; }
        
        /// <summary>
        /// Initializes the client.
        /// </summary>
        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
            ApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        /// <summary>
        /// Searches the specified maximum results.
        /// </summary>
        /// <param name="maxResults">The maximum results.</param>
        /// <param name="searchWord">The search word.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async static Task<List<JObject>> Search(int maxResults, string searchWord, string searchTerm, string orderBy, int index)
        {
            string fields = "&fields=items(id,selfLink,volumeInfo/title,volumeInfo/imageLinks/thumbnail)";
            using (HttpResponseMessage r = await ApiClient.GetAsync($"https://www.googleapis.com/books/v1/volumes?maxResults={maxResults}&projection=lite&q={searchWord}+{searchTerm}&orderBy={orderBy}&startIndex={index}" + fields))
            {
                if (r.IsSuccessStatusCode)
                {
                    JObject json = await r.Content.ReadAsAsync<JObject>();
                    return json.SelectToken("items").ToObject<List<JObject>>();
                }
                else
                {
                    //return null;
                    throw new Exception(r.ReasonPhrase);
                }
            }
        }
        
        /// <summary>
        /// Searches the information.
        /// </summary>
        /// <param name="searchWord">The search word.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async static Task<string> SearchInfo(string searchWord)
        {
            using (HttpResponseMessage r = await ApiClient.GetAsync($"https://www.googleapis.com/books/v1/volumes?maxResults=40&projection=lite&q={searchWord}"))
            {
                if (r.IsSuccessStatusCode)
                {
                    JObject json = await r.Content.ReadAsAsync<JObject>();
                    return json.SelectToken("totalItems").ToString();
                }
                else
                {
                    //return null;
                    throw new Exception(r.ReasonPhrase);
                }
            }
        }
        
        /// <summary>
        /// Gets the spesific book.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async static Task<JObject> GetSpesificBook(string url)
        {
            using (HttpResponseMessage r = await ApiClient.GetAsync(url))
            {
                if (r.IsSuccessStatusCode)
                {
                    JObject json = await r.Content.ReadAsAsync<JObject>();
                    return json.ToObject<JObject>();
                }
                else
                {
                    //return null;
                    throw new Exception(r.ReasonPhrase);
                }
            }
        }
    }
}
