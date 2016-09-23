using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{

    public class PostCategoryViewModel
    {
        public List<Post> posts;
        public SelectList categories;
        public string postCategory { get; set; }
    }
    public class Post
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength =1)]
        public string Title { get; set; }

        [Display(Name = "Last Modified Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(60, MinimumLength = 1)]
        public string Category { get; set; }

        
        [StringLength(6000, MinimumLength = 1)]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }
        public string Author { get; set; }

        public List<Comment> CommentList { get; set; }
    }
}
