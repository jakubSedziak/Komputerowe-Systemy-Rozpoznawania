using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Metryki
{
   public class MetrykaEuklidesowa:Metryki
    {
        public double OdlegloscMiedzyWektorami(double pierwsza,double druga)
        {
            double odleglosc =(pierwsza-druga)*(pierwsza - druga);
            return odleglosc;
        }

       public override string ToString()
        {
            return "MetrykaEuklidesowa";
        }
    }
}
