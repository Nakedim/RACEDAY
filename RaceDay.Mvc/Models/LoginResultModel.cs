namespace RaceDay.Mvc.Models
{
    public class LoginResultModel
    {
        public string? Message { get; set; }
        public UserModel? User { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? Role { get; set; }
    }
}
