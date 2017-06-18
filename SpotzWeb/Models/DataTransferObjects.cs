using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpotzWeb;

namespace SpotzWeb.Models
{


    public class AddedComment
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
    }

    public class UpdateSimpleTextDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class AddTagDto
    {
        public string SpotzId { get; set; }
        public string TagId { get; set; }
        public string Text { get; set; }
    }
    public class RemoveTagDto
    {
        public string SpotzId { get; set; }
        public string TagId { get; set; }
    }
    public class TagDto
    {
        public string TagName { get; set; }
        public string TagId { get; set; }
    }
    public class ImageDto
    {
        public string ImageId { get; set; }
        public string ImageUrl { get; set; }
    }
    public class CommentDto
    {
        public string CommentId { get; set; }
        public string CommentText { get; set; }
        public string UserName { get; set; }
        public string CommentDate { get; set; }
        public string CommentUserImgUrl { get; set; }
    }

    public class SpotzDetailDto
    {
        HelperMethods helper = new HelperMethods();

        public SpotzDetailDto(Guid id)
        {

            SpotzId = id.ToString();
            ImageUrl = helper.GetLatestImageFromId(id);
            Comments = helper.GetCommentsFromSpotz(id);
            Tags = helper.GetTagsFromSpotz(id);
        }

        public string Title { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string SpotzId { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Timestamp { get; set; }
        public string ImageUrl { get; set; }
        public string GravatarUrl { get; set; }
        public string TagsJoined { get; set; }

        public virtual List<CommentDto> Comments { get; set; }
        public virtual List<TagDto> Tags { get; set; }
        public virtual List<ImageDto> Images { get; set; }



    }
}