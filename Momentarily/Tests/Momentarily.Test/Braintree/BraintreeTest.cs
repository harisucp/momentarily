using NUnit.Framework;
using BraintreePayments;
using System;
using BraintreePayments.Models;
namespace Momentarily.Test.Braintree
{
    [TestFixture]
    public class BraintreeTest
    {
        private BraintreeAPI _braintreeAPI;
        public BraintreeTest()
        {
            _braintreeAPI = new BraintreeAPI();
            _braintreeAPI.Init(
                Environment: "sandbox",
                MasterMerchantAccountId: "empeek",
                MerchantId: "t9xcnr5rs965njsv",
                PublicKey: "nn27z3jvmz8hr63r",
                PrivateKey: "d8e05e997e4b8381cfb864a1bbdea3b2"
            );
        }
        [Test]
        public void GetClientToken()
        {            
            try {                
                var getClientTokenResult = _braintreeAPI.GetClientToken();                
                if (getClientTokenResult.IsSuccess)
                {
                    Assert.IsTrue(true);
                    Assert.IsNotEmpty(getClientTokenResult.ClientToken.Token, "Client token is empty");
                }
                else
                {
                    throw new Exception(getClientTokenResult.Message);
                }
            } catch (Exception ex)
            {
                Assert.Fail(ex.Message + ". " + ex.InnerException);
            }            
        }               
        [Test]
        public void CreatePurchaseByToken() 
        {
            try
            {
                var result = _braintreeAPI.CreatePurchaseByToken(
                                Token: "gmv8y6",
                                Amount: 20.00M,
                                MerchantAccountId: "jane_doe_instant_2zzt269q",
                                ServiceFeeAmount: 5.00m
                             );
                if (result.IsSuccess)
                {
                    Assert.IsTrue(true);
                }
                else
                {
                    throw new Exception(result.Message);
                }                
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ". " + ex.InnerException);
            }
        }
        [Test]
        public void GetMerchantAccountById()
        {
            try
            {
                var result = _braintreeAPI.GetMerchantAccountById(                                
                                Id: "jane_doe_instant_2zzt269q"
                             );
                if (result.IsSuccess)
                {
                    Assert.IsTrue(true);
                }
                else
                {
                    throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ". " + ex.InnerException);
            }
        }
        [Test]
        public void CreateMerchantAccount()
        {            
            try
            {
                var result = _braintreeAPI.CreateOrUpdateMerchantAccount(
                    MerchantAccountId: null,
                    Individual: new MerchantAccountIndividual
                    {
                        FirstName = "Jane",
                        LastName = "Doe",
                        Email = "jane@14ladders.com",
                        Phone = "5553334444",
                        DateOfBirth = "1981-11-19",
                        StreetAddress = "111 Main St",
                        Locality = "Chicago",
                        Region = "IL",
                        PostalCode = "60622"
                    },
                    Funding: new MerchantAccountFunding
                    {                        
                        AccountNumber = "1123581321",
                        RoutingNumber = "071101307"
                    });
                if (result.IsSuccess)
                {
                    Assert.IsTrue(true);
                }
                else
                {
                    throw new Exception(result.Message);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + ". " + ex.InnerException);
            }
        }
    }
}
