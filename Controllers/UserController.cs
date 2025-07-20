using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_Learning.Attributes;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.Common;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Constants.Enums;
using Online_Learning.Models.DTOs.Request.Admin;
using Online_Learning.Models.DTOs.Response.Admin;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [DynamicAuthorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách người dùng với phân trang, tìm kiếm và lọc
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<AdminApiResponse<PagedResponse<UserResponse>>>> GetUsers([FromQuery] UserFilterRequest request)
        {
            var result = await _userService.GetUsersAsync(request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Lấy thông tin chi tiết một người dùng
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminApiResponse<UserResponse>>> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(AdminApiResponse<UserResponse>.ErrorResult("ID người dùng không hợp lệ"));

            var result = await _userService.GetUserByIdAsync(id);

            if (result.Success)
                return Ok(result);

            if (result.Message.Contains("Không tìm thấy"))
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Tạo mới tài khoản người dùng (admin thêm thủ công)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AdminApiResponse<UserResponse>>> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(AdminApiResponse<UserResponse>.ErrorResult("Dữ liệu không hợp lệ", errors));
            }

            var result = await _userService.CreateUserAsync(request);

            if (result.Success)
                return CreatedAtAction(nameof(GetUser), new { id = result.Data!.UserId }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AdminApiResponse<UserResponse>>> UpdateUser(string id, [FromBody] UpdateUserRequest request)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(AdminApiResponse<UserResponse>.ErrorResult("ID người dùng không hợp lệ"));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(AdminApiResponse<UserResponse>.ErrorResult("Dữ liệu không hợp lệ", errors));
            }

            var result = await _userService.UpdateUserAsync(id, request);

            if (result.Success)
                return Ok(result);

            if (result.Message.Contains("Không tìm thấy"))
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Xóa mềm tài khoản người dùng
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<AdminApiResponse<bool>>> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(AdminApiResponse<bool>.ErrorResult("ID người dùng không hợp lệ"));

            var result = await _userService.DeleteUserAsync(id);

            if (result.Success)
                return Ok(result);

            if (result.Message.Contains("Không tìm thấy"))
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Khoá / mở khoá tài khoản
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<ActionResult<AdminApiResponse<bool>>> ToggleUserStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(AdminApiResponse<bool>.ErrorResult("ID người dùng không hợp lệ"));

            var result = await _userService.ToggleUserStatusAsync(id);

            if (result.Success)
                return Ok(result);

            if (result.Message.Contains("Không tìm thấy"))
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Reset mật khẩu người dùng
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/reset-password")]
        public async Task<ActionResult<AdminApiResponse<bool>>> ResetPassword(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(AdminApiResponse<bool>.ErrorResult("ID người dùng không hợp lệ"));

            var result = await _userService.ResetPasswordAsync(id);

            if (result.Success)
                return Ok(result);

            if (result.Message.Contains("Không tìm thấy"))
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Cập nhật quyền / vai trò người dùng
        /// </summary>
        [HttpPatch("{id}/assign-role")]
        public async Task<ActionResult<AdminApiResponse<bool>>> AssignRole(string id, [FromBody] AssignRoleRequest request)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(AdminApiResponse<bool>.ErrorResult("ID người dùng không hợp lệ"));

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(AdminApiResponse<bool>.ErrorResult("Dữ liệu không hợp lệ", errors));
            }

            var result = await _userService.AssignRoleAsync(id, request);

            if (result.Success)
                return Ok(result);

            if (result.Message.Contains("Không tìm thấy"))
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Xuất danh sách người dùng ra Excel
        /// </summary>
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel([FromQuery] UserFilterRequest request)
        {
            try
            {
                var excelData = await _userService.ExportUsersToExcelAsync(request);
                var fileName = $"Users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(AdminApiResponse<bool>.ErrorResult($"Lỗi khi xuất Excel: {ex.Message}"));
            }
        }

        /// <summary>
        /// Xuất danh sách người dùng ra PDF
        /// </summary>
        [HttpGet("export/pdf")]
        
        public async Task<IActionResult> ExportToPdf([FromQuery] UserFilterRequest request)
        {
            try
            {
                var pdfData = await _userService.ExportUsersToPdfAsync(request);
                var fileName = $"Users_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                return File(pdfData, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(AdminApiResponse<bool>.ErrorResult($"Lỗi khi xuất PDF: {ex.Message}"));
            }
        }

        /// <summary>
        /// Lấy thống kê tổng quan về người dùng
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<AdminApiResponse<UserStatisticsResponse>>> GetUserStatistics()
        {
            try
        {
                // Get all users for statistics
                var allUsersRequest = new UserFilterRequest { PageSize = int.MaxValue, PageNumber = 1 };
                var allUsersResult = await _userService.GetUsersAsync(allUsersRequest);

                if (!allUsersResult.Success)
                    return BadRequest(allUsersResult);

                var users = allUsersResult.Data!.Data;

                var statistics = new UserStatisticsResponse
                {
                    TotalUsers = users.Count,
                    ActiveUsers = users.Count(u => u.Status == UserStatus.Active),
                    InactiveUsers = users.Count(u => u.Status == UserStatus.Inactive),
                    BannedUsers = users.Count(u => u.Status == UserStatus.Banned),
                    PendingUsers = users.Count(u => u.Status == UserStatus.Pending),
                    AdminUsers = users.Count(u => u.Roles.Contains(UserRole.Admin)),
                    StudentUsers = users.Count(u => u.Roles.Contains(UserRole.Student)),
                    NewUsersThisMonth = users.Count(u => u.CreatedAt >= DateTime.Now.AddMonths(-1)),
                    TotalCoursesEnrolled = users.Sum(u => u.TotalCourses),
                    TotalCoursesCompleted = users.Sum(u => u.CompletedCourses)
                };

                return Ok(AdminApiResponse<UserStatisticsResponse>.SuccessResult(statistics));
            }
            catch (Exception ex)
            {
                return BadRequest(AdminApiResponse<UserStatisticsResponse>.ErrorResult($"Lỗi khi lấy thống kê: {ex.Message}"));
            }
        }

        [HttpGet("Profile")]
        public IActionResult GetUserProfile()
        {
            string msg = _userService.GetUserProfile(User, out UserProfileDto userProfile);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(userProfile));
        }

        [HttpPut("UpdateProfile")]
        public IActionResult UpdateUserProfile(UpdateProfileRequestDto request)
        {
            string msg = _userService.UpdateUserProfile(User, request);
            if (msg.Length > 0) return BadRequest(ApiResponse<string>.ErrorResponse(msg));

            return Ok(ApiResponse<string>.SuccessResponse("", "Profile updated successfully."));
        }
}
