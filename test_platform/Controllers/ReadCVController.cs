using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace test_platform.Controllers
{
   
    public class ReadCVController : Controller
    {
		public IActionResult ViewPDF()
		{
			
			var path = Uri.EscapeUriString(HttpContext.Request.GetDisplayUrl());
            var segments = path.Split('/');
			var lastSegment = segments.LastOrDefault();
            string filePath = "uploads/" + lastSegment;

			

			return View("ViewPDF", filePath);
		}

	}
}