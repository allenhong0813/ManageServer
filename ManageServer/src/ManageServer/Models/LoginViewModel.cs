using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageServer.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "帳號為必填欄位")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required(ErrorMessage = "密碼為必填欄位")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        

    }
}
