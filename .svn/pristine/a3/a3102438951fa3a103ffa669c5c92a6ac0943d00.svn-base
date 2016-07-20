using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt_Password_Generator.Core.Database;

namespace BCrypt_Password_Generator
{
    class Search
    {
        public IQueryable<User> SearchUser(String UserName, String UserEmailAddress)
        {
            User u = new User();
            WebsiteDataContext db1 = new WebsiteDataContext();
            IQueryable<User> search_user = null;

            if (!string.IsNullOrEmpty(UserName))
            {
                search_user = db1.Users.Where(x => x.UserName == (UserName));
                if (search_user != null && !string.IsNullOrEmpty(UserEmailAddress))
                {
                    search_user = search_user.Where(x => x.UserEmailAddress == UserEmailAddress);
                }
                return search_user;
            }

            if (!string.IsNullOrEmpty(UserEmailAddress))
            {
                search_user = db1.Users.Where(x => x.UserEmailAddress == (UserEmailAddress));

            }
            return search_user;
        }

        public IQueryable<User> SearchUser(Guid UserID)
        {
            User u = new User();
            WebsiteDataContext db1 = new WebsiteDataContext();
            IQueryable<User> search_user = null;

            if (UserID != Guid.Empty)
                search_user = db1.Users.Where(x => x.UserID == UserID);

            return search_user;
        }
    }
}
