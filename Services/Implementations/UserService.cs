using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using Online_Learning.Constants.Enums;
using Online_Learning.Models.DTOs.Request.Admin;
using Online_Learning.Models.DTOs.Response.Admin;
using Online_Learning.Models.Entities;
using Online_Learning.Models.DTOs.Request.User;
using Online_Learning.Models.DTOs.Response.User;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Interfaces;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Online_Learning.Constants;

namespace Online_Learning.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<AdminApiResponse<PagedResponse<UserResponse>>> GetUsersAsync(UserFilterRequest request)
        {
            try
            {
                var (users, totalCount) = await _userRepository.GetUsersAsync(request);

                var userResponses = users.Select(MapToUserResponse).ToList();

                var pagedResponse = new PagedResponse<UserResponse>
                {
                    Data = userResponses,
                    TotalRecords = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize),
                    HasNextPage = request.PageNumber < Math.Ceiling((double)totalCount / request.PageSize),
                    HasPreviousPage = request.PageNumber > 1
                };

                return AdminApiResponse<PagedResponse<UserResponse>>.SuccessResult(pagedResponse);
            }
            catch (Exception ex)
            {
                return AdminApiResponse<PagedResponse<UserResponse>>.ErrorResult(string.Format(Messages.ErrorGettingUserList, ex.Message));
            }
        }

        public async Task<AdminApiResponse<UserResponse>> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                    return AdminApiResponse<UserResponse>.ErrorResult(Messages.UserNotFoundVi);
            }

                var userResponse = MapToUserResponse(user);
                return AdminApiResponse<UserResponse>.SuccessResult(userResponse);
            }
            catch (Exception ex)
            {
                return AdminApiResponse<UserResponse>.ErrorResult(string.Format(Messages.ErrorGettingUserInfo, ex.Message));
            }
        }

        public async Task<AdminApiResponse<UserResponse>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(request.Email))
                {
                    return AdminApiResponse<UserResponse>.ErrorResult(Messages.EmailExistsVi);
                }

                var user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = request.Email,
                    FullName = request.FullName,
                    DoB = request.DoB,
                    Gender = request.Gender,
                    Phone = request.Phone,
                    Address = request.Address,
                    AvatarUrl = request.AvatarUrl,
                    Status = (int)request.Status,
                    CreatedAt = DateTime.UtcNow
            };

                // Hash password
                user.Password = _passwordHasher.HashPassword(user, request.Password);

                var createdUser = await _userRepository.CreateUserAsync(user);

                // Assign roles
                if (request.Roles.Any())
                {
                    await _userRepository.UpdateUserRolesAsync(createdUser.UserId, request.Roles);
        }

                // Get updated user with roles
                var userWithRoles = await _userRepository.GetUserByIdAsync(createdUser.UserId);
                var userResponse = MapToUserResponse(userWithRoles!);

                return AdminApiResponse<UserResponse>.SuccessResult(userResponse, Messages.CreateUserSuccess);
            }
            catch (Exception ex)
        {
                return AdminApiResponse<UserResponse>.ErrorResult(string.Format(Messages.ErrorCreatingUser, ex.Message));
            }
        }

        public async Task<AdminApiResponse<UserResponse>> UpdateUserAsync(string userId, UpdateUserRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                    return AdminApiResponse<UserResponse>.ErrorResult(Messages.UserNotFoundVi);
            }

                // Update user properties
            user.FullName = request.FullName;
            user.DoB = request.DoB;
            user.Gender = request.Gender;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.AvatarUrl = request.AvatarUrl;

                var updatedUser = await _userRepository.UpdateUserAsync(user);
                var userResponse = MapToUserResponse(updatedUser);

                return AdminApiResponse<UserResponse>.SuccessResult(userResponse, Messages.UpdateUserSuccess);
            }
            catch (Exception ex)
            {
                return AdminApiResponse<UserResponse>.ErrorResult(string.Format(Messages.ErrorUpdatingUser, ex.Message));
            }
        }

        public async Task<AdminApiResponse<bool>> DeleteUserAsync(string userId)
        {
            try
            {
                var result = await _userRepository.DeleteUserAsync(userId);
                if (!result)
                {
                    return AdminApiResponse<bool>.ErrorResult(Messages.UserNotFoundVi);
                }

                return AdminApiResponse<bool>.SuccessResult(true, Messages.DeleteUserSuccess);
            }
            catch (Exception ex)
            {
                return AdminApiResponse<bool>.ErrorResult(string.Format(Messages.ErrorDeletingUser, ex.Message));
            }
        }

        public async Task<AdminApiResponse<bool>> ToggleUserStatusAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return AdminApiResponse<bool>.ErrorResult(Messages.UserNotFoundVi);
                }

                // Toggle between Active and Inactive
                user.Status = user.Status == (int)UserStatus.Active ? (int)UserStatus.Inactive : (int)UserStatus.Active;

                await _userRepository.UpdateUserAsync(user);

                var statusText = user.Status == (int)UserStatus.Active ? "kích hoạt" : "vô hiệu hóa";
                return AdminApiResponse<bool>.SuccessResult(true, string.Format(Messages.StatusChangeSuccess, statusText));
            }
            catch (Exception ex)
            {
                return AdminApiResponse<bool>.ErrorResult(string.Format(Messages.ErrorChangingStatus, ex.Message));
            }
        }

        public async Task<AdminApiResponse<bool>> ResetPasswordAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return AdminApiResponse<bool>.ErrorResult(Messages.UserNotFoundVi);
                }

                // Generate new password (you might want to send this via email)
                var newPassword = GenerateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, newPassword);

                await _userRepository.UpdateUserAsync(user);

                // In real application, you would send the new password via email
                // For now, we'll just return success
                return AdminApiResponse<bool>.SuccessResult(true, string.Format(Messages.ResetPasswordSuccess, newPassword));
            }
            catch (Exception ex)
            {
                return AdminApiResponse<bool>.ErrorResult(string.Format(Messages.ErrorResettingPassword, ex.Message));
            }
        }

        public async Task<AdminApiResponse<bool>> AssignRoleAsync(string userId, AssignRoleRequest request)
        {
            try
            {
                if (!await _userRepository.UserExistsAsync(userId))
                {
                    return AdminApiResponse<bool>.ErrorResult(Messages.UserNotFoundVi);
                }

                var result = await _userRepository.UpdateUserRolesAsync(userId, request.Roles);
                if (!result)
                {
                    return AdminApiResponse<bool>.ErrorResult(Messages.CannotUpdateRole);
                }

                return AdminApiResponse<bool>.SuccessResult(true, Messages.UpdateRoleSuccess);
            }
            catch (Exception ex)
            {
                return AdminApiResponse<bool>.ErrorResult(string.Format(Messages.ErrorUpdatingRole, ex.Message));
            }
        }

        public async Task<byte[]> ExportUsersToExcelAsync(UserFilterRequest request)
        {
            request.PageSize = int.MaxValue;
            request.PageNumber = 1;

            var (users, _) = await _userRepository.GetUsersAsync(request);

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users");

            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Email";
            worksheet.Cells[1, 3].Value = "Họ tên";
            worksheet.Cells[1, 4].Value = "Ngày sinh";
            worksheet.Cells[1, 5].Value = "Giới tính";
            worksheet.Cells[1, 6].Value = "Điện thoại";
            worksheet.Cells[1, 7].Value = "Địa chỉ";
            worksheet.Cells[1, 8].Value = "Trạng thái";
            worksheet.Cells[1, 9].Value = "Ngày tạo";
            worksheet.Cells[1, 10].Value = "Vai trò";

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var row = i + 2;

                worksheet.Cells[row, 1].Value = user.UserId;
                worksheet.Cells[row, 2].Value = user.Email;
                worksheet.Cells[row, 3].Value = user.FullName;
                worksheet.Cells[row, 4].Value = user.DoB?.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 5].Value = user.Gender == true ? "Nam" : user.Gender == false ? "Nữ" : "";
                worksheet.Cells[row, 6].Value = user.Phone;
                worksheet.Cells[row, 7].Value = user.Address;
                worksheet.Cells[row, 8].Value = GetStatusText((UserStatus)user.Status!);
                worksheet.Cells[row, 9].Value = user.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                worksheet.Cells[row, 10].Value = string.Join(", ", user.Roles.Select(r => GetRoleText((UserRole)r.RoleId)));
            }

            return package.GetAsByteArray();
        }

        public async Task<byte[]> ExportUsersToPdfAsync(UserFilterRequest request)
        {
            request.PageSize = int.MaxValue;
            request.PageNumber = 1;

            var (users, _) = await _userRepository.GetUsersAsync(request);

            using var stream = new MemoryStream();
            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new iText.Layout.Document(pdf);

            var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var title = new Paragraph("Danh sách người dùng")
                .SetFont(titleFont)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER);
            document.Add(title);
            document.Add(new Paragraph(" "));

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 15, 20, 15, 10, 10, 15, 10, 15 }))
                .UseAllAvailableWidth();

            var headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            var headers = new[] { "Email", "Họ tên", "Điện thoại", "Ngày sinh", "Giới tính", "Địa chỉ", "Trạng thái", "Vai trò" };
            foreach (var header in headers)
            {
                table.AddHeaderCell(new Cell().Add(new Paragraph(header).SetFont(headerFont).SetFontSize(10)));
            }

            var dataFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            foreach (var user in users)
            {
                table.AddCell(new Cell().Add(new Paragraph(user.Email).SetFont(dataFont).SetFontSize(9)));
                table.AddCell(new Cell().Add(new Paragraph(user.FullName).SetFont(dataFont).SetFontSize(9)));
                table.AddCell(new Cell().Add(new Paragraph(user.Phone ?? "").SetFont(dataFont).SetFontSize(9)));
                table.AddCell(new Cell().Add(new Paragraph(user.DoB?.ToString("dd/MM/yyyy") ?? "").SetFont(dataFont).SetFontSize(9)));
                table.AddCell(new Cell().Add(new Paragraph(user.Gender == true ? "Nam" : user.Gender == false ? "Nữ" : "").SetFont(dataFont).SetFontSize(9)));
                table.AddCell(new Cell().Add(new Paragraph(user.Address ?? "").SetFont(dataFont).SetFontSize(9)));
                table.AddCell(new Cell().Add(new Paragraph(GetStatusText((UserStatus)user.Status!)).SetFont(dataFont).SetFontSize(9)));
                var roleText = string.Join(", ", user.Roles.Select(r => GetRoleText((UserRole)r.RoleId)));
                table.AddCell(new Cell().Add(new Paragraph(roleText).SetFont(dataFont).SetFontSize(9)));
            }

            document.Add(table);
            document.Close();

            return stream.ToArray();
        }

        private UserResponse MapToUserResponse(User user)
        {
            return new UserResponse
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                DoB = user.DoB,
                Gender = user.Gender,
                Phone = user.Phone,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Status = (UserStatus)user.Status!,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = user.Roles.Select(r => (UserRole)r.RoleId).ToList(),
                TotalCourses = user.CourseEnrollments.Count,
                CompletedCourses = user.CourseEnrollments.Count(ce => ce.Status == (int)EnrollmentStatus.Completed)
            };
        }

        private string GenerateRandomPassword(int length = 8)
        {
            const string chars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GetStatusText(UserStatus status)
        {
            return status switch
        {
                UserStatus.Active => "Hoạt động",
                UserStatus.Inactive => "Không hoạt động",
                UserStatus.Banned => "Bị cấm",
                UserStatus.Pending => "Chờ xác thực",
                UserStatus.Deleted => "Đã xóa",
                _ => "Không xác định"
            };
        }

        private string GetRoleText(UserRole role)
        {
            return role switch
            {
                UserRole.Student => "Học viên",
                UserRole.Admin => "Quản trị viên",
                _ => "Không xác định"
            };
        }

        public string GetUserProfile(ClaimsPrincipal currentUser, out UserProfileDto userProfile)
        {
            userProfile = null;

            var userId = _userRepository.GetUserIdFromClaims(currentUser);

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return "User not found.";
            }

            var roles = _userRepository.GetRolesByUserId(userId);
            userProfile = new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                DoB = user.DoB,
                Gender = user.Gender,
                Phone = user.Phone,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
                Roles = roles.Select(r => r.RoleName).ToList()
            };

            return "";
        }

        public string UpdateUserProfile(ClaimsPrincipal currentUser, UpdateProfileRequestDto request)
        {
            var userId = _userRepository.GetUserIdFromClaims(currentUser);

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return "User not found.";
            }

            user.FullName = request.FullName;
            user.DoB = request.DoB;
            user.Gender = request.Gender;
            user.Phone = request.Phone;
            user.Address = request.Address;
            user.AvatarUrl = request.AvatarUrl;
            user.UpdatedAt = DateTime.Now;

            _userRepository.UpdateUser(user);
            _userRepository.SaveChanges();

            return "";
        }

        private string GetUserIdFromClaims(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                   claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
