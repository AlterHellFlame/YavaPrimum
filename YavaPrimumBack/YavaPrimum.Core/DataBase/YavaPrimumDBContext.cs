using Microsoft.EntityFrameworkCore;
using YavaPrimum.Core.DataBase.Models;

namespace YavaPrimum.Core.DataBase
{
    public class YavaPrimumDBContext : DbContext
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
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<TaskType> TaskType { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Company> Company { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Post)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.Country)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.HR)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении

            modelBuilder.Entity<Candidate>()
                .HasOne(c => c.OP)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении


            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.User)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении

            modelBuilder.Entity<Tasks>()
                .HasOne(t => t.Candidate)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении
        }
    }
}
