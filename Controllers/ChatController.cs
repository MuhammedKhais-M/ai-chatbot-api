using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIChatBot.API.DTOs;
using AIChatBot.API.Services;
using AIChatBot.API.Data;
using AIChatBot.API.Models;
using Microsoft.EntityFrameworkCore;


namespace AIChatBot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly GroqService _groqService;
        private readonly TavilyService _tavilyService;
        private readonly AppDbContext _context;
        public ChatController(GroqService groqService, TavilyService tavilyService, AppDbContext context)
        {
            _groqService = groqService;
            _tavilyService = tavilyService;
            _context = context;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage(ChatRequest request)
        {
            var searchResult =
                await _tavilyService.SearchAsync(request.Message);

            var finalPrompt = $@"
Answer using the latest web information below.

WEB SEARCH RESULT:
{searchResult}

USER QUESTION:
{request.Message}

Give a clear and accurate answer.
";

            var aiResponse =
                await _groqService.AskAI(finalPrompt);

            return Ok(aiResponse);
        }
        //public async Task<IActionResult> SendMessage(ChatRequest request)
        //{
        //    // Create Conversation
        //    var conversation = new Conversation
        //    {
        //        UserId = 1, // Temporary
        //        Title = request.Message
        //    };

        //    _context.Conversations.Add(conversation);
        //    await _context.SaveChangesAsync();

        //    // Save User Message
        //    var userMessage = new Message
        //    {
        //        ConversationId = conversation.Id,
        //        Role = "User",
        //        Content = request.Message
        //    };

        //    _context.Messages.Add(userMessage);
        //    await _context.SaveChangesAsync();

        //    // Get AI Response
        //    var aiResponse =
        //        await _groqService.AskAI(request.Message);

        //    // Save AI Response
        //    var aiMessage = new Message
        //    {
        //        ConversationId = conversation.Id,
        //        Role = "Assistant",
        //        Content = aiResponse
        //    };

        //    _context.Messages.Add(aiMessage);
        //    await _context.SaveChangesAsync();

        //    return Ok(aiResponse);
        //}
        [HttpGet("history/{conversationId}")]
        public IActionResult GetHistory(int conversationId)
        {
            var messages = _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.Id)
                .ToList();

            return Ok(messages);
        }
        [HttpGet("conversations")]
        public IActionResult GetConversations()
        {
            return Ok(_context.Conversations
                .OrderByDescending(c => c.Id)
                .ToList());
        }
    }
}
