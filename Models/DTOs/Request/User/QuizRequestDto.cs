using System.ComponentModel.DataAnnotations;

namespace Online_Learning.Models.DTOs.Request.User;

public class QuizRequestDto
{
    [Required]
    public long QuizId { get; set; }
    
    [Required]
    public List<UserAnswerRequestDto> Answers { get; set; } = new List<UserAnswerRequestDto>();
}

public class UserAnswerRequestDto
{
    [Required]
    public long QuestionId { get; set; }
    
    [Required]
    public long OptionId { get; set; }
} 