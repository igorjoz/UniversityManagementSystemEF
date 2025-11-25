using System.Collections.Generic;

namespace University.Domain
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public int ECTS { get; set; }

        public int? ProfessorId { get; set; }
        public Professor Professor { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Course> Prerequisites { get; set; } = new List<Course>();
        public ICollection<Course> IsPrerequisiteFor { get; set; } = new List<Course>();
    }
}
