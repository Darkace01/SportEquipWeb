using SportEquipWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportEquipWeb.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string searchString, string category)
        {
            var equipment = (from s in db.Equipment
                             select s).Where(e => e.IsDeleted == false);
            if (!String.IsNullOrEmpty(searchString))
            {
                equipment = equipment.Where(s => s.Name.ToLower().Contains(searchString.ToLower())
                                             || s.Category.ToLower().Contains(searchString.ToLower())
                                             );

            }

            if (!(String.IsNullOrEmpty(category)) && category != "All")
            {
                equipment = equipment.Where(s => s.Category.ToLower() == category.ToLower());
            }


            foreach (var item in equipment.ToList())
            {
                if (item.AvailableDate <= DateTime.Now)
                {
                    item.IsAvaible = true;
                }
                else
                {
                    item.IsAvaible = false;
                }
            }
            List<string> categories = (from s in db.Category
                                       select s.Name).ToList();
            ViewBag.SelectedCategory = category;
            categories.Remove(category);
            ViewBag.Categories = categories;


            return View(equipment.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}