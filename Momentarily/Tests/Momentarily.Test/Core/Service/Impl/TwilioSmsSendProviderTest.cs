﻿using NUnit.Framework;
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


        public bool SendOTP(string OTP, string phonenumber, string vc, string countrycode)
                   //from: new Twilio.Types.PhoneNumber("+12017334677"),
                   from: new Twilio.Types.PhoneNumber("+18888028065"),