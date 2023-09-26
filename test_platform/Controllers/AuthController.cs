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
    
    public class AuthController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly string apiAuth = "AIzaSyAmGHmcNm1Ob6lShTCIMF5sPOh8KSl5UsQ";
        private readonly DBService _clientManager;
        private UserInfo _candidate;
        private IUserContext _userContext;


        public AuthController(DBService ClientManager, IUserContext userContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig(apiAuth));
        }


        public IActionResult SignIn()
        {
            var token = HttpContext.Session.GetString("_UserToken");
            if (token != null)
            {
                return RedirectToAction("User", "Test", new { id = _userContext.UserWithRole.User.Id });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInView loginModel)
        {
            
            try
            {
                //log in an existing user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                
                string token = fbAuthLink.FirebaseToken;
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);
                    var client = _clientManager.FirebaseClient;
                    var mail = loginModel.Email;
                    FirebaseResponse responseUser = client.Get("Candidates");
                    var candidates = responseUser.ResultAs<Dictionary<string, UserInfo>>();
                    
                    foreach (var candidate in candidates)
                    {
                        if (candidate.Value.Email == mail)
                        {
                            UserAndRole _userWithRole = new UserAndRole();
                            _candidate = candidate.Value;
                            _userWithRole.User = _candidate;
                            _userWithRole.Role = _candidate.Role;
                            _userContext.SetUserWithRole(_userWithRole);
                            break;
                        }
                    }
                    return RedirectToAction("User", "Test", new { id = _candidate.Id });
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

            return View();
        }
        public IActionResult SignOut()
        {
            HttpContext.Session.Remove("_UserToken");
            _userContext.SetUserWithRole(new UserAndRole());   
            return RedirectToAction("SignIn");
        }
    }
}