using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;

namespace Blog.Controllers
{
    [Produces("application/json")]
    [Route("api/BlogApi")]
    public class BlogApiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BlogApi
        [HttpGet("GetAllBlogPosts")]
        public IEnumerable<Post> GetBlog()
        {
            return _context.Post;
        }

        // GET: api/BlogApi/5
        [HttpGet("GetBlogPost/{id}")]
        public async Task<IActionResult> GetBlog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Post post = await _context.Post.Include(p => p.CommentList).SingleOrDefaultAsync(m => m.ID == id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        // GET: api/BlogApi/GetComments/5
        [HttpGet("GetCommentsForPost/{id}")]
        public async Task<IActionResult> GetComments([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Post post = await _context.Post.Include(p => p.CommentList).SingleOrDefaultAsync(m => m.ID == id);

            if (post == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(post.CommentList);
            }
            //return Ok(post);

        }

        // PUT: api/BlogApi/5
        [HttpPut("EditBlogPost/{id}")]
        public async Task<IActionResult> PutBlog([FromRoute] int id, [FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != post.ID)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogApi
        [HttpPost("CreateBlog")]
        public async Task<IActionResult> PostBlog([FromBody] Post post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Post.Add(post);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BlogExists(post.ID))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBlog", new { id = post.ID }, post);
        }

        // DELETE: api/BlogApi/5
        [HttpDelete("DeleteBlog/{id}")]
        public async Task<IActionResult> DeleteBlog([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Post post = await _context.Post.SingleOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(post);
        }

        private bool BlogExists(int id)
        {
            return _context.Post.Any(e => e.ID == id);
        }
    }
}