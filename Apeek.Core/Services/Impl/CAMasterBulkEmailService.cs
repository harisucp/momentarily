
using Apeek.Common;
using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using Apeek.ViewModels.Models.Impl;
using Momentarily.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Apeek.Core.Services.Impl
{
    public class CAMasterBulkEmailService : ICAMasterBulkEmailService
    {
        private readonly IRepositoryCAMasterBulkEmail _repCAMasterBulkEmail;
        private readonly IRepositorySubscibes _repSubscibes;
        //Subscription _subscriptionServiceNew;
        //private readonly string SubscriberListId = "hMFdFOy89j4lo14A6v5zaw"; 
        //private readonly string SubscriberCampaignListId = "ovqsRJ53jpw763zNUdXxT892Pg";
        public CAMasterBulkEmailService(IRepositoryCAMasterBulkEmail CAMasterBulkEmail, IRepositorySubscibes repSubscibes)
        {
            _repCAMasterBulkEmail = CAMasterBulkEmail;
            _repSubscibes = repSubscibes;
            //_subscriptionServiceNew = new Subscription();
        }

        public bool GetAllCAMasterRecord(CAMasterViewModel record)
        {
            bool result = false;

            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to insert all CA Master Detail "));
                Uow.Wrap(u =>
                {
                    var checkAlreadyExsistRecord = _repCAMasterBulkEmail.Table.Where(x => x.EmailID == record.Email.Trim()).Count();
                    if (checkAlreadyExsistRecord == 0)
                    {

                        CAMasterBulkEmail CAMDetails = new CAMasterBulkEmail();
                        CAMDetails.EmailID = record.Email;
                        CAMDetails.AddedToSendy = false;
                        CAMDetails.CreateBy = 0;
                        CAMDetails.ModBy = 0;
                        CAMDetails.CreateDate = DateTime.Now;
                        CAMDetails.ModDate = DateTime.Now;
                        _repCAMasterBulkEmail.Save(CAMDetails);

                    }
                    result = true;
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to all CA Master Detail:-  " + ex));
            }
            return result;
        }

        public CAMasterViewModel SaveLimitedEmailRecord()
        {
            CAMasterViewModel obj = new CAMasterViewModel();

          List<CAMasterViewModel> list = new List<CAMasterViewModel>();
            int count = 0;

            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to insert email in subscription & sendy."));
                Uow.Wrap(u =>
                {
                    var getTop250Email = _repCAMasterBulkEmail.Table.Where(x => x.AddedToSendy == false).Take(250);
                    if (getTop250Email != null)
                    {
                        foreach (var item in getTop250Email)
                        {
                            var checkAlreadyExsistRecordInSubscriber = _repSubscibes.Table.Where(x => x.Email == item.EmailID.Trim()).Count();
                            if(checkAlreadyExsistRecordInSubscriber ==0)
                            {
                                Subscribes subscriber = new Subscribes();
                                subscriber.Email = item.EmailID;
                                subscriber.SubscribeStatus = true;
                                subscriber.CreateBy = 0;
                                subscriber.ModBy = 0;
                                subscriber.CreateDate = DateTime.Now;
                                subscriber.ModDate = DateTime.Now;
                                _repSubscibes.Save(subscriber);
                                list.Add(new CAMasterViewModel {Email = item.EmailID,AddedToSendy = true,AddedCount = 0,ReturnStatus = false });
                                //subscribeEmail(SubscriberListId, item.EmailID.Trim(), "", true);
                                //subscribeCampaignEmail(SubscriberCampaignListId, item.EmailID.Trim(), "", true);
                                var updateCAMaster = _repCAMasterBulkEmail.Table.Where(x => x.Id == item.Id).FirstOrDefault();
                                updateCAMaster.AddedToSendy = true;
                                updateCAMaster.ModDate = DateTime.Now;
                                _repCAMasterBulkEmail.Update(updateCAMaster);
                            }

                            count++;
                        }
                        obj.cAMasterViewModelsList = list;
                        obj.ReturnStatus = true;
                        obj.AddedCount = count;
                    }

                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to insert email in subscription & Sendy:-  " + ex));
            }
            return obj;
        }

        public bool GetCheckAlreadyUpdateForTheDay()
        {

            bool result = false;

            try
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Trying to check if already email updated for the day."));
                Uow.Wrap(u =>
                {
                  result = _repCAMasterBulkEmail.Table.Any(x => x.AddedToSendy == true && x.ModDate.Date == DateTime.Now.Date);
                });
            }
            catch (Exception ex)
            {
                Ioc.Get<IDbLogger>().LogWarning(LogSource.StockDataService, string.Format("Failed to check if already email updated for the day:-  " + ex));
            }
            return result;
        }


        //public string subscribeEmail(string listId, string email, string name, bool plaintext)
        //{
        //    try
        //    {
        //        _subscriptionServiceNew = new Subscription();
        //        string result = _subscriptionServiceNew.Subscribe(listId, email, name, plaintext);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}

        //public string subscribeCampaignEmail(string listId, string email, string name, bool plaintext)
        //{
        //    try
        //    {
        //        _subscriptionServiceNew = new Subscription();
        //        string result = _subscriptionServiceNew.Subscribe(listId, email, name, plaintext);
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}

    }
}
