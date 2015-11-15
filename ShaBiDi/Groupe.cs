using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi
{
    public enum OrdreGroupe { PAS, SPA };
    public class Groupe
    {
        public int Identifiant { get; private set; }
        public OrdreGroupe Ordre { get; private set; }
        public List<Sujet> MesSujets { get; private set; }

        public Groupe()
        {
            MesSujets = new List<Sujet>();
        }

        public Groupe(int identifiant, OrdreGroupe ordre) : this()
        {
            Identifiant = identifiant;
            Ordre = ordre;
        }

        public void AddSujet(Sujet s)
        {
            MesSujets.Add(s);
        }

        public void AddObservation(Observation obs, Modalite mod, int index)
        {
            MesSujets[index].AddObservation(obs, mod);
        }

    }
}
