using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi
{
    public class Image
    {
        // A voir s'il ne faut pas permettre de modifier les dimansions de l'image
        public static int dimensionsImageX = 1680;
        public static int dimensionsImageY = 900;

        public int Numero { get; private set;  }

        public Image(int numero)
        {
            Numero = numero;
        }
    }
}
