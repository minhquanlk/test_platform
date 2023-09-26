using FireSharp.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using test_platform.Models.Auth;

namespace test_platform.Services
{
    public interface IUserContext
    {
        UserAndRole UserWithRole { get;  }
        void SetUserWithRole(UserAndRole userWithRole);
    }

    public class UserContext : IUserContext
    {
        public UserAndRole UserWithRole { get; private set; }

        public void SetUserWithRole(UserAndRole userWithRole)
        {
            UserWithRole = userWithRole;
        }
    }

}
