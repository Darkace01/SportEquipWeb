using SportEquipWeb.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEquipWeb.Models
{
    public class EquipmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }
        [Display(Name = "Long Description")]
        public string LongDescription { get; set; }
        public bool IsAvaible { get; set; }
        public DateTime AvailableDate { get; set; }

        [Display(Name = "Image")]
        public string ImgPath { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImgFile { get; set; }
    }
}