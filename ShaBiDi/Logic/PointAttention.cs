using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class PointAttention
    {
        // Dans l'immédiat on va créer des variables statiques
        // Point d'attention de référence
        public static Vecteur2 PAref;
        public static Vecteur2 dimEllipse = new Vecteur2(0,0);
        public static List<Vecteur2> pixelsRef;

        // Il s'agit du temps écoulé (en ms) depuis le début de l'expérience pour l'enregistrment de ce point d'attention
        private double _tempsEcoule;
        public double TempsEcoule
        {
            get { return _tempsEcoule; }
            set { _tempsEcoule = value; }
        }

        // Coordonées du point d'attention
        // Elles sont obtenues grâce à la méthode de calcul lancée dans le constructeur
        private Vecteur2 _coordPA;
        public Vecteur2 CoordPA
        {
            get { return _coordPA; }
            set { _coordPA = value; }
        }

        private double _PAX; // en px
        public double PAX
        {
            get { return _PAX; }
            set { _PAX = value; }
        }

        private double _PAY; // en px
        public double PAY
        {
            get { return _PAY; }
            set { _PAY = value; }
        }

        // Le vecteur fourni est celui qui est mentionné dans le fichier de données
        public PointAttention(Vecteur2 vect, double tps)
        {
            _tempsEcoule = tps;

            _PAX = vect.A;
            _PAY = vect.B;

            _coordPA = initialiseCoordPA(vect);
        }

        // Trouve le bon point d'attention à partir du "mauvais"
        public Vecteur2 initialiseCoordPA(Vecteur2 vect)
        {
            return new Vecteur2(vect.A + dimEllipse.A / 2, vect.B + dimEllipse.B / 2);
            // On a besoin de valeurs rondes donc on arrondit
        }



        // Méthode qui permet de déterminer les pixels qui sont dans une ellipse de centre le point d'attention de référence
        // L'ellipse étant la zone d'attention
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

        // Détermine la taille de l'ellipse du point d'attention
        //screenDistance en px, logicalHeight en px, logicalWidth en px, physicalHeight en m, physicalWidth en m, userZ en m
        //physicalWidth = 2.63 ; physicalHeight = 1.61 ; logicalWidth = 1680 ; logicalHeight = 1050 ;screenDistance = 3.833 ;
        public Vecteur2 trouveEllipse(double screenDistance, double logicalHeight, double logicalWidth, double physicalHeight, double physicalWidth, double userZ)
        {
            //Pour commencer on applique ce calcul (avec le calcul de la Tangente en radian)
            double tan = Math.Tan(Math.PI * 12 / 180); // calcul angle de vision à voir 
            double tan2 = Math.Tan((Math.PI * 15 / 180));

            double hauteurEllipse = (tan * (screenDistance - userZ));
            double largeurEllipse = (tan2 * (screenDistance - userZ));
            hauteurEllipse = (hauteurEllipse / physicalHeight) * logicalHeight;
            largeurEllipse = (largeurEllipse / physicalWidth) * logicalWidth;

            return new Vecteur2(largeurEllipse, hauteurEllipse);
        }

        // Méthode qui définit si un pixel est dans une ellipse ou non, de centre (0,0,0)
        // Le premier vecteur a comme première coord la largeur
        // Le second vecteur a comme seconde coord la hauteur
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

        // Méthode pour translater les coordonnées d'un pixel dans un repère pour origine le centre de l'ellipse
        private Vecteur2 translatePixel(int x, int y)
        {
            return new Vecteur2(x - _coordPA.A, y - _coordPA.B);
        }

        // Méthode pour translater tous les pixels de l'ellipse de référence vers la nouvelle ellipse
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



        // Choix 1 on fait le choix de calculer les taux avec le PA seuls
        // x en premiers dans le tableau, y en seconds

        // Cette contribution au taux ne prend en compte que le point d'attention
        public void contributionTaux1(ref bool[,] grille)
        {
            double newX = Math.Floor(_coordPA.A);
            double newY = Math.Floor(_coordPA.B) + ImageExp.dimensionsBandeauLignes;
            if ((newX > 0) && (newX < grille.GetLength(1)) && (newY < 0) && (newY > -grille.GetLength(0)))
            {
                // On met le pixel concerné à true
                grille[(int)newY, (int)newX] = true;
            }

                // Sinon, le PA n'est pas sur l'image
            else { }

        }

        // Cette contribution prend en compte l'ellipse en entier
        public void contributionTaux2(ref bool[,] grille)
        {
            // On incrémente le nombre de millisecondes sur le pixel concerné sur tous les pixels contenus dans l'ellipse
            // Pour cela on réduit la zone de recherche des pixels

            int x, y;

            List<Vecteur2> listPixels = translatePixelsEllipse();
            foreach (Vecteur2 pixel in listPixels)
            {
                x = (int)pixel.A;
                y = (int)(pixel.B * -1) + ImageExp.dimensionsBandeauLignes;
                if ((x > 0) && (x < grille.GetLength(1)) && (y > 0) && (y < grille.GetLength(0)))
                {
                    grille[y, x] = true;
                }
            }

        }

        // Choix 1 : On considère que le temps passé sur un pixel correspond à la moitié du temps depuis le temps précédent
        // + la moitié du temps avec le temps suivant
        public void contributionDensite(ref double[,] grille, double temps)
        {
            int x, y;
            
            List<Vecteur2> listPixels = translatePixelsEllipse();
            foreach (Vecteur2 pixel in listPixels)
            {
                x = (int)pixel.A;
                y = (int)(pixel.B*-1) + ImageExp.dimensionsBandeauLignes;
                if ((x > 0) && (x < grille.GetLength(1)) && (y > 0) && (y < grille.GetLength(0)))
                {
                    grille[y, x] += temps;
                }
            }
                    
        }

        // Méthode qui permet de déterminer si le point d'attention est dans le bandeau ou non
        public bool dansBandeau()
        {
            Console.WriteLine(this._coordPA.B);
            if (this._coordPA.B > -ImageExp.dimensionsBandeauLignes)
                return true;
            else
                return false;
        }

    }
}
