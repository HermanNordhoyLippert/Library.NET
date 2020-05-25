using System.ComponentModel.DataAnnotations;

namespace LibraryNET.Model
{
    /// <summary>
    /// A book object
    /// </summary>
    public class Book
    {
        [Key]
        public string BookId { get; set; }
        public string Title { get; set; }
        public string PublisherDate { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PageCount { get; set; }
        public string Description { get; set; }
        public string imageUrl { get; set; }
        public override string ToString()
        {
            return $"{BookId}";
        }
    }
}
