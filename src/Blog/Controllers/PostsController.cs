using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        IAuthorizationService _authorizationService;

        public PostsController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [AllowAnonymous]
        // GET: Posts
        public async Task<IActionResult> Index(string postCategory, string searchString)
        {
            IQueryable<string> categoryQuery = from m in _context.Post
                                               orderby m.Category
                                               select m.Category;

            var posts = from p in _context.Post
                        select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                posts = posts.Where(s => s.Author.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(postCategory))
            {
                posts = posts.Where(x => x.Category == postCategory);
            }

            var postCategoryVM = new PostCategoryViewModel();
            postCategoryVM.Categories = new SelectList(await categoryQuery.Distinct().ToListAsync());
            postCategoryVM.Posts = await posts.OrderByDescending(x => x.Date).ToListAsync();
            return View(postCategoryVM);
        }

        [AllowAnonymous]
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

        // GET: Posts/CreateComment
        public async Task<IActionResult> CreateComment(int? id)
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
            return View("CreateComment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment([Bind("CommentBody,PostID")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CommentDate = DateTimeOffset.Now;
                comment.CommentAuthor = User.Identity.Name;
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
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

            var commentToDelete = post.CommentList.SingleOrDefault(c => c.ID == id.Value);
            var commentPostAuthors = new CommentPostAuthors();
            commentPostAuthors.CommentAuthor = commentToDelete.CommentAuthor;
            commentPostAuthors.PostAuthor = post.Author;
            if (await _authorizationService.AuthorizeAsync(User, commentPostAuthors, new EditDeleteRequirement()))
            {
                return View(commentToDelete);
            }

            else
            {
                return View("DeleteError");
            }

        }


        // POST: Posts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentConfirmed(int? id, int? PostID)
        {
            var post = await _context.Post.Include(p => p.CommentList).SingleOrDefaultAsync(m => m.ID == PostID);
            if (post == null)
            {
                return NotFound();
            }

            var commentToDelete = post.CommentList.SingleOrDefault(c => c.ID == id.Value);
            var commentPostAuthors = new CommentPostAuthors();
            commentPostAuthors.CommentAuthor = commentToDelete.CommentAuthor;
            commentPostAuthors.PostAuthor = post.Author;
            if (await _authorizationService.AuthorizeAsync(User, commentPostAuthors, new EditDeleteRequirement()))
            {
                post.CommentList.Remove(commentToDelete);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { ID = PostID });
            }
            else
            {
                return View("DeleteError");
            }
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

            var commentToEdit = post.CommentList.SingleOrDefault(c => c.ID == id);
            if (await _authorizationService.AuthorizeAsync(User, commentToEdit, new EditDeleteRequirement()))
            {
                return View(commentToEdit);
            }

            else
            {
                return View("AuthorError");
            }
        }

        // POST: Posts/EditComment/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int id, [Bind("ID,PostID,CommentBody,CommentAuthor")] Comment comment)
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
                        comment.CommentDate = DateTimeOffset.Now;
                        if (await _authorizationService.AuthorizeAsync(User, comment, new EditDeleteRequirement()))
                        {
                            _context.Update(comment);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            return View("AuthorError");
                        }

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
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Body,Category,Title")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Date = DateTimeOffset.Now;
                post.Author = User.Identity.Name;
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        [HttpGet]
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

            else if (await _authorizationService.AuthorizeAsync(User, post, new EditDeleteRequirement()))
            {
                _context.Update(post);
                await _context.SaveChangesAsync();
                return View(post);
            }

            else
            {
                return View("AuthorError");
            }
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Body,Category,Title,Author")] Post post)
        {
            if (id != post.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.Date = DateTimeOffset.Now;
                    if (await _authorizationService.AuthorizeAsync(User, post, new EditDeleteRequirement()))
                    {
                        _context.Update(post);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { ID = post.ID });

                    }
                    else
                    {
                        return View("AuthorError");
                    }

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

            if (await _authorizationService.AuthorizeAsync(User, post, new EditDeleteRequirement()))
            {
                return View(post);
            }
            else
            {
                return View("DeleteError");

            }

        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            if (await _authorizationService.AuthorizeAsync(User, post, new EditDeleteRequirement()))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return new ChallengeResult();
            }

        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.ID == id);

        }


        //TO DO: Clean up
        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.ID == id);
        }
    }
}
