using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaBiDi.Views;
using ShaBiDi.Logic;
using System.Windows.Controls;

/*
 * ShaBiDi - DataViz Tool
 * Outil de visualisation et d'exploitation dans le cadre de l'expérimentation ShaBiDi
 * Développé par Guillaume Carayon et Agathe Claude [3ème année]
 * Sous le tutorat de Damien Marion
 * Et pour le compte de Pierre-Alexandre Favier
 * 
 * Ecole Nationale Supérieure de Cognitique - 2015/2016
 * 
 * http://github.com/Cara64/pfeshabidi
 */

namespace ShaBiDi
{
    /// <summary>
    /// Enumération pour le type de comparaison
    /// </summary>
    public enum TypeComp { Add, Sous, Moy };
    /// <summary>
    /// Enumération pour l'ordre des modalités (S-PA ou PA-S)
    /// </summary>
    public enum OrdreGroupe { PAS, SPA };
    /// <summary>
    /// Enumération pour les modalités PA ou S
    /// </summary>
    public enum Modalite { PA, S };

    /// <summary>
    /// AppData - Classe statique de données et de méthodes d'outils pour l'application
    /// </summary>
    public static class AppData
    {
        #region Constantes d'application
        
        /// <summary>
        /// Distance à l'écran en mètre
        /// </summary>
        public const double SCREEN_DISTANCE = 3.833;
        /// <summary>
        /// Hauteur logique de l'écran en pixel
        /// </summary>
        public const double LOGICAL_HEIGHT = 1050;
        /// <summary>
        /// Longueur logique de l'écran en pixel
        /// </summary>
        public const double LOGICAL_WIDTH = 1680;
        /// <summary>
        /// Hauteur physique de l'écran en mètre
        /// </summary>
        public const double PHYSICAL_HEIGHT = 1.61;
        /// <summary>
        /// Longueur physique de l'écran en mètre
        /// </summary>
        public const double PHYSICAL_WIDTH = 2.63;         

        #endregion


        #region Paramètres d'import

        /// <summary>
        /// Images importés
        /// </summary>
        public static List<ImageExp> ImagesExp = new List<ImageExp>();
        /// <summary>
        /// Groupes importés
        /// </summary>
        public static List<Groupe> GroupesExp = new List<Groupe>();               

        #endregion


  
        #region Indicateurs et comparateurs

        /// <summary>
        /// Liste des indicateurs de taux de recouvrement
        /// </summary>
        public static List<IndicateurTauxRecouvrement> IndicateursTauxRecouvrement = new List<IndicateurTauxRecouvrement>();
        /// <summary>
        /// Liste des indictateurs de densité de recouvrement
        /// </summary>
        public static List<IndicateurDensiteRecouvrement> IndicateursDensiteRecouvrement = new List<IndicateurDensiteRecouvrement>();
        /// <summary>
        /// Liste des indicateurs du nombre d'aller/retour bandeau/image
        /// </summary>
        public static List<IndicateurAllerRetour> IndicateursAllerRetour = new List<IndicateurAllerRetour>();
        /// <summary>
        /// Liste des indicateurs concernant la dispersion du PA
        /// </summary>
        public static List<IndicateurDispersionPA> IndicateursDispersionPA = new List<IndicateurDispersionPA>();

        /// <summary>
        /// Liste des comparateurs des indicateurs de taux de recouvrement
        /// </summary>
        public static List<IndicateurTauxRecouvrement> ComparateursTauxRecouvrement = new List<IndicateurTauxRecouvrement>();
        /// <summary>
        /// Liste des comparateurs des indicateurs de densité de recouvrement
        /// </summary>
        public static List<IndicateurDensiteRecouvrement> ComparateursDensiteRecouvrement = new List<IndicateurDensiteRecouvrement>();
        /// <summary>
        /// Liste des comparateurs d'indicateurs concernant le nombre d'aller-retour
        /// </summary>
        public static List<IndicateurAllerRetour> ComparateursAllerRetour = new List<IndicateurAllerRetour>();
        /// <summary>
        /// Liste des comparateurs d'indicateurs concernant la dispersion du PA
        /// </summary>
        public static List<IndicateurDispersionPA> ComparateursDispersionPA = new List<IndicateurDispersionPA>();
        
        #endregion


        #region Helpers

        /// <summary>
        /// Calcule la moyenne pour l'ensemble des valeurs d'une liste de double
        /// </summary>
        /// <param name="liste">Liste de doubles</param>
        /// <returns>Un double représentant la moyenne</returns>
        public static double calculeMoyenne(List<double> liste)
        {
            double somme = 0;
            foreach (double d in liste)
            {
                somme += d;
            }

            return somme / liste.Count();
        }

        /// <summary>
        /// Calcule la moyenne sur une liste de matrice de doubles
        /// </summary>
        /// <param name="liste">Liste de matrices de doubles</param>
        /// <returns>Une matrice de doubles</returns>
        public static double[,] calculeMoyenne(List<double[,]> liste)
        {
            // le tableau doit faire la taille de l'image
            double[,] moyenne = new double[liste[0].GetLength(0), liste[0].GetLength(1)];

            // Faire la moyenne sur chaque image
            foreach (double[,] d in liste)
            {
                // et sur chaque pixel de l'image
                for (int i = 0; i < d.GetLength(0); i++)
                {
                    for (int j = 0; j < d.GetLength(1); j++)
                    {
                        moyenne[i, j] = moyenne[i, j] + d[i, j];
                    }
                }

            }

            for (int i = 0; i < moyenne.GetLength(0); i++)
            {
                for (int j = 0; j < moyenne.GetLength(1); j++)
                {
                    moyenne[i, j] = moyenne[i, j] / liste.Count();
                }
            }

            return moyenne;
        }

        /// <summary>
        /// Méthode qui génère la liste de l'ensemble des indicateurs
        /// </summary>
        /// <returns>Une liste d'indicateur avec l'ensemble des indicateurs de l'application</returns>
        public static List<Indicateur> genererListeIndicateurs()
        {
            List<Indicateur> res = new List<Indicateur>();

            foreach (IndicateurTauxRecouvrement item in AppData.IndicateursTauxRecouvrement)
                res.Add(item);
            foreach (IndicateurDensiteRecouvrement item in AppData.IndicateursDensiteRecouvrement)
                res.Add(item);
            foreach (IndicateurDispersionPA item in AppData.IndicateursDispersionPA)
                res.Add(item);
            foreach (IndicateurAllerRetour item in AppData.IndicateursAllerRetour)
                res.Add(item);

            return res;
        }

        /// <summary>
        /// Méthode qui génère une liste d'indicateur en fonction d'une autre liste d'indicateur (même type d'indicateur)
        /// </summary>
        /// <param name="ind">Indicateur de référence</param>
        /// <returns>Liste d'indicateur du même type que l'indicateur de référence</returns>
        public static List<Indicateur> genererListeIndicateursDetermines(Indicateur ind)
        {
            List<Indicateur> res = new List<Indicateur>();

            if (ind.GetType().Equals(typeof(IndicateurTauxRecouvrement)))
            {
                foreach (IndicateurTauxRecouvrement item in AppData.IndicateursTauxRecouvrement)
                    res.Add(item);
            }
            else if (ind.GetType().Equals(typeof(IndicateurDensiteRecouvrement)))
            {
                foreach (IndicateurDensiteRecouvrement item in AppData.IndicateursDensiteRecouvrement)
                    res.Add(item);
            }
            else if (ind.GetType().Equals(typeof(IndicateurDispersionPA)))
            {
                foreach (IndicateurDispersionPA item in AppData.IndicateursDispersionPA)
                    res.Add(item);
            }
            else if (ind.GetType().Equals(typeof(IndicateurAllerRetour)))
            {
                foreach (IndicateurAllerRetour item in AppData.IndicateursAllerRetour)
                    res.Add(item);
            }

            return res;
        }

        /// <summary>
        /// Méthode pour convertir une chaîne de caractère en type comparaison 
        /// </summary>
        /// <param name="s">Chaîne de caractère à convertir</param>
        /// <returns>Une valeur TypeComp correspondant</returns>
        public static TypeComp convertStringToTypeComp(string s)
        {
            TypeComp res = TypeComp.Add;
            switch (s)
            {
                case "Addition": res = TypeComp.Add; break;
                case "Soustraction": res = TypeComp.Sous; break;
                case "Moyenne": res = TypeComp.Moy; break;
                default: break;
            }
            return res;
        }

        #endregion

    }
}
