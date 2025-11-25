using System.Collections.Generic;

namespace University.Domain
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UniversityIndex { get; set; } // e.g. "S1001"
        public int StudyYear { get; set; }
        
        public Address Address { get; set; }
        
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
