using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using GeoCoordinatePortable;
using Microsoft.AspNet.Identity;

using SpotzWeb.Models;
using Microsoft.AspNet.Identity.Owin;
using SpotzWeb.DbRepository;

namespace SpotzWeb.Controllers
{
    public class SpotzApiController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helperMethods = new HelperMethods();
        private readonly SpotzDbRepository _dbRepository = new SpotzDbRepository();


        // GET: api/SpotzApi
        // Henter de nærmeste spotzes
        [System.Web.Http.Route("Api/GetSpotzesFromDistance")]
        [System.Web.Http.HttpGet]
        public IEnumerable<SpotzListViewModel> GetSpotzes(string latitude, string longitude)
        {
            var geoCoordsHere = new GeoCoordinate(double.Parse(latitude, CultureInfo.InvariantCulture), double.Parse(longitude, CultureInfo.InvariantCulture));

            //var spotzes = _db.Spotzes.ToList();
            var spotzes = _dbRepository.GetAllSpotzes();

            var vm = from spotz in spotzes
                     let distance = geoCoordsHere.GetDistanceTo(new GeoCoordinate(Convert.ToDouble(spotz.Latitude, CultureInfo.InvariantCulture), Convert.ToDouble(spotz.Longitude, CultureInfo.InvariantCulture)))
                     select new SpotzListViewModel
                     {
                         SpotzId = spotz.SpotzId.ToString(),
                         Distance = distance / 1000,
                         Title = spotz.Title,
                         Description = spotz.Description,
                         UserName = spotz.User.UserName,
                         UserId = spotz.User.Id,
                         Latitude = spotz.Latitude,
                         Longitude = spotz.Longitude,
                         ImageUrl = spotz.GetLatestImage(200),
                         GravatarUrl = spotz.User.GetGravatarUrl(),
                         ShortDescription = spotz.GetShortDescription(),
                         Tags = spotz.Tags.Select(s => new TagDto
                         {
                             TagId = s.TagId.ToString(),
                             TagName = s.TagName
                         })
                         .ToList()
                     };
            try
            {
                var sortedList = vm
                    .Where(o => o.Distance < 100)
                    .OrderBy(o => o.Distance)
                    .Take(7)
                    .ToList();
                return sortedList;
            }
            catch (Exception e)
            {
                var error = e.Message;
                Console.WriteLine(error);
                throw;
            }


        }

        // GET: api/SpotzApi/5
        [System.Web.Http.Route("api/GetImages/{id}"), System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetImages(Guid id)
        {

            var allImages = await _dbRepository.GetAllImagesAsync();
            var spotzImages = new List<ImageDto>();

            foreach (var image in allImages)
            {
                if (image.Spotz.SpotzId == id)
                {
                    spotzImages.Add(new ImageDto
                    {
                        ImageUrl = image.ImageUrl,
                        ImageId = image.ImageId.ToString()
                    });
                }
            }

            return Ok(spotzImages);
        }


        // Henter en spotz til Mobil
        [Route("api/GetOneSpotz/{id}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSpotz(string id)
        {
            var idGuid = Guid.Parse(id);

            var spotz = await _dbRepository.GetSpotzAsync(idGuid);
            if (spotz == null)
            {
                return NotFound();
            }

            var jsonSpotz = new SpotzDetailDto(idGuid)
            {
                Title = spotz.Title,
                Description = spotz.Description,
                ImageUrl = spotz.GetLatestImage(),
                UserName = spotz.User.UserName,
                GravatarUrl = spotz.User.GetGravatarUrl(),
                TagsJoined = spotz.GetTagsJoined(),
                ShortDescription = spotz.GetShortDescription(),
                Tags = spotz.Tags.Select(s => new TagDto
                {
                    TagId = s.TagId.ToString(),
                    TagName = s.TagName
                }).ToList(),
                Comments = spotz.Comments.Select(c => new CommentDto
                {
                    CommentText = c.Text,
                    CommentDate = c.Timestamp.ToShortDateString(),
                    UserName = c.User.UserName,
                    CommentUserImgUrl = c.User.GravatarUrl,

                }).ToList()
            };

            return Ok(jsonSpotz);
        }



        [System.Web.Http.Route("Api/FileUpload/{id}"), System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> FileUpload(string id)
        {


            var spotz = await _db.Spotzes.FindAsync(Guid.Parse(id));


            if (spotz == null)
            {
                //return NotFound();
                var newSpotz = new Spotz
                {
                    SpotzId = Guid.Parse(id),
                    Timestamp = DateTime.Now
                };
                try
                {
                    _db.Spotzes.Add(newSpotz);
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.ToString());
                    throw;
                }

            }

            var request = HttpContext.Current.Request;
            var file = request.Files[0];
            var title = request.Form["title"];
            var imageId = Guid.NewGuid();
            try
            {
                var newImage = new Image
                {
                    Data = _helperMethods.ConvertToByteArray(file.InputStream),
                    Timestamp = DateTime.Now,
                    Filename = title,
                    ImageId = imageId,
                    ImageUrl = Url.Route("Images", new { id = imageId }),
                    Spotz = spotz
                };

                if (spotz != null)
                {
                    spotz.Images.Add(newImage);
                    _db.Entry(spotz).State = EntityState.Modified;
                }

                _db.Images.Add(newImage);
                _db.SaveChanges();
                return Json(new { status = "complete", imgurl = newImage.ImageUrl });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { status = "error", message = e });
            }
        }




        [System.Web.Http.Route("api/SpotzApi/AddComment/")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddComment(AddedComment addedComment)
        {
            if (addedComment == null)
            {
                return NotFound();
            }
            if (addedComment.Comment == null)
            {
                return Json(new { status = "error", message = "No commment added!" });

            }

            var spotz = await _db.Spotzes.FindAsync(Guid.Parse(addedComment.Id));

            var newComment = new Comment
            {
                Timestamp = DateTime.Now,
                User = _db.Users.Find(addedComment.UserId),
                CommentId = Guid.NewGuid(),
                Text = addedComment.Comment,
                Spotz = spotz

            };
            try
            {
                spotz?.Comments.Add(newComment);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return Json(new { status = "error", message = "Error creating comment!" });
            }

            return Json(new { status = "success", message = "Comment created!", comment = addedComment.Comment, user = newComment.User.UserName, gravatarurl = newComment.User.GetGravatarUrl() });

        }

        [System.Web.Http.Route("api/UpdateDescriptionText/")]
        [System.Web.Http.HttpPost]
        public IHttpActionResult UpdateDescriptionText(UpdateSimpleTextDto updateDescriptionText)
        {

            if (updateDescriptionText.Text == "")
            {
                return Json(new { status = "error", message = "No text updated!" });
            }

            var spotz = _db.Spotzes.Find(Guid.Parse(updateDescriptionText.Id));

            try
            {
                if (spotz != null)
                {
                    spotz.Description = updateDescriptionText.Text;
                    _db.Entry(spotz).State = EntityState.Modified;
                }
                _db.SaveChanges();
                return Json(new { status = "success", message = "Description updated!", text = updateDescriptionText.Text });

            }
            catch (Exception)
            {
                return Json(new { status = "error", message = "Error updating description!" });
            }


        }

        [System.Web.Http.Route("Api/UpdateTitleText/"), System.Web.Http.HttpPost]
        public IHttpActionResult UpdateTitleText(UpdateSimpleTextDto updateTitleText)
        {

            if (updateTitleText.Text == "")
            {
                return Json(new { status = "error", message = "No text updated!" });
            }

            var spotz = _db.Spotzes.Find(Guid.Parse(updateTitleText.Id));

            try
            {
                if (spotz != null)
                {
                    spotz.Title = updateTitleText.Text;
                    _db.Entry(spotz).State = EntityState.Modified;
                }
                _db.SaveChanges();
                return Json(new { status = "success", message = "Updated!", text = updateTitleText.Text });
            }
            catch (Exception e)
            {
                return Json(new { status = "error", message = "Error updating!" + e.Message });
            }

        }

        [System.Web.Http.Route("api/DeleteImage/{id}")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> DeleteImage(string id)
        {

            if (id == "")
            {
                return Json(new { status = "error", message = "No image deleted!" });
            }


            var image = _db.Images.Find(Guid.Parse(id));
            //var image = _dbRepository.GetImage(Guid.Parse(id));

            if (image != null)
            {

                try
                {
                    _db.Images.Remove(image);
                    await _db.SaveChangesAsync();
                    //await _dbRepository.RemoveImageAsync(image);
                }
                catch (Exception)
                {
                    return Json(new { status = "error", message = "Error deleting!" });
                }
            }

            return Json(new { status = "success", message = "Deleted!", id = id });

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SpotzExists(Guid id)
        {
            return _db.Spotzes.Count(e => e.SpotzId == id) > 0;
        }
    }
}