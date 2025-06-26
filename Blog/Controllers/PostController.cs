using BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Models;

namespace Blog.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase {
        private readonly IPostService _postService;

        public PostController (IPostService postService) {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPostsAsync () {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostByIdAsync(int id) {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null) {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostAsync([FromBody] Post post) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            await _postService.AddPostAsync(post);
            return CreatedAtAction(nameof(GetPostByIdAsync), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePostAsync(int id, [FromBody] Post post) {
            if (id != post.Id || !ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var updatedPost = await _postService.UpdatePostAsync(post);
            if (updatedPost == null) {
                return NotFound();
            }

            return Ok(updatedPost);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostAsync(int id) {
            var deleted = await _postService.DeletePostAsync(id);
            if (!deleted) {
                return NotFound();
            }

            return NoContent();
        }
    }
}
