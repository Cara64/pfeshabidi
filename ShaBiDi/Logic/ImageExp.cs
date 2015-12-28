using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class ImageExp
    {
        // A voir s'il ne faut pas permettre de modifier les dimansions de l'image
        // C'est ici qu'il faut intervenir si les dimensions changent
        public static int dimensionsImageCol = 1680;
        public static int dimensionsImageLignes = 900;

        public static int dimensionsBandeauCol = 1680;
        public static int dimensionsBandeauLignes = 150;

        public int Numero { get; private set;  }

        private string acces;
        public string Acces
        {
            get { return acces; }
            set { acces = value; }
        }

        // ex pour remplir l'accès : .jpg

        public ImageExp(int numero)
        {
            Numero = numero;
        }
    }
}
