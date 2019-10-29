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
            return View(equipment);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipment.Find(id);

            try
            {
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
                ViewBag.Error = "Delete Failed. Try again. If problem persists, contact administrator";
                return RedirectToAction("Delete", id);
            }

            return RedirectToAction("EquipmentList");
        }

        
        public ActionResult AllUsers()
        {
            var allUsers = (from s in db.Users
                            select s).Where(u => u.UserName != "admin@gmail.com").ToList();


            return View(allUsers.ToList());
        }

        public ActionResult UnlockUser(string id)
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

            try
            {
                applicationUser.IsEnabled = true;
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {

                ViewBag.Error = "Unlock user failed. Try again. If problem persists, contact administrator";
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
            return View(applicationUser);
        }
        
        public ActionResult LockOutConfirmed(string id)
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

            try
            {
                applicationUser.IsEnabled = false;
                db.Entry(applicationUser).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Error = "Lock failed. Try again. If problem persists, contact administrator";
                return RedirectToAction("LockUser", id);
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