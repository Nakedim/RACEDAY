namespace RaceDay.Mvc.Models
{
           public class CreateEventViewModel
        {
            // 1. The main model for the form data
            public RaceEventModel NewEvent { get; set; } = new RaceEventModel();

            // 2. The second model/data type you need (e.g., categories list)
            public List<CategoryModel> AvailableCategories { get; set; } = new List<CategoryModel>();
        }
    }

