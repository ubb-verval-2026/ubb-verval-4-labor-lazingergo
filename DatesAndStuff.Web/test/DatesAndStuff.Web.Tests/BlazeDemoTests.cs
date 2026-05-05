using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DatesAndStuff.Web.Tests;

[TestFixture]
public class BlazeDemoTests
{
    private IWebDriver driver = null!;

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        driver = new ChromeDriver(options);
    }

    [TearDown]
    public void Teardown()
    {
        try
        {
            driver.Quit();
            driver.Dispose();
        }
        catch { }
    }

    [Test]
    public void MexicoCity_To_Dublin_ShouldHaveAtLeastThreeFlights()
    {
        driver.Navigate().GoToUrl("https://blazedemo.com");
        Thread.Sleep(1000);

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        var fromElem = wait.Until(ExpectedConditions.ElementExists(By.Name("fromPort")));
        var fromOption = fromElem.FindElements(By.TagName("option")).First(o => o.Text.Trim() == "Mexico City");
        fromOption.Click();
        Thread.Sleep(1000);

        var toElem = wait.Until(ExpectedConditions.ElementExists(By.Name("toPort")));
        var toOption = toElem.FindElements(By.TagName("option")).First(o => o.Text.Trim() == "Dublin");
        toOption.Click();
        Thread.Sleep(1000);

        var findButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[type='submit']")));
        findButton.Click();
        Thread.Sleep(1000);

        wait.Until(d => d.FindElements(By.CssSelector("table.table tbody tr")).Count >= 1);
        var rows = driver.FindElements(By.CssSelector("table.table tbody tr")).Where(r => r.FindElements(By.TagName("td")).Count > 0).ToList();

        rows.Count.Should().BeGreaterThanOrEqualTo(3, "there should be at least three flights between Mexico City and Dublin");
    }
}
