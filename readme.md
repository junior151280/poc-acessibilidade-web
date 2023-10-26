# AZ400-SeleniumTest

## Introdução

Este repositório inclui o código-fonte de um projeto de prova de conceito destinado a avaliar a acessibilidade de projetos da web. O projeto utiliza tecnologias como .NET 6, o framework de teste NUnit, Selenium e AxeCore.

## Pré-requisitos

- .NET 6
- NUnit
- Deque.AxeCore.Selenium
- Selenium.WebDriver
- Selenium IDE

## AxeCore

O AxeCore é uma ferramenta de código aberto que permite avaliar a acessibilidade de projetos da web. O AxeCore é um conjunto de bibliotecas que permite a avaliação de acessibilidade de projetos da web. Está disponível para diversas linguagens como csharp, Java, Python, Ruby, JavaScript, etc. Para mais informações sobre o AxeCore, basta acessar o ([repositório oficial](https://github.com/dequelabs/axe-core)) no GitHub.

Nesse exemplo foi utilizada a biblioteca ```Deque.AxeCore.Selenium```. Essa biblioteca permite a integração do AxeCore com o Selenium WebDriver, facilitando assim a utilização dos comandos de analisador de acessibilidade.

```csharp
    AxeResult axeResult = new AxeBuilder(WebDriver).Analyze();
```

Para instalar o Deque.AxeCore.Selenium, basta acessar o ([site oficial](https://github.com/dequelabs/axe-core-nuget)) e seguir as instruções de instalação.

## Selenium WebDriver

O Selenium WebDriver é uma ferramenta de código aberto que permite automatizar ações em navegadores web. O Selenium WebDriver é um conjunto de bibliotecas que permite a interação com navegadores web. Está disponível para diversas linguagens como csharp, Java, Python, Ruby, JavaScript, etc. Para instalar o Selenium WebDriver, basta acessar o  ([site oficial](https://www.selenium.dev/documentation/webdriver/getting_started/install_library/)) e seguir as instruções de instalação para a linguagem escolhida.

## Selenium IDE

Para automatizar a navegação no site escolhido, foi utilizado o Selenium IDE. O Selenium IDE é uma ferramenta de código aberto que permite gravar e reproduzir testes no navegador. O Selenium IDE é um complemento do navegador que funciona no Google Chrome e no Firefox. Para instalar o Selenium IDE, basta acessar o site oficial ([Google Chrome](https://chrome.google.com/webstore/detail/selenium-ide/mooikfkahbdckldjjndioackbalphokd) ou [Firefox](https://addons.mozilla.org/en-US/firefox/addon/selenium-ide/))e seguir as instruções de instalação.

Para gravar um teste, basta clicar no botão de gravação e navegar pelo site. O Selenium IDE irá gravar todas as ações realizadas no navegador. Para reproduzir o teste, basta clicar no botão de reprodução. O Selenium IDE irá reproduzir todas as ações gravadas no navegador.

Após a validação das ações realizadas é possível exportar o test, no nosso caso para o ```csharp nunit```. Para isso, basta clicar no botão de exportação e selecionar a linguagem de programação desejada.

Com o resultado do export na mão podemos utilizar o código gerado no nosso projeto de teste de acessibilidade.

## Como criar um projeto de teste

A seguir segue um passo a passo de como criar um projeto zerado de teste NUnit com Selenium e AxeCore.

1. Crie um novo projeto de teste NUnit no Visual Studio ou usando o comando de linha de comando:

```bash
    dotnet new nunit -n SeleniumTest
```

2. Adicione as dependências do projeto:

```bash
    dotnet add package Deque.AxeCore.Selenium
    dotnet add package Selenium.WebDriver
    dotnet add package Selenium.WebDriver.ChromeDriver
    dotnet add package Microsoft.Extensions.Configuration
    dotnet add package Microsoft.Extensions.Configuration.json
    dotnet add package FluentAssertions
```

3. Crie um arquivo de configuração chamado ```appsettings.json``` e adicione o seguinte conteúdo:

```json
{
  "TestSettings": {
    "BaseUrl": "https://www4.fazenda.sp.gov.br/modelo_core_mvc"
  }
}
```

4. Substitua o conteúdo do arquivo ```UnitTest1.cs``` pelo seguinte:

```csharp
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

namespace SeleniumTest
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
            options.AddArguments("--headless"); // Inicia o Chrome no modo headless, sem interface gráfica
            options.AddArguments("--start-maximized"); // Abrir o navegador em tela cheia
            options.AddArguments("--disable-infobars"); // Desativa as infobars
            options.AddArguments("--disable-extensions"); // Desativa extensões
            options.AddArguments("--disable-gpu"); // Desativa a aceleração de hardware somente para Windows
            options.AddArguments("--disable-dev-shm-usage"); // Desativa o uso compartilhado de memória em /dev/shm
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
            if (pageTitle.Text.Contains("Modelo", StringComparison.OrdinalIgnoreCase))
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
                Assert.That(driver.PageSource.Contains("Modelo", StringComparison.OrdinalIgnoreCase), Is.True);
            }
        }

        private void Selenium_CheckProjectPage()
        {
            // Navega até a página do modelo_core_mvc
            driver.Navigate().GoToUrl(baseURL);

            // Encontre e clique no link "Projetos"
            driver.FindElement(By.LinkText("Lista de Projetos")).Click();

            // Verifique se a página contém o texto "Projetos do DTI"
            if (driver.PageSource.Contains("Projetos do", StringComparison.OrdinalIgnoreCase))
            {
                // Execute a verificação de acessibilidade
                AxeResult axeResult = new AxeBuilder(driver).Analyze();
                // Falha se houver violações
                axeResult.Violations.Should().BeEmpty();
            }
            else
            {
                // Falha se o texto não for encontrado
                Assert.That(driver.PageSource.Contains("Projetos do", StringComparison.OrdinalIgnoreCase), Is.True);
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
```

## Execute

Para executar esse projeto, basta abrir o arquivo .sln no Visual Studio 2022 ou executar o comando abaixo:

```bash
dotnet test
```

## Pipeline

Para automatizar a execução dos testes é possível configurar um build pipeline no Azure DevOps. Para isso, basta criar um novo pipeline e configurar o arquivo ```azure-pipelines.yml``` com o seguinte conteúdo:

```yaml

trigger:
- main

pool:
  vmImage: ubuntu-latest

variables:
- name: BuildConfiguration
  value: Release
- name: BuildPlatform
  value: any cpu
- name: system.debug
  value: false
- name: Projects
  value: '**/*.csproj'

steps:
- task: DotNetCoreCLI@2
  displayName: Restore
  inputs:
    command: restore
    projects: '$(Projects)'

- task: DotNetCoreCLI@2
  displayName: Build
  inputs:
    projects: '$(Projects)'
    arguments: '--configuration $(BuildConfiguration)'

- task: DotNetCoreCLI@2
  displayName: Test
  inputs:
    command: test
    projects: $(Projects)
    arguments: '--configuration $(buildConfiguration) --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format="cobertura"'

- task: DotNetCoreCLI@2
  displayName: Publish
  inputs:
    command: publish
    publishWebProjects: True
    arguments: '--configuration $(BuildConfiguration) --output $(build.artifactstagingdirectory)'
    zipAfterPublish: True

- task: PublishCodeCoverageResults@1
  displayName: 'Publish code coverage from $(Agent.TempDirectory)/**/coverage.cobertura.xml'
  inputs:
    codeCoverageTool: Cobertura
    summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'

- task: PublishBuildArtifacts@1
  displayName: 'Publish Artifact'
  inputs:
    PathtoPublish: '$(build.artifactstagingdirectory)'
  condition: succeededOrFailed()
```

## Referencias

- [Accessibility Testing with Selenium and AxeCore](https://github.com/dequelabs/axe-core-nuget/blob/develop/packages/selenium/README.md)
- [Selenium WebDriver](https://www.selenium.dev/documentation/webdriver/)
- [List of Chromium Command Line Switches](https://peter.sh/experiments/chromium-command-line-switches/)
- [Get started with Selenium IDE](https://www.selenium.dev/selenium-ide/docs/en/introduction/getting-started)