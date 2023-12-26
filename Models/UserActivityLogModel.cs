namespace StudentInterestSystem.Models
{
    public class UserActivityLogModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string ActivityType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }
    }
   

}
