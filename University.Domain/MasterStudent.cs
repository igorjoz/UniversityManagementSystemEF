namespace University.Domain
{
    public class MasterStudent : Student
    {
        public string ThesisTopic { get; set; }
        public int? PromoterId { get; set; }
        public Professor Promoter { get; set; }
    }
}
