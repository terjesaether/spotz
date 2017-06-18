using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpotzWeb.Models
{
    public class Spotz
    {

        public Spotz()
        {
        }

        public Guid SpotzId { get; set; }
        public virtual ApplicationUser User { get; set; }


        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
        public virtual List<Tag> Tags { get; set; } = new List<Tag>();
        public virtual List<Image> Images { get; set; } = new List<Image>();


        public string GetLatestImage()
        {
            if (Images.Count > 0)
            {
                return Images.OrderByDescending(i => i.Timestamp).First().ImageUrl;
            }
            return "";
        }

        public string GetLatestImage(int thumbSize)
        {
            if (Images.Count > 0)
            {
                return Images.OrderByDescending(i => i.Timestamp).First().ImageUrl + "?thumb=" + thumbSize;
            }
            return "";
        }

        public string GetShortDescription()
        {
            if (Description.Length > 30)
            {
                return Description.Substring(0, 29) + "... ";
            }
            return Description;
        }

        public string GetTagsJoined()
        {
            string tagString = "";
            if (Tags.Count > 0)
            {
                tagString = string.Join(", ", Tags.Select(t => t.TagName).ToArray());
            }
            return tagString;
        }

        //public string GetGravatarUrl()
        //{
        //    return User.GetGravatarUrl();
        //}



    }
}