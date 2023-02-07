using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using Sendy.Client.Model;
using Xunit;
using Apeek.Core.Services.Impl;
using Apeek.Common.Controllers;

namespace Sendy.Client.Tests
{
    public class SendyClientCreateCampaignTests
	{
	    private readonly MockHttpMessageHandler _httpMessageHandlerMock;
	    private readonly SendyClient _target;
        private string SubscriberListId = "hMFdFOy89j4lo14A6v5zaw";
        private string SignUpListId = "vt9kjw5GnqLH2iyCPZRZFA";

        public static object Config { get; private set; }

        public SendyClientCreateCampaignTests()
	    {
			var baseUri = new Uri("http://sendy.momentarily.com");
		    var apiKey = "aLQ19z2JRZ8goYekv5DM";
		    _httpMessageHandlerMock = new MockHttpMessageHandler();

		    var httpClient = _httpMessageHandlerMock.ToHttpClient();
		    httpClient.BaseAddress = baseUri;

		    _target = new SendyClient(baseUri, apiKey, null, httpClient);
		}

        //  [Fact]
        //  public async Task CreateCampaign_WithValidData_NoSend_ReturnsCampaignCreated()
        //  {
        //   //arrange
        //   var expectedResponse = "Campaign created";
        //var campaign = new Campaign
        //{
        //	BrandId = 1,
        //	FromEmail = "jeroen@klarenbeek.nl",
        //	FromName = "Jeroen",
        //	HtmlText = "<html><body><b>Hi</b></body></html>",
        //	PlainText = "Hi",
        //	Querystring = "querystring=sjaak",
        //	ReplyTo = "hank@klarenbeek.nl",
        //	Subject = "Subjectje",
        //	Title = "Title 1"
        //};

        //   var expectedPostData = new List<KeyValuePair<string, string>>
        //   {
        //    new KeyValuePair<string, string>("from_name", campaign.FromName),
        //    new KeyValuePair<string, string>("from_email", campaign.FromEmail),
        //    new KeyValuePair<string, string>("reply_to", campaign.ReplyTo),
        //    new KeyValuePair<string, string>("title", campaign.Title),
        //    new KeyValuePair<string, string>("subject", campaign.Subject),
        //    new KeyValuePair<string, string>("plain_text", campaign.PlainText),
        //    new KeyValuePair<string, string>("html_text", campaign.HtmlText),
        //    new KeyValuePair<string, string>("brand_id", campaign.BrandId.ToString()),
        //    new KeyValuePair<string, string>("query_string", campaign.Querystring)
        //   };

        //   _httpMessageHandlerMock.Expect("/api/campaigns/create.php")
        //    .WithFormData(expectedPostData)
        //    .Respond("text/plain", expectedResponse);

        //   //act
        //   var result = await _target.CreateCampaignAsync(campaign, false, null);

        ////assert
        //   _httpMessageHandlerMock.VerifyNoOutstandingExpectation();
        //   Assert.True(result.IsSuccess);
        //   Assert.Equal(expectedResponse, result.Response);
        //  }

        //[Fact]
        //public async Task CreateCampaign_WithValidData_AndSend_ReturnsCampaignCreatedAndSending()
        //{
        //	//arrange
        //	var expectedResponse = "Campaign created and now sending";
        //	var listIds = new List<string> {"listId"};
        //	var campaign = new Campaign
        //	{
        //		BrandId = 1,
        //		FromEmail = "jeroen@klarenbeek.nl",
        //		FromName = "Jeroen",
        //		HtmlText = "<html><body><b>Hi</b></body></html>",
        //		PlainText = "Hi",
        //		Querystring = "querystring=sjaak",
        //		ReplyTo = "hank@klarenbeek.nl",
        //		Subject = "Subjectje",
        //		Title = "Title 1"
        //	};

        //	var expectedPostData = new List<KeyValuePair<string, string>>
        //	{
        //		new KeyValuePair<string, string>("from_name", campaign.FromName),
        //		new KeyValuePair<string, string>("from_email", campaign.FromEmail),
        //		new KeyValuePair<string, string>("reply_to", campaign.ReplyTo),
        //		new KeyValuePair<string, string>("title", campaign.Title),
        //		new KeyValuePair<string, string>("subject", campaign.Subject),
        //		new KeyValuePair<string, string>("plain_text", campaign.PlainText),
        //		new KeyValuePair<string, string>("html_text", campaign.HtmlText),
        //		new KeyValuePair<string, string>("brand_id", campaign.BrandId.ToString()),
        //		new KeyValuePair<string, string>("query_string", campaign.Querystring),
        //		new KeyValuePair<string, string>("send_campaign", "1"),
        //		new KeyValuePair<string, string>("list_ids", string.Join(",", listIds))

        //};

        //	_httpMessageHandlerMock.Expect("/api/campaigns/create.php")
        //		.WithFormData(expectedPostData)
        //		.Respond("text/plain", expectedResponse);

        //	//act
        //	var result = await _target.CreateCampaignAsync(campaign, true, new Groups(listIds));

        //	//assert
        //	_httpMessageHandlerMock.VerifyNoOutstandingExpectation();
        //	Assert.True(result.IsSuccess);
        //	Assert.Equal(expectedResponse, result.Response);
        //}

        [Fact]
        public async Task CreateCampaign_WithValidData_AndSend_ReturnsCampaignCreatedAndSending()
        {
            //arrange
            var expectedResponse = "Campaign created and now sending";
            var listIds = new List<string> { SignUpListId };
           var html = MessageTemplate.GetMailTemplate(LanguageController.CurLang, "invoice");
            var campaign = new Campaign
            {
                BrandId = 1,
                FromEmail = "hello@momentarily.com",
                FromName = "Momentarily Team",
                HtmlText = html,
                PlainText = "Hi",
                //Querystring = "querystring=sjaak",
                ReplyTo = "hello@momentarily.com",
                Subject = "Momentarily",
                Title = "Information"
            };

            var expectedPostData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("from_name", campaign.FromName),
                new KeyValuePair<string, string>("from_email", campaign.FromEmail),
                new KeyValuePair<string, string>("reply_to", campaign.ReplyTo),
                new KeyValuePair<string, string>("title", campaign.Title),
                new KeyValuePair<string, string>("subject", campaign.Subject),
                new KeyValuePair<string, string>("plain_text", campaign.PlainText),
                new KeyValuePair<string, string>("html_text", campaign.HtmlText),
                new KeyValuePair<string, string>("brand_id", campaign.BrandId.ToString()),
                //new KeyValuePair<string, string>("query_string", campaign.Querystring),
                new KeyValuePair<string, string>("send_campaign", "1"),
                new KeyValuePair<string, string>("list_ids", string.Join(",", listIds))

        };

            _httpMessageHandlerMock.Expect("/api/campaigns/create.php")
                .WithFormData(expectedPostData)
                .Respond("text/plain", expectedResponse);

            //act
            var result = await _target.CreateCampaignAsync(campaign, true, new Groups(listIds));

            //assert
            _httpMessageHandlerMock.VerifyNoOutstandingExpectation();
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedResponse, result.Response);
        }


    }
}
