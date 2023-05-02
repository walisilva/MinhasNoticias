using System.Net;
using System.Xml;
using HtmlAgilityPack;

namespace MinhasNoticias
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando busca de notícias...\n");

            Scrap.Iniciar();

            Console.WriteLine("Fim.");
        }
    }
}