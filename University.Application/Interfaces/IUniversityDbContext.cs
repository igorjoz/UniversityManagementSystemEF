using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using University.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace University.Application.Interfaces
{
    public interface IUniversityDbContext
    {
        DbSet<Student> Students { get; set; }
        DbSet<Professor> Professors { get; set; }
        DbSet<MasterStudent> MasterStudents { get; set; }
        DbSet<Course> Courses { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<Enrollment> Enrollments { get; set; }
        DbSet<Office> Offices { get; set; }
        DbSet<IndexCounter> IndexCounters { get; set; }
        
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }
    }
}
