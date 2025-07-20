using Online_Learning.Repositories.Implementations;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Implementations;
using Online_Learning.Services.Interfaces;

using Microsoft.AspNetCore.Identity;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Implementations;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Implementations;
using Online_Learning.Services.Interfaces;
using System.Runtime.ConstrainedExecution;
using Online_Learning.Repositories.Implementations.Admin;
using Online_Learning.Repositories.Interfaces.Admin;
using Online_Learning.Services.Implementations.Admin;
using Online_Learning.Services.Interfaces.Admin;

namespace Online_Learning.Configurations
{
	public static class DependencyInjectionConfig
	{
		//DI 
		// ae dki service cac thu trong day
		public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpContextAccessor();

            //Repositories
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<ICourseEnrollmentRepository, CourseEnrollmentRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();

            //Services
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IDiscountService, DiscountService>();
            //AdminUser
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            //dashboard 
            services.AddScoped<IDashboardService, DashboardService>();
            //comment 
            services.AddScoped<ICommentService, CommentService>();
        
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

			//cart
			services.AddScoped<ICartRepository, CartRepository>();
			services.AddScoped<ICartService, CartService>();

            //course
            services.AddScoped<Repositories.Interfaces.ICourseRepository, Repositories.Implementations.CourseRepository>();
			services.AddScoped<Services.Interfaces.ICourseService, Services.Implementations.CourseService>();

			//category
			services.AddScoped<Repositories.Interfaces.ICategoryRepository, Repositories.Implementations.CategoryRepository>();

			//lesson
			services.AddScoped<ILesssonRepository, Repositories.Implementations.LessonRepository>();


			//quiz
			services.AddScoped<IQuizzRepository, QuizzRepository>();
			services.AddScoped<Services.Interfaces.IQuizService, Services.Implementations.QuizService>();

			//email
			services.AddSingleton<IEmailService, EmailService>();



            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IModuleRepository, ModuleRepository>();

            services.AddScoped<Services.Interfaces.Admin.ICourseService, Services.Implementations.Admin.CourseService>();
            services.AddScoped<Repositories.Interfaces.Admin.ICourseRepository, Repositories.Implementations.Admin.CourseRepository>();

            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<ILessonRepository, Repositories.Implementations.Admin.LessonRepository>();

            services.AddScoped<Services.Interfaces.Admin.IQuizService, Services.Implementations.Admin.QuizService>();
            services.AddScoped<IQuizRepository, QuizRepository>();

            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();

            services.AddScoped<IOptionService, OptionService>();
            services.AddScoped<IOptionRepository, OptionRepository>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<Repositories.Interfaces.Admin.ICategoryRepository, Repositories.Implementations.Admin.CategoryRepository>();

            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ILanguageRepository, LanguageRepository>();

            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<ILevelRepository, LevelRepository>();

        }
	}
}