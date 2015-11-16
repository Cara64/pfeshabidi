using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi
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

        public void AddPA(double a, double b, double z, double tps, double tpsP, double tpsS)
        {
            PointsAttentions.Add(new PointAttention(new Vecteur2(a, b), z, tps, tpsP, tpsS));
        }

    }
}
