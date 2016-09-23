using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public int PostID { get; set;}
        [Display(Name = "Comment Date")]
        [DataType(DataType.Date)]
        public DateTime CommentDate { get; set; }

        [StringLength(1000, MinimumLength = 1)]
        [Display(Name = "Comment")]
        public string CommentBody { get; set; }

        [Display(Name = "Comment by")]
        public string CommentAuthor { get; set; }
    }
}
