using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInterestSystem.DB;
using StudentInterestSystem.Models;


namespace StudentInterestSystem.Controllers
{

    public class AdminController : Controller
    {

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


        public AdminController(SISDb SISdbContext)
        {
            this.SISdbContext = SISdbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var interests = await SISdbContext.Students
            .Select(s => s.Interest)
            .Distinct()
            .ToListAsync();

            var student = new AddStudentViewModel();

            if (interests != null)
            {
                student.Interests = interests;
            }
            else
            {
                student.Interests = new List<string>();
            }
            LogUserActivity("admin", "Navigated to Add", "Add Hit");

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddStudentViewModel addStudent)
        {
            if (ModelState.IsValid && addStudent.StartDate < addStudent.EndDate)
            {
                var student = new Student()
                {
                    FullName = addStudent.FullName,
                    RollNumber = addStudent.RollNumber,
                    Email = addStudent.Email,
                    DateOfBirth = addStudent.DateOfBirth,
                    DegreeTitle = addStudent.DegreeTitle,
                    City = addStudent.City,
                    Gender = addStudent.Gender,
                    Department = addStudent.Department,
                    StartDate = addStudent.StartDate,
                    EndDate = addStudent.EndDate,
                    Interest = addStudent.Interest,
                    Subject = addStudent.Subject
                };
                SISdbContext.Students.Add(student);
                SISdbContext.SaveChanges();
                LogUserActivity("admin", "Navigated to post Add", "Create Student");

                return RedirectToAction("Students");
            }
            else
            {
                return View();
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Students()
        {
            var students = await SISdbContext.Students.ToListAsync();
            var stud = new List<ListViewModel>(); // Initialize a list of ListViewModel

            foreach (var s in students)
            {
                var student = new ListViewModel()
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    RollNumber = s.RollNumber,
                    DOB = s.DateOfBirth,
                    DegreeTitle = s.DegreeTitle,
                    City = s.City,
                    Department = s.Department,
                    Interest = s.Interest
                };
                stud.Add(student); // Use Add() method to add items to the list
            }
            LogUserActivity("admin", "Navigated to list", "List");
            return View(stud);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int Id)
        {
            var student=await SISdbContext.Students.FirstOrDefaultAsync(x=>x.Id == Id);
            if (student != null)
            {
                var viewModel = new UpdateStudentViewModel()
                {
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
                LogUserActivity("admin", "Navigated to Edit", "Edit Hit");

                return await Task.Run(()=>View("Edit",viewModel));
            }
            return Redirect("Students");

        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateStudentViewModel model)
        {
            var student = await SISdbContext.Students.FindAsync(model.Id);
            if (student != null)
            {
                student.FullName=model.FullName;
                student.RollNumber=model.RollNumber;
                student.Email=model.Email;
                student.Gender=model.Gender;
                student.Department=model.Department;
                student.StartDate=model.StartDate;
                student.EndDate=model.EndDate;
                student.Interest=model.Interest;
                student.Subject=model.Subject;
                student.DateOfBirth=model.DateOfBirth;
                student.DegreeTitle=model.DegreeTitle;
                student.City=model.City;

                LogUserActivity("admin", "Navigated to Edit", "Edit Student");
                await SISdbContext.SaveChangesAsync();
                return RedirectToAction("Students");

            }
            return RedirectToAction("Students");



        }

        [HttpGet]
        public async Task<IActionResult> View(int Id)
        {
            var student = await SISdbContext.Students.FirstOrDefaultAsync(x => x.Id == Id);
            if (student != null)
            {
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
                LogUserActivity("admin", "Navigated to View", "View Student");

                return await Task.Run(() => View("View", viewModel));
            }
            return Redirect("Students");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int Id)
        {
            var student = await SISdbContext.Students.FirstOrDefaultAsync(x => x.Id == Id);
            if (student != null)
            {
                SISdbContext.Students.Remove(student);
                await SISdbContext.SaveChangesAsync();
                LogUserActivity("admin", "Navigated to delete", "Delete Student");
                return RedirectToAction("Students");
            }
            return RedirectToAction("Students");

        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
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
            var studentAges =  SISdbContext.Students.Select(s => DateTime.Now.Year - s.DateOfBirth.Year).ToList();

            var today = DateTime.Today;
            var thirtyDaysAgo = today.AddDays(-30);
            var random = new Random();

            var dateRange = Enumerable.Range(0, 31)  // Generate a sequence of numbers from 0 to 29
                .Select(offset => thirtyDaysAgo.AddDays(offset))  // Map each number to a date within the last 30 days
                .ToList();

            var submissionCountLast30Days = dateRange
                .GroupJoin(
                    SISdbContext.UserActivityLog
                        .Where(log => log.Timestamp.Date >= thirtyDaysAgo && log.Timestamp.Date <= today
                                    && log.Details == "Create Student")
                        .GroupBy(log => log.Timestamp.Date)
                        .Select(group => new
                        {
                            Date = group.Key,
                            Count = group.Count()
                        }),
                    date => date,
                    activity => activity.Date,
                    (date, activities) => new StudentCountViewModel
                    {
                        label = date.ToString("yyyy-MM-dd"),
                        StudentCount = activities.FirstOrDefault()?.Count ?? 0
                    })
                .ToList();

            var dailyActionCounts = dateRange
                .GroupJoin(
                    SISdbContext.UserActivityLog
                        .Where(log => log.Timestamp.Date >= thirtyDaysAgo && log.Timestamp.Date <= today)
                        .GroupBy(log => log.Timestamp.Date)
                        .Select(group => new
                        {
                            Date = group.Key,
                            Count = group.Count()
                        }),
                    date => date,
                    activity => activity.Date,
                    (date, activities) => new StudentCountViewModel
                    {
                        label = date.ToString("yyyy-MM-dd"),
                        StudentCount = activities.FirstOrDefault()?.Count ?? 0
                    })
                .ToList();

            var now = DateTime.Now;
            var twentyFourHoursAgo = now.AddHours(-24);

            var fifteenMinutesIntervals = Enumerable.Range(0, 24 * 4) // 24 hours * 4 (every 15 minutes)
                .Select(offset => twentyFourHoursAgo.AddMinutes(offset * 15))
                .ToList();

            var actionCountsLast24Hours = fifteenMinutesIntervals
                .Select(intervalStart => new StudentCountViewModel
                {
                    label = $"{intervalStart.ToString("HH:mm")} to {intervalStart.AddMinutes(15).ToString("HH:mm")}",
                    StudentCount = SISdbContext.UserActivityLog
                        .Count(log => log.Timestamp >= intervalStart && log.Timestamp < intervalStart.AddMinutes(15))
                })
                .ToList();


            var activityData = SISdbContext.UserActivityLog
                .Where(log => log.Timestamp >= thirtyDaysAgo)
                .ToList(); // Fetch data from the database


            var hourlyActivityCounts = new Dictionary<DateTime, int>();

            for (int hoursOffset = 0; hoursOffset < 30 * 24; hoursOffset++)
            {
                var specificHour = (thirtyDaysAgo.AddHours(hoursOffset));
                var nextHour = specificHour.AddHours(1);

                if (!hourlyActivityCounts.ContainsKey(specificHour))
                {
                    var activityCount = SISdbContext.UserActivityLog
                        .Count(log => log.Timestamp >= specificHour && log.Timestamp < nextHour);

                    hourlyActivityCounts.Add(specificHour, activityCount);
                }
            }

                var entry = hourlyActivityCounts.Select(entry => new
                {
                    Hour = entry.Key.Hour,
                    Period = entry.Key.ToString("tt"),
                    ActivityCount = entry.Value
                }).ToList();


            var mostActiveHours = hourlyActivityCounts
                 .OrderByDescending(entry => entry.Value >= 2)
                 .Select(entry => $"{entry.Key.Hour} {entry.Key.ToString("tt")}")
                 .Distinct()
                 .Take(5)
                 .ToList();

            var leastActiveHours = hourlyActivityCounts
                .OrderBy(entry => entry.Value ==1)
                .Select(entry => $"{entry.Key.Hour} {entry.Key.ToString("tt")}")
                .Distinct()
                .Take(5)
                .ToList();

            var deadHours = hourlyActivityCounts
                .Where(entry => entry.Value <= 1)
                .OrderBy(entry => entry.Value)
                .Select(entry => $"{entry.Key.Hour} {entry.Key.ToString("tt")}")
                .Distinct()
                .Take(5)
                .ToList();



            var viewModel = new DashboardViewModel
            {
                Top5Interests = top5,
                Bottom5Interests = bottom5,
                DistinctInterests = distinctInterestCount,
                studentStats = studentStats,
                provinceCount = provinceCounts,
                departmentCount = departmentCounts,
                StudentAges= studentAges,
                degreeCount=degreeDistribution,
                genderCount=genderDistribution,
                submissionCount=submissionCountLast30Days,
                activity30Count= dailyActionCounts,
                activity15minCount= actionCountsLast24Hours,
                mostactiveHours=mostActiveHours,
                leastactiveHours=leastActiveHours,
                deadHours=deadHours,
            };
            LogUserActivity("admin", "Navigated to Dashboard", "Admin Dashboard");

            return View(viewModel);
        }


    }
}
