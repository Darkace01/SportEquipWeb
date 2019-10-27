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
        [Display(Name = "Short Description")]
        public string ShortDescription { get; set; }
        [Display(Name = "Long Description")]
        public string LongDescription { get; set; }
        public bool IsAvaible { get; set; }
        public DateTime AvailableDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "Image")]
        public string ImgPath { get; set; }
        [NotMapped]
        public HttpPostedFileBase ImgFile { get; set; }
    }
}