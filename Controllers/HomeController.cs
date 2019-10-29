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
                int res = item.AvailableDate.CompareTo(DateTime.Now);
                if (res > 0)
                {
                    item.IsAvaible = false;
                }
                else if (res == 0)
                {
                    item.IsAvaible = false;
                }
                else
                    item.IsAvaible = true;
            }
            List<string> categories = new List<string>()
            {
                "All","Field","Track"
            };
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