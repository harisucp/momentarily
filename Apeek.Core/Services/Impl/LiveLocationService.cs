using Apeek.Common.Logger;
using Apeek.Entities.Entities;
using Apeek.NH.DataAccessLayer.DataAccess;
using Apeek.NH.Repository.Repositories;
using System;

namespace Apeek.Core.Services.Impl
{
    public class LiveLocationService : ILiveLocationService
    {
        private readonly IRepositoryLiveLocation _repLiveLocation;
        public LiveLocationService(IRepositoryLiveLocation repLiveLocation)
        {
            _repLiveLocation = repLiveLocation;
        }
		public LiveLocation AddLocation(int borrowerId, int requestId, int sharerId, double sharerLatitude, double sharerLongitude, double borrowerLatitude, double borrowerLongitude, DeliverBy deliverBy)
        {
			try
			{
				var id = CreateGuid();
				var liveLocation = new LiveLocation
				{
					BorrowerId = borrowerId,
					ModBy = borrowerId,
					GoodRequestId = requestId,
					SharerId = sharerId,
					DeliverBy = deliverBy,
					LocationId = id,
					ModDate = DateTime.Now,
					CreateBy = borrowerId,
					CreateDate = DateTime.Now,
					BorrowerLatitude = borrowerLatitude,
					BorrowerLongitude = borrowerLongitude,
					SharerLatitude = sharerLatitude,
					SharerLongitude = sharerLongitude,
					RideStarted = true,
					DeliveryConfirm = false,
					ReturnConfirm = false
				};
				Uow.Wrap(uow =>
				{
					_repLiveLocation.Save(liveLocation);
				}, null, LogSource.LiveLocationService);
				var result = this.fetchLocation(id);
				return result;

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

        public bool checkRequest(int requestId)
        {
			try
			{
				var result = false;
				Uow.Wrap(uow =>
				{
					result = _repLiveLocation.CheckRequest(requestId);
				}, null, LogSource.LiveLocationService);
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

        public bool ConfirmDelivery(int requestId)
        {
			try
			{
				Uow.Wrap(uow =>
				{
					LiveLocation liveLocation = null;

					liveLocation = _repLiveLocation.GetByRequestId(requestId);
					if (liveLocation != null)
					{
						liveLocation.DeliveryConfirm = true;
						_repLiveLocation.Update(liveLocation);
					}
				}, null, LogSource.LiveLocationService);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

        public LiveLocation fetchLocation(string locationId)
        {
			try
			{
				LiveLocation liveLocation = new LiveLocation();
				Uow.Wrap(uow =>
				{
					liveLocation = _repLiveLocation.GetLocation(locationId);
				}, null, LogSource.LiveLocationService);
				return liveLocation;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public LiveLocation GetByRequestId(int requestId)
        {
			try
			{
				LiveLocation liveLocation =  new LiveLocation();
				Uow.Wrap(uow =>
				{
					liveLocation = _repLiveLocation.GetByRequestId(requestId);
				}, null, LogSource.LiveLocationService);
				return liveLocation;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

        public bool ReturnConfirm(int requestId)
        {
			try
			{
				Uow.Wrap(uow =>
				{
					LiveLocation liveLocation = null;

					liveLocation = _repLiveLocation.GetByRequestId(requestId);
					if (liveLocation != null)
					{
						liveLocation.ReturnConfirm = true;
						_repLiveLocation.Update(liveLocation);
					}
				}, null, LogSource.LiveLocationService);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

        public bool UpdateBorrowerLocation(string locationId,int borrowerId, double lat, double lng)
        {
			try
			{
				Uow.Wrap(uow =>
				{
					LiveLocation liveLocation = null;

					liveLocation = _repLiveLocation.GetLocation(locationId);
					if (liveLocation != null)
					{
						liveLocation.LocationId = locationId;
						liveLocation.BorrowerId = borrowerId;
						liveLocation.BorrowerLatitude = lat;
						liveLocation.BorrowerLongitude = lng;
						liveLocation.ModBy = borrowerId;
						liveLocation.ModDate = DateTime.Now;
						liveLocation.CreateBy = borrowerId;
						liveLocation.CreateDate = DateTime.Now;
						_repLiveLocation.Update(liveLocation);
					}
				}, null, LogSource.LiveLocationService);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

        public bool UpdateSharerLocation(string locationId, int sharerId, double lat, double lng)
        {
			try
			{
				Uow.Wrap(uow =>
				{
					LiveLocation liveLocation = null;

					liveLocation = _repLiveLocation.GetLocation(locationId);
					if (liveLocation != null)
					{
						liveLocation.SharerId = sharerId;
						liveLocation.SharerLatitude = lat;
						liveLocation.SharerLongitude = lng;
						liveLocation.ModBy = sharerId;
						liveLocation.ModDate = DateTime.Now;
						liveLocation.CreateBy = sharerId;
						liveLocation.CreateDate = DateTime.Now;
						_repLiveLocation.Update(liveLocation);
					}
				}, null, LogSource.LiveLocationService);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		private string CreateGuid()
        {
			return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
