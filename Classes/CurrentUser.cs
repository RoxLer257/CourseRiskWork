using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk_Work.Classes
{
    public static class CurrentUser
    {
        public static string Username { get; set; }
        public static string PasswordHash { get; set; }
        public static string Role { get; set; }
    }
}
