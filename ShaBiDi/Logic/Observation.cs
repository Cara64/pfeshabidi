using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// Observation - Classe qui regroupe pour chaque image une liste d'observation des points d'attentions (coordonnées X,Y)
    /// </summary>
    public class Observation
    {

        #region Attributs et propriétés

        /// <summary>
        /// Images concernées par l'observation
        /// </summary>
        public ImageExp Image { get; private set; }

        /// <summary>
        /// Liste des points d'attention de l'observation
        /// </summary>
        public List<PointAttention> PointsAttentions { get; private set; }

        #endregion


        #region Constructeurs

        /// <summary>
        /// Constructeur de la classe Observation
        /// </summary>
        /// <param name="image">Image de l'expérience pour laquelle on ajoute les observations</param>
        public Observation(ImageExp image)
        {
            Image = image;
            PointsAttentions = new List<PointAttention>();
        }

        #endregion


        #region Autres méthodes

        /// <summary>
        /// Ajout d'un point d'attention dans la liste des points d'attention
        /// </summary>
        /// <param name="a">X du vecteur</param>
        /// <param name="b">Y du vecteur</param>
        /// <param name="tps">temps</param>
        public void AddPA(double a, double b, double tps)
        {
            PointsAttentions.Add(new PointAttention(new Vecteur2(a, b),tps));
        }

        #endregion

    }
}
