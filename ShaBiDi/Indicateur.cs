using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi
{
    public enum TypeComp { add, sous, moy };
    public class Indicateur
    {
        // Paramètres sélectionnés par l'utilisateur
        protected List<int> _mesUsers;
        protected List<OrdreGroupe> _mesOrdres;
        protected bool _pa;
        protected bool _s;

        protected List<Groupe> _tousLesGroupes;
        protected List<Groupe> _mesGroupes;
        public List<Groupe> MesGroupes
        {
            get { return _mesGroupes; }
            set { _mesGroupes = value; }
        }

        public Indicateur(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
        {
            _mesUsers = new List<int>();
            _mesUsers = mesUsers;
            _mesOrdres = new List<OrdreGroupe>();
            _mesOrdres = ordres;

            _pa = pa;
            _s = s;

            _tousLesGroupes = groupes;
            _mesGroupes = new List<Groupe>();
            _mesGroupes = trouveGroupes();

        }

        // On remplit la liste des groupes concernés par l'indicateur
        private List<Groupe> trouveGroupes()
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

        // permet de récupérer les bons sujets par groupe à partir des positions sélectionnées
        protected List<Sujet> trouveSujets(Groupe g)
        {
            List<Sujet> liste = new List<Sujet>();
            foreach (Sujet s in g.MesSujets)
            {
                if (_mesUsers.Contains(s.Position))
                {
                    liste.Add(s);
                }
            }
            return liste;
        }

        // Méthodes pour fusionner les différents attributs de deux indicateurs
        protected List<int> fusionUsers(Indicateur i1, Indicateur i2)
        {
            List<int> newlist = new List<int>();
            foreach (int i in i1._mesUsers)
            {
                if (!newlist.Contains(i))
                {
                    newlist.Add(i);
                }
                else { }
            }
            return newlist;
        }
        protected List<OrdreGroupe> fusionOrdres(Indicateur i1, Indicateur i2)
        {
            List<OrdreGroupe> newlist = new List<OrdreGroupe>();
            foreach (OrdreGroupe o in i1._mesOrdres)
            {
                if (!newlist.Contains(o))
                {
                    newlist.Add(o);
                }
                else { }
            }
            return newlist;
        }
        protected List<Groupe> fusionGroupes(Indicateur i1, Indicateur i2)
        {
            List<Groupe> newlist = new List<Groupe>();
            foreach (Groupe g in i1._mesGroupes)
            {
                if (!newlist.Contains(g))
                {
                    newlist.Add(g);
                }
                else { }
            }
            return newlist;
        }
        protected bool fusionPa(Indicateur i1, Indicateur i2)
        {
            if ((i1._pa) || (i2._pa))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected bool fusionS(Indicateur i1, Indicateur i2)
        {
            if ((i1._s) || (i2._s))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

}
