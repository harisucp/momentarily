using System;
using System.Linq;
using Braintree;
using BraintreePayments.Models;
namespace BraintreePayments
{
    public class BraintreeAPI
    {
        private BraintreeGateway _braintreeGateway;
        private string _masterMerchantAccountId;
        private string _merchantId;
        private string _publicKey;
        private string _privateKey;     
        public void Init(string Environment, string MasterMerchantAccountId, string MerchantId, string PublicKey, string PrivateKey)
        {
            _masterMerchantAccountId = MasterMerchantAccountId;
            _merchantId = MerchantId;
            _publicKey = PublicKey;
            _privateKey = PrivateKey;            
            _braintreeGateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.ParseEnvironment(Environment),
                MerchantId = _merchantId,
                PublicKey = _publicKey,
                PrivateKey = _privateKey
            };            
        }       
        public GetClientTokenResult GetClientToken()
        {
            var result = new GetClientTokenResult();
            result.IsSuccess = true;
            result.ClientToken.Token = _braintreeGateway.ClientToken.generate();
            return result;
        }
        // https://developers.braintreepayments.com/guides/customers/dotnet
        public CreateCustomerResult CreateOrUpdateCustomer(string CustomerId, string FirstName, string LastName, string Email, string PaymentMethodNonce)
        {
            var result = new CreateCustomerResult();
            string validationMessage = null;
            if (string.IsNullOrWhiteSpace(CustomerId)) validationMessage += "CustomerId is empty. ";
            if (string.IsNullOrWhiteSpace(FirstName)) validationMessage += "FirstName is empty. ";
            if (string.IsNullOrWhiteSpace(LastName)) validationMessage += "LastName is empty. ";
            if (string.IsNullOrWhiteSpace(Email)) validationMessage += "Email is empty. ";
            if (string.IsNullOrWhiteSpace(PaymentMethodNonce)) validationMessage += "PaymentMethodNonce is empty. ";
            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                result.Message = validationMessage;
                result.IsSuccess = false;
                return result;
            }            
            var request = new CustomerRequest
            {
                Id = CustomerId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,                
                PaymentMethodNonce = PaymentMethodNonce               
            };
            Result<Braintree.Customer> createResult;
            try
            {
                if (_braintreeGateway.Customer.Find(CustomerId) != null)
                {
                    createResult = _braintreeGateway.Customer.Update(CustomerId, request);
                } else
                {
                    throw new Braintree.Exceptions.NotFoundException();
                }
            }
            catch (Braintree.Exceptions.NotFoundException ex)
            {
                createResult = _braintreeGateway.Customer.Create(request);
            }
            if (createResult.IsSuccess())
            {
                result.IsSuccess = true;                
                result.Customer.Id = createResult.IsSuccess() ? createResult.Target.Id : string.Empty;
                result.Customer.PaymentMethodToken = createResult.Target.PaymentMethods.Any()
                                                        ? createResult.Target.PaymentMethods.Last().Token
                                                        : string.Empty;
            }
            result.Message = createResult.Message;
            return result;
        }
        // https://developers.braintreepayments.com/guides/payment-methods/dotnet
        public CreatePaymentMethodResult CreatePaymentToken(string CustomerId, string Nonce)
        {
            var request = new PaymentMethodRequest
            {                 
                CustomerId = CustomerId,
                PaymentMethodNonce = Nonce
            };
            Result<PaymentMethod> createResult = _braintreeGateway.PaymentMethod.Create(request);            
            return new CreatePaymentMethodResult()
            {
                IsSuccess = createResult.IsSuccess(),
                Message = createResult.Message,
                Token = createResult.IsSuccess() ? createResult.Target.Token : string.Empty
            };
        }
        // https://developers.braintreepayments.com/guides/braintree-marketplace/create/dotnet
        public CreatePurchaseResult CreatePurchaseByToken(string Token, decimal Amount, string MerchantAccountId, decimal ServiceFeeAmount)
        {
            var request = new TransactionRequest
            {
                Amount = Amount,
                MerchantAccountId = MerchantAccountId,
                ServiceFeeAmount = ServiceFeeAmount,
                PaymentMethodToken = Token,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };
            var saleResult = _braintreeGateway.Transaction.Sale(request);
            return new CreatePurchaseResult()
            {
                IsSuccess = saleResult.IsSuccess(),
                Message = saleResult.Message
            };
        }
        // https://developers.braintreepayments.com/guides/braintree-marketplace/onboarding/dotnet
        public CreateOrUpdateMerchantAccountResult CreateOrUpdateMerchantAccount(string MerchantAccountId, MerchantAccountIndividual Individual, MerchantAccountFunding Funding)
        {
            var result = new CreateOrUpdateMerchantAccountResult();
            MerchantAccountRequest request = new MerchantAccountRequest
            {
                Individual = new IndividualRequest
                {
                    FirstName = Individual.FirstName,
                    LastName = Individual.LastName,
                    Email = Individual.Email,
                    Phone = Individual.Phone,
                    DateOfBirth = Individual.DateOfBirth,                    
                    Address = new AddressRequest
                    {
                        StreetAddress = Individual.StreetAddress,
                        Locality = Individual.Locality,
                        Region = Individual.Region,
                        PostalCode = Individual.PostalCode
                    }
                },
                Funding = new FundingRequest
                {                    
                    Destination = FundingDestination.BANK,                    
                    AccountNumber = Funding.AccountNumber,
                    RoutingNumber = Funding.RoutingNumber
                },
                TosAccepted = true,
                MasterMerchantAccountId = this._masterMerchantAccountId,                
            };
            Result<Braintree.MerchantAccount> createOrUpdateResult;
            if (string.IsNullOrWhiteSpace(MerchantAccountId))
            {
                createOrUpdateResult = _braintreeGateway.MerchantAccount.Create(request);
            } else
            {
                createOrUpdateResult = _braintreeGateway.MerchantAccount.Update(MerchantAccountId, request);
            }
            result.IsSuccess = createOrUpdateResult.IsSuccess();
            result.Message = createOrUpdateResult.Message;
            if (createOrUpdateResult.IsSuccess())
            {
                result.MerchantAccount = new Models.MerchantAccount() { Id = createOrUpdateResult.Target.Id };
                result.MerchantAccount.Individual = Individual;
                result.MerchantAccount.Funding = Funding;                
            }
            return result;
        }
        public GetMerchantAccountResult GetMerchantAccountById(string Id)
        {
            var result = new GetMerchantAccountResult();
            var MerchantAccount = _braintreeGateway.MerchantAccount.Find(Id);
            if (MerchantAccount != null)
            {
                result.MerchantAccount = new Models.MerchantAccount
                {
                    Id = MerchantAccount.Id,
                    Individual = new MerchantAccountIndividual
                    {
                        DateOfBirth = MerchantAccount.IndividualDetails.DateOfBirth,
                        Email = MerchantAccount.IndividualDetails.Email,
                        FirstName = MerchantAccount.IndividualDetails.FirstName,
                        LastName = MerchantAccount.IndividualDetails.LastName,
                        Locality = MerchantAccount.IndividualDetails.Address.Locality,
                        Phone = MerchantAccount.IndividualDetails.Phone,
                        PostalCode = MerchantAccount.IndividualDetails.Address.PostalCode,
                        Region = MerchantAccount.IndividualDetails.Address.Region,
                        StreetAddress = MerchantAccount.IndividualDetails.Address.StreetAddress
                    },
                    Funding = new MerchantAccountFunding()
                    {
                        AccountNumber = MerchantAccount.FundingDetails.AccountNumberLast4,
                        RoutingNumber = MerchantAccount.FundingDetails.RoutingNumber
                    }
                };
                result.IsSuccess = true;
            }
            return result;
        }
        public GetCustomerResult GetCustomer(string CustomerId)
        {
            var result = new GetCustomerResult();               
            if (string.IsNullOrWhiteSpace(CustomerId))
            {
                result.Message = "CustomerId is empty.";
                result.IsSuccess = false;
                return result;
            }
            Braintree.Customer customer = null;
            try
            {
                customer = _braintreeGateway.Customer.Find(CustomerId);
                if (customer == null) throw new Braintree.Exceptions.NotFoundException();
            }
            catch (Braintree.Exceptions.NotFoundException ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                return result;
            }
                result.IsSuccess = true;
                result.Customer.Id = customer.Id;
                result.Customer.PaymentMethodToken = customer.PaymentMethods.Any()
                                                        ? customer.PaymentMethods.Last().Token
                                                        : string.Empty;                        
            return result;
        }
    }
}
