using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotzWeb.Models
{
    public class Tag
    {

        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public virtual List<Spotz> Spotzes { get; set; } = new List<Spotz>();
        //public virtual List<TagsToSpotz> TagsToSpotzes { get; set; }
        //public virtual List<string> SpotzesId { get; set; } = new List<string>();
    }
}