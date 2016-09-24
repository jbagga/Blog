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
        public List<Post> Posts { get; set; }
        public SelectList Categories { get; set; }
        public string PostCategory { get; set; }
    }
    public class Post
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength =1)]
        public string Title { get; set; }

        [Display(Name = "Last Modified Date")]
        [DisplayFormat(DataFormatString = "{0:dddd, MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(60, MinimumLength = 1)]
        public string Category { get; set; }

        
        [StringLength(6000, MinimumLength = 1)]
        [Display(Name = "Blog Post")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [Display(Name = "Post by")]
        public string Author { get; set; }

        [Display(Name = "Comments")]
        public List<Comment> CommentList { get; set; }
    }
}
