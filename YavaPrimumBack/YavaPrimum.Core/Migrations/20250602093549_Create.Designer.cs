﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YavaPrimum.Core.DataBase;

#nullable disable

namespace YavaPrimum.Core.Migrations
{
    [DbContext(typeof(YavaPrimumDBContext))]
    [Migration("20250602093549_Create")]
    partial class Create
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Candidate", b =>
                {
                    b.Property<Guid>("CandidateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("CandidateId");

                    b.HasIndex("CountryId");

                    b.HasIndex("PostId");

                    b.ToTable("Candidate");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Company", b =>
                {
                    b.Property<Guid>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CompanyId");

                    b.HasIndex("CountryId");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Country", b =>
                {
                    b.Property<Guid>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneMask")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CountryId");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Notifications", b =>
                {
                    b.Property<Guid>("NotificationsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTimeOfNotify")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsReaded")
                        .HasColumnType("bit");

                    b.Property<Guid>("RecipientUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TasksId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TextMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("NotificationsId");

                    b.HasIndex("RecipientUserId");

                    b.HasIndex("TasksId");

                    b.ToTable("Notifications", null, t =>
                        {
                            t.HasTrigger("trg_AfterUpdate_Notifications");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Post", b =>
                {
                    b.Property<Guid>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PostId");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Tasks", b =>
                {
                    b.Property<Guid>("TasksId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdditionalData")
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)");

                    b.Property<Guid>("CandidateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsArchive")
                        .HasColumnType("bit");

                    b.Property<Guid>("StatusTasksStatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TasksId");

                    b.HasIndex("CandidateId");

                    b.HasIndex("StatusTasksStatusId");

                    b.HasIndex("UserId");

                    b.ToTable("Tasks", null, t =>
                        {
                            t.HasTrigger("trg_AfterInsert_Tasks");

                            t.HasTrigger("trg_AfterUpdate_Tasks");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.TasksStatus", b =>
                {
                    b.Property<Guid>("TasksStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MessageTemplate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TypeStatus")
                        .HasColumnType("int");

                    b.HasKey("TasksStatusId");

                    b.ToTable("TasksStatus", null, t =>
                        {
                            t.HasTrigger("trg_AfterUpdate_TasksStatus");
                        });

                    b.HasAnnotation("SqlServer:UseSqlOutputClause", false);
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImgUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Patronymic")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("PostId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Vacancy", b =>
                {
                    b.Property<Guid>("VacancyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AdditionalData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isClose")
                        .HasColumnType("bit");

                    b.HasKey("VacancyId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Vacancy");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Candidate", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Company", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Notifications", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.User", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.Tasks", "Task")
                        .WithMany()
                        .HasForeignKey("TasksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipient");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Tasks", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.Candidate", "Candidate")
                        .WithMany()
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.TasksStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusTasksStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("Status");

                    b.Navigation("User");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.User", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Vacancy", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
