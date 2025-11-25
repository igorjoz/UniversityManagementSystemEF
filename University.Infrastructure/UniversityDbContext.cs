using Microsoft.EntityFrameworkCore;
using University.Domain;
using University.Application.Interfaces;

namespace University.Infrastructure
{
    public class UniversityDbContext : DbContext, IUniversityDbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<MasterStudent> MasterStudents { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<IndexCounter> IndexCounters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Address (Owned Entity)
            modelBuilder.Entity<Student>().OwnsOne(s => s.Address);
            modelBuilder.Entity<Professor>().OwnsOne(p => p.Address);

            // IndexCounter
            modelBuilder.Entity<IndexCounter>().HasKey(ic => ic.Prefix);

            // Unique Indexes
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.UniversityIndex)
                .IsUnique();

            modelBuilder.Entity<Professor>()
                .HasIndex(p => p.UniversityIndex)
                .IsUnique();

            // Enrollment (Many-to-Many)
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            // Prerequisites (Self-referencing Many-to-Many)
            modelBuilder.Entity<Course>()
                .HasMany(c => c.Prerequisites)
                .WithMany(c => c.IsPrerequisiteFor)
                .UsingEntity(j => j.ToTable("CoursePrerequisites"));

            // Office (One-to-One)
            modelBuilder.Entity<Professor>()
                .HasOne(p => p.Office)
                .WithOne(o => o.Professor)
                .HasForeignKey<Office>(o => o.ProfessorId);

            // MasterStudent Inheritance
            modelBuilder.Entity<MasterStudent>()
                .HasBaseType<Student>();

            // MasterStudent -> Promoter (One-to-Many, SetNull on delete)
            modelBuilder.Entity<MasterStudent>()
                .HasOne(ms => ms.Promoter)
                .WithMany() 
                .HasForeignKey(ms => ms.PromoterId)
                .OnDelete(DeleteBehavior.SetNull);
                
            // Course -> Professor (One-to-Many)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Professor)
                .WithMany(p => p.Courses)
                .HasForeignKey(c => c.ProfessorId);

            // Course -> Department (One-to-Many)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId);
        }
    }
}
