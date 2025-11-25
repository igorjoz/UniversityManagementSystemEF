using System.Collections.Generic;

namespace University.Domain
{
    public class Professor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UniversityIndex { get; set; } // e.g. "P101"
        public string AcademicTitle { get; set; }
        
        public Address Address { get; set; }
        
        public Office Office { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
