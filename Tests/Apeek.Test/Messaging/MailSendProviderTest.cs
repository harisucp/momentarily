using System;
using Apeek.Common;
using Apeek.Messaging.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Apeek.Messaging.Implementations.Email;
using Apeek.Messaging.Implementations;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
namespace Apeek.Test.Messaging
{
    [TestClass]
    public class MailSendProviderTest
    {
        public MailSendProviderTest()
        {                                  
            Ioc.Add(a => a.For<IMailSendProvider>().Use<MailSendProvider>());
            Ioc.Add(a => a.For<ISendProperty>().Use<SendProperty>());
            Ioc.Add(a => a.For<IMessage>().Use<Message>());              
            Ioc.Add(a => a.For<ISettingsDataService>().Use<SettingsDataService>());        
        }
        [TestMethod]
        public void MailSendTest()
        {            
            IMailSendProvider mailSendProvider = Ioc.Get<IMailSendProvider>();                        
            ISettingsDataService settingsDataService = Ioc.Get<ISettingsDataService>();
            ISendProperty sendProperty = settingsDataService.GetEmailSendProperty();
            IMessage message = Ioc.Get<IMessage>();
            message.FromHeader = new FromHeader()
            {
                FromName = "Zuber Max",
                From = "zuber.max.v@gmail.com",
                Subject = "Test email"
            };
            message.To = "yura.lobodzets@gmail.com";
            message.Body = "Test email.";
            Assert.IsTrue(mailSendProvider.Auth(sendProperty));
            Assert.IsTrue(mailSendProvider.SendMessage(message));            
            Assert.IsNotNull(mailSendProvider);
        }
    }
}
