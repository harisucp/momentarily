//using Apeek.Common;
//using Apeek.Common.Definitions;
//using Apeek.Common.Logger;
//using Apeek.Core.Services;
//using Apeek.Core.Services.Impl;
//using Apeek.Entities.Entities;
//using Apeek.ViewModels.Models;
//using Apeek.Web.Framework.ControllerHelpers;
//using Apeek.Web.Framework.Controllers;
//using Momentarily.ViewModels.Models;
//using PayPal.Api;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Momentarily.Web.Areas.Frontend.Controller
//{
//    public class covidController : FrontendController
//    {
//        private readonly CovidControllerHelper _helper;//        private readonly AccountControllerHelper<IRegisterModel> _accountHelper;
//        private string SubscriberListId = "hMFdFOy89j4lo14A6v5zaw";
//        private readonly ISendMessageService _emailMessageService;
//        public covidController(CovidControllerHelper helper)
//        {
//            _helper = helper;
//            _accountHelper = new AccountControllerHelper<IRegisterModel>();
//            _emailMessageService = new SendMessageService();
//        }

//        // GET: Frontend/covid
//        public ActionResult Index()
//        {
//            var covidGoods = _helper.GetAllCovidGoods();
//            return View(covidGoods);
//        }

//        public ActionResult PlaceOrder(int? id)
//        {
//            if (id != null && id > 0)
//            {
//                var covidGood = _helper.GetCovidGood(Convert.ToInt32(id));
//                return View(covidGood);
//            }
//            return RedirectToAction("Index");
//        }
//        [HttpPost]
//        public ActionResult PlaceOrder(CovidGoodViewModel model)
//        {
//            try//            {
//                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Place order Started for:-  " + model.BuyerEmailId + " /covidgoodid:- " + model.CovidGoodId));
//                TempData["model"] = model;
//                APIContext apiContext = PaypalConfiguration.GetAPIContext();//                string payerId = Request.Params["PayerID"];//                if (string.IsNullOrEmpty(payerId))//                {
//                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +//                                "/covid/pay?";//                    string cancelURI = Request.Url.Scheme + "://" + Request.Url.Authority +//                               "/covid/OrderCancelled";//                    var guid = Convert.ToString((new Random()).Next(100000));//                    var createdPayment = CreatePayment(apiContext, baseURI + "guid=" + guid, cancelURI, model);//                    var links = createdPayment.links.GetEnumerator();//                    string paypalRedirectUrl = null;//                    while (links.MoveNext())//                    {//                        Links lnk = links.Current;//                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))//                        {//                            paypalRedirectUrl = lnk.href;//                        }//                    }//                    Session.Add(guid, createdPayment.id);//                    return Redirect(paypalRedirectUrl);//                }
//                else
//                {

//                    return View(model);
//                }
//            }//            catch (Exception ex)//            {
//                Ioc.Get<IDbLogger>().LogMessage(LogSource.HealthDataHelper, string.Format("Place order failed for:-  " + model.BuyerEmailId + " /covidgoodid:- " + model.CovidGoodId));
//                throw ex;//            }
//        }

//        [HttpGet]
//        public ActionResult pay()
//        {
//            try
//            {
//                string payerId = Request.Params["PayerID"];
//                var model = (CovidGoodViewModel)TempData["model"];
//                OrderPayResult result = new OrderPayResult();
//                APIContext apiContext = PaypalConfiguration.GetAPIContext();
//                var guid = Request.Params["guid"];
//                var paymentId = Session[guid] as string;
//                var paymentExecution = new PaymentExecution() { payer_id = payerId };
//                var payment = new Payment() { id = paymentId };
//                // Execute authorization.
//                var executedPayment = payment.Execute(apiContext, paymentExecution);// Execute the payment
//                if (executedPayment.state.ToLower() == "approved")
//                {
//                    model.StatusId = (int)CovidOrderStatus.Paid;
//                    var savedCovidGoodOrder = _helper.SaveCovidGoodOrder(model);
//                    if (savedCovidGoodOrder != null)
//                    {
//                        CovidOrderPaymentDetailViewModel order = new CovidOrderPaymentDetailViewModel();
//                        order.CovidOrderId = savedCovidGoodOrder.Id;
//                        order.PayId = executedPayment.id;
//                        order.Intent = executedPayment.intent;
//                        order.State = executedPayment.state;
//                        order.CreateTime = executedPayment.create_time;
//                        order.UpdateTime = executedPayment.update_time;
//                        order.Cart = executedPayment.cart;
//                        if (executedPayment.transactions != null && executedPayment.transactions.Count > 0)
//                        {
//                            order.Amount = executedPayment.transactions[0].amount.total;
//                            order.Description = executedPayment.transactions[0].description;
//                            order.InvoiceNumber = executedPayment.transactions[0].invoice_number;
//                        }
//                        if (executedPayment.payer != null)
//                        {
//                            order.PayerEmail = executedPayment.payer.payer_info.email;
//                            order.PayerId = executedPayment.payer.payer_info.payer_id; 
//                        }
//                        var savedCovidOrderPaymentDetail = _helper.SaveCovidOrderPaymentDetail(order);

//                        var sendemailRentalReciept = _emailMessageService.SendPaymentEmailTemplateCovidRentalInvoiceDetail(model,order);

//                        if (model.IgnoreMarketingMails == false)
//                        {
//                            bool checkExsistSubscriber = _accountHelper.ExsistSubscriberOnlyEmail(model.BuyerEmailId);
//                            _accountHelper.subscribeEmail(SubscriberListId, model.BuyerEmailId, "", true);
//                            if (!checkExsistSubscriber)
//                            {
//                                bool saveSubscriber = _accountHelper.saveSubscriber(model.BuyerEmailId);
//                            }
//                            else
//                            {
//                                _accountHelper.UpdateSubscriberUnsubscriber(model.BuyerEmailId, model.IgnoreMarketingMails);
//                            }
//                        }
//                        else
//                        {
//                            _accountHelper.unsubscribeEmail(SubscriberListId, model.BuyerEmailId, "", true);
//                            _accountHelper.UpdateSubscriberUnsubscriber(model.BuyerEmailId, model.IgnoreMarketingMails);
//                        }


//                    }
//                    result.IsError = false;
//                    result.Message = "Payment Successful";
//                    result.CovidGoodId = model.CovidGoodId;
//                    result.MailSend = false;
//                    return View(result);
//                }
//                else
//                {
//                    model.StatusId = (int)CovidOrderStatus.Failed;
//                    var savedCovidGood = _helper.SaveCovidGoodOrder(model);
//                    result.IsError = true;
//                    result.Message = "Failed";
//                    result.CovidGoodId = model.CovidGoodId;
//                    result.MailSend = false;
//                    return View(result);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }


//        [HttpGet]
//        public ActionResult OrderCancelled()
//        {
//            OrderPayResult result = new OrderPayResult();
//            result.IsError = true;
//            result.Message = "Order Cancelled";
//            result.CovidGoodId = 0;
//            result.MailSend = false;
//            return View(result);
//        }

//        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
//        {
//            Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Trying to execute covid order."));
//            var paymentExecution = new PaymentExecution() { payer_id = payerId };
//            var payment = new Payment() { id = paymentId };
//            try
//            {
//                payment.Execute(apiContext, paymentExecution);
//            }
//            catch (Exception ex)
//            {
//                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format("Get execute covid order fail. Ex: {0}.", ex));
//            }
//            return payment;
//        }
//        public double calculateTotalPrice(double price,int quantity,double tax)
//        {
//            double total = 0.00;
//            var amount = price * quantity;
//            var taxamount =Math.Round((amount * tax) / 100,2);
//            total = amount + taxamount;
//            return total;
//        }
//        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string cancelURI, CovidGoodViewModel model)
//        {
//            var payment = (dynamic)null;
//            try
//            {
//                Ioc.Get<IDbLogger>().LogMessage(LogSource.PayPalPaymentService, string.Format("Trying to create payment for an order."));
//                var itemList = new ItemList() { items = new List<Item>() };
//                itemList.items.Add(new Item()
//                {
//                    name = model.GoodName,
//                    currency = "USD",
//                    price = Convert.ToString(model.GoodPrice),
//                    quantity = Convert.ToString(model.Quantity),
//                    sku = "sku"
//                });
//                var payer = new Payer() { payment_method = "paypal" };
//                // Configure Redirect Urls here with RedirectUrls object
//                var redirUrls = new RedirectUrls()
//                {
//                    cancel_url = cancelURI,
//                    return_url = redirectUrl
//                };
                
//                var taxAmount =Math.Round(((model.GoodPrice * model.Quantity) * model.Tax) / 100,2);
//                var details = new Details()
//                {
//                    tax = Convert.ToString(taxAmount),
//                    shipping = Convert.ToString(model.DeliveryCharge),
//                    subtotal = Convert.ToString(model.GoodPrice * model.Quantity)
//                };

//                var amount = new Amount()
//                {
//                    currency = "USD",
//                    total = Convert.ToString((model.GoodPrice * model.Quantity)+taxAmount),
//                   details = details
                    
//                };
//                var transactionList = new List<Transaction>();
//                transactionList.Add(new Transaction()
//                {
//                    description = model.OrderDescription + " /BuyerEmailId=  " + model.BuyerEmailId + " /CovidGoodId=  " + model.CovidGoodId,
//                    invoice_number = Convert.ToString((new Random()).Next(100000)),
//                    amount = amount,
//                    item_list = itemList
//                });
//                payment = new Payment()
//                {
//                    intent = "sale",
//                    payer = payer,
//                    transactions = transactionList,
//                    redirect_urls = redirUrls
//                };
//            return payment.Create(apiContext);
//            }
//            catch (Exception ex)
//            {
//                Ioc.Get<IDbLogger>().LogWarning(LogSource.PayPalPaymentService, string.Format(" create payment for an order fail. Ex: {0}.", ex));
//                throw ex;
//            }
//        }




//    }
//}