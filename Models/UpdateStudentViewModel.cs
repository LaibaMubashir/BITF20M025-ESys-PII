namespace StudentInterestSystem.Models
{
    public class UpdateStudentViewModel
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public string RollNumber { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Interest { get; set; }
        public string Department { get; set; }
        public string DegreeTitle { get; set; }
        public string Subject { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
