
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
            // 🧠 Repositories
            services.AddScoped<IRatingRepository, RatingRepository>();
            services.AddScoped<ICourseEnrollmentRepository, CourseEnrollmentRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();

            // ⚙️ Services
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
        }
	}
}