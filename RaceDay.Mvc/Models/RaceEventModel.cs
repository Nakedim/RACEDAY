namespace RaceDay.Mvc.Models
{
    public class RaceEventModel
    {
        public int EventID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public string EventType { get; set; } = string.Empty;

        public int OrganiserID { get; set; }

    }
}
