namespace StudentInterestSystem.Models
{
    public class StudentStats
    {
        public int Studying { get; set; }
        public int RecentlyEnrolled { get; set; }
        public int AboutToGraduate { get; set; }
        public int Graduated { get; set; }
    }
    public class StudentCountViewModel
    {
        public string label { get; set; }
        public int StudentCount { get; set; }
    }

    public class DashboardViewModel
    {
        public int Id { get; set; }
        public List<StudentCountViewModel> Top5Interests { get; set; }
        public List<StudentCountViewModel> Bottom5Interests { get; set; }
        public int DistinctInterests { get; set; }
        public StudentStats studentStats { get; set; }

        public List<StudentCountViewModel> provinceCount { get; set; }
        public List<StudentCountViewModel> departmentCount{ get; set; }
        public List<StudentCountViewModel> degreeCount { get; set; }
        public List<StudentCountViewModel> genderCount { get; set; }
        public List<StudentCountViewModel> submissionCount { get; set; }
        public List<StudentCountViewModel> activity30Count { get; set; }
        public List<StudentCountViewModel> activity15minCount { get; set; }

        public List<string> mostactiveHours { get; set; }
        public List<string> leastactiveHours { get; set; }
        public List<string> deadHours { get; set; }





        public List<int> StudentAges { get; set; }


    }
}
