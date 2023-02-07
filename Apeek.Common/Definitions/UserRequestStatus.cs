namespace Apeek.Common.Definitions{    public enum UserRequestStatus    {        Unknown = 0,        Pending = 1,        Approved = 2,        Declined = 3,        CanceledByBorrower = 4,        Paid = 5,        Released = 6,        Closed = 7,        Reviewing = 8,
        //AmountCharged = 8,
        //DepositCharged = 9,
        Dispute = 10,        Refunded = 11,        NotResponded = 12,        Received = 13,        Returned = 14,        ReturnConfirmed = 15,        Late = 16,        LateConfirmed = 17,        Damaged = 18,        DamagedConfirmed = 19,        LateAndDamaged = 20,        LateAndDamagedConfirmed = 21,
        ClosedWithDispute = 22,        ClosedWithLate = 23,        ClosedWithDamaged = 24,        ClosedWithLateAndDamaged = 25,        CanceledBySharer = 26,
        CanceledBySharerBeforePayment = 27,
        CanceledByBorrowerBeforePayment = 28,    }    public enum DisputeReasons    {
        Late = 1,
        NotReceived = 2,        Damaged = 3,        LateAndDamaged = 4,        Stolen = 5,        Lost = 6,        DeliveredLate = 7,
    }    public enum GlobalCode    {        Pending = 1,        NoIssue = 2,        Notified = 3,        Abusive = 4,        ThankYou = 5,        Welcome = 6,        Owners = 7,        Borrowers = 8,        General = 9,        BannedItems = 10,        ListItems = 11,        Launched = 12,        LaunchingSoon = 13,        NotPermitted = 14,        Phone = 15,        Email = 16,        ThankYouForSubscriber = 17,    }    public enum GlobalCodeCategory    {        AbusiveReport = 1,        Coupon = 2,        FAQ = 3,        PaymentType = 4    }

    public enum GlobalTopics    {
        Account = 1,
        Listing = 2,
        Borrowing = 3,
        General = 4,
        Banneditems = 5,
        Others = 6
    }
    public enum CovidOrderStatus
    {
        Pending = 1,
        Paid = 2,
        Failed = 3,
        Closed = 4,
    }}