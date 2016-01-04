using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// ImageExp - Classe pour caractériser les images utilisées durant les expériences
    /// Il faut modifier ici les dimensions des images si nécessaire
    /// </summary>
    public class ImageExp
    {

        #region Constantes

        /// <summary>
        /// Constante de dimensions de l'image en colonnes
        /// </summary>
        public const int DIM_IMAGE_COL = 1680;
        /// <summary>
        /// Constante de dimensions de l'image en lignes
        /// </summary>
        public const int DIM_IMAGE_ROW = 900;
        /// <summary>
        /// Constante de dimensions du bandeau en colonnes
        /// </summary>
        public const int DIM_BANDEAU_COL = 1680;
        /// <summary>
        /// Constante de dimensions du bandeau en lignes
        /// </summary>
        public const int DIM_BANDEAU_ROW = 150;

        #endregion


        #region Attributs et propriétés

        /// <summary>
        /// Numéro identifiant de l'image
        /// </summary>
        public int Numero { get; private set;  }

        /// <summary>
        /// Moyen d'accès à l'image
        /// </summary>
        private string acces;
        public string Acces
        {
            get { return acces; }
            set { acces = value; }
        }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur de la classe ImageExp
        /// </summary>
        /// <param name="numero"></param>
        public ImageExp(int numero)
        {
            Numero = numero;
        }

        #endregion
    }
}
