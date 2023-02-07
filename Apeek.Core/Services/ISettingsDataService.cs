using Apeek.Common.Interfaces;
using Apeek.Core.Services.Impl.AWS.S3;
using Apeek.Core.Services.Impl.MSMQ;
using Apeek.Messaging.Interfaces;
using Apeek.ViewModels.Settings;
namespace Apeek.Core.Services
{
    public interface ISettingsDataService : IDependency
    {
        ISendProperty GetSmsSendProperty();
        ISendProperty GetSmsSendPropertyAlt();
        ISendProperty GetEmailSendProperty();
        ISendProperty GetEmailSendPropertyForRental();
        ISendProperty GetEmailSendPropertyForMarketing();
        string GetSmtpFrom();
        string GetSmtpFromName();
        string GetSmsFrom();
        string GetEmailContactUs();
        string GetEmailAdmin();
        string GetEmailRental();
        string GetEmailHelp();        string GetEmailMarketing();
        bool GetIsProduction();
        string GetHost();
        string GetGoogleClientId();
        string GetGoogleClientSecret();
        string GetFacebookClientId();
        string GetFacebookClientSecret();
        string GetVkClientId();
        string GetVkClientSecret();
        IExternalFileStorageConfig GetExternalFileStorageConfig();
        MsmqProcessingParams GetMsmqProcessingParams();
        MsmqProcessingParams GetMsmqServerProcessingParams();
        PayPalSetting GetPayPalSetting();
        PinPaymentSetting GetPinPaymentSetting();
        double GetBorrowerPaymentTransactionCommision();
        double GetSharerPaymentTransactionCommision();
        double GetCharity();
        double GetDiliveryPrice();
        string GetPaymentTransactionCurrency();
        string GetBraintreeEnvironment();
        string GetBraintreeMasterMerchantAccountId();
        string GetBraintreeMerchantId();
        string GetBraintreePublicKey();
        string GetBraintreePrivateKey();
        string GetImgFileServerUrl();
        string GetBetaVersion();
    }
}