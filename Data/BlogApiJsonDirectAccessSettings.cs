using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data;

public class BlogApiJsonDirectAccessSettings
{
    public string BlogPostsFolder { get; set; } = string.Empty;
    public string CategoriesFolder {  get; set; } = string.Empty;
    public string TagsFolder { get; set; } = string.Empty;
    public string CommentsFolder { get; set; } = string.Empty;
    public string DataPath { get; set; } = string.Empty;
}
