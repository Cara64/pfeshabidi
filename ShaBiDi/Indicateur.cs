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

        // Permet de calculer le taux de recouvrement d'une image
        // Le dictionnaire des taux est en paramètre pour stocker le taux obtenu
        // Méthode à préciser

        private void calculeTaux(Image i, Dictionary<Image, List<double>> dico, List<Observation> listeObs) {
            int taux = 0;
            // Aouter ici la méthode pour calculer le taux

            //On ajoute _tousLesGroupes taux action la liste
            if (dico.ContainsKey(i))
            {
                dico[i].Add(taux);
            }
            else
            {
                dico.Add(i, new List<double>());
                dico[i].Add(taux);
            }

        }

        private double calculeMoyenne(List<double> liste)
        {
            double somme = 0;
            foreach (double d in liste)
            {
                somme += d;
            }

            return somme / liste.Count();
        }



        // Obtention de la moyenne des taux de recouvrement pour chaque image
        private void determineTaux()
        {

            // On crée la liste provisoire des observations de chaque image
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image
            List<Sujet> sujParGr = new List<Sujet>();

            // Et la liste provisoire des obeservations associées à chaque image
            Dictionary<Image, List<Observation>> dictionary = new Dictionary<Image, List<Observation>>();

            // On crée la liste où on va stocker tous les taux obtenus par image. Elle sera enrichie par chaque groupe
            Dictionary<Image, List<double>> dictionaryTaux = new Dictionary<Image, List<double>>();

            // Sélection des bons sujets
            foreach (Groupe g in _mesGroupes) {
                sujParGr.Clear();
                sujParGr = trouveSujets(g);

                // On nettoie la liste des observations avant dans la remplir, on va y mettre toutes les observations des users concernés du groupe
                obsParGr.Clear();

                // Sélection des bonnes observations. Elles peuvent être mises en vrac puisque chaque observation connait son image
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
                // On nettoie le dictionnaire du groupe précédent
                dictionary.Clear();

                foreach (Observation o in obsParGr)
                {
                    if (dictionary.ContainsKey(o.Image))
                    {
                        dictionary[o.Image].Add(o);
                    }
                    else
                    {
                        dictionary.Add(o.Image, new List<Observation>());
                        dictionary[o.Image].Add(o);

                    }
                }

                // Maintenant, toutes les observations sont triées par image, on va alors déterminer le taux par image que l'on va mettre dans la liste des taux
                // Méthode à réaliser
                foreach (Image i in dictionary.Keys)
                {
                    // La méthode range les taux dans le dictionnaire
                    calculeTaux(i, dictionaryTaux, dictionary[i]);
                }

            }

            // Les taux de tous les groupes sont mentionnés dans dictionaryTaux, ne reste plus qu'à faire la moyenne
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<Image, double> tauxParImage = new Dictionary<Image, double>();

            foreach (Image i in dictionaryTaux.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                tauxParImage.Add (i, calculeMoyenne(dictionaryTaux[i]));
            }

        }
        

    }

}
