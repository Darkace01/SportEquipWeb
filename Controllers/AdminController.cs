using SportEquipWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net;
using SportEquipWeb.Models.Core;
using System.Data.Entity;

namespace SportEquipWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static string deleteEquipError = "";
        private static string unlockUserError = "";
        private static string lockUserError = "";


        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EquipmentList()
        {
            var equipment = (from s in db.Equipment
                             select s).Where(e => e.IsDeleted == false);
            return View(equipment.ToList());
        }

        [HttpGet]
        public ActionResult AllCategory()
        {
            var category = (from s in db.Category
                            select s).ToList();
            return View(category);
        }
        public ActionResult CreateCategory()
        {
            ViewBag.CreateCategoryError = TempData["CategoryCreateError"];
            return View();
        }
        
        public ActionResult CategoryDetail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory([Bind(Include = "Id,Name")]Category category)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    db.Category.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["CategoryCreateError"] = "Failed to Create a Category";
                    return RedirectToAction("CreateCategory");
                }
            }
            return View(category);
        }

        public ActionResult EditCategory(int? id)
        {
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            CategoryViewModel _category = new CategoryViewModel()
            {
                Name = category.Name,
            };
            ViewBag.CreateCategoryError = TempData["CategoryEditError"];
            return View(_category);
        }

        [HttpPost]
        public ActionResult EditCategory([Bind(Include = "Id,Name")]CategoryViewModel categoryView)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Category category = db.Category.Find(categoryView.Id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }

                    category.Name = categoryView.Name;
                    db.Entry(category).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("AllCategory");
                }
                catch (Exception)
                {
                    TempData["CategoryEditError"] = "Failed to Edit a Category";
                    return RedirectToAction("EditCategory", new { id = categoryView.Id });
                }
            }
            return View(categoryView);
        }
        public ActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryDeleteError = TempData["CategoryDeleteError"];
            return View(category);
        }
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategoryConfirmed(int id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Category category = db.Category.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }
                db.Category.Remove(category);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                TempData["CategoryDeleteError"] = "Delete Failed. Try Again";
            }
            return RedirectToAction("AllCategory");
        }
        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
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

            ViewBag.Error = deleteEquipError;
            deleteEquipError = "";

            return View(equipment);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int idPassed = id;
            try
            {

                Equipment equipment = db.Equipment.Find(id);

                equipment.IsDeleted = true;
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();

                string path = Request.MapPath(equipment.ImgPath);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception)
            {
                deleteEquipError = "Delete Failed. Try again. If problem persists, contact administrator";
                return RedirectToAction("Delete", new { id = idPassed });
            }

            return RedirectToAction("EquipmentList");
        }


        public ActionResult AllUsers()
        {
            var allUsers = (from s in db.Users
                            select s).Where(u => u.UserName != "admin@gmail.com").ToList();

            ViewBag.Error = unlockUserError;
            unlockUserError = "";
            return View(allUsers.ToList());
        }

        public ActionResult UnlockUser(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {

                ApplicationUser applicationUser = db.Users.Find(id);
                if (applicationUser == null)
                {
                    return HttpNotFound();
                }


                applicationUser.IsEnabled = true;
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {

                unlockUserError = "Unlock user failed. Try again. If problem persists, contact administrator";
                return RedirectToAction("AllUsers");
            }
            return RedirectToAction("AllUsers");
        }
        public ActionResult LockUser(string id)
        {

            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);

            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.Error = lockUserError;
            lockUserError = "";
            return View(applicationUser);
        }

        public ActionResult LockOutConfirmed(string id)
        {
            string idPassed = id;

            if (String.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            try
            {

                ApplicationUser applicationUser = db.Users.Find(id);
                if (applicationUser == null)
                {
                    return HttpNotFound();
                }

                applicationUser.IsEnabled = false;
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                lockUserError = "Lock failed. Try again. If problem persists, contact administrator";
                return RedirectToAction("LockUser", new { id = idPassed });
            }
            return RedirectToAction("AllUsers");

        }

        public ActionResult AllTransactions()
        {
            var transaction = (from s in db.Transactions
                               select s);
            var allTransaction = transaction.Include(e => e.Equipment).Include(u => u.User).ToList();
            return View(allTransaction);
        }
    }
}