using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LMS.Models.LMSModels
{
    public partial class Team91LMSContext : DbContext
    {
        public Team91LMSContext()
        {
        }

        public Team91LMSContext(DbContextOptions<Team91LMSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Administrators> Administrators { get; set; }
        public virtual DbSet<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual DbSet<Assignments> Assignments { get; set; }
        public virtual DbSet<Classes> Classes { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<Departments> Departments { get; set; }
        public virtual DbSet<EnrolledIn> EnrolledIn { get; set; }
        public virtual DbSet<Professors> Professors { get; set; }
        public virtual DbSet<Students> Students { get; set; }
        public virtual DbSet<Submissions> Submissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=SERVERNAME;User Id=IDNAME;Password=PASSWORD;Database=DATABASENAME");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrators>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<AssignmentCategories>(entity =>
            {
                entity.HasKey(e => e.Acid)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassID");

                entity.HasIndex(e => new { e.AssignmentCategoryName, e.ClassId })
                    .HasName("AssignmentCategoryName")
                    .IsUnique();

                entity.Property(e => e.Acid).HasColumnName("ACID");

                entity.Property(e => e.AssignmentCategoryName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.AssignmentCategories)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssignmentCategories_ibfk_1");
            });

            modelBuilder.Entity<Assignments>(entity =>
            {
                entity.HasKey(e => e.Aid)
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.Acid, e.AssignmentName })
                    .HasName("ACID")
                    .IsUnique();

                entity.Property(e => e.Aid).HasColumnName("AID");

                entity.Property(e => e.Acid).HasColumnName("ACID");

                entity.Property(e => e.AssignmentContents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.AssignmentName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.HasOne(d => d.Ac)
                    .WithMany(p => p.Assignments)
                    .HasForeignKey(d => d.Acid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Assignments_ibfk_1");
            });

            modelBuilder.Entity<Classes>(entity =>
            {
                entity.HasKey(e => e.ClassId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CourseId)
                    .HasName("CourseID");

                entity.HasIndex(e => e.UId)
                    .HasName("uID");

                entity.HasIndex(e => new { e.SemesterYear, e.SemesterSeason, e.CourseId })
                    .HasName("SemesterYear")
                    .IsUnique();

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.SemesterSeason)
                    .IsRequired()
                    .HasColumnType("varchar(6)");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.UId)
                    .IsRequired()
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_2");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Classes)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Classes_ibfk_1");
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CourseId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.SubjectAbv)
                    .HasName("SubjectABV");

                entity.HasIndex(e => new { e.CourseNumber, e.SubjectAbv })
                    .HasName("CourseNumber")
                    .IsUnique();

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.CourseName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.CourseNumber)
                    .IsRequired()
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.SubjectAbv)
                    .IsRequired()
                    .HasColumnName("SubjectABV")
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.SubjectAbvNavigation)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.SubjectAbv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Courses_ibfk_1");
            });

            modelBuilder.Entity<Departments>(entity =>
            {
                entity.HasKey(e => e.SubjectAbv)
                    .HasName("PRIMARY");

                entity.Property(e => e.SubjectAbv)
                    .HasColumnName("SubjectABV")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.DepartmentName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });

            modelBuilder.Entity<EnrolledIn>(entity =>
            {
                entity.HasKey(e => new { e.ClassId, e.UId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.UId)
                    .HasName("uID");

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Grade)
                    .IsRequired()
                    .HasColumnType("varchar(2)");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.EnrolledIn)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EnrolledIn_ibfk_1");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.EnrolledIn)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EnrolledIn_ibfk_2");
            });

            modelBuilder.Entity<Professors>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.SubjectAbv)
                    .HasName("SubjectABV");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.SubjectAbv)
                    .IsRequired()
                    .HasColumnName("SubjectABV")
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.SubjectAbvNavigation)
                    .WithMany(p => p.Professors)
                    .HasForeignKey(d => d.SubjectAbv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Professors_ibfk_1");
            });

            modelBuilder.Entity<Students>(entity =>
            {
                entity.HasKey(e => e.UId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.SubjectAbv)
                    .HasName("SubjectABV");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.SubjectAbv)
                    .IsRequired()
                    .HasColumnName("SubjectABV")
                    .HasColumnType("varchar(4)");

                entity.HasOne(d => d.SubjectAbvNavigation)
                    .WithMany(p => p.Students)
                    .HasForeignKey(d => d.SubjectAbv)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Students_ibfk_1");
            });

            modelBuilder.Entity<Submissions>(entity =>
            {
                entity.HasKey(e => new { e.UId, e.Aid })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Aid)
                    .HasName("AID");

                entity.Property(e => e.UId)
                    .HasColumnName("uID")
                    .HasColumnType("char(8)");

                entity.Property(e => e.Aid).HasColumnName("AID");

                entity.Property(e => e.SubmissionContents)
                    .IsRequired()
                    .HasColumnType("varchar(8192)");

                entity.Property(e => e.SubmissionTime).HasColumnType("datetime");

                entity.HasOne(d => d.A)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.Aid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_2");

                entity.HasOne(d => d.U)
                    .WithMany(p => p.Submissions)
                    .HasForeignKey(d => d.UId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Submissions_ibfk_1");
            });
        }
    }
}
