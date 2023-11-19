using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Task.Web.Models;

namespace Task.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _apiClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient();
            _apiClient.BaseAddress = new Uri("https://localhost:7285/api/Auth/");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(login login)
        {
            string loginJson = JsonConvert.SerializeObject(login);
            StringContent content = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var response = await _apiClient.PostAsync("Login", content);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ResponseToken>();
                string token = result.Token;

                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToAction("Index", "Home");
            }

            return View("Login");

        }


    }

}

