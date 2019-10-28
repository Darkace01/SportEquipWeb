using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportEquipWeb.Models.Core
{
    public class Transaction
    {
        public int Id { get; set; }
        public Equipment Equipment { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime DateCreated { get; set; }

    }
}