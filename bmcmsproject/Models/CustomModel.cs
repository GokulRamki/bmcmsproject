using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace bmcmsproject.Models
{
    public class CustomModel
    {
    }
    public class adminmodel
    {
        [Key]
        public int id { get; set; }
        [Required()]
        [Display(Name = "Username")]
        public string username { get; set; }
        [Required()]
        [Display(Name = "Password")]
        public string password { get; set; }
    }
}