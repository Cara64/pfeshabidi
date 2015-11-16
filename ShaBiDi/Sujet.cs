using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi
{
    public enum Modalite { PA, S };
    public class Sujet
    {
        public int Position { get; private set; }

        public List<Observation> ObservationsPA { get; private set; }
        public List<Observation> ObservationsS { get; private set; }

        public Sujet(int position)
        {
            Position = position;
            ObservationsPA = new List<Observation>();
            ObservationsS = new List<Observation>();
        }

        public void AddObservation(Observation obs, Modalite mod)
        {
            switch (mod)
            {
                case Modalite.PA:
                    ObservationsPA.Add(obs);
                    break;
                case Modalite.S:
                    ObservationsS.Add(obs);
                    break;
                default:
                    break;
            }
        }

        //i est le numéro d el'image concernée
        public void AddPA(int i, Modalite mod, double x, double y, double z, double tps, double tpsP, double tpsS)
        {
            // Il faut trouver le rang de l'observation concernée par l'image donnée
            int indice = 0;

            // Premier cas, l'observation est en modalité PA
            if (mod == Modalite.PA)
            {
                foreach (Observation o in ObservationsPA)
                {
                    if (o.Image.Numero == i)
                    {
                        indice = ObservationsPA.IndexOf(o);
                    }
                    else { }
                }

                ObservationsPA[indice].AddPA(x, y, z, tps, tpsP, tpsS);
            }
            // Deuxième cas, l'observation est en modalité S
            else
            {
                foreach (Observation o in ObservationsS)
                {
                    if (o.Image.Numero == i)
                    {
                        indice = ObservationsS.IndexOf(o);
                    }
                    else { }
                }

                ObservationsS[indice].AddPA(x, y, z, tps, tpsP, tpsS);
            }
        }
    }
}
