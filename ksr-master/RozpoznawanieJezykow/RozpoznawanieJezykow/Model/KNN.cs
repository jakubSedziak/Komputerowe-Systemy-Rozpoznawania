using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RozpoznawanieJezykow.Model
{
    class KNN
    {
        private MaciezWag mw;
        private int K;
        private Metryki.Metryki metryka;
        public KNN(MaciezWag mw,int K,Metryki.Metryki metryka)
        {
            this.metryka = metryka;
            this.K = K;
            this.mw = mw;
        }
        public string Knn()
        {
            int[] arr = ZnajdzKnajblizszych(K);
            var najczezszeEtykiety = new Dictionary<string, int>();
            for (var i = 0; i < arr.Length; i++)
                if (!najczezszeEtykiety.ContainsKey( mw.Etykiety[arr[i]]))
                    najczezszeEtykiety[mw.Etykiety[arr[i]]] = 1;
                else
                    najczezszeEtykiety[mw.Etykiety[arr[i]]]++;

            var max = 0;
            var etykietaKoncowa = "";
            foreach (var variable in najczezszeEtykiety)
                if (variable.Value > max)
                {
                    max = variable.Value;
                    etykietaKoncowa = variable.Key;
                }

            return etykietaKoncowa;
        }
        public double DystansPomiedzyArtykulami(int idPierwszego, int idDrugiego)
        {
            double roznica = 0;
            foreach (var w in mw.MaciezSlowUnikatowych)
                roznica += metryka.OdlegloscMiedzyWektorami(w.waga[idPierwszego], w.waga[idDrugiego]);
            return roznica;
        }
        public int[] ZnajdzKnajblizszych(int k)
        {
            var poszukiwany = 0;
            double minPopszedni = 0;
            var minWskaznik = 0;
            var najblizsze = new int[k];
            for (var j = 0; j < k; j++)
            {
                double minLokalny = 1000000;
                for (var i = 0; i < mw.IloscArtykulow - 1; i++)
                {
                    var dystans = DystansPomiedzyArtykulami(i, mw.IloscArtykulow - 1);
                    if (poszukiwany > 0)
                        minPopszedni = DystansPomiedzyArtykulami(najblizsze[poszukiwany - 1], mw.IloscArtykulow - 1);
                    else
                        minPopszedni = -1;
                    if (dystans < minLokalny && dystans > minPopszedni)
                    {
                        minLokalny = dystans;
                        minWskaznik = i;
                    }
                }

                najblizsze[poszukiwany] = minWskaznik;
                poszukiwany++;
            }

            return najblizsze;
        }
    }
}
