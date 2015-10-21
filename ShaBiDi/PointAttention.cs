using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
{
    class PointAttention
    {
        // Il s'agit du temps écoulé (en ms) depuis le début de l'expérience pour l'enregistrment de ce point d'attention
        private double _tempsEcoule;

        // Coordonées du point d'attention
        private Vecteur2 _coordPA;

        // Informations nécessaires pour calculer l'ellipse :
        private double _userZ ; // en m
        private double _PAX; // en px
        private double _PAY; // en px
        private double _PhysicalWidth ; // en m
        private double _PhysicalHeight ; // en m
        private double _LogicalWidth ; // en px
        private double _LogicalHeight ; // en px
        private double _ScreenDistance ; // en

        public PointAttention(Vecteur2 vect, double tps)
        {
            _tempsEcoule = tps;
            _coordPA = vect;
        }

        // Détermine la taille de l'ellipse du point d'attention
        public Vecteur2 trouveEllipse()
        {

            //Pour commencer on applique ce calcul (avec le calcul de la Tangente en radian)
            double tan = Math.Tan(Math.PI * 12 / 180); // calcul angle de vision à voir 
            double tan2 = Math.Tan((Math.PI * 15 / 180));

            double hauteurEllispe = (tan * (_ScreenDistance - _userZ)) * 2;
            double largeurEllispe = (tan2 * (_ScreenDistance - _userZ)) * 2;
            hauteurEllispe = (hauteurEllispe / _PhysicalHeight) * _LogicalHeight;
            largeurEllispe = (largeurEllispe / _PhysicalWidth) * _LogicalWidth;

            return new Vecteur2(largeurEllispe, hauteurEllispe);
        }

        // Méthode pour la contribution au taux de recouvrement de ce PA
        public void contributionTaux() { }

        // Méthode pour la contribution à la densité de recouvrement de ce PA
        public void contributionDensité() { }
    }
}
