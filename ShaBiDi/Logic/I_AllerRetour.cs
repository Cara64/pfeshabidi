using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class I_AllerRetour : Indicateur
    {
        private Dictionary<ImageExp, double> data;

        public Dictionary<ImageExp, double> Data
        {
            get { return data; }
            set { data = value; }
        }

        public I_AllerRetour(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            Data = new Dictionary<ImageExp, double>();
        }

        // Permet de calculer le nombre d'aller-retour d'une image
        private void calculeAllerRetour(ImageExp i, Dictionary<ImageExp, List<double>> dico, List<Observation> listeObs)
        {

            // Pour chaque observations de l'mage
            // Paramètre qui permet de savoir si le point d'attention d'avant était dans le bandeau ou dans l'image
            bool bandeau;
            int nb = 0;

            foreach (Observation o in listeObs)
            {
                // On regarde si le premier est dans le bandeau ou pas
                bandeau = o.PointsAttentions[0].dansBandeau();

                foreach (PointAttention pa in o.PointsAttentions)
                {
                   bool bTemp = pa.dansBandeau();
                    if (bandeau != bTemp)
                        nb++;

                    // On renseigne le nouveau dernier
                    bandeau = pa.dansBandeau();

                }
            }

            //On ajoute le nombre d'aller-retour calculés à la liste
            if (dico.ContainsKey(i))
            {
                dico[i].Add(nb);
            }
            else
            {
                dico.Add(i, new List<double>());
                dico[i].Add(nb);
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
        public Dictionary<ImageExp, double> determineAllerRetour()
        {

            // On crée la liste provisoire des observations de chaque image (à réinitialiser pour chaque groupe)
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image (à réinitialiser pour chaque groupe)
            List<Sujet> sujParGr = new List<Sujet>();

            // Et la liste provisoire des observations associées à chaque image (par groupe)
            Dictionary<ImageExp, List<Observation>> dictionary = new Dictionary<ImageExp, List<Observation>>();

            // On crée la liste où on va stocker tous les taux obtenus par image. Elle sera enrichie par chaque groupe
            Dictionary<ImageExp, List<double>> dictionaryAllerRetour = new Dictionary<ImageExp, List<double>>();

            // Sélection des bons sujets
            foreach (Groupe g in _mesGroupes)
            {
                sujParGr.Clear();
                sujParGr = trouveSujets(g);

                // On nettoie la liste des observations avant dans la remplir, on va y mettre toutes les observations des users concernés du groupe
                obsParGr.Clear();

                // Sélection des bonnes observations. Elles peuvent être mises en vrac puisque chaque observation connait son image
                foreach (Sujet s in sujParGr)
                {
                    if (_pa)
                    {
                        obsParGr = obsParGr.Concat(s.ObservationsPA).ToList();
                    }
                    if (_s)
                    {
                        obsParGr = obsParGr.Concat(s.ObservationsS).ToList();
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
                foreach (ImageExp i in dictionary.Keys)
                {
                    // La méthode range les taux dans le dictionnaire
                    calculeAllerRetour(i, dictionaryAllerRetour, dictionary[i]);
                }

            }

            // Les taux de tous les groupes sont mentionnés dans dictionaryTaux, ne reste plus qu'à faire la moyenne
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<ImageExp, double> allerRetourParImage = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dictionaryAllerRetour.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                allerRetourParImage.Add(i, calculeMoyenne(dictionaryAllerRetour[i]));
            }
            Data = allerRetourParImage;
            return allerRetourParImage;

        }

        // Méthode de comparaison
        public Dictionary<ImageExp, double> compareAllerRetour(TypeComp type, I_AllerRetour i)
        {
            // Création du nouvel indicateur de comparaison
            I_AllerRetour indicCompare = new I_AllerRetour(fusionUsers(this, i), fusionOrdres(this, i), fusionPa(this, i), fusionS(this, i), fusionGroupes(this, i));

            // On cherche à comparer les deux dictionnaires
            Dictionary<ImageExp, List<double>> dico = new Dictionary<ImageExp,List<double>>();

            // On remplit le dictionnaire avec les données du premier indicateur
            foreach (ImageExp img in this.Data.Keys)
                {
                    if (dico.ContainsKey(img))
                    {
                        dico[img].Add(this.Data[img]);
                    }
                    else
                    {
                        dico.Add(img, new List<double>());
                        dico[img].Add(this.Data[img]);

                    }
                }
            // On remplit le dictionnaire avec les données du second indicateur
            foreach (ImageExp img in i.Data.Keys)
                {
                    if (dico.ContainsKey(img))
                    {
                        dico[img].Add(i.Data[img]);
                    }
                    else
                    {
                        dico.Add(img, new List<double>());
                        dico[img].Add(i.Data[img]);

                    }
                }
            //Puis on procède à la comparaison voulue
            // Ne pas oublier de remplir le dico de l'indicateur à chaque étape
            if (type == TypeComp.add)
            {
                indicCompare.Data = additionner(dico);
            }
            else if (type == TypeComp.sous)
            {
                indicCompare.Data = soustraire(dico);
            }
            else
            {
                indicCompare.Data = moyenner(dico);
            }
            return indicCompare.Data ;
        }

        private Dictionary<ImageExp, double> additionner(Dictionary<ImageExp, List<double>> dico)
        {
            Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dico.Keys)
            {

                // On fait la différence entre les deux éléments de la liste que l'on stocke dans une variable
                // Il faut prévoir le cas où il n'y a qu'une composante
                if (dico[i].Count == 2)
                {
                    dicoCompare.Add(i, dico[i][0] + dico[i][1]);
                }
                else
                {
                    dicoCompare.Add(i, dico[i][0]);
                }
                
            }
            return dicoCompare;
        }
        private Dictionary<ImageExp, double> soustraire(Dictionary<ImageExp, List<double>> dico)
        {
            Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dico.Keys)
            {

                // On fait la différence entre les deux éléments de la liste que l'on stocke dans une variable
                // Il faut prévoir le cas où il n'y a qu'une composante
                if (dico[i].Count == 2)
                {
                    dicoCompare.Add(i, Math.Abs( dico[i][0] - dico[i][1]));
                }
                else
                {
                    dicoCompare.Add(i, dico[i][0]);
                }

            }
            return dicoCompare;
        }
        private Dictionary<ImageExp, double> moyenner(Dictionary<ImageExp, List<double>> dico)
        {
            Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dico.Keys)
            {

                // On fait la différence entre les deux éléments de la liste que l'on stocke dans une variable
                // Il faut prévoir le cas où il n'y a qu'une composante
                if (dico[i].Count == 2)
                {
                    dicoCompare.Add(i, (dico[i][0] + dico[i][1])/2);
                }
                else
                {
                    dicoCompare.Add(i, dico[i][0]);
                }

            }
            return dicoCompare;
        }
    }
}
