using AngleSharp;
using System.Collections.Concurrent;

class UrlsCrawler
{
    static HttpClient client = new();
    static ConcurrentDictionary<string, bool> visited = new();

    public async Task Crawl(string url, string domain)
    {
        if (!visited.TryAdd(url, true))
            return;
        Console.WriteLine($"Checked: {url}");

        string html;
        
        try
        {
            html = await client.GetStringAsync(url);
        }
        catch
        {
            return;
        }

        var context = BrowsingContext.New(Configuration.Default);
        var doc = await context.OpenAsync(req => req.Content(html));

        var links = doc.QuerySelectorAll("a")
            .Select(a => a.GetAttribute("href"))
            .Where(h => h != null)
            .Select(h => new Uri(new Uri(url), h).ToString())
            .Where(h => h.StartsWith(domain)) // остаёмся в своём домене
            .Distinct();

        foreach (var link in links)
        {
            await Crawl(link, domain);
        }
    }

    public List<string> ReturnUrls(string baseUrl)
    {
        List<string> Urls = new List<string>();
        if (!visited.IsEmpty)
        {
            foreach (var UrlEntry in visited)
            {
                Urls.Add(UrlEntry.Key.Substring(baseUrl.Length));
            }
        }
        else
        {
            Console.WriteLine("No URLs found");
        }
        return Urls;
    }
}