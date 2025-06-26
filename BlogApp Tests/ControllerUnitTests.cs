using BLL;
using Blog.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;

namespace BlogApp_Tests
{
    public class ControllerUnitTests
    {
        private readonly Mock<IPostService> _mockPostService;
        private readonly PostController _postController;

        public ControllerUnitTests()
        {
            _mockPostService = new Mock<IPostService>();
            _postController = new PostController(_mockPostService.Object);
        }

        [Fact]
        public async Task GetAllPostsAsync_CallBack_ReturnsOkWithPosts()
        {
            // Arrange
            var expectedPosts = new List<Post> { new Post { Id = 1, Title = "Test" } };
            _mockPostService.Setup(s => s.GetAllPostsAsync()).ReturnsAsync(expectedPosts);

            // Act
            var result = await _postController.GetAllPostsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var posts = Assert.IsAssignableFrom<IEnumerable<Post>>(okResult.Value);
            Assert.Equal(expectedPosts, posts);
        }

        [Fact]
        public async Task GetPostByIdAsync_ValidId_ReturnsOkWithPost()
        {
            // Arrange
            var expectedPost = new Post { Id = 1 };
            _mockPostService.Setup(s => s.GetPostByIdAsync(1)).ReturnsAsync(expectedPost);

            // Act
            var result = await _postController.GetPostByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedPost, okResult.Value);
        }

        [Fact]
        public async Task GetPostByIdAsync_InvalidId_ReturnsNotFound()
        {
            // Arrange
            _mockPostService.Setup(s => s.GetPostByIdAsync(1)).ReturnsAsync((Post)null);

            // Act
            var result = await _postController.GetPostByIdAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task CreatePostAsync_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _postController.ModelState.AddModelError("Title", "Title is required");
            var post = new Post { Id = 1 };

            // Act
            var result = await _postController.CreatePostAsync(post);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreatePostAsync_ValidModel_ReturnsCreatedAtAction()
        {
            // Arrange
            var newPost = new Post { Id = 1, Title = "New Post" };
            _mockPostService.Setup(s => s.AddPostAsync(newPost)).Returns(Task.CompletedTask);

            // Act
            var result = await _postController.CreatePostAsync(newPost);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PostController.GetPostByIdAsync), createdAtActionResult.ActionName);
            Assert.Equal(newPost.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(newPost, createdAtActionResult.Value);
            _mockPostService.Verify(s => s.AddPostAsync(newPost), Times.Once);
        }

        [Fact]
        public async Task UpdatePostAsync_IdIncorrect_ReturnsBadRequest()
        {
            // Arrange
            var post = new Post { Id = 1 };
            int invalidId = 2;

            // Act
            var result = await _postController.UpdatePostAsync(invalidId, post);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdatePostAsync_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var post = new Post { Id = 1 };
            _postController.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _postController.UpdatePostAsync(1, post);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdatePostAsync_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            var post = new Post { Id = 1 };
            _mockPostService.Setup(s => s.UpdatePostAsync(post)).ReturnsAsync((Post)null);

            // Act
            var result = await _postController.UpdatePostAsync(1, post);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdatePostAsync_ValidRequest_ReturnsOkWithPost()
        {
            // Arrange
            var post = new Post { Id = 1, Title = "Updated Post" };
            _mockPostService.Setup(s => s.UpdatePostAsync(post)).ReturnsAsync(post);

            // Act
            var result = await _postController.UpdatePostAsync(1, post);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(post, okResult.Value);
        }

        [Fact]
        public async Task DeletePostAsync_ExistingId_ReturnsNoContent()
        {
            // Arrange
            int id = 1;
            _mockPostService.Setup(s => s.DeletePostAsync(id)).ReturnsAsync(true);

            // Act
            var result = await _postController.DeletePostAsync(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePostAsync_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            int id = 1;
            _mockPostService.Setup(s => s.DeletePostAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _postController.DeletePostAsync(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}