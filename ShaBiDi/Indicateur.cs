using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
{
    class Indicateur
    {
        private List<int> _mesUsers;
        private List<OrdreGroupe> _mesOrdres;
        private bool _pa;
        private bool _s;

        private List<Groupe> _tousLesGroupes;
        private List<Groupe> _mesGroupes;

        public  Indicateur(List<int> positions, List<Groupe> groupes)
        {
            _tousLesGroupes = groupes;
            _mesGroupes = new List<Groupe>();
            _mesGroupes = trouveGroupes();
        }

        // On remplit la liste des groupes concernés par l'indicateur
        public List<Groupe> trouveGroupes()
        {
            List<Groupe> liste = new List<Groupe>();
            foreach (Groupe g in _tousLesGroupes)
            {
                if ((_mesOrdres.Contains(OrdreGroupe.PAS)) && (g.Ordre == OrdreGroupe.PAS))
                {
                    liste.Add(g);
                }
                else { }
                if ((_mesOrdres.Contains(OrdreGroupe.SPA)) && (g.Ordre == OrdreGroupe.SPA))
                {
                    liste.Add(g);
                }
                else { }
            }

            return liste;
        }
    }
}
