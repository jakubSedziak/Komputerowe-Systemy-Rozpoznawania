using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace RozpoznawanieJezykow.Model
{
    internal class MaciezWag
    {
        private readonly string _artykulDoRozpoznania;
        private string[] _etykiety;
        List<string> _wybraneEtykiety = new List<string>();
        private int _iloscArtykulow;
        private int _iloscDokumentow;
        private readonly List<Slowo> _maciezSlowUnikatowych = new List<Slowo>();

        internal List<Slowo> MaciezSlowUnikatowych => _maciezSlowUnikatowych;
        internal List<string> WybraneEtykiety => _wybraneEtykiety;
        internal int IloscArtykulow => _iloscArtykulow;
        internal string[] Etykiety => _etykiety;

        public MaciezWag(string artykulDoRozpoznania,string wybraneEtykiety)
        {
            WybierzEtykiety(wybraneEtykiety);
            _artykulDoRozpoznania = "<REUTERS>" + "<BODY>" + artykulDoRozpoznania + "&#3;</BODY>" + "</REUTERS >";
            Wypelnij();
            WypelnijMaciezAsynchronicznie();
        }

        public void  WybierzEtykiety(string etykiety)
        {
            int i = 0;
            while (i<etykiety.Length)
            {
                string nowaEtykieta = "";
                while (i < etykiety.Length && etykiety[i] == ' '  ) i++;
                while (i < etykiety.Length && etykiety[i] != ' ' )
                {
                    nowaEtykieta += etykiety[i];
                    i++;
                }
                _wybraneEtykiety.Add(nowaEtykieta);
            }
        }

        public void Wypelnij()
        {
            var wcz = new Wczytanie();

            var docId = 0;
            var arr = wcz.wczytaj("zbiorTreningowy");

            _iloscDokumentow = arr.Length;
            var artykulNr = 0;

            for (var i = 0; i < _iloscDokumentow; i++)
            {
                var s = arr[i];
                if (s != null)
                {
                    var dots = new HtmlDocument();
                    dots.LoadHtml(s);
                    foreach (var nodes in dots.DocumentNode.Descendants("REUTERS"))
                        foreach (var node in nodes.Descendants("PLACES"))
                        {
                            if (node != null && node.InnerText != "" && _wybraneEtykiety.Find(x=>x==node.InnerText)!=null)
                            {
                                foreach (var nodeb in nodes.Descendants("BODY"))
                                    if (node != null && node.InnerText != "")
                                        _iloscArtykulow++;
                            }
                        }

                }
            }

            _iloscArtykulow++;

            _etykiety = new string[_iloscArtykulow];
            artykulNr = 0;
            for (var p = 0; p < _iloscDokumentow + 1; p++)
            {
                HtmlDocument dots = null;
                if (p == _iloscDokumentow)
                {

                    var s = _artykulDoRozpoznania;
                    if (s != null)
                    {
                        dots = new HtmlDocument();
                        dots.LoadHtml(s);
                    }
                }
                else
                {
                    var s = arr[p];
                    if (s != null)
                    {
                        dots = new HtmlDocument();
                        dots.LoadHtml(s);
                    }
                }
                if (dots != null)
                    foreach (var nodes in dots.DocumentNode.Descendants("REUTERS"))
                    {
                        var etykieta = "-1";
                        foreach (var node in nodes.Descendants("PLACES"))
                        {
                            if (node != null && node.InnerText != "" && _wybraneEtykiety.Find(x => x == node.InnerText) != null)
                            {
                                foreach (var nodeb in nodes.Descendants("BODY"))
                                {
                                    etykieta = node.InnerText;
                                }
                            }
                        }
                        if (etykieta == "")
                            Console.WriteLine("gowno");
                        _etykiety[artykulNr] = etykieta;
                        if (etykieta != "-1" || p == _iloscDokumentow)
                            foreach (var node in nodes.Descendants("BODY"))
                            {

                                var wyrId = 0;
                                var i = 0;
                                var calyText = node.InnerText;
                                while (calyText[i] != ';')
                                {
                                    var slowo = "";
                                    while (calyText[i] == ' ' || calyText[i] == ',' || calyText[i] == '/' ||
                                           calyText[i] == '-' || calyText[i] == '\r' || calyText[i] == '\n') i++;
                                    while (calyText[i] != ' ' && calyText[i] != ';' && calyText[i] != ',' &&
                                           calyText[i] != '/'
                                           && calyText[i] != '-' && calyText[i] != '\r' && calyText[i] != '\n')
                                    {
                                        slowo += calyText[i];
                                        i++;
                                    }

                                    slowo = new string(slowo.Where(c => char.IsLetter(c)).ToArray());
                                    slowo = slowo.ToLower();
                                    if (slowo.Length > 2)
                                    {
                                        var dodawany = new Slowo(_iloscArtykulow)
                                        {
                                            id_artykulu = artykulNr,
                                            id_wyrazu = wyrId,
                                            tresc = slowo,
                                            etykieta = etykieta
                                        };



                                        var unikatowy = MaciezSlowUnikatowych.Find(x => x.tresc == dodawany.tresc);
                                        if (unikatowy == null)
                                        {
                                            dodawany.czestotliwoscWKolekcji = 1;
                                            dodawany.czestotliwoscWArtykule[artykulNr] = 1;
                                            MaciezSlowUnikatowych.Add(dodawany);
                                        }
                                        else
                                        {
                                            unikatowy.czestotliwoscWKolekcji++;
                                            unikatowy.czestotliwoscWArtykule[artykulNr]++;
                                        }

                                        wyrId++;
                                    }
                                }

                                artykulNr++;

                            }
                    }
                docId++;
            }

           Slowo popszedni = new Slowo(1){czestotliwoscWKolekcji = IloscArtykulow*10000};
            List<Slowo> mniejslow = new List<Slowo>();
            for (int i = 0; i < 1000; i++)
            {
                Slowo max= new Slowo(10000){czestotliwoscWKolekcji = 0};
                int pop;
                foreach (var slowo in MaciezSlowUnikatowych)
                {
                    if (slowo.czestotliwoscWKolekcji > max.czestotliwoscWKolekcji &&
                        slowo.czestotliwoscWKolekcji <= popszedni.czestotliwoscWKolekcji
                         && mniejslow.Find(x=>x.tresc==slowo.tresc)==null)
                    {
                        max = slowo;
                    }
                }
                popszedni = max;
                mniejslow.Add(max);
            }
            _maciezSlowUnikatowych.Clear();
            foreach (var variable in mniejslow)
            {
             _maciezSlowUnikatowych.Add(variable);   
            }
       
           
        }

        public async void WypelnijMaciezAsynchronicznie()
        {
            await WypelnijMaciez();
        }

        public async Task WypelnijMaciez()
        {
            int j = 0;
            Task[] taskArray = new Task[10];
            while (j < MaciezSlowUnikatowych.Count - 1)
            {
                for (int i = 0; i < taskArray.Length; i++)
                {
                    if (j < MaciezSlowUnikatowych.Count - 1)
                    {
                        Slowo sl = MaciezSlowUnikatowych[j];
                        taskArray[i] = Task.Factory.StartNew(() => ObliczWagiSlowa(sl));
                        j++;
                    }
                }
                Task.WaitAll(taskArray.ToArray());
            }


        }

        public Slowo ObliczWagiSlowa(Slowo wyr)
        {
            for (var i = 0; i < _iloscArtykulow; i++)
                if (wyr.czestotliwoscWArtykule[i] == 0)
                {
                    wyr.waga[i] = 0;
                }
                else
                {
                    wyr.waga[i] = wyr.czestotliwoscWArtykule[i] *
                                  Math.Log((double)_iloscArtykulow / wyr.czestotliwoscWKolekcji, 2.0);
                    double tempsumm = 0;
                    foreach (var wyr1 in MaciezSlowUnikatowych)
                    {
                        var wystapienia = wyr1.czestotliwoscWArtykule[i];
                        var temp1 = wystapienia *
                                    Math.Log((double)_iloscArtykulow / wyr1.czestotliwoscWKolekcji, 2);
                        tempsumm += temp1 * temp1;
                    }

                    wyr.waga[i] = wyr.waga[i] / tempsumm;
                }

            return wyr;
        }
    }
}