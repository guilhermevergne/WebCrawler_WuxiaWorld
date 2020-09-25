using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebCrawlerzada.Models;
using WebCrawlerzada.Data;
using HtmlAgilityPack;
using System.Net;

/* Enunciado do professor Marcos Lapa:

Você sabe o que é um Web Crawler?
Como um Web Crawler poderia ser útil?

Missão: 
1. Pesquise sobre Web Crawlers (Quem são, de onde vêm, para onde vão? rsrsrsrsrs) 
2. Crie um WebCrawler para buscar informações de um assunto e de um site de seu interesse (não precisa varrer vários sites na web), como por exemplo: resultados de Jogos de Futebol, Basquete, Voleyball..., Resultados de Loterias, ou ainda dados sobre qualquer outro assunto favorito seu (cinema, teatro, política, TV...). 
3. Armazene os dados que conseguir em um banco de dados estruturado (futuramente usaremos este WebCrawler para criar nossas Web APIs).

Obs: Não utilize uma API pronta, você irá criar sua própria API futuramente! 

 */


namespace WebCrawlerzada
{
    class Program
    {
        private static async Task StartCrawlerAsync()
        {
            string url = "https://m.wuxiaworld.co/category/0/1.html";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var divs = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("section-item-box")).ToList();


            foreach (var div in divs)
            {
                var name = div.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("book-name")).FirstOrDefault().InnerText;
                Console.WriteLine( name );
            }

        }

        static  void Main(string[] args)
        {
            //var url = "https://www.novelupdates.com/";
            //StartCrawlerAsync();
            //startCrawSearch();

            var novels = new List<Novel>();
            try
            {
                Console.WriteLine("\nBuscando lista das light novels mais populares da semana ...\n");

                novels = startCrawSearch(novels);

                Console.WriteLine("\nSalvando no banco ...\n");

                saveInDataBase(novels);

                Console.WriteLine("\nSalvo com Sucesso\n");

            }
            catch (Exception e)
            {
                Console.WriteLine("Houve algum erro na busca de dados\n");
            }


            Console.ReadLine();
        }

        private static void saveInDataBase(List<Novel> novels)
        {
            using (var context = new DataContext())
            {
                foreach (var novel in novels)
                {
                    context.Novels.Add(novel);
                    context.SaveChanges();
                }

            }
        }


        private static List<Novel> startCrawSearch(List<Novel> novels)
        {
            var url = "https://m.wuxiaworld.co/";
            var client = new WebClient();
            string pagina = client.DownloadString(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pagina);


            var divs = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("section")).ToList();
            var lis = divs[1].Descendants("li").ToList();

            foreach (var li in lis)
            {
                string name = li.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("book-name")).FirstOrDefault().InnerText;
                string imgLink = li.Descendants("img").FirstOrDefault().ChildAttributes("src").FirstOrDefault().Value;
                string link = "https://m.wuxiaworld.co/" + li.Descendants("a").Where(node => node.GetAttributeValue("class", "").Equals("item-img"))
                    .FirstOrDefault().ChildAttributes("href").FirstOrDefault().Value;

                Console.WriteLine(name);
                Console.WriteLine(imgLink);
                Console.WriteLine(link + "\n\n");

                
                if (searchInDB(name))
                {
                    var novel = new Novel
                    {
                        Name = name,
                        Link = link,
                        ImgLink = imgLink
                    };
                    novels.Add(novel);
                }
            }

            return novels;
        }

        public static bool searchInDB(string name)
        {
            using (var context = new DataContext())
            {
                Novel novel = new Novel();
                try
                {
                    novel = context.Novels.Where<Novel>(novel => novel.Name == name).FirstOrDefault();

                }
                catch(Exception e)
                {
                    Console.WriteLine("Xablau");
                }
                if(novel == null) return true;
                return false;
            }
        }

        private static string NovelName(string name)
        {


            return name;
        }

    }
}
