using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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
        public DbSet<ArchiveTasks> ArchiveTasks { get; set; }

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
                .HasOne(t => t.ArchiveTasks)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // Указываем NO ACTION при удалении

            modelBuilder.Entity<ArchiveTasks>()
                .HasOne(a => a.Task)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction); // вместо DeleteBehavior.Cascade

            modelBuilder.Entity<ArchiveTasks>()
                .HasOne(a => a.Status)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.ToTable("Tasks");
                entity.ToTable(tb => tb.HasTrigger("trg_AfterUpdate_Tasks"));
                entity.ToTable(tb => tb.HasTrigger("trg_AfterInsert_Tasks"));
            });

            modelBuilder.Entity<TasksStatus>(entity =>
            {
                entity.ToTable("TasksStatus");
                entity.ToTable(tb => tb.HasTrigger("trg_AfterUpdate_TasksStatus"));
            });

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.ToTable("Notifications");
                entity.ToTable(tb => tb.HasTrigger("trg_AfterUpdate_Notifications"));
            });

            /*modelBuilder.Entity<Notifications>(entity =>
            {
                entity.ToTable("Notifications");
                entity.ToTable(tb => tb.HasTrigger("trg_AfterUpdate_Notifications"));
                entity.Property(e => e.IsReaded).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.Property(e => e.IsReaded).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            });*/
        }

    }
}
