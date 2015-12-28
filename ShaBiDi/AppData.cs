using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaBiDi.Views;
using ShaBiDi.Logic;
using System.Windows.Controls;

namespace ShaBiDi
{
    public static class AppData
    {
        #region Constantes d'application
        
        public const double SCREEN_DISTANCE = 3.833;         // distance à l'écran (en mètre)               
        public const double LOGICAL_HEIGHT = 1050;           // hauteur logique de l'écran (en pixel)
        public const double LOGICAL_WIDTH = 1680;            // longueur de l'écran (en pixel)
        public const double PHYSICAL_HEIGHT = 1.61;          // hauteur physique de l'écran (en mètre)
        public const double PHYSICAL_WIDTH = 2.63;           // logique physique de l'écran (en mètre)

        #endregion


        #region Paramètres d'import

        public static List<ImageExp> ImagesExp = new List<ImageExp>();              // images importés
        public static List<Groupe> GroupesExp = new List<Groupe>();                 // groupes importés

        #endregion


        #region Listes de contrôles

        public static List<UserControl> Indicateurs = new List<UserControl>();      // indicateurs générés
        public static List<UserControl> Comparateurs = new List<UserControl>();     // comparateurs générés

        #endregion

    }
}
