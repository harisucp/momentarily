using Apeek.Common.Models;
using Apeek.Core.Services;
using Apeek.Core.Services.Impl;
using Apeek.Entities.Entities;
using Apeek.NH.Repository.Common;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models;
using Momentarily.ViewModels.Models;
using System;
namespace Momentarily.UI.Service.Services.Impl
{
    public class MomentarilyAccountDataService : AccountDataService, IMomentarilyAccountDataService
    {
        private readonly IRepositoryUser _repUser;
        private readonly IRepository<UserPrivilege> _repUserPrivilege;
        private readonly IRepository<Address> _repAddress;
        private readonly IRepositoryPhoneNumber _repUserPhoneNumber;
        private readonly IRepositoryCountries _repCountries;
        private readonly IRepositorySubscibes _repositorySubscibes;
        private readonly ISendMessageService _sendMessageService;
        private readonly IRepositoryUserCoupon _repositoryUserCoupon;
        private readonly IRepositoryPaypalInfoPaymentDetail _repositoryPaypalInfoPaymentDetail;
        private readonly IRepositoryUserGood _repositoryUserGood;
        private readonly IRepositoryGlobalCode _repositoryGlobalCode;
        private readonly IRepositoryGoodRequest _repositoryGoodRequest;
        private readonly IRepositoryDisputes _repositoryDisputes;
        private readonly IRepositoryGoodBooking _repositoryGoodBooking;
        private readonly IRepositoryPaypalPayment _repositoryPaypalPayment;
        private readonly IRepositoryResolvedDisputeDetail _repositoryResolvedDisputeDetail;
        private readonly IRepositoryGoodImg _repositoryGoodImg;
        public MomentarilyAccountDataService(IRepository<Address> repAddress, IRepositoryPhoneNumber repUserPhoneNumber,
            IRepositoryUser repUser, IRepository<UserPrivilege> repUserPrivilege,
            IRepositorySubscibes repositorySubscibes, IRepositoryCountries repCountries, ISendMessageService sendMessageService,
            IRepositoryUserCoupon repositoryUserCoupon, IRepositoryPaypalInfoPaymentDetail repositoryPaypalInfoPaymentDetail,
            IRepositoryUserGood repositoryUserGood, IRepositoryGlobalCode repositoryGlobalCode,
            IRepositoryGoodRequest repositoryGoodRequest, IRepositoryDisputes repositoryDispute,
            IRepositoryGoodBooking repositoryGoodBooking,
            IRepositoryPaypalPayment repositoryPaypalPayment, IRepositoryResolvedDisputeDetail repositoryResolvedDisputeDetail,IRepositoryGoodImg repositoryGoodImg)
            : base(repUser, repUserPrivilege, repUserPhoneNumber, repositorySubscibes, repCountries,
                  sendMessageService, repositoryPaypalInfoPaymentDetail, repositoryUserCoupon,
                  repositoryUserGood, repositoryGlobalCode, repositoryGoodRequest, repositoryDispute,
                  repositoryGoodBooking, repositoryPaypalPayment, repositoryResolvedDisputeDetail, repositoryGoodImg)

        {
            _repUser = repUser;
            _repAddress = repAddress;
            _repUserPhoneNumber = repUserPhoneNumber;
            _repUserPrivilege = repUserPrivilege;
            _repositorySubscibes = repositorySubscibes;
            _repCountries = repCountries;
            _sendMessageService = sendMessageService;
            _repositoryUserCoupon = repositoryUserCoupon;
            _repositoryPaypalInfoPaymentDetail = repositoryPaypalInfoPaymentDetail;
            _repositoryUserGood = repositoryUserGood;
            _repositoryGlobalCode = repositoryGlobalCode;
            _repositoryGoodRequest = repositoryGoodRequest;
            _repositoryDisputes = repositoryDispute;
            _repositoryGoodBooking = repositoryGoodBooking;
            _repositoryPaypalPayment = repositoryPaypalPayment;
            _repositoryResolvedDisputeDetail = repositoryResolvedDisputeDetail;
            _repositoryGoodImg = repositoryGoodImg;
        }
        public override Result<User> Register(IRegisterModel model, Func<User> modelToEntityMapper)
        {
            return base.Register((IRegisterModel)model, () =>
            {
                var user = modelToEntityMapper();
                user.DateOfBirth = (model as MomentarilyRegisterModel).DateOfBirthday;
                return user;
            });
            //if (registerResult.RegisterStatus == Apeek.Entities.Web.RegisterStatus.Success)
            //{
            //    Uow.WrapWithResult(u =>
            //    {
            //        if (model is MomentarilyRegisterModel)
            //        {
            //            registerResult.User.DateOfBirth = (model as MomentarilyRegisterModel).DateOfBirthday;
            //            _repUser.Update(registerResult.User);
            //            return true;
            //        }
            //        return false;
            //    }, null, LogSource.PersonService).OnError(() =>
            //    {
            //        _repUser.Delete(registerResult.User);
            //        registerResult.RegisterStatus = Apeek.Entities.Web.RegisterStatus.Fail;
            //        registerResult.User = null;
            //    }).Run();
            //}
            //return registerResult;
        }
    }
}