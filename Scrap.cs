using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V110.Page;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Reflection;

namespace MinhasNoticias
{
    internal class Scrap
    {
        public static void Iniciar()
        {
            var sitesInfo = GetSitesInfo();
            var sitesList = sitesInfo.ToList();
            sitesList = sitesList
                .OrderBy(x => x.Value["metodo"])
                .ThenBy(x => x.Value["site"])
                .ToList();

            while (true)
            {
                foreach (var site in sitesList)
                {
                    var siteName = site.Value["site"];
                    var siteURL = site.Value["url"];
                    var siteelementoTXT = site.Value["elementoTXT"];
                    var siteelementoURL = site.Value["elementoURL"];
                    var metodo = site.Value["metodo"];

                    Console.WriteLine($"{DateTime.Now.ToString()} - Iniciando consulta do site [{siteName}]\n");

                    if(metodo == "1")
                    {
                        DestaqueWebRequest(siteName, siteURL, siteelementoTXT, siteelementoURL);
                    }
                    else
                    {
                        Destaque(siteName, siteURL, siteelementoTXT, siteelementoURL);
                    }

                }

                Console.WriteLine($"{DateTime.Now.ToString()} - Consulta finalizada. Aguardando 10 min.\n");

                // min * sec * msec
                Thread.Sleep(10*60*1000);
            }


        }
        private static Dictionary<string, Dictionary<string, string>> GetSitesInfo()
        {
            var sitesDict = new Dictionary<string, Dictionary<string, string>>();

            var sitesSection = ConfigurationManager.GetSection("sites") as NameValueCollection;

            if (sitesSection != null)
            {
                foreach (var key in sitesSection.AllKeys)
                {
                    var sitePrefix = key.Split(':')[0];

                    if (!sitesDict.ContainsKey(sitePrefix))
                    {
                        sitesDict[sitePrefix] = new Dictionary<string, string>();
                    }

                    var siteInfoKey = key.Split(':')[1];
                    var siteInfoValue = sitesSection[key];

                    sitesDict[sitePrefix][siteInfoKey] = siteInfoValue;
                }
            }

            return sitesDict;
        }
        private static void Destaque(
            string siteName,
            string url, 
            string elementoTXT, 
            string elementoURL)
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                var chromeDriverService = ChromeDriverService.CreateDefaultService(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                chromeDriverService.HideCommandPromptWindow = true;
                chromeDriverService.SuppressInitialDiagnosticInformation = true;

                //options.AddArgument("headless");
                options.AddArgument("--silent");
                options.AddArgument("log-level=3");
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--start-minimized");


                using (var d = new ChromeDriver(chromeDriverService, options))
                {
                    try
                    {
                        d.Manage().Window.Minimize();
                        d.Navigate().GoToUrl(url);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }

                    string txtDestaque = "";
                    string txtUrl = "";

                    Thread.Sleep(2000);

                    if (d.FindElements(By.XPath(elementoTXT)).Count > 0)
                    {
                        var ele = d.FindElement(By.XPath(elementoTXT));
                        txtDestaque = ele.Text;
                        Console.WriteLine($"    Destaque -> {ele.Text}");
                    }

                    if (d.FindElements(By.XPath(elementoURL)).Count > 0)
                    {
                        var ele = d.FindElement(By.XPath(elementoURL));
                        txtUrl = ele.GetAttribute("href");
                        Console.WriteLine($"    URL -> {txtUrl}");
                    }

                    var txt = $"{siteName.Replace(" ", "_").Replace(".", "")} -- {txtDestaque} -- {txtUrl}";
                    if (!Arquivo.Contem("sites.txt", txt))
                    {
                        // enviar mensagem Telegram...
                        Telegram.EnviarAsync($"#{txt.Replace(" -- ", "\n\n")}");
                        Console.WriteLine($"\n   Nova Notícia!");
                        Arquivo.Escrever("sites.txt", $"{DateTime.Now.ToString()} -- {txt}");
                    }
                    else
                    {
                        Console.WriteLine($"\n    Notícia Antiga!");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro no método 'Destaque()'");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine();

        }
        
        public static void DestaqueWebRequest(
            string siteName,
            string url,
            string elementoTXT,
            string elementoURL)
        {
            try
            {
                // Cria um objeto Uri a partir da URL
                Uri uri = new Uri(url);

                // Obtém a URL principal do site
                string mainUrl = uri.Host;

                // Cria um objeto WebClient para fazer a requisição
                WebClient webClient = new WebClient();

                // Obtém o HTML da página
                string html = webClient.DownloadString(url);

                // Cria um objeto HtmlDocument a partir do HTML
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
            
                // Obtém o elemento desejado usando XPath
                HtmlNode element = document.DocumentNode.SelectSingleNode(elementoTXT);

                string txtDestaque = element.InnerText.Trim();
                Console.WriteLine($"    Destaque -> {txtDestaque}");

                // Obtém o elemento desejado usando XPath
                element = document.DocumentNode.SelectSingleNode(elementoURL);

                string txtUrl = element.GetAttributeValue("href", "");

                if(txtUrl.Substring(0,8) != "https://")
                {
                    txtUrl = mainUrl.Replace("www.", "https://") + txtUrl;
                }

                Console.WriteLine($"    URL -> {txtUrl}");

                var txt = $"{siteName.Replace(" ", "_").Replace(".", "")} -- {txtDestaque} -- {txtUrl}";
                if (!Arquivo.Contem("sites.txt", txt))
                {
                    // enviar mensagem Telegram...
                    Telegram.EnviarAsync($"#{txt.Replace(" -- ", "\n\n")}");
                    Console.WriteLine($"\n   Nova Notícia!");
                    Arquivo.Escrever("sites.txt", $"{DateTime.Now.ToString()} -- {txt}");
                }
                else
                {
                    Console.WriteLine($"\n    Notícia Antiga!");
                }
            }
            catch( Exception e )
            {
                Console.WriteLine("Erro no método 'DestaqueWebRequest()'");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine();
        }
    }

}