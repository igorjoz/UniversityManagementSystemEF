namespace University.Domain
{
    public class Office
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; }
        public string Building { get; set; }
        
        public int ProfessorId { get; set; }
        public Professor Professor { get; set; }
    }
}
