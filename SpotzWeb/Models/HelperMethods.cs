using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace SpotzWeb.Models
{
    public class HelperMethods
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();


        public static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                foreach (byte t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }

        // Henter det siste bildet som er lagt inn:
        public string GetLatestImage(Spotz spotz)
        {
            if (spotz?.Images.Count > 0)
            {
                var url = string.Format($"{spotz.Images.OrderByDescending(i => i.Timestamp).First().ImageUrl}?thumb=300");
                return url;
            }
            return "";
        }

        public string GetLatestImageFromId(Guid id)
        {

            var spotz = _db.Spotzes.Find(id);
            if (spotz != null && spotz.Images.Count > 0)
            {
                return spotz.Images.OrderByDescending(i => i.Timestamp).First().ImageUrl;
            }

            return "";
        }

        public List<CommentDto> GetCommentsFromSpotz(Guid id)
        {
            var spotz = _db.Spotzes.Find(id);
            return spotz?.Comments
                .Select(c => new CommentDto
                {
                    CommentText = c.Text,
                    UserName = c.User.UserName,
                    CommentDate = c.Timestamp.ToShortTimeString()

                })
                .ToList();
        }

        public List<TagDto> GetTagsFromSpotz(Guid id)
        {

            var spotz = _db.Spotzes.Find(id);
            return spotz?.Comments
                .Select(c => new TagDto
                {
                    TagName = c.Text,

                })
                .ToList();
        }


        public byte[] ConvertToByteArray(Stream inputStream)
        {
            var stream = new MemoryStream();

            inputStream.CopyTo(stream);

            return stream.ToArray();
        }

        
    }
}