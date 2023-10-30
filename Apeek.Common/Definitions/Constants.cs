using System;
namespace Apeek.Common
{
    public class Constants
    {
        public const string DelimUrl = "-";
        public const string DelimServices = ",";
        public const string DelimSubDomain = ".";
        public const int DefaultCreateBy = 0;
        public class Port
        {
            public const int Http = 80;
            public const int Https = 443;
        }
        public class Html
        {
            public const string Br = "<br/>";
            public const string N = "\n";
        }
        public class Browse
        {
            public const int DefaultPageSize = 10;
        }
    }
    public enum CreateResult
    {
        Error = 0,
        Success = 1,
        Duplicate = 2,
        LocationDoesNotExist = 3,
        EmailChangeError = 4,
        EmailChangeSuccess = 5,
        VerifySecurityRequestError = 6,
        EmailChangeNotificationSendSuccess = 7,
        EmailChangeNotificationSendError = 8,
        NotAssigned = 9,
        NoAccess = 10,
        ErrorHasBeenModified = 11,
        PaymentError = 12,
        OldPasswordDoesNotMatch = 13,
    }
    [Flags]
    public enum PageName : ulong    {        Other = 1,        Admin = 1 << 1,        Users = 1 << 2,        AdminPwd = 3 << 1,        Home = 2 << 0,        Create = 2 << 1,        Contactus = 2 << 2,        Location = 2 << 3,        PersonList = 2 << 4,        UserServices = 2 << 5,        UserSettings = 2 << 6,        VerifyUserEmailSent = 2 << 7,        Login = 2 << 8,        LoginMessageSent = 2 << 9,        UserPwd = 2 << 10,        UserEmail = 2 << 11,        UserInfo = 2 << 12,        UserInfoPreview = 2 << 13,        UserAccountAssociations = 2 << 14,        NotFound = 2 << 15,        Service = 2 << 16,        User = 2 << 17,        UsersWithService = 2 << 18,        Register = 2 << 19,        UserProfile = 2 << 20,        UserMessage = 2 << 21,        UserMessageConversation = 2 << 22,        UserRequest = 2 << 23,        UserRequests = 2 << 24,        GoodRequest = 2 << 25,        GoodRequests = 2 << 26,        GoodRequestDecline = 2 << 27,        Listing = 2 << 28,        Search = 2 << 29
    }
    public enum Cases
    {
        /// <summary>
        /// Де
        /// </summary>
        Where,
        /// <summary>
        /// Ким
        /// </summary>
        Whom,
        /// <summary>
        /// Xbq
        /// </summary>
        Whose,
        /// <summary>
        /// Кого, чого
        /// </summary>
        Who
    }
    public class Placeholders
    {
        public const string citywhere = "{citywhere}";
        public const string city = "{city}";
        public const string service = "{service}";
        public const string page = "{page}";
    }
    public class Privileges
    {
        public const string CanCreateMultipleUsers = "CAN_CREATE_MULTIPLE_USERS";
        public const string CanEditUsers = "CAN_EDIT_USERS";
        public const string CanViewUsers = "CAN_VIEW_USERS";
        public const string CanDeleteUsers = "CAN_DELETE_USERS";
        public const string CanAccessBackend = "CAN_ACCESS_BACKEND";
        public const string CanEditContent = "CAN_EDIT_CONTENT";
        public const string CanAccessReports = "CAN_ACCESS_REPORTS";
        public const string BackendCanMoveCategories = "BACKEND_CAN_MOVE_CATEGORIES";
        public const string Admin = "ADMIN";
    }
    public class WebFolders
    {
        public const string Images = "/Content/Images/";
    }
    public class DefaultImages
    {
        public const string NoPhoto = "no_photo_{0}.jpg";
    }
    //public class ConstantsImage
    //{
    //    public const string DefaultImageExt = "jpg";
    //    public static string DefaultImageBgColor = "white";
    //}
    public enum NotificationType
    {
        Sms=1,
        Email
    }
    public enum UserResponceType
    {
        NoResponce=0,
        Login
    }
    public enum OperationType
    {
        Insert,
        InsertOriginal,
        Update,
        Delete
    }
    public class UserNotificationEventTypes
    {
        public const int ServiceChanged = 1;
    }
    public class AuditTable
    {
        public const int Service = 1;
    }
    public class AuditAction
    {
        public const int Status = 1;
        public const int Name = 2;
        public const int ReplaceService = 3;
        public const int Parent = 4;
        public const int DeleteService = 5;
    }
    public class UserSecurityDataType
    {
        public const int Email = 1;
    }
    public class DatacolSource
    {
        public const string Slando = "slando";
        public const string Ria = "ria";
    }
    public class ViewErrorText
    {
        public const string UserPwdChanged = "You have changed your password successfully!";
        public const string UserPwdNoChanged = "Oops! We are unable to change your password...";
        public const string UserProfileChanged = "Your information has been saved successfully!";
        public const string UserProfileNoChanged = "Oops! We are unable to save this information...";
        public const string UserEmailChanged = "Your email has been changed successfully!";
        public const string UserEmailNoChanged = "Oops! We are unable to change your email...";
        public const string UserEmailNoChangedSameEmails = "Oops! Same email...";
        public const string UserEmailSendChangeMessage = "We have sent email to {0}.Please, confirm email address change.";
        public const string ConcatUsSend = "Thank you, your message was sent successfully! One of our team will get back to you shortly.";
        public const string AdminPwdChanged = "You have changed your password successfully!";        public const string AdminPwdNoChanged = "Oops! We are unable to change your password...";
    }
    public class NotificationTemplates
    {
        public const string ReceiveMessageNotification = "You receive one message.";
    }
    public class PaymentTransactionType
    {
        public const string SaleType = "sale";
        public const string AuthorizeType = "authorize";
        public const string Payout = "payout";
        public const string Refund = "refund";
    }
    public class MessageCharactersMap
    {
        public const string Quote = "&quote";
    }
}