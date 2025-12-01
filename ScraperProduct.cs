using Microsoft.Playwright;
using ScrapingPlaywright.PageObject;

namespace ScrapingPlaywright.Tests;

public class ScraperProduct
{
    private SimpleProduct product = new();
    private IPlaywright _playwright;
    private IBrowser _browser;
    public IPage page;
    
    BrowserTypeLaunchOptions launchOptions = new()
    {
        Headless = false
    };
    
    public async Task<IPage> Initialize()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(launchOptions);
        return await _browser.NewPageAsync();
    }
    
    public async Task ScrapeProduct(string baseUrl, string productUrl, string productType)
    {
        await page.GotoAsync(baseUrl + productUrl);

        if (productType == "Simple")
        {
            SimpleProductPage simpleProductPage = new SimpleProductPage();

            product.Product = await page.Locator(simpleProductPage.productLocator).TextContentAsync();
            product.Category = (await page.Locator(simpleProductPage.categoryLocator).TextContentAsync()).Substring(9);
            product.Brand = (await page.Locator(simpleProductPage.brandLocator).TextContentAsync()).Substring(6);
            product.Price = (await page.Locator(simpleProductPage.priceLocator).TextContentAsync()).Substring(3);
            product.Availability = (await page.Locator(simpleProductPage.availabilityLocator).TextContentAsync()).Substring(13);
            product.Condition = (await page.Locator(simpleProductPage.conditionLocator).TextContentAsync()).Substring(10);
            product.PhotoUrl = baseUrl.Substring(0, baseUrl.Length - 1) + await page.Locator(simpleProductPage.photoLocator).GetAttributeAsync("src");
        }
    }

    public SimpleProduct GetProduct()
    {
        return product;
    }
}