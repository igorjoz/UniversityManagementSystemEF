using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using University.Application.Interfaces;
using University.Domain;
using University.Application.DTOs;
using Bogus;

namespace University.Application.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityDbContext _context;

        public UniversityService(IUniversityDbContext context)
        {
            _context = context;
        }

        private string GenerateIndex(string prefix)
        {
            var counter = _context.IndexCounters.Find(prefix);
            if (counter == null)
            {
                throw new Exception($"Prefix {prefix} not found.");
            }

            int value = counter.CurrentValue;
            counter.CurrentValue++;
            return $"{prefix}{value}";
        }

        private void DecrementIndexIfLast(string prefix, string index)
        {
            var counter = _context.IndexCounters.Find(prefix);
            if (counter == null) return;

            if (int.TryParse(index.Substring(prefix.Length), out int indexValue))
            {
                if (indexValue == counter.CurrentValue - 1)
                {
                    counter.CurrentValue--;
                }
            }
        }

        public Student AddStudent(Student student)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                student.UniversityIndex = GenerateIndex("S");
                _context.Students.Add(student);
                _context.SaveChanges();
                transaction.Commit();
                return student;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Student GetStudent(int id)
        {
            return _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefault(s => s.Id == id);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return _context.Students.AsNoTracking().ToList();
        }

        public void UpdateStudent(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void DeleteStudent(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var student = _context.Students.Find(id);
                if (student != null)
                {
                    DecrementIndexIfLast("S", student.UniversityIndex);
                    _context.Students.Remove(student);
                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Professor AddProfessor(Professor professor)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                professor.UniversityIndex = GenerateIndex("P");
                _context.Professors.Add(professor);
                _context.SaveChanges();
                transaction.Commit();
                return professor;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Professor GetProfessor(int id)
        {
            return _context.Professors.Find(id);
        }

        public IEnumerable<Professor> GetAllProfessors()
        {
            return _context.Professors.AsNoTracking().ToList();
        }

        public void UpdateProfessor(Professor professor)
        {
            _context.Professors.Update(professor);
            _context.SaveChanges();
        }

        public void DeleteProfessor(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var professor = _context.Professors.Find(id);
                if (professor != null)
                {
                    DecrementIndexIfLast("P", professor.UniversityIndex);
                    _context.Professors.Remove(professor);
                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Course AddCourse(Course course)
        {
            _context.Courses.Add(course);
            _context.SaveChanges();
            return course;
        }

        public Course GetCourse(int id)
        {
            return _context.Courses.Include(c => c.Department).FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Course> GetAllCourses()
        {
            return _context.Courses.Include(c => c.Department).AsNoTracking().ToList();
        }

        public void UpdateCourse(Course course)
        {
            _context.Courses.Update(course);
            _context.SaveChanges();
        }

        public void DeleteCourse(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
        }

        public Department AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
            return department;
        }

        public Department GetDepartment(int id)
        {
            return _context.Departments.Find(id);
        }

        public IEnumerable<Department> GetAllDepartments()
        {
            return _context.Departments.AsNoTracking().ToList();
        }

        public void UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);
            _context.SaveChanges();
        }

        public void DeleteDepartment(int id)
        {
            var department = _context.Departments.Find(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
        }

        public void AddIndexPrefix(string prefix, int initialValue)
        {
            if (_context.IndexCounters.Find(prefix) == null)
            {
                _context.IndexCounters.Add(new IndexCounter { Prefix = prefix, CurrentValue = initialValue });
                _context.SaveChanges();
            }
        }

        public IndexCounter GetIndexCounter(string prefix)
        {
            return _context.IndexCounters.Find(prefix);
        }

        public IEnumerable<IndexCounter> GetAllIndexCounters()
        {
            return _context.IndexCounters.AsNoTracking().ToList();
        }

        public void UpdateIndexCounter(IndexCounter indexCounter)
        {
            _context.IndexCounters.Update(indexCounter);
            _context.SaveChanges();
        }

        public void DeleteIndexCounter(string prefix)
        {
            var counter = _context.IndexCounters.Find(prefix);
            if (counter != null)
            {
                _context.IndexCounters.Remove(counter);
                _context.SaveChanges();
            }
        }

        public void SeedData(int studentsCount, int professorsCount, int coursesCount)
        {
            AddIndexPrefix("S", 1001);
            AddIndexPrefix("P", 101);
            AddIndexPrefix("M", 2001);

            var departments = new[] { "Computer Science", "Mathematics", "Physics", "Biology" };
            foreach (var deptName in departments)
            {
                if (!_context.Departments.Any(d => d.Name == deptName))
                {
                    _context.Departments.Add(new Department { Name = deptName });
                }
            }
            _context.SaveChanges();
            var dbDepartments = _context.Departments.ToList();

            var professorFaker = new Faker<Professor>()
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.AcademicTitle, f => f.Name.JobTitle())
                .RuleFor(p => p.Address, f => new Address { Street = f.Address.StreetAddress(), City = f.Address.City(), PostalCode = f.Address.ZipCode() });

            for (int i = 0; i < professorsCount; i++)
            {
                AddProfessor(professorFaker.Generate());
            }
            var dbProfessors = _context.Professors.ToList();

            var courseFaker = new Faker<Course>()
                .RuleFor(c => c.Name, f => f.Commerce.ProductName())
                .RuleFor(c => c.CourseCode, f => f.Random.AlphaNumeric(5).ToUpper())
                .RuleFor(c => c.ECTS, f => f.Random.Int(2, 8))
                .RuleFor(c => c.DepartmentId, f => f.PickRandom(dbDepartments).Id)
                .RuleFor(c => c.ProfessorId, f => f.PickRandom(dbProfessors).Id);

            for (int i = 0; i < coursesCount; i++)
            {
                AddCourse(courseFaker.Generate());
            }
            var dbCourses = _context.Courses.ToList();

            foreach (var course in dbCourses)
            {
                if (new Random().NextDouble() > 0.7)
                {
                    var prereq = dbCourses.Where(c => c.Id != course.Id).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                    if (prereq != null)
                    {
                        var courseEntity = _context.Courses.Include(c => c.Prerequisites).First(c => c.Id == course.Id);
                        if (!courseEntity.Prerequisites.Any(p => p.Id == prereq.Id))
                        {
                             courseEntity.Prerequisites.Add(prereq);
                        }
                    }
                }
            }
            _context.SaveChanges();

            var studentFaker = new Faker<Student>()
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.StudyYear, f => f.Random.Int(1, 5))
                .RuleFor(s => s.Address, f => new Address { Street = f.Address.StreetAddress(), City = f.Address.City(), PostalCode = f.Address.ZipCode() });

            for (int i = 0; i < studentsCount; i++)
            {
                var student = studentFaker.Generate();
                AddStudent(student);

                int enrollCount = new Random().Next(1, 5);
                var selectedCourses = new HashSet<int>();
                for (int j = 0; j < enrollCount; j++)
                {
                    var course = dbCourses[new Random().Next(dbCourses.Count)];
                    
                    if (selectedCourses.Contains(course.Id)) continue;

                    if (!_context.Enrollments.Any(e => e.StudentId == student.Id && e.CourseId == course.Id))
                    {
                        _context.Enrollments.Add(new Enrollment
                        {
                            StudentId = student.Id,
                            CourseId = course.Id,
                            Semester = 1,
                            Grade = new Random().NextDouble() > 0.1 ? new Random().Next(2, 6) : (double?)null
                        });
                        selectedCourses.Add(course.Id);
                    }
                }
            }
            _context.SaveChanges();
        }

        public Professor GetProfessorWithMostStudents()
        {
            return _context.Professors
                .AsNoTracking()
                .Select(p => new 
                { 
                    Professor = p, 
                    StudentCount = p.Courses.SelectMany(c => c.Enrollments).Count() 
                })
                .OrderByDescending(x => x.StudentCount)
                .Select(x => x.Professor)
                .FirstOrDefault();
        }

        public IEnumerable<CourseGpaDto> GetCourseGPAByDepartment(int departmentId)
        {
            return _context.Courses
                .AsNoTracking()
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new CourseGpaDto
                {
                    CourseName = c.Name,
                    GPA = c.Enrollments.Any(e => e.Grade.HasValue) ? (double)c.Enrollments.Where(e => e.Grade.HasValue).Average(e => e.Grade) : 0,
                    GradedStudentsCount = c.Enrollments.Count(e => e.Grade.HasValue)
                })
                .ToList();
        }

        public Student GetStudentWithHardestSchedule()
        {
            var studentStats = _context.Students
                .AsNoTracking()
                .Select(s => new
                {
                    Student = s,
                    TotalECTS = s.Enrollments.Sum(e => e.Course.ECTS) + 
                                s.Enrollments.SelectMany(e => e.Course.Prerequisites).Select(p => new { p.Id, p.ECTS }).Distinct().Sum(x => x.ECTS)
                })
                .OrderByDescending(x => x.TotalECTS)
                .FirstOrDefault();

            return studentStats?.Student;
        }

        public MasterStudent AddMasterStudent(MasterStudent masterStudent)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                masterStudent.UniversityIndex = GenerateIndex("M");
                _context.MasterStudents.Add(masterStudent);
                _context.SaveChanges();
                transaction.Commit();
                return masterStudent;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public MasterStudent GetMasterStudent(int id)
        {
            return _context.MasterStudents
                .Include(ms => ms.Promoter)
                .Include(ms => ms.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefault(ms => ms.Id == id);
        }

        public IEnumerable<MasterStudent> GetAllMasterStudents()
        {
            return _context.MasterStudents
                .Include(ms => ms.Promoter)
                .AsNoTracking()
                .ToList();
        }

        public void UpdateMasterStudent(MasterStudent masterStudent)
        {
            _context.MasterStudents.Update(masterStudent);
            _context.SaveChanges();
        }

        public void DeleteMasterStudent(int id)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var masterStudent = _context.MasterStudents.Find(id);
                if (masterStudent != null)
                {
                    DecrementIndexIfLast("M", masterStudent.UniversityIndex);
                    _context.MasterStudents.Remove(masterStudent);
                    _context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Office AddOffice(Office office)
        {
            _context.Offices.Add(office);
            _context.SaveChanges();
            return office;
        }

        public Office GetOffice(int id)
        {
            return _context.Offices.Include(o => o.Professor).FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Office> GetAllOffices()
        {
            return _context.Offices.Include(o => o.Professor).AsNoTracking().ToList();
        }

        public void UpdateOffice(Office office)
        {
            _context.Offices.Update(office);
            _context.SaveChanges();
        }

        public void DeleteOffice(int id)
        {
            var office = _context.Offices.Find(id);
            if (office != null)
            {
                _context.Offices.Remove(office);
                _context.SaveChanges();
            }
        }

        public Enrollment AddEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            _context.SaveChanges();
            return enrollment;
        }

        public IEnumerable<Enrollment> GetStudentEnrollments(int studentId)
        {
            return _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == studentId)
                .AsNoTracking()
                .ToList();
        }

        public void UpdateEnrollment(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            _context.SaveChanges();
        }

        public void DeleteEnrollment(int studentId, int courseId)
        {
            var enrollment = _context.Enrollments.Find(studentId, courseId);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                _context.SaveChanges();
            }
        }
    }
}
