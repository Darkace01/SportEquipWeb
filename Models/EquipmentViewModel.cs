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

        [Display(Name = "Is Available ?")]
        public bool IsAvaible { get; set; }

        [Display(Name = "Available Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime AvailableDate { get; set; }


        [Display(Name = "Daily Rate")]
        [Range(0, double.MaxValue)]
        public decimal DailyRate { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Image")]
        public string ImgPath { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImgFile { get; set; }
    }
}