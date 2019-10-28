using Microsoft.AspNet.Identity;
using SportEquipWeb.Models;
using SportEquipWeb.Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            return View();
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

        public ActionResult EquipmentList()
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            var equipment = (from s in db.Equipment
                             select s).Where(u=>u.Owner.Id==applicationUser.Id);
            return View(equipment.ToList());
        }
    }
}