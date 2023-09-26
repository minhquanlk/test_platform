using Firebase.Auth;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using test_platform.Models;
using test_platform.Models.Auth;
using test_platform.Services;

namespace test_platform.Controllers
{
    public class HomeController : Controller
    {
        private DBService _clientManager;
        public HomeController(DBService ClientManager)
        {
            _clientManager = ClientManager;
        }

        private string apiAuth = "AIzaSyAmGHmcNm1Ob6lShTCIMF5sPOh8KSl5UsQ";
        //private static string apiDB = "ToidM7c2xdGklPLnZco0h5xobznmqFTZx3HeXkzK";
        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        //{
        //    AuthSecret = apiDB,
        //    BasePath = "https://quizapp-d066c-default-rtdb.asia-southeast1.firebasedatabase.app"
        //};

        //IFirebaseClient client;

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserInfo candidate)
        {

            FirebaseAuthProvider auth = new FirebaseAuthProvider(
                                new Firebase.Auth.FirebaseConfig(apiAuth));
            
            string randomString = GenerateRandomString(10);
            bool haved = false;
            try
            {
                //create the user

                await auth.CreateUserWithEmailAndPasswordAsync(candidate.Email, randomString);


            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);

                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                haved = true;
             

            }

            if (haved != true)
            {
                try
                {
                    var client = _clientManager.FirebaseClient;
                    var data = candidate;
                    data.Password = randomString;
                    PushResponse response = client.Push("Candidates/", data);

                    data.Id = response.Result.name;
                    SetResponse setResponse = client.Set("Candidates/" + data.Id, data);

                    if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ModelState.AddModelError(string.Empty, "Added Succesfully");
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            

            return View();
            
        }


    }
}