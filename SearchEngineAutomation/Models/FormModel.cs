using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SearchEngineAutomation.Models
{
    public class FormModel
    {
        [Required]
        public string Keywords { get; set; }
        [Required]
        public string Url  { get; set; }
    }
}