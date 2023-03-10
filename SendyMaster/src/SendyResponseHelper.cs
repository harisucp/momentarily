using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sendy.Client.Model;

namespace Sendy.Client
{
	/// <summary>
	/// The Sendy API has an interesting response system. We'd like to know if things were successful or not. This is only possible
	/// based on the response text. This can differ per action, this class tries to translate the response into a more useful reply (hopefully :-))
	/// </summary>
    internal class SendyResponseHelper
    {
	    internal enum SendyActions
	    {
		    Subscribe,
		    Unsubscribe,
		    DeleteSubscriber,
		    SubscriptionStatus,
		    ActiveSubscriberCount,
		    CreateCampaign,
			CreateList
	    }

		/// <summary>
		/// List of actions that have a useful content in case of success, like 'Campaign created'.
		/// </summary>
		private static readonly List<SendyActions> ActionsWithUsefulResponseContent = new List<SendyActions> { SendyActions.SubscriptionStatus, SendyActions.ActiveSubscriberCount, SendyActions.CreateCampaign, SendyActions.CreateList };

	    internal static async Task<SendyResponse> HandleResponse(HttpResponseMessage responseMessage, SendyActions action)
	    {
		    if (responseMessage.IsSuccessStatusCode)
		    {
				var responseContent = await responseMessage.Content.ReadAsStringAsync();
				responseContent = responseContent.Trim();
			    var isSuccess = IsSuccess(responseContent, action);

			    return new SendyResponse
			    {
				    IsSuccess = isSuccess,
				    ErrorMessage = !isSuccess ? responseContent : "",
					Response = isSuccess && ActionsWithUsefulResponseContent.Contains(action) ? responseContent : ""
			    };
		    }
		    return new SendyResponse
		    {
			    IsSuccess = false,
			    ErrorMessage =
				    $"Problem while connecting to your Sendy installation. Statuscode {responseMessage.StatusCode}, error: {responseMessage.Content}"
		    };
	    }

	    private static bool IsSuccess(string responseContent, SendyActions action)
	    {
		    switch (action)
		    {
			    case SendyActions.Subscribe:
			    case SendyActions.Unsubscribe:
			    case SendyActions.DeleteSubscriber:
				    return responseContent == "1";

			    case SendyActions.SubscriptionStatus:
				    var successContent = new List<string> { "Subscribed", "Unsubscribed", "Unconfirmed", "Bounced", "Soft bounced", "Complained" };
				    return successContent.Contains(responseContent);

			    case SendyActions.ActiveSubscriberCount:
			    case SendyActions.CreateList:
				    return int.TryParse(responseContent, out int dummy);

			    case SendyActions.CreateCampaign:
				    successContent = new List<string> { "Campaign created", "Campaign created and now sending" };
				    return successContent.Contains(responseContent);
			    default:
				    throw new ArgumentOutOfRangeException(nameof(action), action, null);
		    }
	    }
	}
}
