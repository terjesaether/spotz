using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace SpotzDroid
{
    // Fjerne?
    //public class SpotzListViewModel
    //{

    //    public string SpotzId { get; set; }


    //    public string UserName { get; set; }

    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    //public string Longitude { get; set; }
    //    //public string Latitude { get; set; }
    //    public string Distance { get; set; }
    //    public string ImageUrl { get; set; }
    //    public string GravatarUrl { get; set; }
    //    //public DateTime Timestamp { get; set; }
    //    public List<TagDto> Tags { get; set; }

    //}

    public class SpotzViewModel
    {
        public string SpotzId { get; set; }

        public string UserName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Distance { get; set; }
        public string ImageUrl { get; set; }
        public string GravatarUrl { get; set; }
        public string TagsJoined { get; set; }
        public string Timestamp { get; set; }
        public List<TagDto> Tags { get; set; }
        public List<CommentDto> Comments { get; set; }
    }

    //public class CommentAndTagDto
    //{
    //    public string Id { get; set; }
    //    public string Text { get; set; }
    //}

    public class TagDto
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
    }

    public class CommentDto
    {
        public string CommentId { get; set; }
        public string CommentText { get; set; }
        public string UserName { get; set; }
        public string CommentUserImgUrl { get; set; }
    }
}