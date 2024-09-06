using AudioSample.Models;
using AudioSample.OpenAI;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;
using System.Diagnostics;
using System.Web;

namespace AudioSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SpeechService _speechService;
        private readonly SpeechToTextConverter speechToTextConverter;
        private readonly IOpenAIProxy openAIProxy;
        public HomeController()
        {
            openAIProxy = new OpenAIProxy("Chat-GPT Key ", "Org-ID");
                  }
       

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ChatCompletionMessage[]> UploadAudio( [FromBody] string recognizedText)
        {
            
                // var recognizedText = await speechToTextConverter.ConvertWavToTextAsync(file);
            var dt =     await openAIProxy.SendChatMessage(recognizedText);

            return dt;
            
          
        }
    }
    }