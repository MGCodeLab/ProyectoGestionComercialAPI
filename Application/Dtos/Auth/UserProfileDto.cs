namespace Application.Dtos.Auth
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<PermissionDto> Permisos { get; set; } = new();
    }
}
