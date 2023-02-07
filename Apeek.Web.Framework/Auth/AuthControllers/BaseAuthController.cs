using System;
using System.Collections.Generic;
using System.Text;
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Core.Services;
using Apeek.Entities.Entities;
using Apeek.ViewModels.Models.Impl;
using DotNetOpenAuth.AspNet;
using Newtonsoft.Json;
namespace Apeek.Web.Framework.Auth.AuthControllers
{
    public abstract class BaseAuthController : IAuthController
    {
        public int? UserId { get; set; }
        protected IAccountDataService _accountDataService;
        protected AuthenticationResult _authenticationResult;
        public abstract string KeyEmail { get; }
        public abstract string KeyExternalId { get; }
        public abstract string KeyName { get; }
        public abstract string KeyLocation { get; }
        public abstract string KeyImageUrl { get; }
        protected virtual string Email { get { return GetExtraData(KeyEmail); } }
        protected virtual string ExternalId { get { return GetExtraData(KeyExternalId); } }
        protected virtual string Name { get { return GetExtraData(KeyName); } }
        protected virtual string Location { get { return GetExtraData(KeyLocation); } }
        protected virtual string ImageUrl { get { return GetExtraData(KeyImageUrl); } }
        protected virtual string PhoneNumber { get; set; }
        protected virtual string Website { get; set; }
        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            strBuilder.Append(string.Format("Provider: {0}; ", _authenticationResult.Provider));
            try
            {
                strBuilder.Append(string.Format("ExternalData: {0}; ", ToStringExternalData()));
            }
            catch { }
            return strBuilder.ToString();
        }
        public AuthenticationResult CreateAuthenticationResult(bool isSuccessful, string provider, string providerUserId, string userName, string email, string imageUrl)
        {
            IDictionary<string, string> extraData = new Dictionary<string, string>();
            extraData.Add(KeyExternalId, providerUserId);
            extraData.Add(KeyName, userName);
            extraData.Add(KeyEmail, email);
            extraData.Add(KeyImageUrl, imageUrl);
            return new AuthenticationResult(isSuccessful, provider, providerUserId, userName, extraData);
        }
        public string ToStringExternalData()
        {
            return JsonConvert.SerializeObject(_authenticationResult.ExtraData, Formatting.Indented);
        }
        protected string GetExtraData(string key)
        {
            if (_authenticationResult.ExtraData.ContainsKey(key))
            {
                return _authenticationResult.ExtraData[key];
            }
            Ioc.Get<IDbLogger>().LogError(LogSource.Account, string.Format("There is no key [{0}] in the external data: [{1}]", key, ToString()));
            return null;
        }
        public User Authenticate(AuthenticationResult authenticationResult, IAccountDataService userDataService, int? userId)
        {
            UserId = userId;
            _accountDataService = userDataService;
            _authenticationResult = authenticationResult;
            Ioc.Get<IDbLogger>().LogMessage(LogSource.AuthController, "ExternalAuth: Trying to Authenticate user: {0}", ToString());
            var user = GetUser();
            UserAccountAssociation accountAssociation = null;
            if (user == null)
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.AuthController, "ExternalAuth: Trying to create new user: {0}", ToString());
                user = CreateUser();
                if (user != null)
                {
                    accountAssociation = AssociateAccount(user, _authenticationResult);
                }
                else
                {
                    Ioc.Get<IDbLogger>().LogError(LogSource.AuthController, "ExternalAuth: Cannot create user: {0}", ToString());
                    return null;
                }
            }
            else
            {
                Ioc.Get<IDbLogger>().LogMessage(LogSource.AuthController, "ExternalAuth: existing user will be authenticated: {0}", ToString());
                accountAssociation = GetAccountAssociation(user, _authenticationResult);
                if (accountAssociation == null)
                {
                    accountAssociation = AssociateAccount(user, _authenticationResult);
                }
                else
                {
                    //if accaunt is associated we have to update some fields
                    if (!string.IsNullOrEmpty(ImageUrl))
                    {
                        accountAssociation.ImageUrl = ImageUrl;
                        _accountDataService.UpdateUserAccountAssociation(accountAssociation);
                    }
                }
            }
            user.AccountAssociationId = accountAssociation.Id;
            return user;
        }
        private User GetUser()
        {
            User user;
            if (UserId.HasValue)
            {
                user = _accountDataService.GetUser(UserId.Value);
                if (user == null)
                    throw new ApeekException(string.Format("While assigning provider: [{0}] to user: {1} the user was not found", _authenticationResult.Provider, UserId.Value));
            }
            else
            {
                user = _accountDataService.GetUserByExternalId(ExternalId, _authenticationResult.Provider) ??
                       _accountDataService.GetUserByEmail(Email);
            }
            return user;
        }
        protected UserAccountAssociation GetAccountAssociation(User user, AuthenticationResult result)
        {
            return _accountDataService.GetAccountAssociation(user.Id, result.Provider);
        }
        private UserAccountAssociation AssociateAccount(User user, AuthenticationResult result)
        {
            var association = new UserAccountAssociation()
            {
                ProviderName = result.Provider,
                ExternalId = ExternalId,
                UserId = user.Id,
                ExtraData = ToStringExternalData(),
                ImageUrl = ImageUrl
            };
            _accountDataService.SaveUserAccountAssociation(association);
            return association;
        }
        private User CreateUser()
        {
            var model = new UserUpdateModel
            {
                FullName = Name,
                Email = Email
            };
            var result = _accountDataService.Update(model, 0);
            return result.CreateResult == CreateResult.Success ? result.Obj : null;
        }
    }
}