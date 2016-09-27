using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Blog.Data;
using Microsoft.EntityFrameworkCore;
using Blog.Models;

namespace Blog.Controllers
{
    
    [Authorize(Policy = "AdministratorOnly")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        IAuthorizationService _authorizationService;
        // GET: Admin
        public ActionResult Index()
        {
            return View("Shared/Index");
        }

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View("Posts/Index");
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            return View("Posts/Create");
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Posts/Create");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View("Posts/Edit");
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Shared/Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
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


        [Authorize(Policy = "AdministratorOnly")]
        public async Task<IActionResult> ModerateComment(int? id, int? PostID)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.Include(p => p.CommentList).SingleOrDefaultAsync(m => m.ID == PostID);
            if (post == null)
            {
                return NotFound();
            }

            int indexToDelete = post.CommentList.FindIndex(c => c.ID == id.Value);
            Comment commentToDelete = post.CommentList[indexToDelete];
            return View(commentToDelete);
        }
    }
}