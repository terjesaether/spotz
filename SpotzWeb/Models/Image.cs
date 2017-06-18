using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotzWeb.Models
{
    public class Image
    {
        public Guid ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string Filename { get; set; }
        public DateTime Timestamp { get; set; }
        public byte[] Data { get; set; }
        public virtual Spotz Spotz { get; set; }
    }
}