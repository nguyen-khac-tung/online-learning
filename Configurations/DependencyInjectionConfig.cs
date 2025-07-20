
using Microsoft.AspNetCore.Identity;
using Online_Learning.Models.Entities;
using Online_Learning.Repositories.Implementations;
using Online_Learning.Repositories.Interfaces;
using Online_Learning.Services.Implementations;
using Online_Learning.Services.Interfaces;
using System.Runtime.ConstrainedExecution;

namespace Online_Learning.Configurations
{
	public static class DependencyInjectionConfig
	{
		//DI 
		// ae dki service cac thu trong day
		public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
            // 🔐 Auth
            services.AddScoped<IAuthService, AuthService>();

            // 👤 User
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // 🔐 Role & Function
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IFunctionRepository, FunctionRepository>();

            // 🔐 OTP
            services.AddScoped<IUserOtpRepository, UserOtpRepository>();

            // 🛒 Cart
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();

            // 📚 Course
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseService, CourseService>();

            // 🏷️ Category
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // 🎬 Lesson
            services.AddScoped<ILesssonRepository, LessonRepository>();

            // ❓ Quiz
            services.AddScoped<IQuizzRepository, QuizzRepository>();
            services.AddScoped<IQuizService, QuizService>();

            // 📧 Email
            services.AddSingleton<IEmailService, EmailService>();

            // ⭐ Rating
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<IRatingService, RatingService>();

            // 🎓 Enrollment
            services.AddScoped<ICourseEnrollmentRepository, CourseEnrollmentRepository>();

            // 🎁 Discount
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IDiscountService, DiscountService>();

            // 📊 Dashboard
            services.AddScoped<IDashboardService, DashboardService>();

            // 💬 Comment
            services.AddScoped<ICommentService, CommentService>();
        }
	}
}
