using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
{
    public class Observation
    {
        public Image Image { get; private set; }
        private List<PointAttention> _pointsAttentions;

        public Observation(Image image)
        {
            Image = image;
            _pointsAttentions = new List<PointAttention>();
        }

        public void AddPA(double a, double b, double tps)
        {
            _pointsAttentions.Add(new PointAttention(new Vecteur2(a, b), tps));
        }
    }
}
