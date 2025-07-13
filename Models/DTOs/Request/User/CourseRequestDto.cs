using Online_Learning.Constants.Enums;

namespace Online_Learning.Models.DTOs.Request.User
{
	public class CourseRequestDto
	{
		// Phân trang
		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 6;
		
		// Tìm kiếm
		public string? SearchTerm { get; set; }
		
		// Filter
		public int? LevelId { get; set; }
		public int? LanguageId { get; set; }
		public List<int>? CategoryIds { get; set; }
		public decimal? MinPrice { get; set; }
		public decimal? MaxPrice { get; set; }
		
		// Sắp xếp
		public string? SortBy { get; set; } // "price", "createdAt", "enrollmentCount"
		public string? SortOrder { get; set; } // "asc", "desc"
	}
}
