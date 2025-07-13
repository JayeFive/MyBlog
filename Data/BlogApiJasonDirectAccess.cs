using Data.Models.Interfaces;
using Data.Models;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Formats.Asn1;

namespace Data;

public class BlogApiJasonDirectAccess : IBlogApi
{
    BlogApiJsonDirectAccessSettings _settings;

    public BlogApiJasonDirectAccess(IOptions<BlogApiJsonDirectAccessSettings> option)
    {
        _settings = option.Value;
        ManageDataPaths();
    }

    private void ManageDataPaths()
    {
        CreateDirectoryIfNotExists(_settings.DataPath);
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.BlogPostsFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.CategoriesFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.TagsFolder}");
        CreateDirectoryIfNotExists($@"{_settings.DataPath}\{_settings.CommentsFolder}");
    }

    private void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    // Helper Methods //
    private async Task<List<T>> LoadAsync<T>(string folder)
    {
        var list = new List<T>();
        foreach(var f in Directory.GetFiles($@"{_settings.DataPath}\{folder}"))
        {
            var json = await File.ReadAllTextAsync(f);
            var blogPost = JsonSerializer.Deserialize<T>(json);
            if (blogPost is not null)
            {
                list.Add(blogPost);
            }
        }
        return list;
    }

    private async Task SaveAsync<T>(string folder, string filename, T item)
    {
        var filepath = $@"{_settings.DataPath}\{folder}\{filename}.json";
        await File.WriteAllTextAsync(filepath, JsonSerializer.Serialize<T>(item));
    }

    private Task DeleteAsync(string folder, string filename)
    {
        var filepath = $@"{_settings.DataPath}\{folder}\{filename}.json";
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
        return Task.CompletedTask;
    }

    // Blog Posts //
    public async Task<int> GetBlogPostCountAsync()
    {
        var list = await LoadAsync<BlogPost>(_settings.BlogPostsFolder);
        return list.Count;
    }

    public async Task<List<BlogPost>> GetBlogPostsAsync(int numberOfPosts, int startIndex)
    {
        var list = await LoadAsync<BlogPost>(_settings.BlogPostsFolder);
        return list.Skip(startIndex).Take(numberOfPosts).ToList();
    }

    public async Task<BlogPost?> GetBlogPostAsync(string id)
    {
        var list = await LoadAsync<BlogPost>(_settings.BlogPostsFolder);
        return list.FirstOrDefault(bp => bp.Id == id);
    }

    public async Task<BlogPost?> SaveBlogPostAsync(BlogPost item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync<BlogPost>(_settings.BlogPostsFolder, item.Id, item);
        return item;
    }

    public async Task DeleteBlogPostAsync(string id)
    {
        await DeleteAsync(_settings.BlogPostsFolder, id);

        var comments = await GetCommentsAsync(id);
        foreach(var comment in comments)
        {
            if (comment != null)
            {
                await DeleteAsync(_settings.CommentsFolder, comment.Id);
            }
        }
    }

    // Categories //
    public async Task<List<Category>> GetCategoriesAsync()
    {
        return await LoadAsync<Category>(_settings.CategoriesFolder);
    }

    public async Task<Category?> GetCategoryAsync(string id)
    {
        var list = await LoadAsync<Category>(_settings.CategoriesFolder);
        return list.FirstOrDefault(c => c.Id == id);
    }

    public async Task<Category?> SaveCategoryAsync(Category item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync<Category>(_settings.CategoriesFolder, item.Id, item);
        return item;
    }

    public async Task DeleteCategoryAsync(string id)
    {
        await DeleteAsync(_settings.CategoriesFolder, id);
    }

    // Tags //
    public async Task<List<Tag>> GetTagsAsync()
    {
        return await LoadAsync<Tag>(_settings.TagsFolder);
    }

    public async Task<Tag?> GetTagAsync (string id)
    {
        var list = await LoadAsync<Tag>(_settings.TagsFolder);
        return list.FirstOrDefault(t => t.Id == id);
    }

    public async Task<Tag?> SaveTagAsync(Tag item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync<Tag>(_settings.TagsFolder, item.Id, item);
        return item;
    }

    public async Task DeleteTagAsync(string id)
    {
        await DeleteAsync(_settings.TagsFolder, id);
    }

    // Comments //
    public async Task<List<Comment>> GetCommentsAsync(string blogPostId)
    {
        var list = await LoadAsync<Comment>(_settings.CommentsFolder);
        return list.Where(t => t.BlogPostId == blogPostId).ToList();
    }

    public async Task<Comment?> SaveCommentAsync(Comment item)
    {
        item.Id ??= Guid.NewGuid().ToString();
        await SaveAsync<Comment>(_settings.CommentsFolder, item.Id, item);
        return item;
    }

    public async Task DeleteCommentAsync(string id)
    {
        await DeleteAsync(_settings.CommentsFolder, id);
    }
}
