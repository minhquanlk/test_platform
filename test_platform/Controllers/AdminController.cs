using Microsoft.AspNetCore.Mvc;
using test_platform.Models.Auth;
using test_platform.Models.Quiz;
using test_platform.Models;
using test_platform.Services;
using FireSharp.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.OleDb;
using Newtonsoft.Json;
using Firebase.Auth;

namespace test_platform.Controllers
{

    public class AdminController : Controller
    {
        FirebaseAuthProvider auth;
        private readonly string apiAuth = "AIzaSyAmGHmcNm1Ob6lShTCIMF5sPOh8KSl5UsQ";
        private readonly DBService _clientManager;
        private UserList _candidate;
        private List<UserList> _userList = new List<UserList>();
        private IUserContext _userContext;
        private readonly IWebHostEnvironment _env;
        private IQuizContext _quizContext;
        public AdminController(DBService ClientManager, IUserContext userContext, IWebHostEnvironment env, IQuizContext quizContext)
        {
            _clientManager = ClientManager;
            _userContext = userContext;
            _env = env;
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig(apiAuth));
            _quizContext = quizContext; 
        }
        static List<int> GetRandomNumbers(List<int> list, int count)
        {
            Random random = new Random(DateTime.Now.Millisecond);

            List<int> randomNumbers = list.OrderBy(x => random.Next()).Take(count).ToList();

            return randomNumbers;
        }
        public IActionResult QuizList()
        {
            return View();
        }
        //public IActionResult UploadQuiz()
        //{
        //    var client = _clientManager.FirebaseClient;
        //    int i = 0;
        //    string sheetPath = Path.Combine(_env.WebRootPath, "data");
        //    sheetPath = Path.Combine(sheetPath, "Bookquestion.xlsx");

        //    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sheetPath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=1'";
        //    using (OleDbConnection conn = new OleDbConnection(ConnectionString))
        //    {

        //        DataTable dt = new DataTable();
        //        conn.Open();
        //        using (OleDbCommand comm = new OleDbCommand())
        //        {
        //            comm.Connection = conn;
        //            comm.CommandText = "select * from [Sheet1$A1:N121]";
        //            using (OleDbDataAdapter da = new OleDbDataAdapter(comm))
        //            {

        //                da.Fill(dt);
        //            }
        //        }
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            MultipleChoice _question = new MultipleChoice();
        //            _question.CorrectAnswer = new Dictionary<string, string>();
        //            _question.Choices = new Dictionary<string, string>();
        //            if (row[0].ToString() == "Done")
        //            {
        //                _question.IdPic = Convert.ToInt32(row[1]);
        //                _question.Id = row[2].ToString();
        //                _question.Topic = row[3].ToString();
        //                _question.Type = "single";
        //                _question.QuestionPic = Convert.ToString(row[5]);
        //                _question.QuestionText = row[6].ToString();
        //                _question.Point = Convert.ToDouble(row[7].ToString());
        //                _question.CorrectAnswer.Add("ans1", row[8].ToString());
        //                for (int j = 1; j < 6; j++)
        //                {
        //                    string choice = "choice" + j;
        //                    if (row[8 + j].ToString() != "")
        //                    {
        //                        _question.Choices.Add(choice, row[8 + j].ToString());
        //                    }
        //                    else { break; }

        //                }
        //                PushResponse response = client.Push("TestQuestion/", _question);
        //            }
        //            i++;
        //        }
        //    }

        //    return RedirectToAction("QuizList");
        //}
        //public async Task<IActionResult> UploadAccount()
        //{
        //    var client = _clientManager.FirebaseClient;
        //    int i = 0;
        //    string sheetPath = Path.Combine(_env.WebRootPath, "data");
        //    sheetPath = Path.Combine(sheetPath, "BookAccount.xlsx");

        //    string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + sheetPath + ";Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;MAXSCANROWS=1'";
        //    using (OleDbConnection conn = new OleDbConnection(ConnectionString))
        //    {

        //        DataTable dt = new DataTable();
        //        conn.Open();
        //        using (OleDbCommand comm = new OleDbCommand())
        //        {
        //            comm.Connection = conn;
        //            comm.CommandText = "select * from [Sheet1$]";
        //            using (OleDbDataAdapter da = new OleDbDataAdapter(comm))
        //            {

        //                da.Fill(dt);
        //            }
        //        }
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            UserInfo _user = new UserInfo();

        //            _user.CVName = row[0].ToString();
        //            _user.Email = row[1].ToString();
        //            _user.FullName = row[2].ToString();
        //            _user.Password = row[3].ToString();
        //            _user.Role = "User";
        //            await auth.CreateUserWithEmailAndPasswordAsync(_user.Email, _user.Password);
        //            //SetResponse response = client.Set("TestAccount/" + _user.Id, _user);
        //            PushResponse response = client.Push("Candidates/", _user);

        //            _user.Id = response.Result.name;
        //            SetResponse setResponse = client.Set("Candidates/" + _user.Id, _user);
        //            i++;
        //        }
        //    }

        //    return RedirectToAction("QuizList");
        //}
        //public IActionResult CreateTestQuest()
        //{
        //    List<int> numberList = new List<int>();
        //    List<int> logicalList = new List<int>();
        //    List<int> deducetiveList = new List<int>();
        //    List<int> deducetiveStarList = new List<int>();
        //    List<int> problemList = new List<int>();
        //    List<int> criticalList = new List<int>();
        //    for (int i = 1; i <= 40; i++)
        //    {
        //        numberList.Add(i);
        //    }
        //    for (int i = 41; i <= 52; i++)
        //    {
        //        logicalList.Add(i);
        //    }
        //    for (int i = 53; i <= 61; i++)
        //    {
        //        deducetiveList.Add(i);
        //    }
        //    for (int i = 62; i <= 70; i++)
        //    {
        //        deducetiveStarList.Add(i);
        //    }
        //    for (int i = 71; i <= 74; i++)
        //    {
        //        problemList.Add(i);
        //    }
        //    for (int i = 75; i <= 83; i++)
        //    {
        //        criticalList.Add(i);
        //    }

        //    var client = _clientManager.FirebaseClient;
        //    FirebaseResponse responseUser = client.Get("Candidates");
        //    var candidates = responseUser.ResultAs<Dictionary<string, UserInfo>>();
        //    FirebaseResponse responseAccount = client.Get("TestAccount");
        //    var testAccount = responseAccount.ResultAs<Dictionary<string, UserInfo>>();
        //    FirebaseResponse responseQuestion = client.Get("TestQuestion");
        //    var quesList = responseQuestion.ResultAs<Dictionary<string, MultipleChoice>>();
        //    int numCan = 0;
        //    foreach (var candidate in candidates)
        //    {
        //        numCan++;
        //        List<string> testList = new List<string>();
        //        int index = 0;
        //        string id = candidate.Key;
        //        List<int> numbericalQues = GetRandomNumbers(numberList, 15);
        //        List<int> logicalQues = GetRandomNumbers(logicalList, 7);
        //        List<int> deducetiveQues = GetRandomNumbers(deducetiveList, 4);
        //        List<int> deducetiveStarQues = GetRandomNumbers(deducetiveStarList, 4);
        //        List<int> criticalQues = GetRandomNumbers(criticalList, 6);
        //        List<int> resultIndex = numbericalQues.Concat(logicalQues).Concat(deducetiveQues).Concat(deducetiveStarQues).Concat(problemList).Concat(criticalQues).ToList();
        //        foreach (var pair in quesList)
        //        {
        //            index++;

        //            if (resultIndex.Contains(index))
        //            {
        //                testList.Add(pair.Key);
        //            }
        //        }
        //        if (numCan > 597)
        //        {
        //            SetResponse response = client.Set("TestAccountQuest/" + id, testList);
        //        }
        //    }







        //    return RedirectToAction("QuizList");
        //}
        public IActionResult UserList()
        {
            
            var token = HttpContext.Session.GetString("_UserToken");
            if (token != null)
            {
                try
                {
                    var client = _clientManager.FirebaseClient;
                    FirebaseResponse responseQuiz = client.Get("Candidates");
                    var userList = responseQuiz.ResultAs<Dictionary<string, UserList>>();
                    // Tìm candidate dựa trên email (thay "example@email.com" bằng email thực tế)

                    foreach (var candidate in userList)
                    {
                        _candidate = candidate.Value;
                        if (_candidate.Role == "User")
                        {
                            if (_candidate.Major == "Other")
                            {
                                _candidate.Major = _candidate.otherMajor;
                            }
                            if (_candidate.School == "Other")
                            {
                                _candidate.School = _candidate.otherSchool;
                            }
                            if (_candidate.City == "Other")
                            {
                                _candidate.City = _candidate.otherCity;
                            }
                            _userList.Add(_candidate);
                        }
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
        
        public FileResult DownloadFile(string fileName)
        {
            //Build the File Path.
            string path = Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(path, fileName);
            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}