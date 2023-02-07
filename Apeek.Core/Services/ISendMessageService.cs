using System;
using System.Collections.Generic;
using Apeek.Common.Interfaces;
using Apeek.Common.Models;
using Apeek.Entities.Entities;
using Apeek.Messaging.Interfaces;
using Apeek.ViewModels.Models;
using Apeek.ViewModels.Models.Impl;
using Momentarily.ViewModels.Models;

namespace Apeek.Core.Services
{
    public interface ISendMessageService : IDependency
    {
        bool SendVia(IMessageSendProvider messageSendProvider, IMessage message, ISendProperty sendProperty);
        bool SendEmailAccountActivationMessage(User user, List<Address> addresses, string verificationUrl);
        bool SendEmailQuickAccountActivationMessage(string email, string verificationUrl,string firstname);
        bool SendEmailBulkMailSendToUnknownMessage(string email, string verificationUrl, string serviceName);
        IMessage GetEmailLoginMessage(User user, string loginUrl);
        IMessage GetSmsRestorePwdMessage(string phoneNumber, string tempPwd);
        bool SendSmsUserInvitationMessage(string phoneNumberTo, string userName, string userLogin, string userPwd);
        bool SendEmailContactUs(ContactUsEntry contactUsEntry);
        bool SendEmailContactUsForUser(ContactUsEntry contactUsEntry);
        void SendAdminNotificationEmail_Errors(string messageBody, string logSource);
        bool SendEmailVerifySecurityDataMessage(UserSecurityDataChangeRequest request, string username);
        bool SendEmailChangePasswordMessage(string email,string firstname);
        IMessage GetEmailRestorePwdMessage(User user, string loginUrl);
        bool SendEmailChangeEmailMessage(string email, string activationUrl);
        bool SendEmailFollowUpMessage(string email, string firstname);
        bool SendEmailGetMessageMessage(string email, string userFrom, string linkToMessage);
        bool SendEmailGetBookingMessage(string email, string userFrom, string listing, string linkToRequest);
        bool SendEmailApproveBookingMessage(string email, string listing, string linkToRequest,
            string linkToPay,string userName);
        bool SendEmailDeclineBookingMessage(string email, string listing, string linkToRequest, string userName);
        bool SendEmailPayRequestMessage(string email, string listing, string linkToRequest);
        bool SendEmailDepositRequestMessage(string email, string listing, string linkToRequest);
        bool SendEmailSharerStartDispute(string listing, string sahrerName, string disputeMessage, int requestId, string emailId);
        bool SendEmailSeekerStartDispute(string listing, int requestId, string seekerName, string disputeMessage,string emailId);
        bool SendConfirmEmailToSharer(string email, string listing, string startDate, string endDate, string price, string pickupLocation, UserContactInfo contactInfo, string linkToRequest);
        bool SendConfirmEmailToSeeker(string email, string listing, string startDate, string endDate, string bookingPrice, string serviceFee, string deposit, string totalPrice,string pickupLocation, UserContactInfo sharerInfo, string linkToRequest);
        bool SendReviewMessageForSeeker(string email,string sharerName, string listing, string linkToReview);
        bool SendReviewMessageForSharer(string email, string seekerName, string listing, string linkToReview);
        //bool SendEmailSharerWroteReview(string email, string sharerName, string listing, string linkToReview);
        bool SendEmailReviewFromSharer(string email, string sharerName, string listing, string linkToReview,int userId);
        //bool SendEmailSeekerWroteReview(string email, string seekerName, string listing, string linkToReview);

        bool SendEmailReviewFromBorrower(string email, string seekerName, string listing, string linkToReview, int userId);

        bool SendEmailSeekerReadReview(string email, string sharerName, string text, string listing, string linkToReview);
        bool SendEmailSharerReadReview(string email, string seekerName, string text, string listing, string linkToReview);
        bool SendPaymentEmailTemplate(PayPal.Api.Payment payment, Result<GoodRequestViewModel> request,string url);
        bool SendAbusiveReminder(int goodid, User userid,string url);

        bool SendEmailThankYouTemplate(int borrowerId, string borrowerName, string borrowerEmail, string promoCode, string url, double discountAmount,string discountType);
        bool SendPaymentEmailTemplateInvoiceDetail(PayPal.Api.Payment payment, Result<GoodRequestViewModel> request, string url, User sharerInfo, User borrowerInfo, string sharerPhone, string borrowerPhone, string getPickupLocation);        bool SendCancelEmailTemplateDetailForBorrower(PaypalPayment payment, Result<GoodRequestViewModel> request, string refundAmount);        bool SendPaymentEmailTemplateInvoiceDetailForOwnerEmail(PayPal.Api.Payment payment, Result<GoodRequestViewModel> request, string url, User sharerInfo, User borrowerInfo, string sharerPhone, string borrowerPhone, string getPickupLocation);
        bool SendPaymentEmailTemplateOfferReceivedForSharer(User user, GoodRequest request, string url);
        bool SendEmailNewsLetterTemplate(string userEmail, string url);
        bool SendEmailWelcomeTemplate(string userEmail,string userName, string url);
        bool SendLaunchSoonMessage(string email, DateTime launchingdate, string username, string coupon,double couponDiscount,string couponType);
        bool SendLaunchedMessage(string email, string username, string coupon, double couponDiscount, string couponType);
        bool SendBlockedUserMessage(string email, string firstname, string status);
        bool SendTemporaryDeletedUserMessage(string email, string firstname);
        bool SendTemporaryDeletedUserItemsMessage(string email, string firstname,string itemName);
        bool SendFinalConfirmation(string email, string status, string borrowerName,string sharerName);
        bool SendEmailThankYouTemplateForSubscribing(int borrowerId, string borrowerName, string borrowerEmail, string promoCode, string url, double discountAmount, string discountType);

        bool SendEmailCancelBookingByBorrower(int userid,string username,string email);
        bool SendEmailCancelBookingBySharer(int userid, string username, string email, string coupon);
        bool SendEmailBookingDeniedForBorrower(int userid, string username, string email,string couponCode,string itemName, string message);
        bool SendEmailAfterTransationComplete(string email, string borrowerName, int bookingId);
        bool SendPaymentEmailTemplateCovidRentalInvoiceDetail(CovidGoodViewModel model, CovidOrderPaymentDetailViewModel orderDesc);
        bool SendForUserMessageAfterCovidRentalClose(string email, string buyerName);
    }
}