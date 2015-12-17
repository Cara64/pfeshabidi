using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class PointAttention
    {
        // Il s'agit du temps écoulé (en ms) depuis le début de l'expérience pour l'enregistrment de ce point d'attention
        private double _tempsEcoule;
        public double TempsEcoule
        {
            get { return _tempsEcoule; }
            set { _tempsEcoule = value; }
        }

        // Il nous faut également le temps d'avant et le temps d'après
        private double _tempsPrec;
        public double TempsPrec
        {
            get { return _tempsPrec; }
            set { _tempsPrec = value; }
        }

        private double _tempsSuiv;
        public double TempsSuiv
        {
            get { return _tempsSuiv; }
            set { _tempsSuiv = value; }
        }

        // Coordonées du point d'attention
        // Elles sont obtenues grâce à la méthode de calcul lancée dans le constructeur
        private Vecteur2 _coordPA;
        public Vecteur2 CoordPA
        {
            get { return _coordPA; }
            set { _coordPA = value; }
        }

        //Largeur (première coordonnée) et hauteur (seconde coordonnée) de l'ellipse (à calculer dans le constructeur)
        private Vecteur2 _dimEllipse;
        public Vecteur2 DimEllipse
        {
            get { return _dimEllipse; }
            set { _dimEllipse = value; }
        }

        // Informations nécessaires pour calculer l'ellipse :
        private double _userZ; // en m
        public double UserZ
        {
            get { return _userZ; }
            set { _userZ = value; }
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

        private double _PhysicalWidth ; // en m
        private double _PhysicalHeight ; // en m
        private double _LogicalWidth ; // en px
        private double _LogicalHeight ; // en px
        private double _ScreenDistance ; // en

        // Le vecteur fourni est celui qui est mentionné dans le fichier de données
        public PointAttention(Vecteur2 vect, double z, double tps, double tpsP, double tpsS)
        {
            _tempsEcoule = tps;
            _tempsPrec = tpsP;
            _tempsSuiv = tpsS;

            _userZ = z ;
            _PAX = vect.A;
            _PAY = vect.B;

            _PhysicalWidth = 2.63 ;
            _PhysicalHeight = 1.61 ; 
            _LogicalWidth = 1680 ; 
            _LogicalHeight = 1050 ;
            _ScreenDistance = 3.833 ;

            _dimEllipse = trouveEllipse();
            _coordPA = trouveBonPA(vect);
        }

        // Détermine la taille de l'ellipse du point d'attention
        public Vecteur2 trouveEllipse()
        {

            //Pour commencer on applique ce calcul (avec le calcul de la Tangente en radian)
            double tan = Math.Tan(Math.PI * 12 / 180); // calcul angle de vision à voir 
            double tan2 = Math.Tan((Math.PI * 15 / 180));

            double hauteurEllispe = (tan * (_ScreenDistance - _userZ));
            double largeurEllispe = (tan2 * (_ScreenDistance - _userZ));
            hauteurEllispe = (hauteurEllispe / _PhysicalHeight) * _LogicalHeight;
            largeurEllispe = (largeurEllispe / _PhysicalWidth) * _LogicalWidth;

            return new Vecteur2(largeurEllispe, hauteurEllispe);
        }

        // Trouve le bon point d'attention à partir du "mauvais"
        public Vecteur2 trouveBonPA(Vecteur2 vect)
        {
            return new Vecteur2(vect.A + _dimEllipse.A / 2, vect.B + _dimEllipse.B / 2);
            // On a besoin de valeurs rondes donc on arrondit
        }



        // Choix 1 on fait le choix de calculer les taux avec le PA seuls
        // x en premiers dans le tableau, y en seconds

        // Cette contribution au taux ne prend en compte que le point d'attention
        public void contributionTaux1(ref bool[,] grille)
        {
            double newX = Math.Floor(_coordPA.A);
            double newY = Math.Floor(_coordPA.B);
            if ((newX > 0) && (newX < grille.GetLength(1)) && (newY > 0) && (newY < grille.GetLength(0)))
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

                foreach (Vecteur2 pix in trouvePixelsBool(grille))
                {
                    if (dansEllipse(pix))
                    {
                        grille[(int)pix.B, (int)pix.A] = true;
                    }
                }

        }

        // Cette contribution prend en compte l'ellipse

        // On peut envisager d'autres méthodes de contribution au taux (on en testera plusieurs)

        // Choix 1 : On considère que le temps passé sur un pixel correspond à la moitié du temps depuis le temps précédent
        // + la moitié du temps avec le temps suivant
        public void contributionDensité(ref double[,] grille)
        {

                // On incrémente le nombre de millisecondes sur le pixel concerné sur tous les pixels contenus dans l'ellipse
                // Pour cela on réduit la zone de recherche des pixels

                foreach (Vecteur2 pix in trouvePixelsDouble(grille))
                {
                    if (dansEllipse(pix))
                    {
                        grille[(int)pix.B, (int)pix.A] = grille[(int)pix.B, (int)pix.A] + (_tempsSuiv - _tempsPrec) / 2;
                    }
                }

        }

        // Méthode qui définit si un pixel est dans une ellipse ou non
        // Le premier vecteur a comme première coord la largeur
        // Le second vecteur a comme seconde coord la hauteur
        public bool dansEllipse(Vecteur2 pixel)
        {
            // Il faut travailler dans le nouveau repère du centre de l'ellipse
            Vecteur2 newPixel = translatePixel(pixel);

            // On détermine l'équation de la droite y=a*x passant par le pixel et le centre de l'ellipse, dans le repère de l'ellipse
            // Calcul du coef directeur coef
            double coef = (newPixel.B) / (newPixel.A);

            // L'ellipse a pour équation (x^2/a^2)+(y^2/b^2)=1
            // On cherche le point M(x,y) appartenant au contour de l'ellipse et à la droite
            // Calcul du discriminant de x^2*[(1/a^2)+(coef^2/b^2)]-1=0
            double facteurX = 1 / Math.Pow(_dimEllipse.A,2) + Math.Pow(coef,2) / Math.Pow(_dimEllipse.B,2);
            double discrim = 4 * facteurX;
            // Calcul d'une des deux solutions de x possibles (peu importe laquelle)
            double x1 = Math.Sqrt(discrim) / (2 * facteurX);
            // On cherche la coordonnée y correspondant à cette solution grâce à l'équation de départ;
            double y1 = coef * x1;

            // Calcul la distance de ce point au centre
            double dSol = Math.Sqrt(Math.Pow(x1,2) + Math.Pow(y1,2));
            // On calcule la distance du pixel au centre
            double dPix = Math.Sqrt(Math.Pow(newPixel.A,2) + Math.Pow(newPixel.B,2));

            // On compra eles deux distances et on conclut sur l'appartenance du pixel à l'ellipse
            if (dPix <= dSol)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Méthode pour translater les coordonnées d'un pixel dans un repère pour origine le centre de l'ellipse
        private Vecteur2 translatePixel(Vecteur2 pixel)
        {
            return new Vecteur2(pixel.A - _coordPA.A, pixel.B - _coordPA.B);
        }

        // Méthode pour trouver les pixels de recherche restreints autour du point d'attention
        // Méthode à améliorer pour restreindre la liste
        // Version pour un tableau de double
        private List<Vecteur2> trouvePixelsDouble(double[,] grille)
        {
            List<Vecteur2> liste = new List<Vecteur2>();
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    liste.Add(new Vecteur2(j, i));
                }
            }
            return liste;
        }
        // Version pour un tableau de bool
        private List<Vecteur2> trouvePixelsBool(bool[,] grille)
        {
            List<Vecteur2> liste = new List<Vecteur2>();
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    liste.Add(new Vecteur2(j, i));
                }
            }
            return liste;
        }


    }
}
