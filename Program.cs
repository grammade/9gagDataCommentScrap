// See https://aka.ms/new-console-template for more information
using _9gagScrap;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text.Json;

Console.WriteLine("scrap init");
var foundList = new List<PostComment>(); 
var chromeOptions = new ChromeOptions();
chromeOptions.AddArgument("log-level=3");
using (var driver = new ChromeDriver(options: chromeOptions)) {
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
    var cap = driver.Capabilities["acceptInsecureCerts"];
    driver.Navigate().GoToUrl("file:///E:/_repos/9gagScrap/page.html");
    var comments = driver.FindElements(By.XPath("html/body/table[5]/tbody/tr//a"));
    var linkList = comments.Select(c => c.GetAttribute("href")).ToList();

    var iteration = 1;
    foreach (var link in linkList) {
        driver.Navigate().GoToUrl(link);
        IWebElement? highlight = null;
        try {
            highlight = driver.FindElement(By.XPath("//section[@class='comment-list-item comment-list-item_highlighted']"));
        }
        catch {
            try {
                highlight = driver.FindElement(By.XPath("//section[@class='comment-list-item comment-list-item_reply comment-list-item_highlighted']"));
            }catch {}
        }
        if (highlight is null)
            continue;

        var text = highlight.FindElement(By.ClassName("comment-list-item__text")).Text;
        foundList.Add(new PostComment {
            link = link,
            comment = text
        });
        Console.WriteLine("Iteration: "+ iteration++);
        Console.WriteLine(text+"\n");
    }
    var json = JsonSerializer.Serialize(foundList);
    await File.WriteAllTextAsync("E:\\_repos\\9gagScrap\\result.json", json);
}

return 0;