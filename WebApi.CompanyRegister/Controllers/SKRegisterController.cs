using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
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
        private CompanyDetailsModel model;
        private string companyId;

        [HttpGet]
        [ResponseType(typeof(CompanyDetailsModel))]
        public IHttpActionResult GetCompanyDetailsById(string id)
        {
            companyId = id;

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
            th.Join(1000);
            
            return Ok(model);
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
                    "Miesto podnikania:",
                    "Sídlo:",
                    "Bydlisko:",
                    "IČO:",
                    "Právna forma:",
                    "Predmet činnosti:",
                    "Štatutárny orgán:",
                    "Dátum aktualizácie údajov:"
                };

                var pNodes2 = from tableNode in tableNodes
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
                    Name = pNodes2.FirstOrDefault(n => n.Name == "Obchodné meno")?.Value.FirstOrDefault(),
                    Ico = pNodes2.FirstOrDefault(p => p.Name == "IČO")?.Value.FirstOrDefault(),
                    Location = pNodes2.FirstOrDefault(p => p.Name == "Miesto podnikania")?.Value.FirstOrDefault(),
                    PostCode = GetPostCode(pNodes2.FirstOrDefault(p => p.Name == "Sídlo" || p.Name == "Bydlisko")?.Value.FirstOrDefault()),
                    CompanyType = pNodes2.FirstOrDefault(p => p.Name == "Právna forma")?.Value.FirstOrDefault(),
                    Executives = pNodes2.FirstOrDefault(p => p.Name == "Štatutárny orgán")?.Value,
                    Activities = pNodes2.FirstOrDefault(p => p.Name == "Predmet činnosti")?.Value
                };
            }
        }

        private string GetPostCode(string address)
        {
            var match = Regex.Match(address, @"^(\d{3}\s\d{2})", RegexOptions.None);
            return match.Success ? match.Result("$1") : null;
        }
    }
}
