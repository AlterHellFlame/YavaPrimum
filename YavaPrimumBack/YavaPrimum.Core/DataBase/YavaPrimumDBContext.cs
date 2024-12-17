using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DataBase
{
    public class YavaPrimumDBContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-LOAQH83;Database=YavaPrimumDB;
				Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
        }

        public DbSet<Candidate> Candidate { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserRegisterInfo> UserRegisterInfo { get; set; }
        public DbSet<Post> Post { get; set; }
    }
}
