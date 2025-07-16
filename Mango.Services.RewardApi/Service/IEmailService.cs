
using Mango.Services.RewardApi.Message;

namespace Mango.Services.RewardApi.Service
{
    public interface IEmailService
    {
        Task UpdateRewards(RewardMessage rewardMessage);
    }
}
