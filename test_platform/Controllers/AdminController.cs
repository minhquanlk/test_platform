using Microsoft.AspNetCore.Mvc;
using test_platform.Models.Auth;
using Firebase.Auth;
using Newtonsoft.Json;

using test_platform.Models.Quiz;
using test_platform.Models;
using test_platform.Services;
using FireSharp.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace test_platform.Controllers
{

    public class AdminController : Controller
    {
        
        private readonly DBService _clientManager;
        private UserInfo _candidate;
        private List<UserInfo> _userList = new List<UserInfo>();
        private IUserContext _userContext;


        public AdminController(DBService ClientManager, IUserContext userContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
        }


        public IActionResult UserList()
        {
            
            var token = HttpContext.Session.GetString("_UserToken");
            if (token != null)
            {
                try
                {
                    var client = _clientManager.FirebaseClient;
                    FirebaseResponse responseQuiz = client.Get("Candidates");
                    var userList = responseQuiz.ResultAs<Dictionary<string, UserInfo>>();
                    // Tìm candidate dựa trên email (thay "example@email.com" bằng email thực tế)

                    foreach (var candidate in userList)
                    {
                        _candidate = candidate.Value;
                        _userList.Add(_candidate);
                    }
                    return View(_userList);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "connect database error");

                }
            }
            return RedirectToAction("SignIn", "Auth");
        }
    }
}