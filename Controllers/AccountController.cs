using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentInterestSystem.DB;
using StudentInterestSystem.Models;

namespace StudentInterestSystem.Controllers
{
    public class AccountController : Controller
    {
        public SISDb SISdbContext { get; }

        public AccountController(SISDb SISdbContext)
        {
            this.SISdbContext = SISdbContext;

        }
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
        public ActionResult LogIn()
        {
            LogUserActivity("hit", "Navigated to LogIn","LogIn");
            return View();
        }

        public ActionResult Register()
        {
            LogUserActivity("hit", "Navigated to Register", "Register");
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Verify(AccountViewModel accountViewModel)
        {
            var account = await SISdbContext.Accounts.FirstOrDefaultAsync(a => a.Username == accountViewModel.Username && a.Password == accountViewModel.Password);

            if(account!=null)
            {
                if (accountViewModel.Username == "admin")
                {
                    return Redirect("/Admin/Dashboard");

                }
                else
                {
                    var student = await SISdbContext.Students.FirstOrDefaultAsync(s => s.Email == accountViewModel.Username);
                    return Redirect("/User/View/" + student.Id);
                }
            }
            else
            {
                return Redirect("LogIn");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Register(AccountViewModel accountViewModel)
        {
            if (ModelState.IsValid && accountViewModel.Password.Length>=8)
            {
                var user = new AccountViewModel()
                {
                   Username=accountViewModel.Username,
                   Password=accountViewModel.Password
                };
                var student = await SISdbContext.Students.FirstOrDefaultAsync(s => s.Email == accountViewModel.Username);
                SISdbContext.Accounts.Add(user);
                SISdbContext.SaveChanges();
                return Redirect("/User/View/" + student.Id);
            }
            else
            {
                return View();
            }

            
        }
        public async Task<IActionResult> Logout()
        {
            LogUserActivity("hit", "Log Out", "LogOut");
            return RedirectToAction("Login", "Account"); // Redirect to login page
        }

    }
}
