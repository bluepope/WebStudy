using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace BluePope.WebApp.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string x)
        {
            var y = Request.Query["y"];

            ViewData["x"] = x;
            ViewData["y"] = y;
        }
    }
}
