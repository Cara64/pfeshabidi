using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
{
    public enum OrdreGroupe { PAS, SPA };
    public class Groupe
    {
        public int Identifiant { get; private set; }
        public OrdreGroupe Ordre { get; private set; }

        private List<Sujet> _mesSujets;

        public Groupe(int identifiant, OrdreGroupe ordre)
        {
            Identifiant = identifiant;
            Ordre = ordre;

            _mesSujets = new List<Sujet>();
        }

        public void AddSujet(Sujet s)
        {
            _mesSujets.Add(s);
        }

        public void AddObservation(Observation obs, Modalite mod, int index)
        {
            _mesSujets[index].AddObservation(obs, mod);
        }

    }
}
