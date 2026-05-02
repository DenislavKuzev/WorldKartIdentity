using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using OpenAI.Chat;
using System.ClientModel;
using System.Diagnostics;
using WorldKartIdentity.Database;
using WorldKartIdentity.ViewModel;

namespace WorldKartIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [Authorize] // ?????????, ?? ???? ??????? ??????????? ??????
        public async Task<IActionResult> Privacy()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<string> GptResponse(string prompt)
        {
            string model = "gpt-4.1-nano"; // Specify the model (e.g., gpt-4)
            string apiKey = Environment.GetEnvironmentVariable("AI_KEY");
            ChatClient chatClient = new ChatClient(model, apiKey);

            // Create messages using the specific message types
            List<ChatMessage> messages = new List<ChatMessage>
            {

                 ChatMessage.CreateSystemMessage("You are a helpful assistant."),
                 ChatMessage.CreateUserMessage(prompt)
            };

            try
            {
                // Send chat request (synchronous)
                ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages);

                if (result?.Value != null)
                {
                    // Access the response content directly through the flattened property
                    return result.Value.Content[0].Text;
                }
                else
                {
                    return "Error: The result value is null or the operation was unsuccessful.";
                }
            }
            catch (Exception ex)
            {
                return "An error occurred: " + ex.Message;
            }
        }
    }
}
