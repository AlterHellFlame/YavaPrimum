using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DataBase
{
    public class YavaPrimumDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=DESKTOP-LOAQH83;Database=YavaPrimum;
                Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;");
        }

        public DbSet<Candidate> Candidate { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<TasksStatus> TasksStatus { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Notifications> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Country)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict); // Изменяем на Restrict для избежания каскадного удаления

            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.User)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении

            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.CandidatePost)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.Candidate)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении


            modelBuilder.Entity<Notifications>()
                .HasOne(t => t.Status)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении


            modelBuilder.Entity<Tasks>()
           .ToTable(tb => tb.HasTrigger("trg_updTask_crtNotification"));
        }

    }
}
