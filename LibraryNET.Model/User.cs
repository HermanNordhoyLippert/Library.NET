using System.ComponentModel.DataAnnotations;

namespace LibraryNET.Model
{
    /// <summary>
    /// A user
    /// </summary>
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public override string ToString()
        {
            return $"{Username}";
        }
    }
}
