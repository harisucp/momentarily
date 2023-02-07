namespace Momentarily.Common.Definitions
{
    public enum SystemMessageType
    {
        BookingRequest = 1,
        ApproveRequest = 2,
        DeclineRequest = 3,
        PayRequest = 4,
        DepositRequest = 5,
        SharerDispute = 6,
        SeekerDispute = 7,
        SeekerNeedReview=8,
        SharerNeedReview=9,
    }
    public enum RentPeriod
    {
        None = 0,
        Day = 1,
        Week = 2,
        Month = 3,
        Any = 4
    }
    public enum SortBy
    {
        PriceLowToHigh = 1,
        PriceHighToLow = 2,
        LeastRented = 3,
        MostRented = 4,
        LeastRated = 5,
        MostRated = 6
    }
}
