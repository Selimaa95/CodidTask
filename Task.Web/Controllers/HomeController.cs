using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Task.Web.Models;

namespace Task.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _apiClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient();
            _apiClient.BaseAddress = new Uri("https://localhost:7285/UploadFile/");
        }

        #region Index

        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (token is null)
            {
                return RedirectToAction("Login", "Account");
            }

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _apiClient.GetAsync("files");

            if (response.IsSuccessStatusCode)
            {
                var files = await response.Content.ReadFromJsonAsync<List<FileModel>>();
                return View(files);
            }
            else
            {
                return View(new List<FileModel>());
            }

        }
        #endregion

        #region Upload
        [HttpGet]
        public async Task<IActionResult> UploadFile()
        {
            var token = Request.Cookies["AuthToken"];
            if (token is null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var token = Request.Cookies["AuthToken"];
            if (token is null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var stream = file.OpenReadStream())
            {
                var content = new MultipartFormDataContent
                {
                    { new StreamContent(stream), "file", file.FileName }
                };

                _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _apiClient.PostAsync("upload", content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "File uploaded successfully.";
                }
                else
                {
                    ViewBag.Message = "Error uploading file.";
                }
            }

            return RedirectToAction("Index");
        }


        #endregion

        #region Download

        public async Task<IActionResult> DownloadFile(int id)
        {
            var token = Request.Cookies["AuthToken"];
            if (token is null)
            {
                return RedirectToAction("Login", "Account");
            }

            _apiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _apiClient.GetAsync($"download/{id}");

            if (response.IsSuccessStatusCode)
            {
                var fileContent = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition.FileName;

                return File(fileContent, "application/octet-stream", fileName);
            }
            else
            {
                return NotFound("File not found.");
            }
        }
        #endregion
    }
}
