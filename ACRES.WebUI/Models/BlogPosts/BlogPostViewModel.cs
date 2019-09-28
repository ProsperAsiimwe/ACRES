using ACRES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ACRES.WebUI.Models.BlogPosts
{
    public class BlogPostViewModel
    {
        public BlogPostViewModel() {}

        public BlogPostViewModel(BlogPost BlogPost) {

            SetFromBlogPost(BlogPost);
        }

        public int BlogPostId { get; set; }

        
        [Display(Name = "Blog Image")]
        
        public HttpPostedFileBase BlogImage { get; set; }

        //[Required(ErrorMessage = "Enter blog Title")]
        [Required]
        [Display(Name = "Title")]
        [StringLength(50)]
        public string Title { get; set; }

        //[Required(ErrorMessage = "Enter blog Author")]
        [Required]
        [Display(Name = "Author")]
        [StringLength(50)]
        public string Author { get; set; }

        //[Required(ErrorMessage = "Enter blog Content")]
        [Required]
        [Display(Name = "Content")]
        [StringLength(10000)]
        public string Content { get; set; }
        
        public BlogPost ParseAsEntity(BlogPost BlogPost)
        {
            if (BlogPost == null)
            {
                BlogPost = new BlogPost();
            }

            //BlogPost.BlogImage = BlogImage;
            BlogPost.Title = Title;
            BlogPost.Author = Author;            
            BlogPost.Content = Content;
         
            return BlogPost;
        }

        protected void SetFromBlogPost(BlogPost BlogPost)
        {
            this.BlogPostId = BlogPost.BlogPostId;
            //this.BlogImage = BlogPost.BlogImage;
            this.Title = BlogPost.Title;
            this.Author = BlogPost.Author;            
            this.Content = BlogPost.Content;
           
        }

    }
}