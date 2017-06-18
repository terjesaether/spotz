using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SpotzWeb.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }

        //[Column("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Timestamp { get; set; }
        //[Column("SpotzId")]
        public virtual Spotz Spotz { get; set; }
    }
}