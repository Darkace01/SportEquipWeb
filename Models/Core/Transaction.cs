using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SportEquipWeb.Models.Core
{
    public class Transaction
    {
        public int Id { get; set; }
        public Equipment Equipment { get; set; }
        [Display(Name ="Number of days rented")]
        public int NumberOfDaysRented { get; set; }
        public decimal Amount { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime DateCreated { get; set; }

    }
}