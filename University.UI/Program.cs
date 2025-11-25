using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using University.Application.Interfaces;
using University.Application.Services;
using University.Application.DTOs;
using University.Domain;
using University.Infrastructure;

namespace University.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = ConfigureServices();
            var service = serviceProvider.GetRequiredService<IUniversityService>();
            
            // Apply migrations
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
                // context.Database.EnsureDeleted(); // Optional: for clean start
                context.Database.Migrate();
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("University Management System");
                Console.WriteLine("1. Seed Data");
                Console.WriteLine("2. List Students");
                Console.WriteLine("3. List Professors");
                Console.WriteLine("4. List Index Counters");
                Console.WriteLine("5. Delete Student (Test Rollback)");
                Console.WriteLine("6. Query: Professor with most students");
                Console.WriteLine("7. Query: Course GPA by Department");
                Console.WriteLine("8. Query: Student with hardest schedule");
                Console.WriteLine("0. Exit");
                Console.Write("Select option: ");

                var key = Console.ReadLine();

                switch (key)
                {
                    case "1":
                        Console.WriteLine("Seeding data...");
                        service.SeedData(50, 10, 20);
                        Console.WriteLine("Data seeded.");
                        break;
                    case "2":
                        var students = service.GetAllStudents();
                        foreach (var s in students)
                        {
                            Console.WriteLine($"ID: {s.Id}, Index: {s.UniversityIndex}, Name: {s.FirstName} {s.LastName}, City: {s.Address?.City}");
                        }
                        break;
                    case "3":
                        var professors = service.GetAllProfessors();
                        foreach (var p in professors)
                        {
                            Console.WriteLine($"ID: {p.Id}, Index: {p.UniversityIndex}, Name: {p.FirstName} {p.LastName} ({p.AcademicTitle})");
                        }
                        break;
                    case "4":
                        var counters = service.GetAllIndexCounters();
                        foreach (var c in counters)
                        {
                            Console.WriteLine($"Prefix: {c.Prefix}, Next Value: {c.CurrentValue}");
                        }
                        break;
                    case "5":
                        Console.Write("Enter Student ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int id))
                        {
                            try
                            {
                                service.DeleteStudent(id);
                                Console.WriteLine("Student deleted.");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                        }
                        break;
                    case "6":
                        var prof = service.GetProfessorWithMostStudents();
                        if (prof != null)
                            Console.WriteLine($"Professor: {prof.FirstName} {prof.LastName}");
                        else
                            Console.WriteLine("No professor found.");
                        break;
                    case "7":
                        var depts = service.GetAllDepartments();
                        Console.WriteLine("Departments:");
                        foreach (var d in depts) Console.WriteLine($"{d.Id}: {d.Name}");
                        Console.Write("Enter Department ID: ");
                        if (int.TryParse(Console.ReadLine(), out int deptId))
                        {
                            var results = service.GetCourseGPAByDepartment(deptId);
                            foreach (CourseGpaDto r in results)
                            {
                                Console.WriteLine($"Course: {r.CourseName}, GPA: {r.GPA:F2}, Graded Students: {r.GradedStudentsCount}");
                            }
                        }
                        break;
                    case "8":
                        var hardStudent = service.GetStudentWithHardestSchedule();
                        if (hardStudent != null)
                            Console.WriteLine($"Student: {hardStudent.FirstName} {hardStudent.LastName} ({hardStudent.UniversityIndex})");
                        else
                            Console.WriteLine("No student found.");
                        break;
                    case "0":
                        return;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<UniversityDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=UniversityDB;Trusted_Connection=True;MultipleActiveResultSets=true"));

            services.AddScoped<IUniversityDbContext>(provider => provider.GetService<UniversityDbContext>());
            services.AddScoped<IUniversityService, UniversityService>();

            return services.BuildServiceProvider();
        }
    }
}
