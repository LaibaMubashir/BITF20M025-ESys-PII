using Azure.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentInterestSystem.Models
{
    public class AccountViewModel
    {
        [Key] // Add this attribute to define the primary key
        public int Id { get; set; }
        [Required(ErrorMessage = "*Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Username { get; set; }
        [Required(ErrorMessage = "*Please enter password")]
        public string Password { get; set; }
    }

}
