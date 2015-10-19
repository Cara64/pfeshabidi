using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
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
    }
}
