using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using SpotzWeb.Models;
using System.Net;
using System.Web.Http;
using System.Net.Http;

namespace SpotzWeb.Controllers
{
    public class LoginController : ApiController
    {

        private ApplicationDbContext _db = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public LoginController()
        {
        }

        public LoginController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // Denne er hjemmelaget!
        // POST: /Account/Login
        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        public async Task<HttpResponseMessage> Login(string email, string password, bool rememberMe)
        {

            var result = await SignInManager.PasswordSignInAsync(email, password, rememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Request.CreateResponse(HttpStatusCode.Accepted, "Success");

                case SignInStatus.LockedOut:
                //return Json(new { foo = "bar", baz = "Blech" });

                case SignInStatus.Failure:
                default:
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Please Enter valid UserName and Password");
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        public async Task<HttpResponseMessage> Register(string email, string password, bool rememberMe)
        {
            if (ModelState.IsValid)
            {
                var mailHashed = HelperMethods.CreateMd5(email.Trim());

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    GravatarUrl = string.Format($@"https://www.gravatar.com/avatar/{mailHashed}")
                };
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return Request.CreateResponse(HttpStatusCode.Accepted, "Success");
                }
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong!");
            }
            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Something went wrong!");

        }


    }
}
