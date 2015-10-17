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

        private List<Observation> _observationsPA;
        private List<Observation> _observationsS;

        public Sujet(int position)
        {
            Position = position;
            _observationsPA = new List<Observation>();
            _observationsS = new List<Observation>();
        }

        public void AddObservation(Observation obs, Modalite mod)
        {
            switch (mod)
            {
                case Modalite.PA:
                    _observationsPA.Add(obs);
                    break;
                case Modalite.S:
                    _observationsS.Add(obs);
                    break;
                default:
                    break;
            }
        }
    }
}
