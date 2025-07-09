using Online_Learning.Models.Entities;
using Online_Learning.Constants;
using Online_Learning.Constants.Enums;

namespace Online_Learning.Models.DTOs.Response.User
{
	public class ModuleResponseDTO
	{
		public long ModuleId { get; set; }

		public string ModuleName { get; set; } = null!;

		public int ModuleNumber { get; set; }

		public List<LessonResponseDTO> Lessons { get; set; } = new List<LessonResponseDTO>();

		public List<QuizResponseDTO> Quizzes { get; set; } = new List<QuizResponseDTO>();

		public ModuleResponseDTO(Module module)
		{
			ModuleId = module.ModuleId;
			ModuleName = module.ModuleName;
			ModuleNumber = module.ModuleNumber;

			// Map Lessons
			if (module.Lessons != null)
			{
				Lessons = module.Lessons
					.Where(l => l.Status == (int)LessonStatus.Active)
					.OrderBy(l => l.LessonNumber)
					.Select(l => new LessonResponseDTO(l))
					.ToList();
			}

			
		}
        public ModuleResponseDTO()
        {
            
        }
    }
}
