using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class Observation
    {
        public Image Image { get; private set; }

        public List<PointAttention> PointsAttentions { get; private set; }

        public Observation(Image image)
        {
            Image = image;
            PointsAttentions = new List<PointAttention>();
        }

        public void AddPA(double a, double b, double tps)
        {
            PointsAttentions.Add(new PointAttention(new Vecteur2(a, b),tps));
        }

    }
}
