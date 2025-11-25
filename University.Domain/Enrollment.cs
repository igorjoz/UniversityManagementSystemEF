namespace University.Domain
{
    public class Enrollment
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public double? Grade { get; set; }
        public int Semester { get; set; }
    }
}
