using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi
{
    public class Image
    {
        public static int dimensionsImageX;
        public static int dimensionsImageY;

        public int Numero { get; private set;  }

        public Image(int numero)
        {
            Numero = numero;
        }
    }
}
