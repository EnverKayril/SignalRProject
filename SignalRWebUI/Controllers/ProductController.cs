using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SignalRWebUI.Dtos.ProductDtos;

namespace SignalRWebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7228/api/Product/GetProductsWithCategories");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData);
                return View(values);
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            var client = _httpClientFactory.CreateClient();
            var response = client.GetAsync("https://localhost:7228/api/Product");
            var jsonData = await response.Result.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData);
            List<SelectListItem> values2 = (from x in values
                                            select new SelectListItem
                                            {
                                                Text = x.ProductName,
                                                Value = x.ProductId.ToString()
                                            }).ToList();
            ViewBag.v = values2;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            createProductDto.ProductStatus = true;
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(createProductDto);
            StringContent stringContent = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7228/api/Product", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View(createProductDto);
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"https://localhost:7228/api/Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProduct(int id)
        {
            var clientCat = _httpClientFactory.CreateClient();
            var responseCat = clientCat.GetAsync("https://localhost:7228/api/Product");
            var jsonDataCat = await responseCat.Result.Content.ReadAsStringAsync();
            var valuesCat = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonDataCat);
            List<SelectListItem> values2 = (from x in valuesCat
                                            select new SelectListItem
                                            {
                                                Text = x.ProductName,
                                                Value = x.ProductId.ToString()
                                            }).ToList();
            ViewBag.v = values2;

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7228/api/Product/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<UpdateProductDto>(jsonData);

                return View(values);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
        {
            updateProductDto.ProductStatus = true;
            var client = _httpClientFactory.CreateClient();
            var jsonData = JsonConvert.SerializeObject(updateProductDto);
            var stringContent = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7228/api/Product/", stringContent);
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"StatusCode: {response.StatusCode}");
            Console.WriteLine($"Reason: {response.ReasonPhrase}");
            Console.WriteLine($"Content: {errorContent}");

            var clientCat = _httpClientFactory.CreateClient();
            var responseCat = await clientCat.GetAsync("https://localhost:7228/api/Product");
            var jsonDataCat = await responseCat.Content.ReadAsStringAsync();
            var valuesCat = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonDataCat);
            List<SelectListItem> values2 = (from x in valuesCat
                                            select new SelectListItem
                                            {
                                                Text = x.ProductName,
                                                Value = x.ProductId.ToString()
                                            }).ToList();
            ViewBag.v = values2;

            return View(updateProductDto);
        }

    }
}
