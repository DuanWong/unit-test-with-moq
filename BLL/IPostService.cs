using Models;

namespace BLL {
    public interface IPostService {
        Task<IEnumerable<Post>> GetAllPostsAsync ();
        Task<Post> GetPostByIdAsync (int id);
        Task AddPostAsync (Post post);
        Task<Post> UpdatePostAsync (Post post);
        Task<bool> DeletePostAsync (int id);
    }
}
