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
        private IWebDriver driver; // Instância do Selenium WebDriver
        private IConfiguration configuration; // Instância do ConfigurationBuilder
        private string baseURL; // URL base do site

        [SetUp]
        public void Setup()
        {
            // Carregar as configurações do arquivo appsettings.json
            configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", false, false).Build();
            baseURL = configuration["TestSettings:BaseUrl"];

            // Configurar e personalizar o comportamento do Google Chrome
            ChromeOptions options = new();
            // Personalizar o comportamento do Google Chrome para o teste
            options.AddArguments("--headless");
            options.AddArguments("start-maximized"); // Abrir o navegador em tela cheia
            options.AddArguments("disable-infobars"); // Desabilitar as infobars
            options.AddArguments("--disable-extensions"); // Desabilitar extensões
            options.AddArguments("--disable-gpu"); // Desabilitar a GPU somente para Windows
            options.AddArguments("--disable-dev-shm-usage"); // overcome limited resource problems
            options.AddArguments("--no-sandbox"); // Bypass OS security model
            driver = new ChromeDriver(options);
        }

        [TearDown]
        public void TeardownTest()
        {
            // Verifica se a instância do WebDriver não é nula
            if (driver != null)
            {
                // Tira uma captura de tela se o teste falhar
                TakeScreenshot();
                // Fecha o navegador e libera os recursos
                driver.Quit();
                driver.Dispose();
            }
        }

        [Test]
        public void AccessibilityTest_InitialPage()
        {
            // Executa o teste de acessibilidade na página inicial
            Selenium_CheckInitialPage();
        }

        [Test]
        public void AccessibilityTest_ProjectPage()
        {
            // Executa o teste de acessibilidade na página de projetos
            Selenium_CheckProjectPage();
        }


        private void Selenium_CheckInitialPage()
        {
            // Navega até a página do modelo_core_mvc
            driver.Navigate().GoToUrl(baseURL);

            // Encontre o titulo da página
            IWebElement pageTitle = driver.FindElement(By.CssSelector("h1"));

            // Verifique se a página contém o texto "Modelo Sefaz"
            if (pageTitle.Text.Contains("Modelo Sefaz", StringComparison.OrdinalIgnoreCase))
            {
                // Execute a verificação de acessibilidade
                AxeResult axeResult = new AxeBuilder(driver).Analyze();

                // Imprime as violações
                foreach (var violation in axeResult.Violations)
                {
                    Console.WriteLine(violation.Description);
                }
                // Falha se houver violações
                Assert.That(axeResult.Violations.Count, Is.EqualTo(0));
            }
            else
            {
                // Falha se o texto não for encontrado
                Assert.That(driver.PageSource.Contains("Modelo Sefaz", StringComparison.OrdinalIgnoreCase), Is.True);
            }
        }

        private void Selenium_CheckProjectPage()
        {
            // Navega até a página do modelo_core_mvc
            driver.Navigate().GoToUrl(baseURL);

            // Encontre e clique no link "Projetos"
            driver.FindElement(By.LinkText("Lista de Projetos")).Click();

            // Verifique se a página contém o texto "Projetos do DTI"
            if (driver.PageSource.Contains("Projetos do DTI", StringComparison.OrdinalIgnoreCase))
            {
                // Execute a verificação de acessibilidade
                AxeResult axeResult = new AxeBuilder(driver).Analyze();
                // Falha se houver violações
                axeResult.Violations.Should().BeEmpty();
            }
            else
            {
                // Falha se o texto não for encontrado
                Assert.That(driver.PageSource.Contains("Projetos do DTI", StringComparison.OrdinalIgnoreCase), Is.True);
            }
        }

        private void TakeScreenshot()
        {
            // O teste falhou, tire uma captura de tela
            if ((TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed) || 
                (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Inconclusive))
            {
                // Define o caminho e o nome do arquivo da captura de tela
                string screenshotPath = Directory.GetCurrentDirectory() + "\\" + TestContext.CurrentContext.Test.Name + ".png";

                // Salve a captura de tela
                ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(screenshotPath);

                // Adicione o anexo ao contexto do teste
                TestContext.AddTestAttachment(screenshotPath, $@"Captura de Tela - {TestContext.CurrentContext.Test.Name}");
            }
        }
    }
}