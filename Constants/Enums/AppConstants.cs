namespace Online_Learning.Constants.Enums
{
    public static class AppConstants
    {
        // Pagination
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 100;

        // File upload
        public const int MaxFileSize = 10 * 1024 * 1024; // 10MB
        public const string AllowedImageExtensions = ".jpg,.jpeg,.png,.gif,.webp";
        public const string AllowedVideoExtensions = ".mp4,.avi,.mov,.wmv,.flv";
        public const string AllowedDocumentExtensions = ".pdf,.doc,.docx,.ppt,.pptx";

        // Course
        public const int MaxCourseTitleLength = 200;
        public const int MaxCourseDescriptionLength = 2000;
        public const int MinCoursePrice = 0;
        public const int MaxCoursePrice = 1000000;

        // User
        public const int MinPasswordLength = 6;
        public const int MaxPasswordLength = 50;
        public const int MaxUserNameLength = 100;
        public const int MaxEmailLength = 100;
        public const int MaxPhoneLength = 20;
        public const int MaxAddressLength = 500;

        // Quiz
        public const int MinQuizTime = 1; // minutes
        public const int MaxQuizTime = 180; // minutes
        public const int MinPassScore = 0;
        public const int MaxPassScore = 100;
        public const int MaxQuestionsPerQuiz = 100;

        // Lesson
        public const int MaxLessonTitleLength = 200;
        public const int MaxLessonContentLength = 10000;
        public const int MinLessonDuration = 1; // minutes
        public const int MaxLessonDuration = 480; // minutes (8 hours)

        // Comment
        public const int MaxCommentLength = 1000;

        // Discount
        public const int MaxDiscountCodeLength = 20;
        public const int MaxDiscountDescriptionLength = 500;
        public const int MinDiscountPercentage = 0;
        public const int MaxDiscountPercentage = 100;

        // Validation messages
        public const string RequiredFieldMessage = "Trường này là bắt buộc";
        public const string InvalidEmailMessage = "Email không hợp lệ";
        public const string InvalidPhoneMessage = "Số điện thoại không hợp lệ";
        public const string PasswordTooShortMessage = "Mật khẩu phải có ít nhất {0} ký tự";
        public const string PasswordTooLongMessage = "Mật khẩu không được vượt quá {0} ký tự";
        public const string FileTooLargeMessage = "File quá lớn. Kích thước tối đa là {0}MB";
        public const string InvalidFileTypeMessage = "Loại file không được hỗ trợ";

        // Date formats
        public const string DateFormat = "dd/MM/yyyy";
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        public const string TimeFormat = "HH:mm:ss";

        // Cache keys
        public const string CourseCacheKey = "Course_{0}";
        public const string UserCacheKey = "User_{0}";
        public const string CategoryCacheKey = "Category_{0}";
        public const string QuizCacheKey = "Quiz_{0}";

        // Cache duration (minutes)
        public const int CourseCacheDuration = 30;
        public const int UserCacheDuration = 15;
        public const int CategoryCacheDuration = 60;
        public const int QuizCacheDuration = 10;
    }
}