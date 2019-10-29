using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SportEquipWeb.Models;
using SportEquipWeb.Models.Core;

namespace SportEquipWeb.Controllers
{
    
    public class EquipmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public EquipmentController(ApplicationUserManager userManager)
        {

            UserManager = userManager;
        }
        public EquipmentController()
        {

        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Equipment
        //[Authorize(Roles = "Owner,Admin,User")]
        public ActionResult Index(string searchString,string category)
        {

            var equipment = (from s in db.Equipment
                             select s).Where(e => e.IsDeleted == false);
            if (!String.IsNullOrEmpty(searchString))
            {
                equipment = equipment.Where(s => s.Name.ToLower().Contains(searchString.ToLower())
                                             || s.Category.ToLower().Contains(searchString.ToLower())
                                             );

            }

            if(!(String.IsNullOrEmpty(category)) && category != "All")
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


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null || equipment.IsDeleted == true)
            {
                return HttpNotFound();
            }

            int res = equipment.AvailableDate.CompareTo(DateTime.Now);
            if (res > 0)
            {
                ViewBag.Available = false;
            }else if(res == 0){
                ViewBag.Available = false;
            }
            else
                ViewBag.Available = true;

            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // GET: Equipment/Delete/5
        [Authorize(Roles = "Owner,Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null || equipment.IsDeleted==true)
            {
                return HttpNotFound();
            }
            ViewBag.Error = TempData["DeleteEquipError"];
            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            string imgPath = "";
            try
            {
                Equipment equipment = db.Equipment.Find(id);
                imgPath = equipment.ImgPath;
                equipment.IsDeleted = true;
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["DeleteEquipError"] = "Error occured while deleteing equipment. Try again. If error persists, contact administrator";
                return RedirectToAction("Delete", id);
            }

            string path = Request.MapPath(imgPath);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public ActionResult OrderNow(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                Equipment equipment = db.Equipment.Find(id);
                if (equipment == null || equipment.IsDeleted == true)
                {
                    return HttpNotFound();
                }

                string userId = User.Identity.GetUserId();
                ApplicationUser applicationUser = db.Users.Find(userId);


                Transaction transaction = new Transaction()
                {
                    Equipment = equipment,
                    User = applicationUser,
                    DateCreated = DateTime.Now,
                };
                db.Transactions.Add(transaction);
                db.SaveChanges();
            }
            catch (Exception)
            {
                TempData["OrderError"] = "Failed to Order. Try again";
                return RedirectToAction("ConfirmOrder", id);
            }
            return RedirectToAction("OrderComfirmation");
        }

        //[HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult ComfirmOrder(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if(equipment == null ||equipment.IsDeleted==true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            int res = equipment.AvailableDate.CompareTo(DateTime.Now);
            if (res > 0)
            {
                equipment.IsAvaible = false;
            }
            else if (res == 0)
            {
                equipment.IsAvaible = false;
            }
            else
                equipment.IsAvaible = true;

            if (equipment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Error = TempData["OrderError"];
            return View(equipment);
        }
        [Authorize(Roles = "User")]
        public ActionResult UserTransactions()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);

            var transaction = (from s in db.Transactions
                             select s);
            var userTransaction = transaction.Where(t => t.User.Id == userId).Include(e => e.Equipment).ToList();
            return View(userTransaction);
        }

        

        


        public ActionResult UserDetails(string id)
        {

            if (String.IsNullOrEmpty(id))
            {

            }
            
            ApplicationUser applicationUser = db.Users.Find(id);

            if (applicationUser == null)
            {

            }

            if (UserManager.IsLockedOut(id))
            {
                ViewBag.Locked = true;
            }
            else
            {
                ViewBag.Locked = false;
            }
            return View(applicationUser);
        }

        public ActionResult OrderComfirmation()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
