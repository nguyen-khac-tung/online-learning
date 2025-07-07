using Online_Learning.Repositories.Implementations;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Implementations;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Configurations
{
	public static class DependencyInjectionConfig
	{
		//DI 
		public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			// mấy con vợ cấu hình DI ở đây nhé

			//course
			services.AddScoped<ICourseRepository, CourseRepository>();
			services.AddScoped<ICourseService, CourseService>();

			//category
			services.AddScoped<ICategoryRepository, CategoryRepository>();

		}
	}
}
