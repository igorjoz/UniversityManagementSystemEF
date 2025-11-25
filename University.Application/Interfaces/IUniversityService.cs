using System.Collections.Generic;
using University.Domain;
using University.Application.DTOs;

namespace University.Application.Interfaces
{
    public interface IUniversityService
    {
        // Students
        Student AddStudent(Student student);
        Student GetStudent(int id);
        IEnumerable<Student> GetAllStudents();
        void UpdateStudent(Student student);
        void DeleteStudent(int id);

        // Professors
        Professor AddProfessor(Professor professor);
        Professor GetProfessor(int id);
        IEnumerable<Professor> GetAllProfessors();
        void UpdateProfessor(Professor professor);
        void DeleteProfessor(int id);

        // Courses
        Course AddCourse(Course course);
        Course GetCourse(int id);
        IEnumerable<Course> GetAllCourses();
        void UpdateCourse(Course course);
        void DeleteCourse(int id);

        // Departments
        Department AddDepartment(Department department);
        Department GetDepartment(int id);
        IEnumerable<Department> GetAllDepartments();
        void UpdateDepartment(Department department);
        void DeleteDepartment(int id);

        // MasterStudents
        MasterStudent AddMasterStudent(MasterStudent masterStudent);
        MasterStudent GetMasterStudent(int id);
        IEnumerable<MasterStudent> GetAllMasterStudents();
        void UpdateMasterStudent(MasterStudent masterStudent);
        void DeleteMasterStudent(int id);

        // Offices
        Office AddOffice(Office office);
        Office GetOffice(int id);
        IEnumerable<Office> GetAllOffices();
        void UpdateOffice(Office office);
        void DeleteOffice(int id);

        // Enrollments
        Enrollment AddEnrollment(Enrollment enrollment);
        IEnumerable<Enrollment> GetStudentEnrollments(int studentId);
        void UpdateEnrollment(Enrollment enrollment);
        void DeleteEnrollment(int studentId, int courseId);

        // Index Counters
        void AddIndexPrefix(string prefix, int initialValue);
        IndexCounter GetIndexCounter(string prefix);
        IEnumerable<IndexCounter> GetAllIndexCounters();
        void UpdateIndexCounter(IndexCounter indexCounter);
        void DeleteIndexCounter(string prefix);

        // Generator
        void SeedData(int studentsCount, int professorsCount, int coursesCount);

        // Queries
        Professor GetProfessorWithMostStudents();
        IEnumerable<CourseGpaDto> GetCourseGPAByDepartment(int departmentId);
        Student GetStudentWithHardestSchedule();
    }
}
