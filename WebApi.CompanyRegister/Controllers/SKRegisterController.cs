using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Windows.Forms;
using HtmlAgilityPack;
using Infrastructure.Common;
using Infrastructure.Common.Models;

namespace WebApi.CompanyRegister.Controllers
{
    public class SKRegisterController : ApiController
    {
        private bool searchPage = false;
        private Uri baseUrl = new Uri("http://orsr.sk/search_ico.asp");
        private Uri baseUrl2 = new Uri("https://www.indexpodnikatela.sk/");
        private CompanyDetailsModel model;
        private string companyId;
        private readonly HttpClient client;

        public SKRegisterController()
        {
            this.client = new HttpClient();
        }

        [HttpGet]
        [ResponseType(typeof(CompanyDetailsModel))]
        public IHttpActionResult GetCompanyDetailsById(string id)
        {
            companyId = id.Replace(" ", string.Empty);

            CompanyDetailsModel response = null;
            var th = new Thread(() =>
            {
                var br = new WebBrowser();
                br.DocumentCompleted += Br_DocumentCompleted;
                br.Navigate(baseUrl);
                Application.Run();
                response = model;
            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            th.Join(5000);
            
            return Ok(model);
        }

        [HttpGet]
        [ResponseType(typeof(CompanyDetailsModel))]
        public IHttpActionResult GetCompanyDetailsById2(string id)
        {
            return InternalServerError();

            //companyId = id.Replace(" ", string.Empty);

            //var url = new Uri(baseUrl2, id);

            //CompanyDetailsModel response = null;
            //var th = new Thread(() =>
            //{
            //    var br = new WebBrowser();
            //    br.DocumentCompleted += Br_DocumentCompleted2;
            //    br.Navigate(url);
            //    Application.Run();
            //    response = model;
            //});
            //th.SetApartmentState(ApartmentState.STA);
            //th.Start();
            //th.Join(8000);

            //return Ok(model);
        }

        [HttpGet]
        [ResponseType(typeof(CompanyDetailsModel))]
        public async Task<IHttpActionResult> GetCompanyDetailsById3(string id)
        {
            companyId = id.Replace(" ", string.Empty);

            var url = new Uri(baseUrl2, id);

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Company does not exist.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(responseContent);

            var node = doc.DocumentNode.Descendants("dl").FirstOrDefault(htmlNode => htmlNode.Attributes["class"].Value == "dl-horizontal info");

            var dts = node.SelectNodes("dt");
            var dds = node.SelectNodes("dd");

            var pNode = dts.Zip(dds,
                (dt, dd) => new
                {
                    Name = HttpUtility.HtmlDecode(dt.InnerText),
                    Value = HttpUtility.HtmlDecode(dd.InnerText)
                });

            model = new CompanyDetailsModel
            {
                Ico = pNode.FirstOrDefault(n => n.Name == "IČO")?.Value,
                Dic = pNode.FirstOrDefault(n => n.Name == "DIČ")?.Value,
                CompanyType = pNode.FirstOrDefault(n => n.Name == "Právna forma")?.Value,
                PostCode = GetPostCode2(pNode.FirstOrDefault(n => n.Name == "Sídlo")?.Value),
                Name = GetCompanyName(pNode.FirstOrDefault(n => n.Name == "Sídlo")?.Value)
            };

            return Ok(model);

        }

        private void Br_DocumentCompleted2(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var document = ((WebBrowser)sender).Document;            
            string url = document.Url.AbsoluteUri;

            if (url.Contains("hladaj"))
            {
                return;
            }

            string lastUrlPart = url.Split('/').Last();
            if (!string.IsNullOrEmpty(lastUrlPart) && lastUrlPart.All(char.IsNumber))
            {
                WebClient webClient = new WebClient { Encoding = Encoding.UTF8 };
                string page = webClient.DownloadString(url);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page);

                var node = doc.DocumentNode.Descendants("dl").FirstOrDefault(htmlNode => htmlNode.Attributes["class"].Value == "dl-horizontal info");

                var dts = node.SelectNodes("dt");
                var dds = node.SelectNodes("dd");

                var pNode = dts.Zip(dds,
                    (dt, dd) => new
                    {
                        Name = HttpUtility.HtmlDecode(dt.InnerText),
                        Value =HttpUtility.HtmlDecode(dd.InnerText)
                    });

                model = new CompanyDetailsModel
                {
                    Ico = pNode.FirstOrDefault(n => n.Name == "IČO")?.Value,
                    Dic = pNode.FirstOrDefault(n => n.Name == "DIČ")?.Value,
                    CompanyType = pNode.FirstOrDefault(n => n.Name == "Právna forma")?.Value,
                    PostCode = GetPostCode2(pNode.FirstOrDefault(n => n.Name == "Sídlo")?.Value),
                    Name = GetCompanyName(pNode.FirstOrDefault(n => n.Name == "Sídlo")?.Value)
                };
            }
        }

        private void Br_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var document = ((WebBrowser)sender).Document;

            if (!searchPage)
            {
                foreach (HtmlElement element in document.GetElementsByTagName("input"))
                {
                    if (element.GetAttribute("name") == "ICO")
                    {
                        element.SetAttribute("Value", companyId);
                        break;
                    }
                }
                foreach (HtmlElement element in document.GetElementsByTagName("input"))
                {
                    if (element.GetAttribute("value") == " Hľadaj ")
                    {
                        element.InvokeMember("click");
                        searchPage = true;
                        break;
                    }
                }
            }

            string url = document.Url.AbsoluteUri;
            string hrefValue = null;
            if (url.Contains("SID"))
            {
                WebClient webClient = new WebClient();
                string page = webClient.DownloadString(url);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page);

                foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    string linkName = link.InnerText;
                    if (linkName == "Aktuálny")
                    {
                        hrefValue = link.GetAttributeValue("href", string.Empty);
                    }
                }
            }

            string fullLink = ("http://orsr.sk/" + hrefValue).Replace("&amp;", "&");
            if (fullLink.Contains("ID"))
            {
                WebClient webClient = new WebClient();
                string page = webClient.DownloadString(fullLink);

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(page);

                var tableNodes = doc.DocumentNode.Descendants("table");

                var keys = new HashSet<string>
                {
                    "Obchodné meno:",
                    "Sídlo:",
                    "IČO:",
                    "Právna forma:"
                };

                var pNodes = from tableNode in tableNodes
                    from cellInRow in tableNode.Descendants("tr").First().Descendants("td")
                    where cellInRow.InnerText.Trim().Replace("&nbsp;", string.Empty).IsAnyOf(keys.ToArray())
                    select cellInRow.ParentNode into dataNodes                                                 // rows which represent useful data
                    select new
                    {
                        Name = dataNodes.Descendants("td").First().InnerText.Trim().Replace("&nbsp;", string.Empty).Replace(":", string.Empty),
                        Value = (from cell in dataNodes.Descendants("td")
                            where cell.FirstChild.Name == "table"
                            from t in cell.Descendants("table")
                            select t.ChildNodes["tr"].Descendants("td").First().InnerText.Trim().Replace("&nbsp;", string.Empty)).ToList()
                    };

                model = new CompanyDetailsModel
                {
                    Name = pNodes.FirstOrDefault(n => n.Name == "Obchodné meno")?.Value.FirstOrDefault(),
                    Ico = pNodes.FirstOrDefault(p => p.Name == "IČO")?.Value.FirstOrDefault(),
                    PostCode = GetPostCode(pNodes.FirstOrDefault(p => p.Name == "Sídlo")?.Value.FirstOrDefault()),
                    CompanyType = pNodes.FirstOrDefault(p => p.Name == "Právna forma")?.Value.FirstOrDefault(),
                };
            }
        }

        private string GetPostCode(string address)
        {
            if (string.IsNullOrEmpty(address))
                return null;

            var match = Regex.Match(address, @"(\d{3}\s\d{2})", RegexOptions.None);
            return match.Success ? match.Result("$1") : null;
        }

        private string GetPostCode2(string address)
        {
            if (string.IsNullOrEmpty(address))
                return null;

            string cleanString = address.Replace("\t", string.Empty);

            var match = Regex.Match(cleanString, @"(\d{5})", RegexOptions.None);
            return match.Success ? match.Result("$1") : null;
        }

        private string GetCompanyName(string address)
        {
            if (string.IsNullOrEmpty(address))
                return null;

            string cleanString = address.Replace("\t", string.Empty).Remove(0,2);

            return cleanString.Split('\n').FirstOrDefault();
        }
    }
}
