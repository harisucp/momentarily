using Apeek.Common.Interfaces;
using Apeek.Entities.Entities;
using Apeek.Entities.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apeek.Core.Services
{
    public interface ILiveLocationService : IDependency
    {
        bool UpdateSharerLocation(string locationId, int sharerId, double lat, double lng);
        bool UpdateBorrowerLocation(string locationId,int borrowerId,double lat, double lng);
        LiveLocation AddLocation(int borrowerId,int requestId,int sharerId, double sharerLatitude, double sharerLongitude, double borrowerLatitude, double borrowerLongitude, DeliverBy deliverBy);
        LiveLocation fetchLocation(string locationId);
        bool checkRequest(int requestId);
        LiveLocation GetByRequestId(int requestId);
        bool ConfirmDelivery(int requestId);
        bool ReturnConfirm(int requestId);
    }
}
