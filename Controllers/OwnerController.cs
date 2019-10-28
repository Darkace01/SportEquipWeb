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
        // GET: Owner
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            var equipment = (from s in db.Equipment
                             select s).Where(u => u.Owner.Id == userId);

            ViewBag.OwnnerName = applicationUser.UserName;
            return View(equipment.ToList());
        }



        // GET: Owner/Create
        [Authorize(Roles = "Owner")]
        public ActionResult Create()
        {
            ViewBag.Category = (from s in db.Category
                                select s.Name).ToList();
            return View();
        }

        // POST: Owner/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
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

        public ActionResult EquipmentList()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);
            var equipment = (from s in db.Equipment
                             select s).Where(u=>u.Owner.Id==userId);

            ViewBag.OwnnerName = applicationUser.UserName;
            return View(equipment.ToList());
        }

        // GET: Owner/Edit/5
        [Authorize(Roles = "Owner")]
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

            EquipmentViewModel eq = new EquipmentViewModel()
            {
                Id = equipment.Id,
                Name=equipment.Name,
                ShortDescription=equipment.ShortDescription,
                LongDescription=equipment.LongDescription,
                IsAvaible=equipment.IsAvaible,
                ImgPath=equipment.ImgPath,
                AvailableDate=equipment.AvailableDate,
            };

            return View(eq);
        }

        // POST: Owner/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Edit([Bind(Include = "Id,Name,ShortDescription,LongDescription,AvailableDate,ImgFile")] Equipment eqViewModel)
        {

            if (ModelState.IsValid)
            {
                Equipment equipment = db.Equipment.Find(eqViewModel.Id);
                if (eqViewModel.ImgFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(eqViewModel.ImgFile.FileName);
                    string extension = Path.GetExtension(eqViewModel.ImgFile.FileName);

                    fileName = fileName + DateTime.Now.ToString("yyyymmddhhmmssfff") + extension;
                    equipment.ImgPath = "~/Content/IMAGES/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Content/IMAGES/"), fileName);
                    eqViewModel.ImgFile.SaveAs(fileName);
                }

                equipment.Name = eqViewModel.Name;
                equipment.ShortDescription = eqViewModel.ShortDescription;
                equipment.LongDescription = eqViewModel.LongDescription;
                equipment.AvailableDate = eqViewModel.AvailableDate;
                equipment.IsAvaible = eqViewModel.IsAvaible;



                db.Entry(equipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eqViewModel);
        }

    }
}


