using System.Text.RegularExpressions;

namespace Kids.Services
{
    public class SlugMacker
    {
        public static string GenerateSlug(string title)
        {
            title = title.ToLower().Trim();
            title = Regex.Replace(title, @"[\u064B-\u0652]", ""); // remove Arabic diacritics
            title = title.Replace(" ", "-");
            title = Regex.Replace(title, @"[^a-z0-9\u0600-\u06FF\-]", ""); // فقط حروف و اعداد و فارسی
            return title;
        }

    }
}
