using Online_Learning.Repositories.Implementations;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Implementations;
using Online_Learning.Services.Interfaces;

namespace Online_Learning.Configurations
{
	public static class DependencyInjectionConfig
	{
		//DI 
		// ae dki service cac thu trong day
		public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
            // mấy con vợ cấu hình DI ở đây nhé

            //auth
			services.AddScoped<IAuthService, AuthService>();

			//user
			services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            //role
            services.AddScoped<IRoleRepository, RoleRepository>();

			//function
			services.AddScoped<IFunctionRepository, FunctionRepository>();

			//userotp
			services.AddScoped<IUserOtpRepository, UserOtpRepository>();

            //course
            services.AddScoped<ICourseRepository, CourseRepository>();
			services.AddScoped<ICourseService, CourseService>();

			//category
			services.AddScoped<ICategoryRepository, CategoryRepository>();

			//lesson
			services.AddScoped<ILesssonRepository, LessonRepository>();
			
			//email
			services.AddSingleton<IEmailService, EmailService>();
		}
	}
}