namespace Mango.Services.AuthAPI.Models.Dto
{
    public class AssignRoleRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
