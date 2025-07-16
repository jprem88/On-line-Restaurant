
using Mango.Services.RewardApi.Data;
using Mango.Services.RewardApi.Message;
using Mango.Services.RewardApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.RewardApi.Service
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;
        public EmailService(DbContextOptions<AppDbContext> options)
        {
            _dbOptions = options;
        }

       

        public async Task UpdateRewards(RewardMessage rewardMessage)
        {
            Rewards reward = new Rewards()
            {
                OrderId = rewardMessage.OrderId,
                RewardActivity = rewardMessage.RewardsActivity,
                UserId = rewardMessage.UserId
            };
            await using var _db = new AppDbContext(_dbOptions);
            await _db.Rewards.AddAsync(reward);
            await _db.SaveChangesAsync();
            //StringBuilder emialBuilder = new StringBuilder();
            //emialBuilder.AppendLine("<br/>Order reward wining email");
            //emialBuilder.AppendLine("<br/> your have earned" + rewardMessage.RewardsActivity+" on order id" + rewardMessage.OrderId);
            //emialBuilder.Append("<br/>");
            //await SendMessage(emialBuilder.ToString(), cartDto.CartHeader.Email);
        }
 
    }
}
