using System.ComponentModel.DataAnnotations;

namespace Task.Web.Models
{
    public class login
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
