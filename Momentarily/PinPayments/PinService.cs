using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PinPayments.Models;
using System.Net;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using PinPayments.Infrastructure;
using PinPayments.Actions;
namespace PinPayments
{
    public class PinService
    {
        private bool LiveMode { get; set; }
        private Urls _urls;
        public PinService()
        {
        }
        public PinService(string pinKey)
        {
            PinPaymentsConfig.SetApiKey(pinKey);
        }
        public Urls Urls
        {
            get { return _urls; }
            set { _urls = value; }
        }
        public CardCreateResponse CardCreate(Card c)
        {
            var url = Urls.Card;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");
            var response = Requestor.PostString(url, postData);
            return JsonConvert.DeserializeObject<CardCreateResponse>(response);
        }
        public Charges Charges()
        {
            var url = Urls.Charges;
            var response = Requestor.GetString(url);
            var result = JsonConvert.DeserializeObject<Charges>(response);
            return result;
        }
        public Charges CustomerCharges(string customerToken)
        {
            var url = Urls.CustomerCharges.Replace("{token}", customerToken);
            var response = Requestor.GetString(url);
            var result = JsonConvert.DeserializeObject<Charges>(response);
            return result;
        }
        public ChargeDetail Charge(string token)
        {
            var url = Urls.Charges + token;
            var response = Requestor.GetString(url);
            var result = JsonConvert.DeserializeObject<ChargeDetail>(response);
            return result;
        }
        public ChargeResponse Charge(PostCharge c)
        {
            var url = Urls.Charge;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");
            if (c.Card != null)
            {
                postData += "&card[number]=" + c.Card.CardNumber + "&card[expiry_month]=" + c.Card.ExpiryMonth + "&card[expiry_year]=" + c.Card.ExpiryYear + "&card[cvc]=" + c.Card.CVC + "&card[name]=" + c.Card.Name;
                postData += "&card[address_line1]=" + c.Card.Address1 + "&card[address_line2]=" + c.Card.Address2 + "&card[address_city]=" + c.Card.City + "&card[address_postcode]=" + c.Card.Postcode;
                postData += "&card[address_state]=" + c.Card.State + "&card[address_country]=" + c.Card.Country;
            }
            else if (c.CustomerToken != null)
            {
                postData += "&customer_token=" + c.CustomerToken;
            }
            else if (c.CardToken != null)
            {
                postData += "&card_token=" + c.CardToken;
            }
            else
            {
                throw new PinException(HttpStatusCode.BadRequest, null, "You need to supply either the Card, the Customer Token or a Card Token for payment");
            }
            var response = Requestor.PostString(url, postData);
            return JsonConvert.DeserializeObject<ChargeResponse>(response);
        }
        public Charges ChargesSearch(ChargeSearch cs, int? page)
        {
            var url = ParameterBuilder.ApplyAllParameters(cs, Urls.ChargesSearch);
            if (page != null)
            {
                url += "&page=" + page.ToString();
            }
            var response = Requestor.GetString(url);
            return JsonConvert.DeserializeObject<Charges>(response);
        }
        public CustomerAdd CustomerAdd(Customer c)
        {
            var url = Urls.CustomerAdd;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");
            var response = Requestor.PostString(url, postData);
            var customerAdd = JsonConvert.DeserializeObject<CustomerAdd>(response);
            return customerAdd;
        }
        public CustomerUpdate CustomerUpate(Customer c)
        {
            var url = Urls.CustomerAdd + "/" + c.Token;
            var postData = ParameterBuilder.ApplyAllParameters(c, "");
            var response = Requestor.PutString(url, postData);
            var result = JsonConvert.DeserializeObject<CustomerUpdate>(response);
            return result;
        }
        public Customers Customers()
        {
            return Customers(null);
        }
        public Customers Customers(int? page)
        {
            var url = Urls.Customers;
            if (page != null)
            {
                url += "?page=" + page.ToString();
            }
            var response = Requestor.GetString(url);
            var result = JsonConvert.DeserializeObject<Customers>(response);
            return result;
        }
        public CardCreateResponse AddCard(string customerToken, string cardToken)
        {
            var url = Urls.CastomerCards;
            var response = Requestor.PostString(url.Replace("{token}", customerToken), "card_token=" + cardToken);
            var result = JsonConvert.DeserializeObject<CardCreateResponse>(response);
            return result;
        }
        public ChargeResponse Capture(string chargeToken)
        {
            var url = Urls.Capture;
            var response = Requestor.PutString(url.Replace("{token}", chargeToken), "");
            var result = JsonConvert.DeserializeObject<ChargeResponse>(response);
            return result;
        }
        public RefundResponse Refund(string chargeToken, int amount)
        {
            var url = Urls.Refund;
            var response = Requestor.PostString(url.Replace("{token}", chargeToken), "amount=" + amount.ToString());
            var result = JsonConvert.DeserializeObject<RefundResponse>(response);
            return result;
        }
        public RefundsResponse Refunds(string chargeToken)
        {
            var url = Urls.Refund;
            var response = Requestor.GetString(url.Replace("{token}", chargeToken));
            var result = JsonConvert.DeserializeObject<RefundsResponse>(response);
            return result;
        }
        public Customer Customer(string token)
        {
            var url = Urls.Customers + "/" + token;
            var response = Requestor.GetString(url);
            var customer = JsonConvert.DeserializeObject<CustomerAdd>(response);
            return customer.Response;
        }
        public RecipientAdd PostRecipient(RecipientPost recipient)
        {
            var url = Urls.Recipients;
            var postData = ParameterBuilder.ApplyAllParameters(recipient, "");
            if (recipient.Bank != null)
            {
                postData += "&bank_account[name]=" + recipient.Bank.Name + "&bank_account[bsb]=" + recipient.Bank.BSB +
                            "&bank_account[number]=" + recipient.Bank.Number;
            }
            else if (!String.IsNullOrWhiteSpace(recipient.BankToken))
            {
                postData += "&bank_account_token=" + recipient.BankToken;
            }
            var response = Requestor.PostString(url, postData);
            var recipientAdd = JsonConvert.DeserializeObject<RecipientAdd>(response);
            return recipientAdd;
        }
        public RecipientResponse UpdateRecipient(RecipientPost recipient, string token)
        {
            var url = Urls.Recipients+"/" + token;
            var postData = ParameterBuilder.ApplyAllParameters(recipient, "");
            if (recipient.Bank != null)
            {
                postData += "&bank_account[name]=" + recipient.Bank.Name + "&bank_account[bsb]=" + recipient.Bank.BSB +
                            "&bank_account[number]=" + recipient.Bank.Number;
            }
            else if (!String.IsNullOrWhiteSpace(recipient.BankToken))
            {
                postData += "&bank_account_token=" + recipient.BankToken;
            }
            var response = Requestor.PutString(url, postData);
            var recipientResponse = JsonConvert.DeserializeObject<RecipientResponse>(response);
            return recipientResponse;
        }
        public RecipientResponse Recipient(string token)
        {
            var url = Urls.Recipients+"/"+token;
            var response = Requestor.GetString(url);
            var recipientAdd = JsonConvert.DeserializeObject<RecipientResponse>(response);
            return recipientAdd;
        }
        public TransferAdd PostTransfer(TransferPost transfer)
        {
            var url = Urls.Transfer;
            var postData = ParameterBuilder.ApplyAllParameters(transfer, "");
            var response = Requestor.PostString(url, postData);
            var transferAdd = JsonConvert.DeserializeObject<TransferAdd>(response);
            return transferAdd;
        }
    }
}
