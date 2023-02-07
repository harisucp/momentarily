namespace Apeek.Common.EventManager.DataTracker
{
    public class ProcessStatus
    {
        public const int Error = -1;
        public const int Unprocessed = 0;
        public const int Processed = 1;
        public const int InProgress = 2;
        public const int Duplicate = 3;
    }
    public class DataPoints
    {
        public static string User = "User";
        public static string UserId = "UserId";
        public static string LocationId = "LocationId";
        public static string ServiceId = "ServiceId";
        public static string ServiceName = "ServiceName";
        public static string ServiceStatus = "ServiceStatus";
        public static string ReindexType = "ReindexType";
        public static string ServiceIds = "ServiceIds";
        public static string LocationIds = "LocationIds";
    }
    public class ReindexType
    {
        public const string UpdateLocationServiceIndexByPersonId = "UpdateLocationServiceIndexByPersonId";
        public const string DeleteLocationServiceIndexByPersonId = "DeleteLocationServiceIndexByPersonId";
        public const string UpdateLocationServiceIndexTreeByServiceId = "UpdateLocationServiceIndexTreeByServiceId";
        public const string UpdateLocationServiceIndex = "UpdateLocationServiceIndex";
        public const string DeleteLocationServiceIndex = "DeleteLocationServiceIndex";
        public const string UpdateLocationServiceIndexByLocationId = "UpdateLocationServiceIndexByLocationId";
    }
}