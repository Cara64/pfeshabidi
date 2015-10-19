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

        public PointAttention(Vecteur2 vect, double tps)
        {
            _tempsEcoule = tps;
            _coordPA = vect;
        }
    }
}
