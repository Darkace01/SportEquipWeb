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
using SportEquipWeb.Models;
using SportEquipWeb.Models.Core;

namespace SportEquipWeb.Controllers
{
    
    public class EquipmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Equipment
        //[Authorize(Roles = "Owner,Admin,User")]
        public ActionResult Index(string searchString)
        {
            var equipment = db.Equipment.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    equipment = equipment.Where(s => s.Name.ToLower().Contains(searchString.ToLower())
                                                 || s.Owner.UserName.ToLower().Contains(searchString.ToLower())
                                                 || s.Category.Name.ToLower().Contains(searchString.ToLower())
                                                 ).ToList();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            foreach (var item in equipment)
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
            return View(equipment);
        }

        // GET: Equipment/Details/5
        [Authorize(Roles = "Owner,Admin,User")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);

            int res = equipment.AvailableDate.CompareTo(DateTime.Now);
            if (res > 0)
            {
                equipment.IsAvaible = false;
            }else if(res == 0){
                equipment.IsAvaible = false;
            }
            else
                equipment.IsAvaible = true;

            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // GET: Equipment/Create
        [Authorize(Roles = "Owner,Admin")]
        public ActionResult Create()
        {
            ViewBag.Category = (from s in db.Category
                                select s.Name).ToList();
            return View();
        }

        // POST: Equipment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,ShortDescription,LongDescription,AvailableDate,ImgFile,ApplicationUserId,Owner,CategoryId,Category")] Equipment equipment)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);

            if (ModelState.IsValid)
            {
                try
                {

                    if (equipment.ImgFile != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(equipment.ImgFile.FileName);
                        string extension = Path.GetExtension(equipment.ImgFile.FileName);
                        if (!(extension.ToLower() == ".jpg" || extension.ToLower() == ".png" || extension.ToLower() == ".jpeg"))
                        {
                            ViewBag.ImgError = "File is not an image";
                            return View(equipment);
                        }
                        fileName = fileName + DateTime.Now.ToString("yyyymmddhhmmssfff") + extension;
                        equipment.ImgPath = "~/Content/IMAGES/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/IMAGES/"), fileName);
                        equipment.ImgFile.SaveAs(fileName);
                    }
                    equipment.ApplicationUserId = userId;
                    equipment.Owner = applicationUser;
                    db.Equipment.Add(equipment);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return View(equipment);
        }

        // GET: Equipment/Edit/5
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,ShortDescription,LongDescription,AvailableDate,ImgPath")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(equipment);
        }

        // GET: Equipment/Delete/5
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
            {
                return HttpNotFound();
            }
            return View(equipment);
        }

        // POST: Equipment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipment.Find(id);
            db.Equipment.Remove(equipment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
