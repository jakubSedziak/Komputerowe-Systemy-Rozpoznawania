using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metryki
{
   public class MetrykaManhattan:Metryki
    {
        public double OdlegloscMiedzyWektorami(double pierwsza, double druga)
        {
            double odleglosc =Math.Abs((pierwsza- druga));
            return odleglosc;
        }
        public override string ToString()
        {
            return "MetrykaMannhatan";
        }
    }
}
