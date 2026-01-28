using API.Interfaces;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Helpers;

namespace API.Controllers
{
    public class MessagesController(IMessageRepository messageRepository, IMemberRepository memberRepository) : BaseApiController
    {
        [HttpPost]
        // send message 
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            // get the sender and recipient
            var sender = await memberRepository.GetMemberByIdAsync(User.GetMemberId());
            var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            // null check 
            if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId)
            {
                return BadRequest("Cannot send this message.");
            }

            // save in databsae
            var message = new Message
            {
                SenderId = sender.Id,
                Content = createMessageDto.Content,
                RecipientId = recipient.Id,
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) return message.ToDto();

            return BadRequest("Failed to send this message");
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessageByContainer([FromQuery] MessageParams messageParams)
        {
            messageParams.MemberId = User.GetMemberId();

            return await messageRepository.GetMessagesForMember(messageParams);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
        {
            return Ok(await messageRepository.GetMessageThread(User.GetMemberId(), recipientId));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(string id)
        {
            // get the current user 
            var memberId = User.GetMemberId();

            var message = await messageRepository.GetMessage(id);

            if (message == null) return BadRequest("Cannot delete the message");

            if (message.SenderId != memberId && message.RecipientId != memberId) return BadRequest("You can not delete this message.");

            if (message.SenderId == memberId) message.SenderDeleted = true;
            if (message.RecipientId == memberId) message.RecipientDeleted = true;

            // if both parties have deleted the message, then delete the message

            if(message is {SenderDeleted: true, RecipientDeleted: true })
            {
                messageRepository.DeleteMessage(message);
            }

            if (await memberRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the message.");
        }
    }
}
