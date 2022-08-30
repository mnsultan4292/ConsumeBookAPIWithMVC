using ConsumeBookAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsumeBookAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BookModel> bookModel = new List<BookModel>();
            using (var httpclient = new HttpClient())
            {
                using (var response = await httpclient.GetAsync("https://localhost:7195/api/Book/GetMasterBook"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    bookModel = JsonConvert.DeserializeObject<List<BookModel>>(apiResponse);
                }
            }
            return View(bookModel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(BookModel _bookModel)
        {

            string API = string.Empty;
            if (_bookModel.BookId <= 0)
                API = "https://localhost:7195/api/Book/PostMasterBook";
            else
                API = "https://localhost:7195/api/Book/UpdateMasterBook";

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(_bookModel), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(API, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (Convert.ToInt32(apiResponse) <= 0)
                        TempData["Success"] = "Something went wrong";
                    else
                        TempData["Success"] = _bookModel.BookId <= 0 ? "Data Inserted Successfully" : "Updated Successfully";
                }
            }
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteBook(int BookId)
        {
            using (var httpclient = new HttpClient())
            {
                using (var response = await httpclient.DeleteAsync("https://localhost:7195/api/Book/DeleteMasterBook/" + BookId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    if (Convert.ToInt32(apiResponse) <= 0)
                        TempData["Success"] = "Something went wrong";
                    else
                        TempData["Success"] = "Deleted Successfully";
                }
            }
            return RedirectToAction("Index");
        }
        public void UpdateBook(int BookId)
        {

        }
        public async Task<JsonResult> GetBook(int BookId)
        {
            BookModel bookModel = new BookModel();
            using (var httpclient = new HttpClient())
            {
                using (var response = await httpclient.GetAsync("https://localhost:7195/api/Book/GetMasterBookById/" + BookId))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    bookModel = JsonConvert.DeserializeObject<BookModel>(apiResponse);
                }
            }
            return Json(bookModel);
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
    }
}
