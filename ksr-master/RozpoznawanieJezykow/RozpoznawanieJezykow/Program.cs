using HtmlAgilityPack;
using Newtonsoft.Json;
using RozpoznawanieJezykow.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Metryki;

namespace RozpoznawanieJezykow
{
    class Program
    {
       static List<string> _wybraneEtykiety = new List<string>();
        static void Main(string[] args)
        {
            List<string> wczytane = new List<string>();
            List<string> wyznaczone = new List<string>();
            int K = 0;
            string wybraneEtykiety = "";
            string artykulDoWyznaczenia = "";
            Console.WriteLine("Wczytaj K");
            K =Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Wprowadz metryke");
            Console.WriteLine("Metryka euklidesa:1  ,  Mannhattan:2");
            int metryka = Convert.ToInt32(Console.ReadLine());
            Metryki.Metryki metryki = null;
            if (metryka == 1)
                metryki = new Metryki.MetrykaEuklidesowa();
            if(metryka==2)
                metryki = new Metryki.MetrykaManhattan();

           wybraneEtykiety= System.IO.File.ReadAllText(@"ZbiorTreningowy/ZadaneEtykiety.txt");
            WybierzEtykiety(wybraneEtykiety);
            var wcz = new Wczytanie();
            var docId = 0;
            var arr = wcz.wczytaj("zbiorTestowy");
            var artykulNr = 0;
            int licznik = 0;
            MaciezWag mw;
            for (var i = 0; i < arr.Length; i++)
            {
                var s = arr[i];
                if (s != null)
                {
                    var dots = new HtmlDocument();
                    dots.LoadHtml(s);
                    foreach (var nodes in dots.DocumentNode.Descendants("REUTERS"))
                    {
                        if (licznik < 150)
                        {
                            bool jedziemy = false;
                            foreach (var node in nodes.Descendants("PLACES"))
                            {
                                if (node != null && node.InnerText != "" && _wybraneEtykiety.Find(x => x == node.InnerText) != null)
                                    foreach (var nodeb in nodes.Descendants("BODY"))
                                    {
                                        wczytane.Add(node.InnerText);
                                        jedziemy = true;
                                    }
                            }

                            if (jedziemy)
                            {
                                foreach (var node in nodes.Descendants("BODY"))
                                {
                                    mw = new MaciezWag(node.InnerText, wybraneEtykiety);
                                    KNN knn = new KNN(mw, K, metryki);
                                    wyznaczone.Add(knn.Knn());
                                }

                                licznik++;
                            }
                        }
                    }
                }
            }

            string wynik = "";
            int procentZbieznosci = 0;
            for (int i = 0;  i < wyznaczone.Count;  i++)
            {
                wynik += wczytane[i] + "  " + wyznaczone[i]+System.Environment.NewLine;
                if (wczytane[i] == wyznaczone[i])
                    procentZbieznosci++;
            }

            wynik +=Convert.ToString((double)procentZbieznosci / wyznaczone.Count);
            string nazwaPliku = @"Wyniki/"+ metryki.ToString() + "_" + Convert.ToString(K)+".txt";
            System.IO.File.WriteAllText(nazwaPliku,wynik);





            //  Console.WriteLine(knn.Knn());
            Console.ReadKey();
            Console.ReadLine();
        }
        public static void WybierzEtykiety(string etykiety)
        {
            int i = 0;
            while (i < etykiety.Length)
            {
                string nowaEtykieta = "";
                while (i < etykiety.Length && etykiety[i] == ' ') i++;
                while (i < etykiety.Length && etykiety[i] != ' ')
                {
                    nowaEtykieta += etykiety[i];
                    i++;
                }
                _wybraneEtykiety.Add(nowaEtykieta);
            }
        }
    }
}
