using Mango.Services.EmailApi.Data;
using Mango.Services.EmailApi.Models;
using Mango.Services.EmailApi.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailApi.Service
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<AppDbContext> _dbOptions;
        public EmailService(DbContextOptions<AppDbContext> options)
        {
            _dbOptions = options;
        }

       

        public async Task EmailCartLog(CartDto cartDto)
        {
            StringBuilder emialBuilder = new StringBuilder();
            emialBuilder.AppendLine("<br/>Cart email requested");
            emialBuilder.AppendLine("<br/> Total" + cartDto.CartHeader.CartTotal);
            emialBuilder.Append("<br/>");
            emialBuilder.Append("<ul>");
            foreach(var item in cartDto.CartDetails)
            {
                emialBuilder.Append("<li>");
                emialBuilder.Append(item.Product.Name + "*" + item.Count);
                emialBuilder.Append("<li>");
            }
            emialBuilder.Append("</ul>");
            await SendMessage(emialBuilder.ToString(), cartDto.CartHeader.Email);
        }

        public async Task EmailUserLog(RegistrationRequestDto registrationRequestDto)
        {
            StringBuilder emialBuilder = new StringBuilder();
            emialBuilder.AppendLine("<br/>User Account registration");
            emialBuilder.AppendLine("<br/> Hi" + registrationRequestDto.Name +"your account created successfully");
            emialBuilder.AppendLine("<br/> your user name is"+registrationRequestDto.Email +"and password is"+ registrationRequestDto.Password );
            emialBuilder.Append("<br/>");

            await SendMessage(emialBuilder.ToString(), registrationRequestDto.Email);
        }

        private async Task SendMessage(string message,string email)
        {
            try
            {
                EmailLogger emailLogger = new EmailLogger
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message

                };
                await using var _db = new AppDbContext(_dbOptions);
              await  _db.EmailLoggers.AddAsync(emailLogger);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
