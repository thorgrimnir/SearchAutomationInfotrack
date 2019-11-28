using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SearchEngineAutomation.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(Models.FormModel form)
        {
            //make sure there is some form of input
            if (!ModelState.IsValid)
            {
                return View("~/Views/Home/PleaseTryAgain.cshtml");
            }

            string keywords = form.Keywords;
            //spaces must be replaced with +, for use in google search query
            keywords = keywords.Replace(" ", "+");
            string url = form.Url;

            //create http request
            HttpClient client = new HttpClient();
            //directly produce only the first 100 results with &num=100
            var response = client.GetStringAsync($"https://www.google.com/search?q= {keywords} &num=100");

            //parse html content with regular expression to find urls
            var r = Regex.Matches(response.Result, @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>\S+))", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            List<string> ListOfUrls = new List<string>();

            //iterate through regex matches and populate the list
            foreach (Match m in r)
            {
                //filter out urls which are not part of the results rankings, all google results start with /url?q=
                string validLink = "/url?q=";
                if (m.Groups[1].Value.StartsWith(validLink))
                {
                    ListOfUrls.Add(m.Groups[1].Value);
                }
            }

            List<int> SearchResult = new List<int>();

            //iterate through list of urls and retrieve rank of specified url
            for (int i = 0; i < ListOfUrls.Count; i++)
            {
                if (ListOfUrls[i].Contains(url))
                {
                    //add 1 to list to show actual ranking and not 0 based list
                    SearchResult.Add(i + 1);
                }
            }

            //convert list to a single string for ease of use with ViewBag
            SearchResult.ConvertAll(delegate (int i) { return i.ToString(); });
            string searchConverted = string.Join(",", SearchResult);

            ViewBag.searchConverted = searchConverted;

            //if no results have been found, display the PleaseTryAgain view. Otherwise redirect to SearchResult view
            bool check = string.IsNullOrEmpty(searchConverted);
            if(check == false)
            {
                return View("~/Views/Home/SearchResult.cshtml");
            }
            else
            {
                return View("~/Views/Home/PleaseTryAgain.cshtml");
            }
        }

    }
}