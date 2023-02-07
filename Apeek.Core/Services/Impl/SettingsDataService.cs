using System;
using System.Collections.Generic;
using System.Globalization;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Core.Services.Impl.AWS.S3;
using Apeek.Core.Services.Impl.MSMQ;
using Apeek.Common.Interfaces;
using Apeek.Entities.Constants;
using Apeek.Entities.Entities;
using Apeek.Messaging.Interfaces;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Common;
using Apeek.ViewModels.Settings;
namespace Apeek.Core.Services.Impl
{
    public class SettingsDataService : ISettingsDataService
    {
        private List<Setting> _settings;
        public SettingsDataService()
        {
            if (!RepCache<Setting>.Loaded)
            {
                _settings = new List<Setting>();
                var res = Uow.Wrap(u =>
                {
                    _settings = new RepCache<Setting>().List();
                });
            }
            else
            {
                _settings = new RepCache<Setting>().List();
            }
        }
        public ISendProperty GetSmsSendProperty()
        {
            return new SendProperty()
            {
                Login = _settings.Find(x => x.Key == SettingKey.SmsProviderLogin).Value,
                Pwd = _settings.Find(x => x.Key == SettingKey.SmsProviderPwd).Value
            };
        }
        public ISendProperty GetSmsSendPropertyAlt()
        {
            return new SendProperty()
            {
                Login = _settings.Find(x => x.Key == SettingKey.SmsProviderLoginAlt).Value,
                Pwd = _settings.Find(x => x.Key == SettingKey.SmsProviderPwdAlt).Value
            };
        }
        public ISendProperty GetEmailSendProperty()
        {
            return new SendProperty()
            {
                ProviderHost = _settings.Find(x => x.Key == SettingKey.SmtpHost).Value,
                Login = _settings.Find(x => x.Key == SettingKey.SmtpUser).Value,
                ProviderPort = Convert.ToInt32(_settings.Find(x => x.Key == SettingKey.SmtpPort).Value),
                Pwd = _settings.Find(x => x.Key == SettingKey.SmtpPwd).Value,
            };
        }

        public ISendProperty GetEmailSendPropertyForRental()
        {
            return new SendProperty()
            {
                ProviderHost = _settings.Find(x => x.Key == SettingKey.SmtpHost).Value,
                //Login = _settings.Find(x => x.Key == SettingKey.SmtpUserRental).Value,
                //Pwd = _settings.Find(x => x.Key == SettingKey.SmtpPwdRental).Value,
                ProviderPort = Convert.ToInt32(_settings.Find(x => x.Key == SettingKey.SmtpPort).Value),
                Login = _settings.Find(x => x.Key == SettingKey.SmtpUser).Value,
                Pwd = _settings.Find(x => x.Key == SettingKey.SmtpPwd).Value,
            };
        }

        public ISendProperty GetEmailSendPropertyForMarketing()
        {
            return new SendProperty()
            {
                ProviderHost = _settings.Find(x => x.Key == SettingKey.SmtpHost).Value,
                //Login = _settings.Find(x => x.Key == SettingKey.SmtpUserMarketing).Value,
                //Pwd = _settings.Find(x => x.Key == SettingKey.SmtpPwdMarketing).Value,
                ProviderPort = Convert.ToInt32(_settings.Find(x => x.Key == SettingKey.SmtpPort).Value),
                Login = _settings.Find(x => x.Key == SettingKey.SmtpUser).Value,
                Pwd = _settings.Find(x => x.Key == SettingKey.SmtpPwd).Value,
            };
        }

        public ISendProperty GetEmailSendPropertyComingHelp()
        {
            return new SendProperty()
            {
                ProviderHost = _settings.Find(x => x.Key == SettingKey.SmtpHost).Value,
                //Login = _settings.Find(x => x.Key == SettingKey.SmtpUserHelp).Value,
                //Pwd = _settings.Find(x => x.Key == SettingKey.SmtpPwdHelp).Value,
                ProviderPort = Convert.ToInt32(_settings.Find(x => x.Key == SettingKey.SmtpPort).Value),
                Login = _settings.Find(x => x.Key == SettingKey.SmtpUser).Value,
                Pwd = _settings.Find(x => x.Key == SettingKey.SmtpPwd).Value,
            };
        }


        public string GetSmtpFrom()
        {
            return _settings.Find(x => x.Key == SettingKey.SmtpFrom).Value;
        }
        public string GetSmtpFromName()
        {
            return _settings.Find(x => x.Key == SettingKey.SmtpFromName).Value;
        }
        public string GetSmsFrom()
        {
            return "TOPHANDS";
        }
        public string GetEmailContactUs()
        {
            return _settings.Find(x => x.Key == SettingKey.EmailContactUs).Value;
        }
        public string GetEmailAdmin()
        {
            return _settings.Find(x => x.Key == SettingKey.EmailAdmin).Value;
        }
        public string GetEmailRental()
        {
            return _settings.Find(x => x.Key == SettingKey.EmailRental).Value;
        }
        public string GetEmailHelp()        {            return _settings.Find(x => x.Key == SettingKey.SmtpUserHelp).Value;        }        public string GetEmailMarketing()
        {            return _settings.Find(x => x.Key == SettingKey.SmtpUserMarketing).Value;        }
        public bool GetIsProduction()
        {
            return bool.Parse(_settings.Find(x => x.Key == SettingKey.IsProduction).Value);
        }
        public string GetHost()
        {
            return _settings.Find(x => x.Key == SettingKey.Host).Value;
        }
        public double GetBorrowerPaymentTransactionCommision()
        {
            return double.Parse(_settings.Find(x => x.Key == SettingKey.BorrowerPaymentTransactionCommisionKey).Value, CultureInfo.InvariantCulture);
        }
        public double GetSharerPaymentTransactionCommision()
        {
            return double.Parse(_settings.Find(x => x.Key == SettingKey.SharerPaymentTransactionCommisionKey).Value, CultureInfo.InvariantCulture);
        }
        public double GetCharity()
        {
            return double.Parse(_settings.Find(x => x.Key == SettingKey.Charity).Value, CultureInfo.InvariantCulture);
        }
        public double GetDiliveryPrice()
        {
            return double.Parse(_settings.Find(x => x.Key == SettingKey.DiliveryPrice).Value, CultureInfo.InvariantCulture);
        }
        public string GetPaymentTransactionCurrency()
        {
            return _settings.Find(x => x.Key == SettingKey.PaymentTransactionCurrencyKey).Value;
        }
        public IExternalFileStorageConfig GetExternalFileStorageConfig()
        {
            var exFileStorageConfig = new ExternalFileStorageConfig();
            if (_settings.Count > 0)
            {
                exFileStorageConfig = new ExternalFileStorageConfig()
                {
                    AccessKey = _settings.Find(x => x.Key == SettingKey.AwsS3AccessKey).Value,
                    SecretKey = _settings.Find(x => x.Key == SettingKey.AwsS3SecretKey).Value,
                    BucketName = _settings.Find(x => x.Key == SettingKey.AwsS3BucketName).Value,
                    PreSignedUrlTimeout = Convert.ToInt32(_settings.Find(x => x.Key == SettingKey.AwsS3PreSignedUrlTimeout).Value),
                };
            }
            return exFileStorageConfig;
        }
        public PayPalSetting GetPayPalSetting()
        {
            try
            {
                var configDictionary = new Dictionary<string, string>
                {
                    {"mode", _settings.Find(x => x.Key == SettingKey.PayPalModeKey).Value},
                    {
                        "connectionTimeout",
                        _settings.Find(x => x.Key == SettingKey.PayPalConnectionTimeoutKey).Value
                    },
                    {"requestRetries", _settings.Find(x => x.Key == SettingKey.PayPalRequestRetriesKey).Value},
                    {
                        "account1.apiUsername",
                        _settings.Find(x => x.Key == SettingKey.PayPalAccount1ApiUsernameKey).Value
                    },
                    {
                        "account1.apiPassword",
                        _settings.Find(x => x.Key == SettingKey.PayPalAccount1ApiPasswordKey).Value
                    },
                    {
                        "account1.apiSignature",
                        _settings.Find(x => x.Key == SettingKey.PayPalAccount1ApiSignatureKey).Value
                    },
                    {
                        "account1.applicationId",
                        _settings.Find(x => x.Key == SettingKey.PayPalAccount1ApiAppIdKey).Value
                    }
                };
                return new PayPalSetting
                {
                    ClientId = _settings.Find(x => x.Key == SettingKey.PayPalClientIdKey).Value,
                    ClientSecret = _settings.Find(x => x.Key == SettingKey.PayPalClientSecretKey).Value,
                    EmailId = _settings.Find(x => x.Key == SettingKey.PayPalEmailIdKey).Value,
                    Config = configDictionary
                };
            }
            catch (Exception ex)
            {
                //Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Get PayPal setting fail. Ex: {0}.", ex));
            }
            return new PayPalSetting();
        }
        public PinPaymentSetting GetPinPaymentSetting()
        {
            try
            {
                return new PinPaymentSetting
                {
                    TestMode =bool.Parse(_settings.Find(x => x.Key == SettingKey.PinPaymentTestModeKey).Value),
                    LivePublishKey = _settings.Find(x => x.Key == SettingKey.PinPaymentLivePublishKeyKey).Value,
                    LiveSecretKey = _settings.Find(x => x.Key == SettingKey.PinPaymentLiveSecretKeyKey).Value,
                    TestPublishKey= _settings.Find(x => x.Key == SettingKey.PinPaymentTestPublishKeyKey).Value,
                    TestSecretKey = _settings.Find(x => x.Key == SettingKey.PinPaymentTestSecretKeyKey).Value
                };
            }
            catch (Exception ex)
            {
                //Ioc.Get<IDbLogger>().LogWarning(LogSource.ImageProcessor, string.Format("Get PayPal setting fail. Ex: {0}.", ex));
            }
            return new PinPaymentSetting();
        }
        public List<IpAddressBlackList> GetIpAddressBlackList()
        {
            List<IpAddressBlackList> blackLists = new List<IpAddressBlackList>();
            if (!RepCache<IpAddressBlackList>.Loaded)
            {
                var res = Uow.Wrap(u =>
                {
                    blackLists = new RepCache<IpAddressBlackList>().List();
                });
            }
            else
            {
                blackLists = new RepCache<IpAddressBlackList>().List();
            }
            return blackLists;
        }
        public List<Dns> GetDnses()
        {
            List<Dns> dnses = new List<Dns>();
            if (!RepCache<Dns>.Loaded)
            {
                Uow.Wrap(u =>
                {
                    dnses = new RepCache<Dns>().List();
                });
            }
            else
            {
                dnses = new RepCache<Dns>().List();
            }
            return dnses;
        }
        public Dns GetCurrentDns()
        {
            var dnses = GetDnses();
            foreach (var dns in dnses)
            {
                if (dns.IsDefault)
                    return dns;
            }
            throw new Exception(string.Format("Cannot found default dns in the database"));
        }
        public MsmqProcessingParams GetMsmqProcessingParams()
        {
            return new MsmqProcessingParams()
            {
                FormatterType = QueueFormatter.Binary,
                QueueSendName = @".\Private$\tophands_img",
                QueueReceiveName = null
            };
        }
        public MsmqProcessingParams GetMsmqServerProcessingParams()
        {
            return new MsmqProcessingParams()
            {
                FormatterType = QueueFormatter.Binary,
                QueueSendName = null,
                QueueReceiveName = @".\Private$\tophands_img"
            };
        }
        public string GetImgFileServerUrl()
        {
            return _settings.Find(x => x.Key == SettingKey.ImgFileServerUrl).Value;
        }
        #region social
        public string GetGoogleClientId()
        {
           // return null;
            return _settings.Find(x => x.Key == SettingKey.GooglePlusId).Value;
        }
        public string GetGoogleClientSecret()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.GooglePlusSecret).Value;
        }
        public string GetFacebookClientId()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.FacebookId).Value;
        }
        public string GetFacebookClientSecret()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.FacebookSecret).Value;
        }
        public string GetVkClientId()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.VkId).Value;
        }
        public string GetVkClientSecret()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.VkSecret).Value;
        }
        #endregion
        public string GetBraintreeEnvironment()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.BraintreeEnvironment).Value;
        }
        public string GetBraintreeMasterMerchantAccountId()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.BraintreeMasterMerchantAccountId).Value;
        }
        public string GetBraintreeMerchantId()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.BraintreeMerchantId).Value;
        }
        public string GetBraintreePublicKey()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.BraintreePublicKey).Value;
        }
        public string GetBraintreePrivateKey()
        {
            //return null;
            return _settings.Find(x => x.Key == SettingKey.BraintreePrivateKey).Value;
        }
        public string GetBetaVersion()        {            return _settings.Find(x => x.Key == SettingKey.BetaVersionKey).Value;        }
    }
}