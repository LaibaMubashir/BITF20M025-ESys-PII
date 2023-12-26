using Microsoft.EntityFrameworkCore;
using StudentInterestSystem.Models;

namespace StudentInterestSystem.DB
{
    public class SISDb : DbContext
    {
        public SISDb(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<AccountViewModel> Accounts { get; set; }
        public DbSet<UserActivityLogModel>UserActivityLog { get; set; }
    }
}
