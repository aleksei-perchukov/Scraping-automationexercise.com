using Microsoft.Playwright;

namespace ScrapingPlaywright.Tests;

public class Tests
{
    async Task Test1()
    {
        var URL = "http://www.google.com";
        using var playwright = await Playwright.CreateAsync();
        BrowserTypeLaunchOptions launchOptions = new BrowserTypeLaunchOptions()
        {
            Headless = false
        };
        await using var browser = await playwright.Chromium.LaunchAsync(launchOptions);
        var page = await browser.NewPageAsync();
        await page.GotoAsync(URL);
        await page.ClickAsync("");
        string test2 = await page.GetByTestId("123").TextContentAsync();
        string test = await page.GetAttributeAsync("123", "name");

    }
}