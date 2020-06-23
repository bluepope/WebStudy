using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BluePope.WebMvc.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Text;
using BluePope.WebMvc.Filters;

namespace BluePope.WebMvc.Controllers
{
    //[ConvertRequestBodyToFormData] //컨트롤러에 적용도 가능
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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


        #region Dapper 적용 예제
        public IActionResult Sample()
        {
            return View();
        }

        [Route("/SampleData")]
        [HttpGet]
        public JsonResult GetSampleData(string search)
        {
            //core가 아닌 asp.net mvc 에서는 , 뒤에 JsonRequestBehavior.AllowGet 를 추가해줘야합니다.
            return Json(DataModel.GetList(search));
        }

        //case 1
        /*
        [Route("/SampleData")]
        [HttpPost]
        [Consumes("application/json")]
        public IActionResult SaveSampleDataApi([FromBody]List<DataModel> input)
        {
            return SaveSampleData(input);
        }
        */

        [Route("/SampleData")]
        [HttpPost]
        [ConvertRequestBodyToFormData] //FromBody를 FormData로 변환
        public IActionResult SaveSampleData(List<DataModel> input)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(new { msg = "잘못된 요청입니다" });
            }

            return Json(input);

            using (var conn = new SqlConnection("connectionstring"))
            {
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in input)
                        {
                            switch (item.ItemState)
                            {
                                case ModelBase.ItemStateEnum.Deleted:
                                    item.Delete(conn);
                                    break;
                                case ModelBase.ItemStateEnum.Added:
                                    item.Insert(conn);
                                    break;
                                case ModelBase.ItemStateEnum.Modified:
                                    item.Update(conn);
                                    break;
                            }
                        }

                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return BadRequest(new { msg = ex.Message });
                    }
                }
            }

            return Ok();
        }
        #endregion
    }
}
