using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// PointAttention - Classe qui permet de modéliser le point d'attention
    /// </summary>
    public class PointAttention
    {

        #region Attributs et propriétées
     
        /// <summary>
        /// Point d'attention de référence pour le calcul de l'ellipse de reference
        /// </summary>
        public static Vecteur2 PAref;
        /// <summary>
        /// Dimension de l'ellipse
        /// </summary>
        public static Vecteur2 dimEllipse = new Vecteur2(0,0);
        /// <summary>
        /// Liste des pixels de référence
        /// </summary>
        public static List<Vecteur2> pixelsRef;

        /// <summary>
        /// Temps écoulé en ms depuis le début de l'expérience pour l'enregistrement de ce point d'attention
        /// </summary>
        private double _tempsEcoule;
        public double TempsEcoule
        {
            get { return _tempsEcoule; }
            set { _tempsEcoule = value; }
        }

        /// <summary>
        /// Coordonnées du point d'attention
        /// Obtenues grâce à la méthode de calcul lancée dans le constructeur
        /// </summary>
        private Vecteur2 _coordPA;
        public Vecteur2 CoordPA
        {
            get { return _coordPA; }
            set { _coordPA = value; }
        }

        /// <summary>
        /// Coordonnée X du point d'attention récupérée dans le fichier d'origine
        /// </summary>
        private double _PAX; // en px
        public double PAX
        {
            get { return _PAX; }
            set { _PAX = value; }
        }

        /// <summary>
        /// Coordonnée Y du PA récupére dans le fichier d'origine
        /// </summary>
        private double _PAY; // en px
        public double PAY
        {
            get { return _PAY; }
            set { _PAY = value; }
        }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur de la classe PointAttention
        /// </summary>
        /// <param name="vect">Vecteur fourni mentionné dans le fichier de données</param>
        /// <param name="tps">Temps depuis le début de l'enregistrement</param>
        public PointAttention(Vecteur2 vect, double tps)
        {
            _tempsEcoule = tps;

            _PAX = vect.A;
            _PAY = vect.B;

            _coordPA = initialiseCoordPA(vect);
        }

        #endregion


        #region Méthodes

        /// <summary>
        /// Méthode qui trouve le bon point d'attention à partir du "mauvais"
        /// </summary>
        /// <param name="vect">Le vecteur mauvais PA</param>
        /// <returns>Le bon vecteur PA</returns>
        public Vecteur2 initialiseCoordPA(Vecteur2 vect)
        {
            return new Vecteur2(vect.A + dimEllipse.A / 2, vect.B + dimEllipse.B / 2);
            // On a besoin de valeurs rondes donc on arrondit
        }

        /// <summary>
        /// Méthode qui permet de déterminer les pixels qui sont dans une ellipse de centre le point d'attention de référence
        /// L'ellipse étant la zone d'attention
        /// </summary>
        /// <param name="screenDistance">Distance à l'écran</param>
        /// <param name="logicalHeight">Hauteur logique en px</param>
        /// <param name="logicalWidth">Longueur logique en px</param>
        /// <param name="physicalHeight">Hauteur physique en m</param>
        /// <param name="physicalWidth">Longueur logique en m</param>
        /// <param name="userY">Y du PA</param>
        public void pixelsEllipse(double screenDistance, double logicalHeight, double logicalWidth, double physicalHeight, double physicalWidth, double userY)
        {
            // Le PA avec lequel on fait le calcul devient el PA de référence :
            PAref = this._coordPA;
            //Largeur (première coordonnée) et hauteur (seconde coordonnée) de l'ellipse (à calculer dans le constructeur)
            dimEllipse = trouveEllipse(screenDistance, logicalHeight, logicalWidth, physicalHeight, physicalWidth, userY);

            // Recherche de la liste des pixels associés
            List<Vecteur2> laListe = new List<Vecteur2>();

            // Pour cela on réduit la zone de recherche des pixels pour trouver ceux qui sont dans l'ellipse
            int min_x = (int)Math.Floor(_coordPA.A - dimEllipse.A);
            int min_y = (int)Math.Floor(_coordPA.B - dimEllipse.B);// + Image.dimensionsBandeauLignes);
            int max_x = min_x + (int)dimEllipse.A * 2;
            int max_y = min_y + (int)dimEllipse.B * 2;// +Image.dimensionsBandeauCol;

            for (int y = min_y; y < max_y; y++)
                for (int x = min_x; x < max_x; x++)
                    if (dansEllipse(translatePixel(x, y)))
                        laListe.Add(new Vecteur2(x,y));

            // on met à jour la liste de pixels de référence
            pixelsRef = laListe;

        }

        /// <summary>
        /// Détermine la taille de l'ellipse du point d'attention
        /// </summary>
        /// <param name="screenDistance">Distance à l'écran</param>
        /// <param name="logicalHeight">Hauteur logique en px</param>
        /// <param name="logicalWidth">Longueur logique en px</param>
        /// <param name="physicalHeight">Hauteur physique en m</param>
        /// <param name="physicalWidth">Longueur logique en m</param>
        /// <param name="userY">Y du PA</param>    
        public Vecteur2 trouveEllipse(double screenDistance, double logicalHeight, double logicalWidth, double physicalHeight, double physicalWidth, double userY)
        {
            //Pour commencer on applique ce calcul (avec le calcul de la Tangente en radian)
            double tan = Math.Tan(Math.PI * 12 / 180); // calcul angle de vision à voir 
            double tan2 = Math.Tan((Math.PI * 15 / 180));

            double hauteurEllipse = (tan * (screenDistance - userY));
            double largeurEllipse = (tan2 * (screenDistance - userY));
            hauteurEllipse = (hauteurEllipse / physicalHeight) * logicalHeight;
            largeurEllipse = (largeurEllipse / physicalWidth) * logicalWidth;

            return new Vecteur2(largeurEllipse, hauteurEllipse);
        }

        /// <summary>
        /// Méthode qui définit si un pixel est dans une ellispe ou non, de centre (0,0,0)
        /// </summary>
        /// <param name="pixel">Pixel à déterminé</param>
        /// <returns>Booléen qui indique si le pixel est dans l'ellipse ou non</returns>
        public bool dansEllipse(Vecteur2 pixel)
        {

            // On détermine l'équation de la droite y=a*x passant par le pixel et le centre de l'ellipse, dans le repère de l'ellipse
            // Calcul du coef directeur coef
            double coef = (pixel.B) / (pixel.A);

            // L'ellipse a pour équation (x^2/a^2)+(y^2/b^2)=1
            // On cherche le point M(x,y) appartenant au contour de l'ellipse et à la droite
            // Calcul du discriminant de x^2*[(1/a^2)+(coef^2/b^2)]-1=0
            double facteurX = 1 / Math.Pow(dimEllipse.A, 2) + Math.Pow(coef, 2) / Math.Pow(dimEllipse.B, 2);
            double discrim = 4 * facteurX;
            // Calcul d'une des deux solutions de x possibles (peu importe laquelle)
            double x1 = Math.Sqrt(discrim) / (2 * facteurX);
            // On cherche la coordonnée y correspondant à cette solution grâce à l'équation de départ;
            double y1 = coef * x1;

            // Calcul la distance de ce point au centre
            double dSol = Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2));
            // On calcule la distance du pixel au centre
            double dPix = Math.Sqrt(Math.Pow(pixel.A, 2) + Math.Pow(pixel.B, 2));

            // On compare les deux distances et on conclut sur l'appartenance du pixel à l'ellipse
            if (dPix <= dSol)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Méthode pour translater les coordonnées d'un pixel dans un repère ayant pour origine le centre de l'ellipse
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Un vecteur translaté</returns>
        private Vecteur2 translatePixel(int x, int y)
        {
            return new Vecteur2(x - _coordPA.A, y - _coordPA.B);
        }

        /// <summary>
        /// Méthode pour translater tous les pixels de l'ellipse de référence vers la nouvelle ellipse
        /// </summary>
        /// <returns>Une liste de vecteur translaté</returns>
        private List<Vecteur2> translatePixelsEllipse()
        {
            int transX = (int)Math.Floor(this._coordPA.A - PAref.A);
            int transY = Math.Abs((int)Math.Floor(this._coordPA.B - PAref.B));

            List<Vecteur2> liste = new List<Vecteur2>();

            foreach (Vecteur2 v in pixelsRef)
            {
                liste.Add(new Vecteur2(v.A + transX, v.B + transY));
            }
            return liste;
        }

        /// <summary>
        /// Contribution au taux qui ne prend en compte que le point d'attention
        /// </summary>
        /// <param name="grille">grille de booléen en référence</param>
        public void contributionTauxPointAttention(ref bool[,] grille)
        {
            double newX = Math.Floor(_coordPA.A);
            double newY = Math.Floor(_coordPA.B) + ImageExp.DIM_BANDEAU_ROW;
            if ((newX > 0) && (newX < grille.GetLength(1)) && (newY < 0) && (newY > -grille.GetLength(0)))
            {
                // On met le pixel concerné à true
                grille[(int)newY, (int)newX] = true;
            }

                // Sinon, le PA n'est pas sur l'image
            else { }

        }

        /// <summary>
        /// Contribution au taux prend en compte l'ellipse en entier
        /// </summary>
        /// <param name="grille">Grille de booléen passé en référence</param>
        public void contributionTauxEllipse(ref bool[,] grille)
        {
            // On incrémente le nombre de millisecondes sur le pixel concerné sur tous les pixels contenus dans l'ellipse
            // Pour cela on réduit la zone de recherche des pixels

            int x, y;

            List<Vecteur2> listPixels = translatePixelsEllipse();
            foreach (Vecteur2 pixel in listPixels)
            {
                x = (int)pixel.A;
                y = (int)(pixel.B * -1) + ImageExp.DIM_BANDEAU_ROW;
                if ((x > 0) && (x < grille.GetLength(1)) && (y > 0) && (y < grille.GetLength(0)))
                {
                    grille[y, x] = true;
                }
            }

        }

        /// <summary>
        /// Contribution à la densité
        /// </summary>
        /// <param name="grille">grille de booléen en référence</param>
        /// <param name="temps"></param>
        public void contributionDensite(ref double[,] grille, double temps)
        {
            // Choix 1 : On considère que le temps passé sur un pixel correspond à la moitié du temps depuis le temps précédent
            // + la moitié du temps avec le temps suivant
            int x, y;
            
            List<Vecteur2> listPixels = translatePixelsEllipse();
            foreach (Vecteur2 pixel in listPixels)
            {
                x = (int)pixel.A;
                y = (int)(pixel.B*-1) + ImageExp.DIM_BANDEAU_ROW;
                if ((x > 0) && (x < grille.GetLength(1)) && (y > 0) && (y < grille.GetLength(0)))
                {
                    grille[y, x] += temps;
                }
            }
                    
        }

        /// <summary>
        /// Méthode qui permet de déterminer si le PA est dans le bandeau ou non
        /// </summary>
        /// <returns></returns>
        public bool dansBandeau()
        {
            return (this._coordPA.B > -ImageExp.DIM_BANDEAU_ROW);
        }

        #endregion

    }
}
