using Apeek.Common.Interfaces;
using Apeek.Common.UrlHelpers;
using Apeek.Core.Services;
using Momentarily.ViewModels.Models;
namespace Momentarily.UI.Service.Services
{
    public interface IMomentarilyUserMessageService : IUserMessageService, IDependency
    {
        bool SendSystemMessage(UserSystemMessageCreateModel messageModel);
        bool SendBookingGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
        bool SendApproveGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
        bool SendDeclineGoodRequestMessage(int authorId, int receiverId, int requestId, string message,
            QuickUrl quickUrl);
        bool SendPayGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
        bool SendDepositGoodRequestMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
        bool SendSahrerStartDisputeMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
        bool SendSeekerStartDisputeMessage(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
        bool SendReviewMessageForSeeker(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
        bool SendReviewMessageForSharer(int authorId, int receiverId, int requestId, QuickUrl quickUrl);
    }
}
