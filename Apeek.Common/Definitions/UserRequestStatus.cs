﻿namespace Apeek.Common.Definitions
        //AmountCharged = 8,
        //DepositCharged = 9,
        Dispute = 10,
        ClosedWithDispute = 22,
        CanceledBySharerBeforePayment = 27,
        CanceledByBorrowerBeforePayment = 28,
        Late = 1,
        NotReceived = 2,
    }

    public enum GlobalTopics
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
    }