using DijkstraShortestPathCalculator.pages;
using DijkstraShortestPathCalculator.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using Allure.Commons;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;

namespace DijkstraShortestPathCalculator.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class Tests
{
    private IWebDriver _driver;
    
    [SetUp]
    public void Setup()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());
        var options = new ChromeOptions();
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--remote-allow-origins=*");
        _driver = new ChromeDriver(options);

        // Set implicit wait
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(8);
        Console.WriteLine("Session ID: " + ((ChromeDriver)_driver).SessionId);
    }

    [TearDown]
    public void TearDown()
    {
        if (_driver != null)
        {
            _driver.Quit();
            _driver.Dispose();
            _driver = null;
        }
    }
    
    [TearDown]
    public void TearDownFinally()
    {
    }
    
    [Test]
    [Category("UI")]
    [Category("Regression")]
    public void PerformRandomMode()
    {
        // 1. Open homepage
        // 2. Verify random mode exist
        // 3. Enable random mode
        // 4. Click the Calculate Random button.
        // 5. Verify the result description and total distance is displayed showing the correct From and To nodes.
        // From Node Name = “A”, To Node Name = ”D”: A, B, C, D
        // Total Distance: 10
        
        // 1. Open homepage
        var homePage = new HomePage(_driver);
        homePage.Navigate();
        
        // 2. Verify random mode exist
        Assert.That(homePage.IsEnableRandomDisplayed(), Is.True);
        
        // 3. Enable random mode
        homePage.ToggleRandomMode();
        Thread.Sleep(3000); // Wait for Select to change to something else

        var initialNodeValues = new
        {
            From = homePage.GetFromNodeValue(),
            To = homePage.GetToNodeValue()
        };
        var dij = new DijkstraPathFinder();
        var (path, cost) = dij.FindShortestPath(initialNodeValues.From, initialNodeValues.To);
        
        // 4. Click the Calculate Random button.
        homePage.ClickCalculateRandom();
        
        // 5. Verify the result description and total distance is displayed showing the correct From and To nodes.
        var actualResult = homePage.GetResultStatement();
        var expectedResult = $"From Node Name = “{initialNodeValues.From}”, To Node Name = ”{initialNodeValues.To}”: {string.Join(", ", path)}".Trim() + "\n" + $"Total Distance: {cost}".Trim();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }
    
    [Test]
    [Category("UI")]
    [Category("Regression")]
    public void RefreshingTheRandomNodes()
    {
        // 1. Open homepage
        // 2. Enable random mode
        // 3. Verify refresh random modes exist. Take values of the From and To Nodes.
        // 4. Click the refresh icon
        // 5. Verify From or To values have been changed
        // 6. Click the Calculate Random button.
        // 7. Verify the result description and total distance is displayed showing the correct From and To nodes. Message should read as:

        // 1. Open homepage
        var homePage = new HomePage(_driver);
        homePage.Navigate();
        
        // 2. Enable random mode
        homePage.ToggleRandomMode();
        
        // 3. Verify refresh random modes exist. Take values of the From and To Nodes.
        Assert.That(homePage.IsRefreshRandomDisplayed(), Is.True);
        Thread.Sleep(3000); // Wait for Select to change to something else

        var initialNodeValues = new
        {
            From = homePage.GetFromNodeValue(),
            To = homePage.GetToNodeValue()
        };
        
        // 4. Click the refresh icon
        homePage.ClickRefreshRandom();
        Thread.Sleep(3000); // Wait for Select to change to something else

        var refreshedNodeValues = new
        {
            From = homePage.GetFromNodeValue(),
            To = homePage.GetToNodeValue()
        };
        var dij = new DijkstraPathFinder();
        var (path, cost) = dij.FindShortestPath(refreshedNodeValues.From, refreshedNodeValues.To);
        // 5. Verify From or To values have been changed
        Assert.That(refreshedNodeValues, Is.Not.EqualTo(initialNodeValues));
        
        // 6. Click the Calculate Random button.
        homePage.ClickCalculateRandom();
        
        // 7. Verify the result description and total distance is displayed showing the correct From and To nodes.
        var actualResult = homePage.GetResultStatement();
        var expectedResult = $"From Node Name = “{refreshedNodeValues.From}”, To Node Name = ”{refreshedNodeValues.To}”: {string.Join(". ", path)}".Trim() + "\n" + $"Total Distance: {cost}".Trim();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }

    [Test]
    [Category("UI")]
    [Category("Regression")]
    public void SelectionOfNodes()
    {
        // 1. Open homepage
        // 2. Turn ON random mode. Verify the From and To node is disabled
        // 3. Turn OFF random mode. Verify the From and To node is enabled
        // 4. Select a node from the From node dropdown.
        // 5. Select a node from the To node dropdown.
        // 6. Click the Calculate button.
        // 7. Verify the result description and total distance is displayed showing the correct From and To nodes.
        // 8. Click Clear
        // 9. Verify nodes are cleared
        // 10. Set another From and To value
        // 11. Verify the result description and total distance is displayed showing the correct From and To nodes.

        
        // 1. Open homepage
        var homePage = new HomePage(_driver);
        var dij = new DijkstraPathFinder();
        homePage.Navigate();
        
        // 2. Turn ON random mode. Verify the From and To node is disabled
        homePage.ToggleRandomMode();
        Thread.Sleep(3000); // Wait for Select to change to something else
        
        Assert.That(homePage.IsFromNodeSelectionEnabled(), Is.False);
        Assert.That(homePage.IsToNodeSelectionEnabled(), Is.False);

        // 3. Turn OFF random mode. Verify the From and To node is enabled
        homePage.ToggleRandomMode();
        Assert.That(homePage.IsClearDisplayed(), Is.True);
        
        // Workaround to that delayed change in the node value
        _driver.Navigate().Refresh();
        Thread.Sleep(3000); // Wait for Select to change to something else

        Assert.That(homePage.IsFromNodeSelectionEnabled(), Is.True);
        Assert.That(homePage.IsToNodeSelectionEnabled(), Is.True);
        
        // Get input data and expected result
        var firstData = new
        {
            From = homePage.GenerateRandomValue(1, 9),
            To = homePage.GenerateRandomValue(1, 9)
        };
        var (firstPath, firstCost) = dij.FindShortestPath(firstData.From, firstData.To);
        
        var secondData = new
        {
            From = homePage.GenerateRandomValue(1, 9),
            To = homePage.GenerateRandomValue(1, 9)
        };
        var (secondPath, secondCost) = dij.FindShortestPath(secondData.From, secondData.To);
        
        // 4. Select a node from the From node dropdown.
        homePage.SelectFromNodeValue(firstData.From);
        
        // 5. Select a node from the To node dropdown.
        homePage.SelectToNodeValue(firstData.To);
        
        // 6. Click the Calculate Random button.
        homePage.ClickCalculate();
        
        // 7. Verify the result description and total distance is displayed showing the correct From and To nodes.
        var actualResult = homePage.GetResultStatement();
        var expectedResult = $"From Node Name = “{firstData.From}”, To Node Name = ”{firstData.To}”: {string.Join(". ", firstPath)}".Trim() + "\n" + $"Total Distance: {firstCost}".Trim();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
        
        // 8. Click Clear
        homePage.ClickClear();
        
        // 9. Verify nodes are cleared
        Assert.That(homePage.IsFromNodeCleared(), Is.True);
        Assert.That(homePage.IsToNodeCleared(), Is.True);
        
        // 10. Set another From and To value and calculate
        _driver.Navigate().Refresh();
        homePage.IsFromNodeSelectionEnabled();
        homePage.SelectFromNodeValue(secondData.From);
        homePage.SelectToNodeValue(secondData.To);
        homePage.ClickCalculate();

        // 11. Verify the result description and total distance is displayed showing the correct From and To nodes.
        actualResult = homePage.GetResultStatement();
        expectedResult = $"From Node Name = “{secondData.From}”, To Node Name = ”{secondData.To}”: {string.Join(". ", secondPath)}".Trim() + "\n" + $"Total Distance: {secondCost}".Trim();
        Assert.That(actualResult, Is.EqualTo(expectedResult));
    }
}