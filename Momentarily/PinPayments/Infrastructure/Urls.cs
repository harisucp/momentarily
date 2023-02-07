using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace PinPayments.Infrastructure
{
    public class Urls
    {
        private string _baseUrl;
        public Urls(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public string Card
        {
            get { return BaseUrl + "/1/cards"; }
        }
        public string ChargesSearch
        {
            get { return BaseUrl + "/1/charges/search"; }
        }
        public string Charge
        {
            get { return BaseUrl + "/1/charges"; }
        }
        public string Charges
        {
            get { return BaseUrl + "/1/charges/"; }
        }
        public  string CustomerAdd
        {
            get { return BaseUrl + "/1/customers"; }
        }
        public  string Customers
        {
            get { return BaseUrl + "/1/customers"; }
        }
        public  string CustomerCharges
        {
            get { return BaseUrl + "/1/customers/{token}/charges"; }
        }
        public  string Refund
        {
            get { return BaseUrl + "/1/charges/{token}/refunds"; }
        }
        public string Capture
        {
            get { return BaseUrl + "/1/charges/{token}/capture"; }
        }
        public string CastomerCards
        {
            get { return BaseUrl + "/1/customers/{token}/cards"; }
        }
        public string Recipients
        {
            get { return BaseUrl + "/1/recipients"; }
        }
        public string Transfer
        {
            get { return BaseUrl + "/1/transfers"; }
        }
        private  string BaseUrl
        {
            get { return _baseUrl; }
        }
    }
}
