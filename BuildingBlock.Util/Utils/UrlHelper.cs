using System;
using System.Collections.Generic;
using System.Text;

namespace BuildingBlock.Util.Utils
{
    public static class UrlHelper
    {
        public static string Slugify(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            // Convert to lowercase
            string slug = input.ToLowerInvariant();
            // Replace spaces with hyphens
            slug = slug.Replace(" ", "-");
            // Remove invalid characters
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", string.Empty);
            // Trim hyphens from the start and end
            slug = slug.Trim('-');
            return slug;
        }
    }
}
