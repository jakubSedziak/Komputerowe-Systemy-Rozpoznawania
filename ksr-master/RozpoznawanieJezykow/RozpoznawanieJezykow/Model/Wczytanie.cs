using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RozpoznawanieJezykow.Model
{
    class Wczytanie
    {
        public string[] wczytaj(string sciezka)
        {
            string[] documents = new string[22];
            for(int i=0; i<22;i++)
            {
                string s = "/reut2-0";
                if (i < 10)
                    s += "0" + Convert.ToString(i);
                else
                    s += Convert.ToString(i);
                s += ".sgm";
                if(System.IO.File.Exists(@sciezka + s))
                documents[i]= System.IO.File.ReadAllText(@sciezka+s);
            }

            return documents;
        }
    }
}
