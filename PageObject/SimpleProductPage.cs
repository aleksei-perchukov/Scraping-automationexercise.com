namespace ScrapingPlaywright.PageObject;

public class SimpleProductPage
{
    private string _productDetails = "//div[@class='product-details']";
    private string _productInformation => _productDetails + "//div[@class='product-information']";
    public string productLocator => _productInformation + "//h2";
    public string categoryLocator =>  _productInformation + "//p[contains(., 'Category:')]";
    public string priceLocator => _productInformation + "/span/span[contains(text(), 'Rs. ')]";
    public string brandLocator => _productInformation + "/p[contains(., 'Brand:')]";
    public string conditionLocator => _productInformation + "/p[contains(., 'Condition:')]";
    public string availabilityLocator => _productInformation +  "/p[contains(., 'Availability:')]";
    public string photoLocator => _productDetails + "//div[@class='view-product']/img";
}