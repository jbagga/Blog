using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            Post post;
            if (id == null)
            {
                post = await _context.Post.OrderByDescending(x => x.Date).FirstOrDefaultAsync();
            }
            else
            {
                post = await _context.Post.Include(p => p.CommentList).SingleOrDefaultAsync(m => m.ID == id);
            }

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
