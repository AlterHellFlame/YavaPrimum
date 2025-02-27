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
    [Migration("20250103111748_ImageRemane")]
    partial class ImageRemane
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
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("HRUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("InterviewStatus")
                        .HasColumnType("int");

                    b.Property<Guid?>("OPUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SecondName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SurName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telephone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CandidateId");

                    b.HasIndex("CountryId");

                    b.HasIndex("HRUserId");

                    b.HasIndex("OPUserId");

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

                    b.HasKey("CountryId");

                    b.ToTable("Country");
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

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.TaskType", b =>
                {
                    b.Property<Guid>("TaskTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaskTypeId");

                    b.ToTable("TaskType");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Tasks", b =>
                {
                    b.Property<Guid>("TasksId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CandidateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<Guid>("TaskTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TasksId");

                    b.HasIndex("CandidateId");

                    b.HasIndex("TaskTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImgUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SecondName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SurName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserRegisterInfoId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserRegisterInfoId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.UserRegisterInfo", b =>
                {
                    b.Property<Guid>("UserRegisterInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserRegisterInfoId");

                    b.ToTable("UserRegisterInfo");
                });

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Candidate", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.User", "HR")
                        .WithMany()
                        .HasForeignKey("HRUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.User", "OP")
                        .WithMany()
                        .HasForeignKey("OPUserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("YavaPrimum.Core.DataBase.Models.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");

                    b.Navigation("HR");

                    b.Navigation("OP");

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

            modelBuilder.Entity("YavaPrimum.Core.DataBase.Models.Tasks", b =>
                {
                    b.HasOne("YavaPrimum.Core.DataBase.Models.Candidate", "Candidate")
                        .WithMany()
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.TaskType", "TaskType")
                        .WithMany()
                        .HasForeignKey("TaskTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YavaPrimum.Core.DataBase.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("TaskType");

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

                    b.HasOne("YavaPrimum.Core.DataBase.Models.UserRegisterInfo", "UserRegisterInfo")
                        .WithMany()
                        .HasForeignKey("UserRegisterInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");

                    b.Navigation("Post");

                    b.Navigation("UserRegisterInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
