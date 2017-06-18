using SpotzWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace SpotzWeb.DbRepository
{
    public class SpotzDbRepository
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();
        private readonly HelperMethods _helperMethods = new HelperMethods();

        public ApplicationUser GetUserByUserName(string userName)
        {
            return _db.Users.SingleOrDefault(u => u.UserName == userName);
        }

        public ApplicationUser GetUserByUserId(Guid id)
        {
            return _db.Users.Find(id);
        }

        // IMAGES
        public async Task<List<Image>> GetAllImagesAsync()
        {
            return await _db.Images.ToListAsync();
        }

        // SPOTZ

        public async Task<List<Spotz>> GetAllSpotzesAsync()
        {
            return await _db.Spotzes.ToListAsync();
        }

        public List<Spotz> GetAllSpotzes()
        {
            return _db.Spotzes.ToList();
        }

        public async Task<Spotz> GetSpotzAsync(Guid id)
        {
            return await _db.Spotzes.FindAsync(id);
        }

        // Lagrer ny spotz
        public Spotz AddSpotz(AddSpotzViewModel vm, HttpContext httpContext)
        {
            var user = _db.Users.Find(httpContext.User.Identity.GetUserId());

            var spotz = new Spotz
            {
                SpotzId = Guid.NewGuid(),
                Title = vm.Title,
                Description = vm.Description ?? "",
                User = user,
                Timestamp = DateTime.Now,
                Latitude = vm.Latitude ?? "",
                Longitude = vm.Longitude ?? ""
            };

            var listOfTags = new List<Tag>();

            // Lager liste med tags fra kommaseparert string som skal legges i Spotz
            if (vm.TagNames != null)
            {
                var listOfTagNames = vm.TagNames.TrimEnd(',').Split(',').ToList();

                foreach (var tag in listOfTagNames)
                {

                    var currentTag = _db.Tags.FirstOrDefault(t => t.TagName == tag);
                    // Hvis tag IKKE fins fra før:
                    if (currentTag == null)
                    {

                        currentTag = new Tag
                        {
                            TagName = tag,
                            TagId = Guid.NewGuid()
                        };

                        listOfTags.Add(currentTag);
                        currentTag.Spotzes.Add(spotz);

                        _db.Tags.Add(currentTag);
                        _db.Entry(currentTag).State = EntityState.Added;
                    }
                    else
                    {
                        listOfTags.Add(currentTag);
                        currentTag.Spotzes.Add(spotz);

                        _db.Entry(currentTag).State = EntityState.Modified;
                    }

                }


            }

            spotz.Tags = listOfTags;

            var request = httpContext.Request;
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
                    ImageUrl = string.Format($"/Images/{imageId}"),
                    Spotz = spotz
                };

                spotz.Images.Add(newImage);

                _db.Spotzes.Add(spotz);
                _db.Images.Add(newImage);
                _db.Entry(spotz).State = EntityState.Added;
                _db.Entry(newImage).State = EntityState.Added;

                _db.SaveChanges();
                return spotz;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }



        public Spotz GetSpotz(Guid id)
        {
            return _db.Spotzes.Find(id);
        }

        // TAGS

        public List<Tag> GetAllTags()
        {
            return _db.Tags.ToList();
        }

        public Tag GetTag(string tagName)
        {
            return _db.Tags.FirstOrDefault(t => t.TagName == tagName);
        }

        public Image GetImage(Guid id)
        {
            return _db.Images.Find(id);
        }

        public async Task RemoveImageAsync(Image image)
        {
            _db.Images.Remove(image);
            await _db.SaveChangesAsync();
        }
    }
}