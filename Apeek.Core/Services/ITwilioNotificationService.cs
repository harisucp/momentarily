using Apeek.Common.Interfaces;
using Apeek.Entities.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Core.Services
{
    public interface ITwilioNotificationService : IDependency
    {
        bool SendMessage(string message, string phoneNumber, string countryCode, string verificationCode, string OTP);
        bool UpdateTwilioMessage(int userId, TwilioMessageType messageType);
        bool BookingConfirmation(string phoneNumber, string countryCode, int userId, string ItemName, DateTime bookingTime);
        bool PaymentConfirmation(string phoneNumber, string countryCode, int userId, string amount, string itemName);
        bool DeliveryStatus(string phoneNumber, string countryCode, int userId);
        bool PickupStatus(string phoneNumber, string countryCode, int userId);
        bool CancellationAlert(string phoneNumber, string countryCode, int userId, string itemName, DateTime dateTime);
        bool RentalDueDate(string phoneNumber, string countryCode, int userId);
        bool UpcomingAppointmentsForPickup(string phoneNumber, string countryCode, int userId);
        bool UpcomingAppointmentsForDelivery(string phoneNumber, string countryCode, int userId);
        bool LastMinuteChangesAndUpdates(string phoneNumber, string countryCode, int userId);
        bool TemporarySecurityCode(string phoneNumber, string countryCode, int userId);
        bool SendOTPVerificationCode(string phoneNumber, string countryCode, string verificationCode, string OTP, int userId);
        string  GenerateOTP();

    }
}
