using System.ComponentModel.DataAnnotations;

namespace StudentInterestSystem.Models
{
    public class AddStudentViewModel
    {
        [Required(ErrorMessage = "*Please enter full name")]
        [StringLength(50)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "*Please enter roll number")]
        [StringLength(10)]
        public string RollNumber { get; set; }

        [Required(ErrorMessage = "*Please enter email")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "*Please select gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "*Please enter date of birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "*Please select a city")]
        public string City { get; set; }

        [Required(ErrorMessage = "*Please select an Interest")]
        public string Interest { get; set; }

        [Required(ErrorMessage = "*Please select a department")]
        public string Department { get; set; }

        [Required(ErrorMessage = "*Please select a degree title")]
        public string DegreeTitle { get; set; }

        [Required(ErrorMessage = "*Please select a Subject")]

        public string Subject { get; set; }

        [Required(ErrorMessage = "*Please enter start date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "*Please enter end date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public List<string> Interests { get; set; } = new List<string>();


    }
}
