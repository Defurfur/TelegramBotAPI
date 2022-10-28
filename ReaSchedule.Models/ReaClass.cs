namespace ReaSchedule.Models
{
    public class ReaClass : IIdentifyable<int>
    {
        public int Id { get; set; }
        public int ScheduleDayId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassType { get; set; } = string.Empty;
        public string OrdinalNumber { get; set; } = string.Empty;
        public string Audition { get; set; } = string.Empty;
        public string Professor { get; set; } = string.Empty;
        public string Subgroup { get; set; } = string.Empty;


    }
}
  


