using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Entities.Entities.Enums
{
    public enum TwilioMessageType
    {
        [EnumMember (Value = "Your booking for  is confirmed for ##### at ####")]
        BookingConfirmation = 1,
        [EnumMember(Value = "Payment of ### for  is successful. Your receipt is on its way to your email.")]
        PaymentConfirmation,
        [EnumMember(Value = "Your  is on the way! ETA: ########.")]
        DeliveryStatus,
        [EnumMember(Value = "Your  is on the way! ETA: ########.")]
        PickupStatus,
        [EnumMember(Value = "Your booking for  on ########## has been canceled. Check your email for more details.")]
        CancellationAlert,
        [EnumMember(Value = "Reminder: Your rental for  is due on ##### at ####. Please prepare for return.")]
        RentalDueDate,
        [EnumMember(Value = "Reminder: Your rental for [Item Name] is due on [Date] at [Time]. Please prepare for return.")]
        UpcomingAppointmentsForPickup,
        [EnumMember(Value = "Your scheduled pickup for  is tomorrow at ##########.")]
        UpcomingAppointmentsForDelivery,
        [EnumMember(Value = "Your scheduled delivery for  is tomorrow at ##########.")]
        LastMinuteChangesAndUpdates,
        [EnumMember(Value = "Your momentarily verification code is ########.")]
        TemporarySecurityCode,
        [EnumMember(Value = "Hello, in order to activate your account on momentarily, please enter ######## , or click on this link to activate")]
        SendOTPVerificationCode
    }
}
