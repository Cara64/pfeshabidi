using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// Vecteur2 - Classe pour définir une coordonnées X,Y
    /// </summary>
    public class Vecteur2
    {
        #region Attributs et propriétés

        /// <summary>
        /// Coordonnée X
        /// </summary>
        public double A { get; set; }
        /// <summary>
        /// Coordonnée Y
        /// </summary>
        public double B { get; set; }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur du Vecteur2
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Vecteur2(double a, double b)
        {
            A = a;
            B = b;
        }

        #endregion


        #region Méthodes

        /// <summary>
        /// Méthode de calcul de distance
        /// </summary>
        /// <param name="other">Vecteur dont on veut calculer </param>
        /// <returns></returns>
        public double Distance(Vecteur2 other)
        {
            return Math.Sqrt((other.A - this.A) * (other.A - this.A) + (other.B - this.B) * (other.B - this.B));
        }

        #endregion
    }
}
