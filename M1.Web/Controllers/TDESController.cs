using M1.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace M1.Web.Controllers
{

    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "TripleDESApi:Scope")]
    public class TDESController : Controller
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<TripleDESApiOption> _tdesApiOption;
        public TDESController(
            ITokenAcquisition tokenAcquisition,
            IHttpClientFactory httpClientFactory,
            IOptions<TripleDESApiOption> options)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClientFactory = httpClientFactory;
            _tdesApiOption = options;

        }
        public IActionResult Encrypt()
        {
            ViewBag.Result = string.Empty;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Encrypt([Required] string plainText)
        {
            ViewBag.Result = string.Empty;
            ViewBag.Success = false;
            if (ModelState.IsValid)
            {
                var client = await GetHttpClient();
                var response = await client.PostAsync("/api/crypto/tdes/encrypt", new StringContent(JsonConvert.SerializeObject(new
                {
                    plainText = plainText
                }), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    EncryptResponse encryptResponse = JsonConvert.DeserializeObject<EncryptResponse>(responseJson);
                    ViewBag.Result = encryptResponse.CipherText;
                    ViewBag.Success = true;
                }
                else
                {
                    ViewBag.Result = "An error occured";
                }
            }
            else
            {
                ViewBag.Result = "Invalid body payload";
            }
            return View();
        }


        public IActionResult Decrypt()
        {
            ViewBag.Result = string.Empty;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Decrypt([Required] string cipherText)
        {
            ViewBag.Result = string.Empty;
            ViewBag.Success = false;
            if (ModelState.IsValid)
            {
                var client = await GetHttpClient();
                var response = await client.PostAsync("/api/crypto/tdes/decrypt", new StringContent(JsonConvert.SerializeObject(new
                {
                    cipherText = cipherText
                }), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();
                    DecryptResponse encryptResponse = JsonConvert.DeserializeObject<DecryptResponse>(responseJson);
                    ViewBag.Result = encryptResponse.PlainText;
                    ViewBag.Success = true;
                }
                else
                {
                    ViewBag.Result = "An error occured";
                }
            }
            else
            {
                ViewBag.Result = "Invalid body payload";
            }
            return View();
        }


        private async Task<HttpClient> GetHttpClient()
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(_tdesApiOption.Value.BaseUrl);
            string token = await _tokenAcquisition.GetAccessTokenForUserAsync(new string[]
            {
                _tdesApiOption.Value.Scope
            });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
