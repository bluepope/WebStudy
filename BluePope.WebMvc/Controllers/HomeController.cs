using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BluePope.WebMvc.Models;
using System.Data.SqlClient;

namespace BluePope.WebMvc.Controllers
{
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

        /// <summary>
        /// BypassFormDataInputFormatter 을 이용하여 먼저 formdata 확인 후 없으면 body에서 받음
        /// </summary>
        /// <param name="input"></param>
        /// <param name="apiInput"></param>
        /// <returns></returns>
        [Route("/SampleData")]
        [HttpPost]
        //[Consumes("application/x-www-form-urlencoded")]
        public IActionResult SaveSampleData(List<DataModel> input, [FromBody]List<DataModel> apiInput)
        {
            if (apiInput != null)
                input = apiInput;

            if (ModelState.IsValid == false)
            {
                return BadRequest(new { msg = "잘못된 요청입니다" });
            }

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
