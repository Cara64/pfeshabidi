using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
{
    class Indicateur
    {
        // Paramètres électionnés par l'utilisateur
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
        private List<Sujet> trouveSujets(Groupe g)
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


        // Obtention de la moyenne des taux de recouvrement pour chaque image
        private void determineTaux()
        {

            // On crée la liste provisoire des observations de chaque image
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image
            List<Sujet> sujParGr = new List<Sujet>();

            // Sélection des bons sujets
            foreach (Groupe g in _mesGroupes) {
                sujParGr.Clear();
                sujParGr = trouveSujets(g);

                // On nettoie la liste des observations avant dans la remplir, on va y mettre toutes les observations des users concernés du groupe
                obsParGr.Clear();

                // Sélection des bonnes observations. Elles peuvent être mises en vrac puisque chaue observation connait son image
                foreach (Sujet s in sujParGr)
                {
                    if (_pa)
                    {
                        obsParGr.Concat(s.ObservationsPA);
                    }
                    if (_s)
                    {
                        obsParGr.Concat(s.ObservationsS);
                    }
                }
                // Ensuite on va trier les observations par images en les regroupant grâce à leur numéro


            
            }
        }

    }

}
