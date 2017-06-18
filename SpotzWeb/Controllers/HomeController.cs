using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SpotzWeb.Models;
using SpotzWeb.DbRepository;
using System.Collections.Generic;

namespace SpotzWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly SpotzDbRepository _dbRepository = new SpotzDbRepository();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Users()
        {
            ViewBag.Message = "Your application description page.";

            //var vm = new List<ApplicationUser>();
            var vm = _db.Users.ToList();

            return View(vm);
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
          
            return View();
        }

        // Gets Spotz by Tag

        [HttpGet, Route("Home/GetSpotzByTag/{tagName}")]
        public ActionResult GetSpotzByTag(string tagName)
        {
            if (tagName == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var firstOrDefault = _db.Tags.FirstOrDefault(t => t.TagName == tagName);
            if (firstOrDefault == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var spotzez = firstOrDefault.Spotzes.ToList();

            ViewBag.TagName = tagName;

            return View(spotzez);
        }

        [HttpGet]
        public ActionResult GetSpotzByUser(string id)
        {

            var user = _db.Users.Find(id);

            var spotzes = _dbRepository.GetAllSpotzes()
                .Where(s => s.User.UserName == user.UserName)
                .ToList();

            ViewBag.UserName = user.UserName;

            return View(spotzes);
        }

        // GET: Spotz/Details/5
        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var spotz = await _dbRepository.GetSpotzAsync(id.Value);
            if (spotz == null)
            {
                return HttpNotFound();
            }

            var vm = new SpotzDetailViewModel(spotz);

            return View(vm);
        }



        [HttpGet, Authorize]
        public ActionResult NewzSpotz()
        {
            ViewBag.NewGuid = Guid.NewGuid();

            var vm = new AddSpotzViewModel();

            return View(vm);
        }

        [HttpPost, Authorize]
        public ActionResult NewSpotz(AddSpotzViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("AddSpotzError");
            }

            if (vm.Latitude == null || vm.Longitude == null)
            {
                return Json(new { status = "error", message = "No coordinates" });
            }

            HttpContext httpContext = System.Web.HttpContext.Current;

            var spotz = _dbRepository.AddSpotz(vm, httpContext);

            if (spotz == null)
            {
                return Json(new { status = "error", message = "No saving!" });
            }
            return RedirectToAction("Index");


        }



    }


}