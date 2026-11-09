namespace RaceDay.Mvc.Models
{
    public class LoginResultModel
    {
        
        public UserModel? User { get; set; }
        public string? Token { get; set; }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}

