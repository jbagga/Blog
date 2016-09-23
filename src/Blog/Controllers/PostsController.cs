using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string postCategory,string searchString)
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
            postCategoryVM.categories = new SelectList(await categoryQuery.Distinct().ToListAsync());
            postCategoryVM.posts = await posts.ToListAsync();
            //foreach(var item in postCategoryVM.categories)
            //{
            //    postCategoryVM.selectedCategories.Add(item.Value);
            //}

            return View(postCategoryVM);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.Include(p => p.CommentList).SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/PostComment
        public async Task<IActionResult> PostComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }
            var model = new Comment();
            model.PostID = post.ID;
            return View("PostComment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostComment([Bind("CommentBody, ID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CommentDate = DateTime.Now;
                if (User.Identity.Name == null) comment.CommentAuthor = "anonymous";
                else comment.CommentAuthor = User.Identity.Name;
                _context.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else return View();
        }

        // GET: Posts/DeleteComment/5
        public async Task<IActionResult> DeleteComment(int? id, int? PostID)
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

            if (commentToDelete.CommentAuthor == User.Identity.Name || post.Author == User.Identity.Name)
            {

                return View(commentToDelete);
            }
            else return View("DeleteError");
        }

        // POST: Posts/Delete/5
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentConfirmed(int id, int PostID)
        {
            var post = await _context.Post.Include(p => p.CommentList).SingleOrDefaultAsync(m => m.ID == PostID);
            int indexToDelete = post.CommentList.FindIndex(c => c.ID == id);
            Comment commentToDelete = post.CommentList[indexToDelete];
            post.CommentList.RemoveAt(indexToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { ID = PostID });
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> EditComment(int? id, int? PostID)
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

            int indexToEdit = post.CommentList.FindIndex(c => c.ID == id.Value);
            Comment commentToEdit = post.CommentList[indexToEdit];

            if (commentToEdit.CommentAuthor == User.Identity.Name)
            {

                return View(commentToEdit);
            }
            else return View("AuthorError");
        }

        // POST: Posts/EditComment/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int id, [Bind("ID,PostID,CommentBody")] Comment comment)
        {
            {
                if (id != comment.ID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        comment.CommentDate = DateTime.Now;
                        comment.CommentAuthor = User.Identity.Name;
                        _context.Update(comment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CommentExists(comment.ID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("Details", new { ID = comment.PostID });
                }
                return View(comment);
            }
        }



        // GET: Posts/Create
        public IActionResult Create()
        {
            if (User.Identity.Name == null)
            {
                return View("Login");
            }
            else return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Body,Category,Title")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Date = DateTime.Now;
                post.Author = User.Identity.Name;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.Author != User.Identity.Name)
            {
                return View("AuthorError");
            }

            else
            {

                return View(post);
            }
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Body,Category,Title")] Post post)
        {
            if (id != post.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.Date = DateTime.Now;
                    post.Author = User.Identity.Name;
                    _context.Update(post);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.Author == User.Identity.Name) return View(post);
            else return View("DeleteError");
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(m => m.ID == id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.ID == id);
        }

        private bool CommentExists(int id)
        {
            foreach(var post in _context.Post.ToList())
            {
                foreach (var comment in post.CommentList)
                {
                    if (comment.ID==id)
                    {
                        return true;
                    }
        
                }
            }
            return false; 
        }

    }
}
