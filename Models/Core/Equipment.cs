using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SportEquipWeb.Models.Core
{
    public class Equipment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; } = DateTime.Now;
        public string ApplicationUserId { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }
        //[Display(Name = "Intro")]
        //public string ShortDescription { get; set; }
        [Display(Name = "Details")]
        public string LongDescription { get; set; }
        [Display(Name ="Is Available ?")]
        public bool IsAvaible { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name ="Available Date")]
        public DateTime AvailableDate { get; set; }
        [Required]
        public string Category { get; set; }

        [Range(0,double.MaxValue)]
        [Display(Name ="Daily Rate")]
        public decimal DailyRate { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Image")]
        public string ImgPath { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImgFile { get; set; }


        public string MyProperty { get; set; }
    }
}