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
        public ActionResult Index(string searchString)
        {
            
            var equipment = (from s in db.Equipment
                             select s);
            if (!String.IsNullOrEmpty(searchString))
            {
                try
                {
                    equipment = equipment.Where(s => s.Name.ToLower().Contains(searchString.ToLower())
                                                 || s.Owner.UserName.ToLower().Contains(searchString.ToLower())
                                                 || s.Category.Name.ToLower().Contains(searchString.ToLower())
                                                 );
                }
                catch (Exception ex)
                {

                    throw;
                }
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
           return View(equipment.ToList());
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
            return View(equipment);
        }

        // POST: Equipment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
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
        [Authorize(Roles = "Owner,Admin")]
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
        [Authorize(Roles = "Owner,Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Equipment equipment = db.Equipment.Find(id);
            db.Equipment.Remove(equipment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public ActionResult OrderNow(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Equipment equipment = db.Equipment.Find(id);
            if (equipment == null)
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
            return RedirectToAction("Index");
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
            if(equipment == null)
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
            return View(equipment);
        }
        [Authorize(Roles = "User")]
        public ActionResult UserTransactions(int id = 0)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(userId);

            var transaction = (from s in db.Transactions
                             select s);
            var userTransaction = transaction.Where(t => t.User.Id == userId).Include(e => e.Equipment).ToList();
            return View(userTransaction);
        }

        [Authorize(Roles ="Admin")]
        public ActionResult AllTransactions()
        {
            var transaction = (from s in db.Transactions
                               select s);
            var allTransaction = transaction.Include(e => e.Equipment).Include(u => u.User).ToList();
            return View(allTransaction);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AllUsers()
        {
            var allUsers = (from s in db.Users
                            select s).Where(u=>u.UserName!="admin@gmail.com").ToList();

            
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

            applicationUser.IsEnabled = true;
            db.Entry(applicationUser).State = EntityState.Modified;
            db.SaveChanges();
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

        [Authorize(Roles ="Admin")]
        public ActionResult EquipmentList()
        {
            var equipment = (from s in db.Equipment
                             select s);
            return View(equipment.ToList());
        }
     
        [Authorize(Roles = "Admin")]
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

            applicationUser.IsEnabled = false;
            db.Entry(applicationUser).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AllUsers");
            
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
