namespace Online_Learning.Constants.Enums
{
    public class CommentConstants
    {
        public static class Status
        {
            public const int Pending = 0;
            public const int Approved = 1;
            public const int Rejected = 2;
        }

        public static class Messages
        {
            public const string CommentNotFound = "Comment not found";
            public const string CommentCreated = "Comment created successfully";
            public const string CommentUpdated = "Comment updated successfully";
            public const string CommentDeleted = "Comment deleted successfully";
            public const string CommentApproved = "Comment approved successfully";
            public const string CommentRejected = "Comment rejected successfully";
            public const string CommentReported = "Comment reported successfully";
            public const string UnauthorizedAccess = "You are not authorized to perform this action";
            public const string InvalidCommentContent = "Comment contains inappropriate content";
        }

        public static class ReportTypes
        {
            public const int Spam = 1;
            public const int Inappropriate = 2;
            public const int Offensive = 3;
            public const int Other = 4;
        }
}
}
