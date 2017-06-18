using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace SpotzWeb.Models
{
    public class SpotzDetailViewModel
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public SpotzDetailViewModel(Spotz spotz)
        {
            Spotz = spotz;
            Tags = GetTagsArray();
        }
        public Spotz Spotz { get; set; }

        public double Distance { get; set; }

        public string[] Tags { get; set; }

        public string GetLatestImage()
        {
            if (Spotz.Images.Count > 0)
            {
                return Spotz.Images.OrderByDescending(i => i.Timestamp).First().ImageUrl;
            }
            return "";
        }

        public List<Tag> GetSomeTags(int number)
        {
            return Spotz.Tags.Take(number).ToList();
        }

        private string[] GetTagsArray()
        {
            return _db.Tags.Select(t => t.TagName).ToArray();
        }
    }

    public class SpotzListViewModel
    {
        public SpotzListViewModel()
        {
            Tags = new List<TagDto>();
            Comments = new List<CommentDto>();
        }

        public string SpotzId { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public double Distance { get; set; }
        public string ImageUrl { get; set; }
        public string GravatarUrl { get; set; }
        public List<TagDto> Tags { get; set; }
        public List<CommentDto> Comments { get; set; }

    }
    public class SpotzIndexViewModelWithTags
    {
        public SpotzIndexViewModelWithTags(Spotz spotz)
        {
            Spotz = spotz;
        }
        public Spotz Spotz { get; set; }
        public double Distance { get; set; }

    }

    public class AddSpotzViewModel
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        public AddSpotzViewModel()
        {
            Tags = GetTagsArray();
        }

        public string SpotzId { get; set; }
        [Required]
        [StringLength(20)]

        public string Title { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string UserName { get; set; }
        public string[] Tags { get; set; }

        public string TagNames { get; set; }

        private string[] GetTagsArray()
        {
            return _db.Tags.Select(t => t.TagName).ToArray();
        }

    }
}