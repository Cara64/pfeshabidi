using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
{
    public class Observation
    {
        private Image Image { get; set; }
        private List<Vecteur2> _pointsAttentions;

        public Observation(Image image)
        {
            Image = image;
            _pointsAttentions = new List<Vecteur2>();
        }

        public void AddVecteur(double a, double b)
        {
            _pointsAttentions.Add(new Vecteur2(a, b));
        }
    }
}
