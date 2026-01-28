using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message?> GetMessage(string messageId)
        {
            return await context.Messages.FindAsync(messageId); 
        }

        public Task<PaginatedResult<MessageDto>> GetMessagesForMember()
        {
            throw new NotImplementedException();
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

            // get the container
            query = messageParams.Container switch
            {
                "Outbox" => query.Where(x => x.SenderId == messageParams.MemberId && x.SenderDeleted == false),
                _ => query.Where(x => x.RecipientId == messageParams.MemberId && x.RecipientDeleted == false),
            };

            var messageQuery = query.Select(MessageExtensions.ToDtoProjection());

            return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
        {
            // get all unread messages sent to current user by a specific user and also mark them as read
            await context.Messages
                .Where(x => x.RecipientId == currentMemberId && x.RecipientDeleted == false && x.SenderId == recipientId && x.SenderDeleted == false && x.DateRead == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.DateRead, DateTime.UtcNow));

            // get whole conversation between current user and one specific user
            return await context.Messages
                .Where(x => (x.RecipientId == currentMemberId && x.SenderId == recipientId) || (x.SenderId == currentMemberId && x.RecipientId == recipientId))
                .OrderBy(x => x.MessageSent)
                .Select(MessageExtensions.ToDtoProjection())
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0; 
        }
    }
}
