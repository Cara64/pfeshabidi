using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class Vecteur2
    {
        public double A { get; set; }
        public double B { get; set; }

        public Vecteur2(double a, double b)
        {
            A = a;
            B = b;
        }

        public double Distance(Vecteur2 other)
        {
            return Math.Sqrt((other.A - this.A) * (other.A - this.A) + (other.B - this.B) * (other.B - this.B));
        }
    }
}
