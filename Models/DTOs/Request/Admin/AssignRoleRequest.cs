using Online_Learning.Constants.Enums;
using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.Admin
{
    public class AssignRoleRequest
    {
        [Required(ErrorMessage = "Danh sách vai trò là bắt buộc")]
        public List<UserRole> Roles { get; set; } = new List<UserRole>();
    }
}
