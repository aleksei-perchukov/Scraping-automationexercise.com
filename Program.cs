using ScrapingPlaywright.PageObject;
using ScrapingPlaywright.Tests;

namespace ScrapingPlaywright;
class Program
{
    static async Task Main()
    {
        Console.WriteLine("Enter website URL: ");
        string baseUrl = Console.ReadLine();
        string startUrl = $"https://{baseUrl}/";
        string urlsFileName = baseUrl + "_urls.txt";
        string outputFileName =  baseUrl + "_data.xlsx";
        List<string> productUrls = new List<string>(); 
        if (!File.Exists(urlsFileName) || File.ReadLines(urlsFileName).Count() == 0)
        {
            string domain = startUrl;

            UrlsCrawler urlsCrawler = new UrlsCrawler();
            await urlsCrawler.Crawl(startUrl, domain);
            productUrls = urlsCrawler.ReturnUrls(startUrl)
                .Where(url => url.Contains("product_details/") && !url.Contains("#")).ToList();
            File.WriteAllLines(urlsFileName, productUrls);
        }
        else
        {
            productUrls = File.ReadAllLines(urlsFileName).ToList();
        }

        List<string> columns = new()
        {
            "Product",
            "Category",
            "Brand",
            "Price",
            "Condition",
            "Availability",
            "Photo"
        };
        ExcelHelper excelHelper = new ExcelHelper(outputFileName, columns);
        excelHelper.CreateFile();
        try
        {
            ScraperProduct scraper = new ScraperProduct();
            scraper.page = await scraper.Initialize();
            int currentIndex = 1;
            int allIndex = productUrls.Count;
            foreach (var productUrl in productUrls)
            {

                await scraper.ScrapeProduct(startUrl, productUrl, "Simple");
                SimpleProduct product = scraper.GetProduct();
                List<string> productValues = new List<string>()
                {
                    product.Product,
                    product.Category,
                    product.Brand,
                    product.Price,
                    product.Condition,
                    product.Availability,
                    product.PhotoUrl
                };
                excelHelper.AddRow(productValues);
                Console.WriteLine($"[{currentIndex}/{allIndex}] : {product.Category} - {product.Product}");
                currentIndex++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }

        Console.WriteLine($"{startUrl} website products data saved in '{outputFileName}' file");
    }
}
