using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Online_Learning.Models.Entities;

public partial class OnlineLearningContext : DbContext
{
    public OnlineLearningContext()
    {
    }

    public OnlineLearningContext(DbContextOptions<OnlineLearningContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategory> CourseCategories { get; set; }

    public virtual DbSet<CourseEnrollment> CourseEnrollments { get; set; }

    public virtual DbSet<CourseImage> CourseImages { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscussionLesson> DiscussionLessons { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<LessonProgress> LessonProgresses { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAnswer> UserAnswers { get; set; }

    public virtual DbSet<UserCertificate> UserCertificates { get; set; }

    public virtual DbSet<UserQuizResult> UserQuizResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("CartItem");

            entity.Property(e => e.CartItemId)
                .ValueGeneratedNever()
                .HasColumnName("CartItemID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.CategoryId)
                .ValueGeneratedNever()
                .HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(255);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");
            entity.Property(e => e.Acceptor)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CertificateId).HasColumnName("CertificateID");
            entity.Property(e => e.CourseName).HasMaxLength(255);
            entity.Property(e => e.Creator)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LanguageId).HasColumnName("LanguageID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.StudyTime).HasMaxLength(50);

            entity.HasOne(d => d.Language).WithMany(p => p.Courses)
                .HasForeignKey(d => d.LanguageId)
                .HasConstraintName("FK_Courses_Languages");

            entity.HasOne(d => d.Level).WithMany(p => p.Courses)
                .HasForeignKey(d => d.LevelId)
                .HasConstraintName("FK_Courses_Levels");
        });

        modelBuilder.Entity<CourseCategory>(entity =>
        {
            entity.HasKey(e => new { e.CategoryId, e.CourseId });

            entity.ToTable("CourseCategory");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");

            entity.HasOne(d => d.Category).WithMany(p => p.CourseCategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseCategory_Categories");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseCategories)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseCategory_Courses");
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(e => new { e.CourseId, e.UserId });

            entity.ToTable("CourseEnrollment");

            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseEnrollment_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseEnrollment_User");
        });

        modelBuilder.Entity<CourseImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.Property(e => e.ImageId)
                .ValueGeneratedNever()
                .HasColumnName("ImageID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseImages)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourseImages_Courses");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.ToTable("Discount");

            entity.Property(e => e.DiscountId)
                .ValueGeneratedNever()
                .HasColumnName("DiscountID");
            entity.Property(e => e.Code)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.CreatedAt).HasMaxLength(25);
            entity.Property(e => e.Creator)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FixValue).HasColumnType("money");
            entity.Property(e => e.MaxValue).HasColumnType("money");
            entity.Property(e => e.MinPurchase).HasColumnType("money");

            entity.HasOne(d => d.CreatorNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.Creator)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Discount_User");
        });

        modelBuilder.Entity<DiscussionLesson>(entity =>
        {
            entity.HasKey(e => e.DiscussionId);

            entity.Property(e => e.DiscussionId).HasColumnName("DiscussionID");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.ParentCommentId).HasColumnName("ParentCommentID");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");

            entity.HasOne(d => d.Lesson).WithMany(p => p.DiscussionLessons)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("FK_DiscussionLessons_Lessons");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK_DiscussionLessons_Parent");

            entity.HasOne(d => d.User).WithMany(p => p.DiscussionLessons)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_DiscussionLessons_User");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.Property(e => e.LanguageId)
                .ValueGeneratedNever()
                .HasColumnName("LanguageID");
            entity.Property(e => e.LanguageName).HasMaxLength(255);
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.LessonName).HasMaxLength(255);
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");

            entity.HasOne(d => d.Module).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Lessons_Modules");
        });

        modelBuilder.Entity<LessonProgress>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LessonId });

            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");
            entity.Property(e => e.LessonId).HasColumnName("LessonID");

            entity.HasOne(d => d.Lesson).WithMany(p => p.LessonProgresses)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("FK_LessonProgresses_Lessons");

            entity.HasOne(d => d.User).WithMany(p => p.LessonProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_LessonProgresses_User");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.Property(e => e.LevelId)
                .ValueGeneratedNever()
                .HasColumnName("LevelID");
            entity.Property(e => e.LevelName).HasMaxLength(255);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");
            entity.Property(e => e.ModuleName).HasMaxLength(255);

            entity.HasOne(d => d.Course).WithMany(p => p.Modules)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_Modules_Courses");
        });

        modelBuilder.Entity<Option>(entity =>
        {
            entity.Property(e => e.OptionId).HasColumnName("OptionID");
            entity.Property(e => e.OptionText).HasMaxLength(255);
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.Options)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_Options_Questions");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId)
                .ValueGeneratedNever()
                .HasColumnName("OrderID");
            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymetMethod).HasMaxLength(50);
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK_Orders_Discount");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_User");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.Property(e => e.OrderItemId)
                .ValueGeneratedNever()
                .HasColumnName("OrderItemID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.Course).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_OrderItem_Courses");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItem_Orders");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.QuestionName).HasMaxLength(255);
            entity.Property(e => e.QuizId).HasColumnName("QuizID");

            entity.HasOne(d => d.Quiz).WithMany(p => p.Questions)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK_Questions_Quizzes");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
            entity.Property(e => e.QuizName).HasMaxLength(255);

            entity.HasOne(d => d.Module).WithMany(p => p.Quizzes)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Quizzes_Modules");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(250);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.Rolers).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RolerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoles_Roles"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UserRoles_User"),
                    j =>
                    {
                        j.HasKey("UserId", "RolerId");
                        j.ToTable("UserRoles");
                        j.IndexerProperty<string>("UserId")
                            .HasMaxLength(36)
                            .IsUnicode(false)
                            .IsFixedLength()
                            .HasColumnName("UserID");
                        j.IndexerProperty<int>("RolerId").HasColumnName("RolerID");
                    });
        });

        modelBuilder.Entity<UserAnswer>(entity =>
        {
            entity.Property(e => e.UserAnswerId).HasColumnName("UserAnswerID");
            entity.Property(e => e.AnswerText).HasMaxLength(255);
            entity.Property(e => e.OptionId).HasColumnName("OptionID");
            entity.Property(e => e.QuestionId).HasColumnName("QuestionID");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");

            entity.HasOne(d => d.Option).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.OptionId)
                .HasConstraintName("FK_UserAnswers_Options");

            entity.HasOne(d => d.Question).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.QuestionId)
                .HasConstraintName("FK_UserAnswers_Questions");

            entity.HasOne(d => d.User).WithMany(p => p.UserAnswers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserAnswers_User");
        });

        modelBuilder.Entity<UserCertificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId);

            entity.ToTable("UserCertificate");

            entity.Property(e => e.CertificateId)
                .ValueGeneratedNever()
                .HasColumnName("CertificateID");
            entity.Property(e => e.CourseId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CourseID");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");

            entity.HasOne(d => d.Course).WithMany(p => p.UserCertificates)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK_UserCertificate_Courses");

            entity.HasOne(d => d.User).WithMany(p => p.UserCertificates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserCertificate_User");
        });

        modelBuilder.Entity<UserQuizResult>(entity =>
        {
            entity.Property(e => e.UserQuizResultId).HasColumnName("UserQuizResultID");
            entity.Property(e => e.QuizId).HasColumnName("QuizID");
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UserId)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("UserID");

            entity.HasOne(d => d.Quiz).WithMany(p => p.UserQuizResults)
                .HasForeignKey(d => d.QuizId)
                .HasConstraintName("FK_UserQuizResults_Quizzes");

            entity.HasOne(d => d.User).WithMany(p => p.UserQuizResults)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserQuizResults_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

	internal async Task<object> FindAsync(int id)
	{
		throw new NotImplementedException();
	}

	partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
