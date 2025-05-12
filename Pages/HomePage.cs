using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace DijkstraShortestPathCalculator.pages;

public class HomePage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    
    public HomePage(IWebDriver driver)
    {
        _driver = driver;
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(8));
    }

    private By _byEnableRandomMode = By.XPath("//*[text()='Enable Random Mode']");
    private By _byRefreshRandomMode = By.CssSelector("[class*='re-calculate-random'][class*='ml-']");
    private By _byCalculate = By.XPath("//*[text()='Calculate']");
    private By _byCalculateRandom = By.XPath("//*[text()='Calculate Random']");
    private By _byFromNode = By.XPath("(//*[contains(@class, 'css-') and contains(@class, '-control')])[1]");
    private By _byToNode = By.XPath("(//*[contains(@class, 'css-') and contains(@class, '-control')])[2]");
    private By _byClear = By.XPath("//*[text()='Clear']");
    private By _byResultDescription = By.CssSelector("[class*='result-card-inner'] p:nth-of-type(1)");
    private By _byTotalDistance = By.CssSelector("[class*='result-card-inner'] p:nth-of-type(2)");

    public void Navigate()
    {
        _driver.Navigate().GoToUrl("https://curious-halva-9294ed.netlify.app/");
    }

    public void ToggleRandomMode()
    {
        _driver.FindElement(_byEnableRandomMode).Click();
    }
    
    public bool IsEnableRandomDisplayed()
    {
        return _driver.FindElement(_byEnableRandomMode).Displayed;
    }

    public void ClickRefreshRandom()
    {
        _driver.FindElement(_byRefreshRandomMode).Click();
        _wait.Until(_ =>  _driver.FindElement(_byCalculateRandom).Displayed);
    }
    
    public bool IsRefreshRandomDisplayed()
    {
        _wait.Until(_ =>  _driver.FindElement(_byRefreshRandomMode).Displayed);
        return _driver.FindElement(_byRefreshRandomMode).Displayed;
    }

    public void ClickCalculate()
    {
        _wait.Until(_ =>  _driver.FindElement(_byCalculate).Displayed);
        _driver.FindElement(_byCalculate).Click();
        _wait.Until(_ =>  _driver.FindElement(_byCalculate).Displayed);

    }

    public void ClickCalculateRandom()
    {
        _wait.Until(_ =>  _driver.FindElement(_byCalculateRandom).Displayed);
        _driver.FindElement(_byCalculateRandom).Click();
        _wait.Until(_ =>  _driver.FindElement(_byCalculateRandom).Displayed);
    }

    public bool IsCalculateRandomDisplayed()
    {
        return _driver.FindElement(_byCalculateRandom).Displayed;
    }
    
    private string GetNodeValue(By by)
    {
        var element = _driver.FindElement(by);
        _wait.Until(_ => element.Displayed);
        return element.GetAttribute("textContent")!;
    }
    
    public string GetFromNodeValue()
    {
        return GetNodeValue(_byFromNode);
    }
    
    public string GetToNodeValue()
    {
        return GetNodeValue(_byToNode);
    }

    public bool IsFromNodeCleared()
    {
        return _driver.FindElement(_byFromNode).GetAttribute("textContent") == "Select";
    }
    
    public bool IsToNodeCleared()
    {
        return _driver.FindElement(_byFromNode).GetAttribute("textContent") == "Select";
    }
    
    public bool IsFromNodeSelectionEnabled()
    {
        var element = _driver.FindElement(_byFromNode);
        return element.GetAttribute("aria-disabled") != "true";
    }
    
    public bool IsToNodeSelectionEnabled()
    {
        var element = _driver.FindElement(_byToNode);
        return element.GetAttribute("aria-disabled") != "true";
    }

    public void SelectNodeValue(string value, By triggerBy, int selectId)
    {
        const string options = "ABCDEFGHI";
        var index = options.IndexOf(value, StringComparison.Ordinal);

        _driver.FindElement(triggerBy).Click();

        if (index != -1)
        {
            var byDropDown = By.CssSelector($"[id*='react-select-{selectId}-option-{index}']");
            _driver.FindElement(byDropDown).Click();
        }
        else
        {
            throw new ArgumentException($"Invalid value: {value}");
        }
    }

    public void SelectFromNodeValue(string value)
    {
        SelectNodeValue(value, _byFromNode, 2);
    }
    
    public void SelectToNodeValue(string value)
    {
        SelectNodeValue(value, _byToNode, 3);
    }

    public string GetResultStatement()
    {
        var result = _driver.FindElement(_byResultDescription).Text;
        var totalDistance = _driver.FindElement(_byTotalDistance).Text;
        return $"{result}\n{totalDistance}";
    }
    
    public string GenerateRandomValue(int start, int end)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript("window.open();");
        var allTabs = _driver.WindowHandles;
        string newTab = allTabs[allTabs.Count - 1];

        // Switch to new tab
        string mainTab = _driver.CurrentWindowHandle;
        _driver.SwitchTo().Window(newTab);
        _driver.Navigate().GoToUrl($"http://2g.be/twitch/randomnumber.php?defstart={start}&defend={end}");
        var value = _driver.FindElement(By.TagName("body")).Text.Trim();
        // Close new tab
        _driver.Close();

        // Switch back to main tab
        _driver.SwitchTo().Window(mainTab);

         var index = Convert.ToInt32(value)-1;
         var selectionString = "ABCDEFGHI";
         return selectionString[index].ToString();
    }
    
    public void ClickClear()
    {
        _driver.FindElement(_byClear).Click();
    }
    
    public bool IsClearDisplayed()
    {
        return _driver.FindElement(_byClear).Displayed;
    }
}