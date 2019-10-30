using Microsoft.AspNet.Identity;
using SportEquipWeb.Models;
using SportEquipWeb.Models.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SportEquipWeb.Controllers
{
    [Authorize(Roles ="Owner")]
    public class OwnerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        static List<string> Categories = new List<string>()
        {
            "Track","Field"
        };
        private static string editEquipError = "";
        private static string deleteEquipError = "";
        // GET: Owner
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            var equipment = (from s in db.Equipment
                             select s).Where(u => u.Owner.Id == userId && u.IsDeleted==false);

            ViewBag.OwnnerName = applicationUser.UserName;
            return View(equipment.ToList());
        }
        public ActionResult Dashboard()
        {
            return View();
        }


        // GET: Owner/Create
        [Authorize(Roles = "Owner")]
        public ActionResult Create()
        {
            ViewBag.Categories = Categories;
                   
            return View();
        }

        // POST: Owner/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Create([Bind(Include = "Id,Name,ShortDescription,LongDescription,ImgFile,ApplicationUserId,Owner,CategoryId,Category,DailyRate")] Equipment equipment)
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

                        fileName = fileName + DateTime.Now.ToString("yyyymmddhhmmssfff") + extension;
                        equipment.ImgPath = "~/Content/IMAGES/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/IMAGES/"), fileName);
                        equipment.ImgFile.SaveAs(fileName);
                    }
                    equipment.ApplicationUserId = userId;
                    equipment.Owner = applicationUser;
                    equipment.IsDeleted = false;
                    equipment.AvailableDate = DateTime.Now;
                    db.Equipment.Add(equipment);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //yet to handle
                    TempData["CreateError"] = "Failed to create equipment. Try again";
                    return RedirectToAction("Create", equipment.Id);
                    //throw;
                }
            }
            ViewBag.Categories = Categories;
            return View(equipment);
        }

        public ActionResult EquipmentList()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            var equipment = (from s in db.Equipment
                             select s).Where(u=>u.Owner.Id==userId && u.IsDeleted==false);

            ViewBag.OwnnerName = applicationUser.UserName;
            return View(equipment.ToList());
        }

        // GET: Owner/Edit/5
       
        public ActionResult Edit(int? id)
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

            EquipmentViewModel eq = new EquipmentViewModel()
            {
                Id = equipment.Id,
                Name=equipment.Name,
                ShortDescription=equipment.ShortDescription,
                LongDescription=equipment.LongDescription,
                ImgPath=equipment.ImgPath,
                
                DailyRate=equipment.DailyRate,
            };

            
            ViewBag.SelectedCategory = equipment.Category;
            Categories.Remove(equipment.Category);
            ViewBag.Categories = Categories;

            ViewBag.Error = editEquipError;
            editEquipError = "";

            return View(eq);
        }

        // POST: Owner/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]       
        public ActionResult Edit([Bind(Include = "Id,Name,ShortDescription,LongDescription,Category,ImgFile,DailyRate")]EquipmentViewModel eqViewModel)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    Equipment equipment = db.Equipment.Find(eqViewModel.Id);
                    string previousImagePath = "";
                    if (eqViewModel.ImgFile != null)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(eqViewModel.ImgFile.FileName);
                        string extension = Path.GetExtension(eqViewModel.ImgFile.FileName);

                        fileName = fileName + DateTime.Now.ToString("yyyymmddhhmmssfff") + extension;
                        previousImagePath =  equipment.ImgPath;
                        equipment.ImgPath = "~/Content/IMAGES/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/Content/IMAGES/"), fileName);
                        eqViewModel.ImgFile.SaveAs(fileName);
                    }

                    equipment.Name = eqViewModel.Name;
                    equipment.ShortDescription = eqViewModel.ShortDescription;
                    equipment.LongDescription = eqViewModel.LongDescription;
                    
                    equipment.IsAvaible = eqViewModel.IsAvaible;
                    equipment.DailyRate = eqViewModel.DailyRate;
                    equipment.Category = eqViewModel.Category;

                    db.Entry(equipment).State = EntityState.Modified;
                    db.SaveChanges();

                    //delete previous image
                    string path = Request.MapPath(previousImagePath);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    editEquipError = "Error occured while editing equipment. Try again.";
                    return RedirectToAction("Edit", new { id = eqViewModel.Id });
                    //throw;
                }
            }
            return View(eqViewModel);
        }

        public ActionResult Transactions()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);

            var transaction = (from s in db.Transactions
                               select s);
            var userTransaction = transaction.Where(t => t.Equipment.Owner.Id == userId).Include(e => e.Equipment).Include(u => u.User).ToList();
            return View(userTransaction);
        }

        // GET: Owner/Delete/5
        [Authorize(Roles = "Owner")]
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

        // POST: Owner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
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
                deleteEquipError = "Error occured while deleting. Try again.";
                return RedirectToAction("Delete",new { id = idPassed });
                
                //throw;
            }
            return RedirectToAction("Index");
        }


    }
}


