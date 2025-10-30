namespace projectMvc.Models.models
{
    public class DailyMenuItemStats
    {
        public int Id { get; set; }

        public int ManuItemId { get; set; }
        public  ManuItem MenuItem { get; set; }

        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
    }
}
