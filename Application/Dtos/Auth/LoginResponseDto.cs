namespace Application.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public UserProfileDto User { get; set; } = new();
    }
}
