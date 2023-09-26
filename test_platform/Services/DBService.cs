using FireSharp.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using test_platform.Models.Auth;

namespace test_platform.Services
{
    public class DBService
    {
        private string apiDB = "ToidM7c2xdGklPLnZco0h5xobznmqFTZx3HeXkzK";
        private string basePath = "https://quizapp-d066c-default-rtdb.asia-southeast1.firebasedatabase.app";
        public IFirebaseClient FirebaseClient { get; private set; }

        public DBService()
        {
            IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
            {
                AuthSecret = apiDB,
                BasePath = basePath
            };

            FirebaseClient = new FireSharp.FirebaseClient(config);
        }
    }
   
}
