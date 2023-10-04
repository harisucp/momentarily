using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.Entities.Entities.Enums;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;


namespace Apeek.Core.Services.Impl
{
	public class TwilioNotificationService : LangDataService, ITwilioNotificationService
	{
		private string _accountSid = "AC3e8cc97b9e49c3d698f747ea2292dfb9";
		private string _authToken = "168bdbfacaff52583d76b7c2610675cc";
		private string _phoneNumber = "+18884030490";
		private readonly IRepositoryTwilioMessage _repTwilio;
		private readonly IRepositoryTwilioLogMessages _repoTwilioLogMessages;
		private readonly IRepositoryUser _repUser;

		public TwilioNotificationService(IRepositoryTwilioMessage repTwilio, IRepositoryTwilioLogMessages repoTwilioLogMessages, IRepositoryUser repositoryUser)
		{
			_repTwilio = repTwilio;
			_repoTwilioLogMessages = repoTwilioLogMessages;
			_repUser = repositoryUser;
		}
		// Service created by mubeen 
		//public bool SendMessage(string text, string phoneNumber, string countryCode, string verificationCode, string OTP)
		//{
		//	try
		//	{
		//		var url = "";
		//		if (verificationCode != null)
		//		{
		//			url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
		//			HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/User/VerifyMobileLink?vc=" + verificationCode;
		//			string anchor = "<a href='" + url + "'>Verify Me</a>";
		//			url = anchor;

		//		}

		//		else
		//		{
		//			url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
		//			HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Account/Unsubscribe";
		//			string anchors = "<a href='" + url + "'>Unsubscribe</a>";
		//			url = anchors;   

		//		}

		//		TwilioClient.Init(_accountSid, _authToken);
		//		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
		//											| SecurityProtocolType.Tls11
		//											| SecurityProtocolType.Tls12
		//											| SecurityProtocolType.Ssl3;
		//		var message = MessageResource.Create(
		//			body: text + "       " + url,
		//			from: new Twilio.Types.PhoneNumber(_phoneNumber),
		//			to: new Twilio.Types.PhoneNumber("+" + countryCode + phoneNumber)
		//		);
		//		return true;
		//	}

		//	catch (Exception ex)
		//	{
		//		return false;
		//	}
		//}


		public bool SendMessage(string text, string phoneNumber, string countryCode, string verificationCode, string OTP)
        {
            try
            {
                var url = "";
                if (verificationCode != null)
                {
                    url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/User/VerifyMobileLink?vc=" + verificationCode;
                }

                else
                {
                    url = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                        HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/Account/Unsubscribe";
                }

                TwilioClient.Init(_accountSid, _authToken);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                       | SecurityProtocolType.Tls11
                                                       | SecurityProtocolType.Tls12
                                                       | SecurityProtocolType.Ssl3;
                var message = MessageResource.Create(
                    body: text + " " + "<a href='" + url + "'>Link</a>", //" [Link](" + url + ")", //text + " " + url,
					from: new Twilio.Types.PhoneNumber(_phoneNumber),
                    to: new Twilio.Types.PhoneNumber("+" + countryCode + phoneNumber)
                );
                return true;
            }

            catch (Exception ex)
            {
                return false;
            }
        }



        public bool BookingConfirmation(string phoneNumber, string countryCode, int userId, string ItemName, DateTime bookingTime)
		{
			var value = ConvertEnumToString(TwilioMessageType.BookingConfirmation);
			value = value.Insert(17, ItemName);
			value = value.Replace("##########", bookingTime.ToString("dd/MM/yyyy"));
			value = value.Replace("##########", bookingTime.ToString("hh:mm:ss tt"));
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.BookingConfirmation);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}




		public bool PaymentConfirmation(string phoneNumber, string countryCode, int userId, string amount, string itemName)
		{
			var value = ConvertEnumToString(TwilioMessageType.PaymentConfirmation);
			value = value.Replace("###", amount);
			value = value.Insert(19, " " + itemName + " ");
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{

				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.PaymentConfirmation);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}
		public bool DeliveryStatus(string phoneNumber, string countryCode, int userId)
		{
			var value = ConvertEnumToString(TwilioMessageType.DeliveryStatus);
			value = value.Insert(5, "Item Name");
			value = value.Replace("########", "Time of Delivery");
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.DeliveryStatus);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}
		public bool PickupStatus(string phoneNumber, string countryCode, int userId)
		{
			var value = ConvertEnumToString(TwilioMessageType.PickupStatus);
			value = value.Insert(5, "Item Name");
			value = value.Replace("########", "Time of PickUp");
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.PickupStatus);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}
		public bool CancellationAlert(string phoneNumber, string countryCode, int userId, string itemName, DateTime dateTime)
		{
			var value = ConvertEnumToString(TwilioMessageType.CancellationAlert);
			value = value.Insert(17, itemName);
			value = value.Replace("##########", dateTime.ToString("dd/MM/yyyy"));
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.CancellationAlert);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}
		public bool RentalDueDate(string phoneNumber, string countryCode, int userId)
		{
			var value = ConvertEnumToString(TwilioMessageType.RentalDueDate);
			value = value.Insert(26, "Item Name");
			value = value.Replace("#####", "Date");
			value = value.Replace("####", "Time");
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.RentalDueDate);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(TwilioMessageType.RentalDueDate.ToString(), phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}

		//Not Implemented Yet from here
		public bool LastMinuteChangesAndUpdates(string phoneNumber, string countryCode, int userId)
		{
			var value = ConvertEnumToString(TwilioMessageType.PaymentConfirmation);
			value = value.Insert(17, "Item Name");
			value = value.Replace("########", "DateTime");
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.LastMinuteChangesAndUpdates);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(TwilioMessageType.LastMinuteChangesAndUpdates.ToString(), phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}
		// to here
		public bool UpcomingAppointmentsForPickup(string phoneNumber, string countryCode, int userId)
		{
			var value = ConvertEnumToString(TwilioMessageType.UpcomingAppointmentsForPickup);
			value = value.Insert(25, "Item Name");
			value = value.Replace("##########", "DateTime");
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.UpcomingAppointmentsForPickup);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}
		public bool UpcomingAppointmentsForDelivery(string phoneNumber, string countryCode, int userId)
		{
			var value = ConvertEnumToString(TwilioMessageType.UpcomingAppointmentsForDelivery);
			value = value.Insert(25, "Item Name");
			value = value.Replace("##########", "DateTime");
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.UpcomingAppointmentsForDelivery);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
					return true;
				}
			}
			return false;
		}

		public bool TemporarySecurityCode(string phoneNumber, string countryCode, int userId)
		{
			var securityCode = GenerateOTP();
			var value = ConvertEnumToString(TwilioMessageType.TemporarySecurityCode);
			value = value.Replace("########", securityCode);
			User user = GetUser(userId);
			if (user != null && !user.GeneralUpdate)
			{
				var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.TemporarySecurityCode);
				if (messageUpdatedOrNot)
				{
					var result = SendMessage(value, phoneNumber, countryCode, null, null);
					if (result)
					{
						SaveTwilioLogMessage(userId, value);
					}
				}
				return true;
			}
			return false;
	}


	public bool SendOTPVerificationCode(string phoneNumber, string countryCode, string verificationCode, string OTP, int userId)
	{
		var value = ConvertEnumToString(TwilioMessageType.SendOTPVerificationCode);
		value = value.Replace("########", OTP);
		User user = GetUser(userId);
		if (user != null && user.GeneralUpdate)
		{
			var messageUpdatedOrNot = UpdateTwilioMessage(userId, TwilioMessageType.SendOTPVerificationCode);

			if (messageUpdatedOrNot)
			{
				var result = SendMessage(value, phoneNumber, countryCode, verificationCode, OTP);
				if (result)
				{
					SaveTwilioLogMessage(userId, value);
				}
			}
				return true;
			}

		return false;
	}


	public bool UpdateTwilioMessage(int userId, TwilioMessageType messageType)
	{
		try
		{
			var getTodayAvailibility = false;
			Uow.Wrap(uow =>
			{
				getTodayAvailibility = _repTwilio.CheckTodayMessageAvailibilty(DateTime.UtcNow);
				if (getTodayAvailibility)
				{
					var twilioMessage = new TwilioMessage
					{
						Status = true,
						TwilioMessagesText = messageType,
						CreateDate = DateTime.Now,
						CreateBy = userId,
						UserId = userId,
						ModDate = DateTime.Now,
						ModBy = userId
					};
					_repTwilio.Save(twilioMessage);
				}
				else
				{
					var twilioMessage = new TwilioMessage
					{
						Status = false,
						TwilioMessagesText = messageType,
						CreateDate = DateTime.Now,
						CreateBy = userId,
						UserId = userId,
						ModDate = DateTime.Now,
						ModBy = userId
					};
					_repTwilio.Save(twilioMessage);
				}
			}, null, LogSource.TwilioNotificationService);
			if (getTodayAvailibility == true) return true;
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}


	public string GenerateOTP()
	{
		string characters = "1234567890";
		string otp = string.Empty;
		for (int i = 0; i < 8; i++)
		{
			string character = string.Empty;
			do
			{
				int index = new Random().Next(0, characters.Length);
				character = characters.ToCharArray()[index].ToString();
			} while (otp.IndexOf(character) != -1);
			otp += character;
		}
		return otp;
	}

	#region Private Methods
	private string ConvertEnumToString(TwilioMessageType twilioMessageType)
	{
		var value = "";
		Enum messsage = twilioMessageType;
		FieldInfo fieldInfo = messsage.GetType().GetField(messsage.ToString());
		EnumMemberAttribute enumMemberAttribute = (EnumMemberAttribute)fieldInfo
						.GetCustomAttribute(typeof(EnumMemberAttribute));
		if (enumMemberAttribute != null)
		{
			value = enumMemberAttribute.Value;
		}
		return value;
	}

	private bool SaveTwilioLogMessage(int userId, string message)
	{
		try
		{
			Uow.Wrap(uow =>
			{
				var twilioLogMessage = new TwilioLogMessage
				{
					UserId = userId,
					ModBy = userId,
					ModDate = DateTime.Now,
					Message = message,
					CreateBy = userId,
					CreateDate = DateTime.Now
				};
				_repoTwilioLogMessages.Save(twilioLogMessage);
			}, null, LogSource.TwilioNotificationService);
			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
	}


private User GetUser(int userId)
{
	try
	{
		User user = null;

		Uow.Wrap(uow =>
		{
			user = _repUser.GetUser(userId);
		});

		return user;
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
		return null;
	}
}

	#endregion
}
}
