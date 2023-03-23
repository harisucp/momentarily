using NUnit.Framework;using Momentarily.Core.Service.Impl;using Apeek.Messaging.Interfaces;using Apeek.Messaging.Implementations;using Twilio;using Twilio.Rest.Api.V2010.Account;using System.Net;using System;using System.Web;namespace Momentarily.Test.Core.Service.Impl{    [TestFixture]    public class TwilioSmsSendProviderTest    {
        //[Test]
        //public void SmsSendTrialAccountTest()
        //{
        //    var twilioSmsSendProvider = new TwilioSmsSendProvider();
        //    twilioSmsSendProvider.Auth(new SendProperty()
        //    {
        //        Login = "AC26e127702e116cd59adc355037439885",
        //        Pwd = "0d3e3e52ae584c6e3075d6c284f09b9b"                
        //    });
        //    var result = twilioSmsSendProvider.SendMessage(new Message() {
        //        FromHeader = new FromHeader()
        //        {
        //             From = "+15005550006"
        //        },
        //        To = "+380638344889",
        //        Body = "Hello from Momentarily"
        //    });            
        //    Assert.IsTrue(result);
        //}
        //public void SendOTP()
        //{
        //    var twilioSmsSendProvider = new TwilioSmsSendProvider();
        //    twilioSmsSendProvider.Auth(new SendProperty()
        //    {
        //        Login = "AC4378b5f34c9ab9ef0f27c4d848569478",
        //        Pwd = "d6b6048d5de5093ad331de2290da832b"
        //    });
        //    var result = twilioSmsSendProvider.SendMessage(new Message()
        //    {
        //        FromHeader = new FromHeader()
        //        {
        //            From = "+12017334677"
        //        },
        //        To = "+9100098043296",
        //        Body = "Hello from Momentarily"
        //    });
        //    Assert.IsTrue(result);
        //}


        public bool SendOTP(string OTP, string phonenumber, string vc, string countrycode)        {            try            {                const string accountSid = "AC3e8cc97b9e49c3d698f747ea2292dfb9";                const string authToken = "168bdbfacaff52583d76b7c2610675cc";                var url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +               HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/User/VerifyMobileLink?vc=" + vc;                TwilioClient.Init(accountSid, authToken);                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls                                                    | SecurityProtocolType.Tls11                                                    | SecurityProtocolType.Tls12                                                    | SecurityProtocolType.Ssl3;                var message = MessageResource.Create(                   body: "Hello, in order to activate your account on momentarily, please enter  " + OTP + " , or click on this link to activate   " + url,
                   //from: new Twilio.Types.PhoneNumber("+12017334677"),
                   from: new Twilio.Types.PhoneNumber("+18884030490"),                   to: new Twilio.Types.PhoneNumber("+" + countrycode + phonenumber)
                );                return true;            }            catch (Exception ex)            {                return false;            }        }        public string GenerateOTP()        {            string characters = "1234567890";            string otp = string.Empty;            for (int i = 0; i < 8; i++)            {                string character = string.Empty;                do                {                    int index = new Random().Next(0, characters.Length);                    character = characters.ToCharArray()[index].ToString();                } while (otp.IndexOf(character) != -1);                otp += character;            }            return otp;        }    }}