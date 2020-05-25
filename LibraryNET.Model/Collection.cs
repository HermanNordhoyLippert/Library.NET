using System.ComponentModel.DataAnnotations;

namespace LibraryNET.Model
{
    /// <summary>
    /// A users Collection of books
    /// </summary>
    public class Collection
    {
        [Key]
        public int Id { get; set; }
        public string userId { get; set; }
        public string bookId { get; set; }
    }
}
