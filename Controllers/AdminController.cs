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

                //throw;
            }

            return RedirectToAction("EquipmentList");
        }


    }
}