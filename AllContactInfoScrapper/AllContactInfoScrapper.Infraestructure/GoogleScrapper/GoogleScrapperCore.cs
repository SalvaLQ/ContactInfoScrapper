using AllContactInfoScrapper.Domain;
using PuppeteerSharp;
using System.Net;

namespace AllContactInfoScrapper.Infraestructure.GoogleScrapper
{
    public class GoogleScrapperCore
    {
        public string CountryDomain { get; set; }
        public string Language { get; set; }
        public int DelayMin { get; set; }
        public int DelayMax { get; set; }
        public int ResultsPerPage { get; set; }
        public int NumPages { get; set; }
        public GoogleScrapperCore(string countryDomain, string language, int delayMin, int delayMax, int resultsPerPage, int numPages)
        {
            CountryDomain = countryDomain;
            Language = language;
            DelayMin = delayMin;
            DelayMax = delayMax;
            ResultsPerPage = resultsPerPage;
            NumPages = numPages;
        }


        public async Task<List<ContactInfo>> GetContactInfosAsync(string text, IProgress<ContactInfo> progress, CancellationToken token, bool Captcha = false)
        {
            List<ContactInfo> ContactInfos = new List<ContactInfo>();
            if (token.IsCancellationRequested)
                return ContactInfos;

            string DownloadPath = System.IO.Path.Combine(Infraestructure.Files.FilesUtils.CurretDirectory(), ".local-chromium");
            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions { Path = DownloadPath });
            await browserFetcher.DownloadAsync();
            if (browserFetcher != null)
                progress.Report(new ContactInfo() { Title = "##DOWNLOADING##" });
            var Revision = await browserFetcher.DownloadAsync();
            progress.Report(new ContactInfo() { Title = "##DOWNLOADED##" });
            bool Head = true;
            if (Captcha)
                Head = false;
            else
                Head = true;

            var NavOpt = new NavigationOptions()
            {
                WaitUntil = new[] {
                    WaitUntilNavigation.Networkidle2,
                 WaitUntilNavigation.Load,
                 WaitUntilNavigation.DOMContentLoaded,
                 WaitUntilNavigation.Networkidle0
                },
                Timeout = 0
            };

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = Head,
                ExecutablePath = Revision.GetExecutablePath(),                
                Timeout = 0,
                Args = new string[]
                {

                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-infobars",
                    "--window-position=0,0",
                    "--ignore-certifcate-errors",
                    "--ignore-certifcate-errors-spki-list",

                }

            });
            //var slow3G = Puppeteer.NetworkConditions[NetworkConditions.Slow3G];

            browser.DefaultWaitForTimeout = 0;
            await using var page = await browser.NewPageAsync();
            //await page.EmulateNetworkConditionsAsync(slow3G);
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = 1920 + Random.Shared.Next(1, 100),
                Height = 3000 + Random.Shared.Next(1, 100),
                DeviceScaleFactor = 1,
                HasTouch = false,
                IsLandscape = false,
            });
            RandomUserAgent userAgent = new RandomUserAgent();
            await page.SetUserAgentAsync(userAgent.GetRandomUserAgent());

            //string searchDomain = $"https://www{CountryDomain}/ncr?num={ResultsPerPage}";
            string searchDomain = $"https://www{CountryDomain}/?q=&num={ResultsPerPage}";

            if (Language != "any")
                searchDomain += "&hl=" + Language;

            await page.GoToAsync(searchDomain, NavOpt);

            if (await page.QuerySelectorAsync(ScrapperCssSelectors.ACCEPT_COOKIES) != null)
            {
                await page.ClickAsync(ScrapperCssSelectors.ACCEPT_COOKIES);
                await page.WaitForNavigationAsync(NavOpt);
            }

            await page.TypeAsync(ScrapperCssSelectors.SEARCH_FIELD, text);
            await page.Keyboard.PressAsync("Enter");
            await page.WaitForNavigationAsync(NavOpt);
            if (page.Url.ToLower().Contains("sorry"))
            {
                if (!Captcha)
                {
                    await browser.CloseAsync();
                    throw new Exception("Captcha");
                }
                else
                    await page.WaitForNavigationAsync(NavOpt);

            }
            int idx = 1;
            PhoneExtractor PhoneEx = new PhoneExtractor();
            EmailExtractor EmailEx = new EmailExtractor();
            Random rnd = new Random();
            HtmlAgilityPack.HtmlWeb HtmlWeb = new HtmlAgilityPack.HtmlWeb();
            HtmlWeb.PreRequest = delegate (HttpWebRequest webReq)
            {
                webReq.Timeout = 2000;
                return true;
            };
            for (int i = 0; i < NumPages; i++)
            {
                if (page.Url.ToLower().Contains("sorry"))
                {
                    if (!Captcha)
                    {
                        await browser.CloseAsync();
                        throw new Exception("Captcha");
                    }
                    else
                        await page.WaitForNavigationAsync(NavOpt);
                }
                await page.SetUserAgentAsync(userAgent.GetRandomUserAgent());
                IElementHandle[] Items = await page.QuerySelectorAllAsync(ScrapperCssSelectors.RESULT_DIV);
                var tasks = Items.Select(async itm =>
                {
                    ContactInfo info = new ContactInfo();
                    info.Url = await ExtractLinkAsync(itm);
                    info.Title = await ExtractTitleAsync(itm);
                    info.Description = await ExtractDescriptionAsync(itm);
                    info.Phone = PhoneEx.GetPhones(info.Description);
                    info.Email = EmailEx.GetEmails(info.Description);
                    if (string.IsNullOrEmpty(info.Phone) & string.IsNullOrEmpty(info.Email))
                    {
                        var Web = await HtmlWeb.LoadFromWebAsync(info.Url);
                        if (Web.DocumentNode != null)
                        {
                            string WebText = Web.DocumentNode.InnerText;
                            if (string.IsNullOrEmpty(info.Phone))
                                info.Phone = PhoneEx.GetPhones(WebText);
                            if (string.IsNullOrEmpty(info.Email))
                                info.Email = EmailEx.GetEmails(WebText);
                        }
                    }
                    ContactInfos.Add(info);
                    progress.Report(info);
                });
                await Task.WhenAll(tasks);

                var next = await page.QuerySelectorAsync(ScrapperCssSelectors.NEXT_PAGE);
                if (next != null)
                {
                    await next.ClickAsync();
                    await page.WaitForNavigationAsync(NavOpt);

                    await Task.Delay(rnd.Next(DelayMin, DelayMax) * 1000);
                }
                else
                {
                    return (ContactInfos);
                }
                if (token.IsCancellationRequested)
                {
                    return ContactInfos;
                }
                idx++;

            }
            if (token.IsCancellationRequested)
                return ContactInfos;
            return ContactInfos;
        }

        private async Task<string> ExtractDataAsync(IElementHandle Element, string CssSelector, String Property)
        {
            string Value = string.Empty;

            var HtmlElem = await Element.QuerySelectorAsync(CssSelector);
            if (HtmlElem != null)
            {
                var Prop = await HtmlElem.GetPropertyAsync(Property);
                if (Prop != null)
                {
                    var JsonValue = await Prop.JsonValueAsync();
                    if (JsonValue != null)
                        Value = JsonValue.ToString();
                }
            }
            return (Value);
        }


        private async Task<string> ExtractLinkAsync(IElementHandle Element)
        {
            string Value;
            Value = await ExtractDataAsync(Element, "a", "href");            
            return (Value);
        }
        private async Task<string> ExtractTitleAsync(IElementHandle Element)
        {
            string Value;
            Value = await ExtractDataAsync(Element, "h3", "innerText");            
            return (Value);
        }
        private async Task<string> ExtractDescriptionAsync(IElementHandle Element)
        {
            string Value = string.Empty;


            var HtmlElems = await Element.QuerySelectorAllAsync(ScrapperCssSelectors.DESCRIPTION_DIV);
            if (HtmlElems != null && HtmlElems.Length > 0)
            {
                var Prop = await HtmlElems.Last().GetPropertyAsync("innerText");
                if (Prop != null)
                {
                    var JsonValue = await Prop.JsonValueAsync();
                    if (JsonValue != null)
                    {
                        Value = JsonValue.ToString();
                    }
                }
            }
            return (Value);
        }


    }
}
