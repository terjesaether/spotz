using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SpotzWeb.Models;
using Extensionmethods;

namespace SpotzWeb.Controllers
{
    public class TagsApiController : ApiController
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helperMethods = new HelperMethods();

        // GET: api/TagsApi
        public IEnumerable<TagDto> Get(string id)
        {
            //var spotz = _db.Spotzes.Find(Guid.Parse(id));

            var guidId = Guid.Parse(id);
            var tags = _db.Tags.ToList();
            var spotz = _db.Spotzes.Find(guidId);

            var spotzTags = spotz.Tags.Select(s => new TagDto
            {
                TagId = s.TagId.ToString(),
                TagName = s.TagName
            }).ToList();



            return spotzTags;
        }


        // DELETE: api/Tags/5
        [Route("Api/RemoveTag/"), HttpPost]
        public IHttpActionResult RemoveTag(RemoveTagDto dto)
        {

            var tag = _db.Tags.Find(Guid.Parse(dto.TagId));

            var spotz = _db.Spotzes.Find(Guid.Parse(dto.SpotzId));

            try
            {
                if (spotz != null)
                {
                    spotz.Tags.Remove(tag);
                    //tag?.Spotzes.Remove(spotz);
                    _db.Entry(spotz).State = EntityState.Modified;

                }


                _db.SaveChanges();
                return Json(new { status = "success", message = "Tag removed!" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Json(new { status = "error", message = "Error deleting tag!" });
            }

        }


        [Route("Api/AddTag/"), Authorize, HttpPost]
        public IHttpActionResult AddTag(UpdateSimpleTextDto dto)
        {
            if (dto == null)
            {
                return Json(new { status = "error", message = "Tag null!" });
            }

            var tagName = dto.Text.CapitalizeFirstLetter().Trim();

            // Current spotz:
            var spotz = _db.Spotzes.Find(Guid.Parse(dto.Id));

            // Hvis tag på current Spotz eksisterer, send ut!:
            if (spotz != null && spotz.Tags.Any(t => t.TagName == tagName))
            {
                return Json(new { status = "tagextists", message = "Tag exists on Spotz!" });
            }

            Tag addedTag;
            // Hvis tag med det samme navnet fins i databasen:
            if (_db.Tags.Any(t => t.TagName == tagName))
            {
                addedTag = _db.Tags.FirstOrDefault(t => t.TagName == tagName);

            }
            // Tag er helt ny:
            else
            {
                //var listOfSpotz = new List<Spotz> { spotz };

                addedTag = new Tag
                {
                    TagName = tagName,
                    //Spotzes = listOfSpotz,
                    TagId = Guid.NewGuid()
                };

                // Legger til tag i databasen

                _db.Tags.Add(addedTag);
                _db.Entry(addedTag).State = EntityState.Added;
            }

            // Legger til tagen på spotz
            spotz?.Tags.Add(addedTag);
            try
            {
                _db.Entry(spotz).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return Json(new { status = "success", message = "Tag added!", text = tagName, tagid = addedTag.TagId.ToString() });
        }

        // Slettes?
        //[Route("Api/AddTagNewSpotz/"), Authorize, HttpPost]
        //public IHttpActionResult AddTagNewSpotz(UpdateSimpleTextDto dto)
        //{
        //    if (dto == null)
        //    {
        //        return Json(new { status = "error", message = "Tag null!" });
        //    }

        //    var tagName = dto.Text.CapitalizeFirstLetter().Trim();



        //    Tag addedTag;
        //    // Sjekker om tag med det samme navnet fins i databasen:
        //    if (_db.Tags.Any(t => t.TagName == tagName))
        //    {
        //        addedTag = _db.Tags.FirstOrDefault(t => t.TagName == tagName);

        //        //if (addedTag != null)
        //        //{
        //        //    addedTag.Spotzes.Add(tempSpotz);
        //        //    _db.Entry(addedTag).State = EntityState.Modified;

        //        //}
        //    }
        //    // Hvis Tag er helt ny:
        //    else
        //    {

        //        // Hvis tagen er helt ny (fins det ingen andre Spotz med den tagen):
        //        //addedTag = new Tag
        //        //{
        //        //    TagName = tagName,
        //        //    Spotzes = listOfSpotz,
        //        //    TagId = Guid.NewGuid()
        //        //};

        //        //// Legger til tag i databasen
        //        //_db.Tags.Add(addedTag);
        //        //_db.Entry(addedTag).State = EntityState.Added;

        //    }

        //    try
        //    {
        //        //_db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(e.Message);
        //        throw;
        //    }

        //    //return Json(new { status = "success", message = "Tag added!", text = tagName, tagid = addedTag.TagId.ToString() });
        //    return Json(new { status = "success", message = "Tag added!", text = tagName, tagid = "" });

        //}

    }

}



