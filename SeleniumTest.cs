using Deque.AxeCore.Commons;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Deque.AxeCore.Selenium;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Net.Mail;

namespace Az400_SeleniumTest
{
    public class SeleniumTests
    {
        private IWebDriver driver;
        private IConfiguration configuration;
        private string baseURL;
        private readonly string browser = string.Empty;
        private readonly bool acceptNextAlert = true;

        [SetUp]
        public void Setup()
        {
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, false).Build();
            baseURL = configuration["TestSettings:BaseUrl"];

            // Configurar o driver do Chrome
            ChromeOptions options = new();
            options.AddArguments("--headless");
            options.AddArguments("start-maximized"); // open Browser in maximized mode
            options.AddArguments("disable-infobars"); // disabling infobars
            options.AddArguments("--disable-extensions"); // disabling extensions
            options.AddArguments("--disable-gpu"); // applicable to windows os only
            options.AddArguments("--disable-dev-shm-usage"); // overcome limited resource problems
            options.AddArguments("--no-sandbox"); // Bypass OS security model
            this.driver = new ChromeDriver(options);
        }

        [TearDown]
        public void TeardownTest()
        {
            if (driver != null)
            {
                TakeScreenshot();
                driver.Quit();
                driver.Dispose();
            }
        }

        [Test]
        public void AccessibilityTest_InitialPage()
        {
            Selenium_CheckInitialPage();
        }

        [Test]
        public void AccessibilityTest_ProjectPage()
        {
            Selenium_CheckProjectPage();
        }


        private void Selenium_CheckInitialPage()
        {
            // Navega até a página do modelo_core_mvc
            driver.Navigate().GoToUrl(baseURL);

            // Encontre o titulo da página
            IWebElement pageTitle = driver.FindElement(By.CssSelector("h1"));

            // Verifique se a página contém o texto "Modelo Sefaz"
            if (driver.PageSource.Contains("Modelo Sefaz", StringComparison.OrdinalIgnoreCase))
            {
                AxeResult axeResult = new AxeBuilder(driver).Analyze();
                //axeResult.Violations.Should().BeEmpty();

                foreach (var violation in axeResult.Violations)
                {
                    Console.WriteLine(violation.Description);
                }
                Assert.That(axeResult.Violations.Count, Is.EqualTo(0));
            }
            else
            {
                Assert.That(driver.PageSource.Contains("Modelo Sefaz", StringComparison.OrdinalIgnoreCase), Is.True);
            }
        }

        private void Selenium_CheckProjectPage()
        {
            // Navega até a página do modelo_core_mvc
            driver.Navigate().GoToUrl(baseURL);

            // Encontre e clique no link "Projetos"
            driver.FindElement(By.LinkText("Lista de Projetos")).Click();

            // Encontre o titulo da página
            IWebElement pageTitle = driver.FindElement(By.CssSelector("h1"));

            // Verifique se a página contém o texto "Projetos do DTI"
            if (driver.PageSource.Contains("Projetos do DTI", StringComparison.OrdinalIgnoreCase))
            {
                AxeResult axeResult = new AxeBuilder(driver).Analyze();
                axeResult.Violations.Should().BeEmpty();
            }
            else
            {
                Assert.That(driver.PageSource.Contains("Projetos do DTI", StringComparison.OrdinalIgnoreCase), Is.True);
            }
        }

        private void TakeScreenshot()
        {
            // O teste falhou, tire uma captura de tela
            if ((TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed) || (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Inconclusive))
            {
                // Define o caminho e o nome do arquivo da captura de tela
                string screenshotPath = Directory.GetCurrentDirectory() + "\\" + TestContext.CurrentContext.Test.Name + ".png";

                ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(screenshotPath);

                // Adicione o anexo ao contexto do teste
                TestContext.AddTestAttachment(screenshotPath, $@"Captura de Tela - {TestContext.CurrentContext.Test.Name}");
            }
        }
    }
}