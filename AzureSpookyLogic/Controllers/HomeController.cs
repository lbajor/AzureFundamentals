using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureSpookyLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace AzureSpookyLogic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _blobServiceClient = blobServiceClient;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SpookyRequest spookyRequest, IFormFile file)
        {
            spookyRequest.Id = Guid.NewGuid().ToString();
            using var httpClient = _httpClientFactory.CreateClient();
            var jsonContent = JsonConvert.SerializeObject(spookyRequest);
            var logicAppUrl = _configuration["LogicAppUrl"];
            if (string.IsNullOrWhiteSpace(logicAppUrl))
            {
                throw new InvalidOperationException("The LogicAppUrl configuration setting is required.");
            }

            using (var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = await httpClient.PostAsync(logicAppUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();
            }

            if (file != null)
            {
                var fileName = spookyRequest.Id + Path.GetExtension(file.FileName);
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("logic-app-holder");
                var blobClient = containerClient.GetBlobClient(fileName);
                var httpHeaders = new BlobHttpHeaders()
                {
                    ContentType = file.ContentType
                };

                await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
