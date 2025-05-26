using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

using System.Text;
using Kids.Context;
using Kids.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Kids.Controllers;

[Route("sitemap.xml")]

public class SiteMapController : Controller
{
    private readonly KidsContext _context;

    public SiteMapController(KidsContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var baseUrl = "https://skilschool.com";

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        var urls = new List<XElement>();

        // صفحه اصلی
        urls.Add(CreateUrlElement(ns, $"{baseUrl}/", DateTime.Now, "daily", "1.0"));

        // دوره‌ها
        var courses = _context.Courses.Where(p => p.IsActive && !p.IsDeleted).ToList();
        foreach (var c in courses)
        {
            urls.Add(CreateUrlElement(ns,
                $"{baseUrl}/Course/{c.Id}/{SlugMacker.GenerateSlug(c.CourseName)}",
                c.CreateDate,
                "weekly",
                "0.8"
            ));
        }

        // مقالات
        var articles = _context.Articles.Where(p => p.IsActive && !p.IsDeleted).ToList();
        foreach (var a in articles)
        {
            urls.Add(CreateUrlElement(ns,
                $"{baseUrl}/Article/{a.Id}/{SlugMacker.GenerateSlug(a.ArticleName)}",
                a.CreateDate,
                "weekly",
                "0.7"
            ));
        }

        var xml = new XDocument(
            new XDeclaration("1.0", "UTF-8", "yes"),
            new XElement(ns + "urlset", urls)
        );

        return Content(xml.ToString(), "text/xml", Encoding.UTF8);
    }

    private XElement CreateUrlElement(XNamespace ns, string loc, DateTime lastmod, string changefreq, string priority)
    {
        return new XElement(ns + "url",
            new XElement(ns + "loc", loc),
            new XElement(ns + "lastmod", lastmod.ToString("yyyy-MM-dd")),
            new XElement(ns + "changefreq", changefreq),
            new XElement(ns + "priority", priority)
        );
    }

}