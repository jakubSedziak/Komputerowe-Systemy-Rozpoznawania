using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RozpoznawanieJezykow.Model
{
    [Serializable]
    class Slowo
    {
        public Slowo(int iloscDokumentow)
        {
            czestotliwoscWArtykule = new int[iloscDokumentow+1];
            waga = new double[iloscDokumentow+1];
            for (int i = 0; i < iloscDokumentow; i++)
                waga[i] = new double();
        }
        public int id_wyrazu { get; set; }
        public int id_artykulu { get; set; }
        public string etykieta { get; set; }
        public string tresc { get; set; }
        public int[] czestotliwoscWArtykule { get; set; }
        public int czestotliwoscWKolekcji { get; set; }
        public double[] waga { get; set; }
    }
}
