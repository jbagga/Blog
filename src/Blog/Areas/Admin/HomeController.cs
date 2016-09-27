using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Blog.Areas.Admin
{
    [Area("Admin")]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Home
        // GET: Posts
        public async Task<IActionResult> Index(string postCategory, string searchString)
        {
            IQueryable<string> categoryQuery = from m in _context.Post
                                               orderby m.Category
                                               select m.Category;

            var posts = from p in _context.Post
                        select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.Author.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(postCategory))
            {
                posts = posts.Where(x => x.Category == postCategory);
            }

            var postCategoryVM = new PostCategoryViewModel();
            postCategoryVM.Categories = new SelectList(await categoryQuery.Distinct().ToListAsync());
            postCategoryVM.Posts = await posts.OrderByDescending(x => x.Date).ToListAsync();
            return View(postCategoryVM);
        }

        // GET: Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}