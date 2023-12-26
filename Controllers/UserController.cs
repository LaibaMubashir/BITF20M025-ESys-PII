using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using StudentInterestSystem.DB;
using StudentInterestSystem.Models;

namespace StudentInterestSystem.Controllers
{
    public class UserController : Controller
    {
        public int Id { get; set; }

        public SISDb SISdbContext { get; }
        public void LogUserActivity(string user, string details, string activity)
        {
            var logEntry = new UserActivityLogModel()
            {
                Username = user,
                ActivityType = activity,
                Timestamp = DateTime.Now,
                Details = details
            };

            SISdbContext.UserActivityLog.Add(logEntry);
            SISdbContext.SaveChanges();
        }

        public UserController(SISDb SISdbContext)
        {
            this.SISdbContext = SISdbContext;

        }
        [HttpGet]
        public async Task<IActionResult> View(int Id)
        {

            var student = await SISdbContext.Students.FirstOrDefaultAsync(x => x.Id == Id);
            
            if (student != null)
            {
                this.Id = Id;
                var viewModel = new Student()
                {
                    Id = student.Id,
                    FullName = student.FullName,
                    RollNumber = student.RollNumber,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth,
                    DegreeTitle = student.DegreeTitle,
                    City = student.City,
                    Gender = student.Gender,
                    Department = student.Department,
                    StartDate = student.StartDate,
                    EndDate = student.EndDate,
                    Interest = student.Interest,
                    Subject = student.Subject
                };
                LogUserActivity(student.Email, "Navigated to User View", "User View");

                return await Task.Run(() => View("View", viewModel));
            }

            return Redirect("Dashboard");

        }
        [HttpGet]
        public async Task<IActionResult> Dashboard(int id)
        {
            var student = await SISdbContext.Students.FirstOrDefaultAsync(x => x.Id == id);


            var top5Interests = await SISdbContext.Students
                .GroupBy(s => s.Interest)
                .Select(g => new
                {
                    Interest = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            var top5 = top5Interests.Select(interest => new StudentCountViewModel
            {
                label = interest.Interest,
                StudentCount = interest.Count
            }).ToList();
            var bottom5Interests = await SISdbContext.Students
            .GroupBy(s => s.Interest)
            .Select(g => new
            {
                Interest = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Count)
            .Take(5)
            .ToListAsync();

            var bottom5 = bottom5Interests.Select(interest => new StudentCountViewModel
            {
                label = interest.Interest,
                StudentCount = interest.Count
            }).ToList();


            var distinctInterestCount = await SISdbContext.Students
                 .Select(s => s.Interest)
                 .Distinct()
                 .CountAsync();

            var currentDate = DateTime.Now;

            var studentStats = new StudentStats
            {
                Studying = await SISdbContext.Students
                    .Where(s => s.StartDate <= currentDate && s.EndDate >= currentDate)
                    .CountAsync(),

                RecentlyEnrolled = await SISdbContext.Students
                    .Where(s => s.StartDate >= currentDate.AddMonths(-3)) // Assuming "recently" means within the last 3 months
                    .CountAsync(),

                AboutToGraduate = await SISdbContext.Students
                    .Where(s => s.EndDate >= currentDate && s.EndDate <= currentDate.AddMonths(6)) // Assuming "about to graduate" is within the next 6 months
                    .CountAsync(),

                Graduated = await SISdbContext.Students
                    .Where(s => s.EndDate < currentDate)
                    .CountAsync()
            };

            var cityProvinceMap = new Dictionary<string, List<string>>
            {
                { "Punjab", new List<string> {"Lahore", "Faisalabad", "Islamabad","Rawalpindi", "Gujranwala", "Multan", "Sargodha", "Sialkot", "Bahawalpur", "Sheikhupura",
            "Gujrat", "Jhelum", "Sahiwal", "Sargodha", "Okara", "Kasur", "Mianwali", "Chiniot", "Mandi Bahauddin", "Khanewal",
            "Jhang", "Attock", "Nankana Sahib", "Khushab", "Bhakkar", "Toba Tek Singh", "Rajanpur", "Layyah", "Vehari" } },
                { "Sindh",new List<string> {"Karachi", "Hyderabad", "Sukkur", "Larkana", "Nawabshah", "Mirpur Khas", "Jacobabad", "Shikarpur", "Dadu",
            "Kashmore", "Shahdad Kot", "Badin", "Ghotki", "Tando Allahyar", "Sanghar", "Umerkot", "Thatta", "Khairpur",
            "Naushahro Feroze", "Jamshoro", "Hala", "Matiari"}},
                {"KPK", new List<string> {"Peshawar", "Abbottabad", "Mardan", "Kohat", "Swabi", "Nowshera", "Charsadda", "Dera Ismail Khan", "Mansehra",
            "Chitral", "Batagram", "Bannu", "Haripur", "Hangu", "Upper Dir", "Lower Dir", "Tank", "Lakki Marwat", "Swat"}},
                {"Balochistan", new List<string> {"Quetta", "Gwadar", "Khuzdar", "Chaman", "Turbat", "Hub", "Sibi", "Zhob", "Nasirabad", "Loralai", "Kharan",
            "Killa Saifullah", "Barkhan", "Washuk", "Ziarat"}}
            };

            var allStudents = await SISdbContext.Students.ToListAsync();

            var provinceCounts = allStudents
                .GroupBy(s => cityProvinceMap.FirstOrDefault(x => x.Value.Contains(s.City)).Key)
                .Select(g => new StudentCountViewModel
                {
                    label = g.Key,
                    StudentCount = g.Count()
                })
                .ToList();


            var departmentCounts = await SISdbContext.Students
                   .GroupBy(s => s.Department)
                   .Select(g => new StudentCountViewModel
                   {
                       label = g.Key,
                       StudentCount = g.Count()
                   })
                   .ToListAsync();

            var degreeDistribution = await SISdbContext.Students
                .GroupBy(s => s.DegreeTitle)
                .Select(g => new StudentCountViewModel
                {
                    label = g.Key,
                    StudentCount = g.Count()
                })
                .ToListAsync();

            var genderDistribution = await SISdbContext.Students
                .GroupBy(s => s.Gender)
                .Select(g => new StudentCountViewModel
                {
                    label = g.Key,
                    StudentCount = g.Count()
                })
                .ToListAsync();


            // Fetching student ages from the database
            var studentAges = SISdbContext.Students.Select(s => DateTime.Now.Year - s.DateOfBirth.Year).ToList();

            var viewModel = new DashboardViewModel
            {
                Top5Interests = top5,
                Bottom5Interests = bottom5,
                DistinctInterests = distinctInterestCount,
                studentStats = studentStats,
                provinceCount = provinceCounts,
                departmentCount = departmentCounts,
                StudentAges = studentAges,
                degreeCount = degreeDistribution,
                genderCount = genderDistribution,
                Id=id,
            };

            LogUserActivity(student.Email, "Navigated to User Dashboard", "User Dashboard");
            return View(viewModel);
        }
    }
}
