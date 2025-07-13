using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Interfaces;

public interface IBlogApi
{
    Task<int> GetBlogPostCountAsync();
    Task<BlogPost?> GetBlogPostAsync(string id);
    Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex);
    Task<BlogPost?> SaveBlogPostAsync(BlogPost item);
    Task DeleteBlogPostAsync(string id);
    Task<Category?> GetCategoryAsync(string id);
    Task<List<Category>> GetCategoriesAsync();
    Task<Category?> SaveCategoryAsync(Category item);
    Task DeleteCategoryAsync(string id);
    Task<Tag?> GetTagAsync(string id);
    Task<List<Tag>> GetTagsAsync();
    Task<Tag?> SaveTagAsync(Tag item);
    Task DeleteTagAsync(string id);
    Task<List<Comment>> GetCommentsAsync(string blogPostId);
    Task<Comment?> SaveCommentAsync(Comment item);
    Task DeleteCommentAsync(string id);
}
