using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class LoginViewModel
    {
        //[Required(ErrorMessageResourceName = "UsernameRequired",
        //          ErrorMessageResourceType = typeof(Login))]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        //[Required(ErrorMessageResourceName = "PasswordRequired",
        //           ErrorMessageResourceType = typeof(Login))]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool IsAdmin{ get; set; }


    }
}
