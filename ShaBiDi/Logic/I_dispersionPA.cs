using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class I_DispersionPA : Indicateur
    {
        private Dictionary<ImageExp, double> data;

        public Dictionary<ImageExp, double> Data
        {
            get { return data; }
            set { data = value; }
        }

        public I_DispersionPA(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            Data = new Dictionary<ImageExp, double>();
        }

        // Permet de calculer la dispersion des PA à chaque instant t d'une image d'une image
        // Le dictionnaire des dispersions est en paramètre pour stocker la dispersion obtenue
        private void calculeDispersion(ImageExp i, Dictionary<ImageExp, List<double>> dico, List<Observation> listeObs)
        {
            //Liste des distances entres les PA à un instant t
            List<double> distances = new List<double>();

            //Dispersion provisoire
            double disp = 0;

            // Somme de toutes les dipersions de l'expérience (1 image, 1 groupe)
            double sommeDisp = 0;

            // On veut pouvoir regrouper tous les points d'attention du même instant t
            for (int t = 0; t < listeObs[0].PointsAttentions.Count(); t++)
            {
                double sommeDist = 0;
                int n = 0;

                // On va faire la moyenne de trois distance à chaque instant t pour déterminer la dispersion de chaque instant t
                for (int j = 0; j < listeObs.Count(); j++)
                {
                    for (int k = j + 1; k < listeObs.Count(); k++)
                    {
                        sommeDist += listeObs[j].PointsAttentions[t].CoordPA.Distance(listeObs[k].PointsAttentions[t].CoordPA);
                        n++;
                    }
                }

                disp = sommeDist / n;
                sommeDisp += disp;
            }

            // On calculcule la dispersion moyenne sur une image
            double moyDisp = sommeDisp / listeObs[0].PointsAttentions.Count();
                

            //On ajoute _tousLesGroupes taux action la liste
            if (dico.ContainsKey(i))
            {
                dico[i].Add(moyDisp);
            }
            else
            {
                dico.Add(i, new List<double>());
                dico[i].Add(moyDisp);
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
        public Dictionary<ImageExp, double> determineDispersion()
        {

            // On crée la liste provisoire des observations de chaque image (à réinitialiser pour chaque groupe)
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image (à réinitialiser pour chaque groupe)
            List<Sujet> sujParGr = new List<Sujet>();

            // Et la liste provisoire des observations associées à chaque image (par groupe)
            Dictionary<ImageExp, List<Observation>> dictionary = new Dictionary<ImageExp, List<Observation>>();

            // On crée la liste où on va stocker tous les taux obtenus par image. Elle sera enrichie par chaque groupe
            Dictionary<ImageExp, List<double>> dictionaryDispersion = new Dictionary<ImageExp, List<double>>();

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

                // Maintenant, toutes les observations sont triées par image, on va alors déterminer la dispersion moyenne par image

                foreach (ImageExp i in dictionary.Keys)
                {
                    // La méthode range les taux dans le dictionnaire
                    calculeDispersion(i, dictionaryDispersion, dictionary[i]);
                }

            }

            // Les taux de tous les groupes sont mentionnés dans dictionaryTaux, ne reste plus qu'à faire la moyenne
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<ImageExp, double> dispersionParImage = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dictionaryDispersion.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                dispersionParImage.Add(i, calculeMoyenne(dictionaryDispersion[i]));
            }
            Data = dispersionParImage;
            return dispersionParImage;

        }

        // Méthode de comparaison
        public Dictionary<ImageExp, double> compareDispersion(TypeComp type, I_DispersionPA i)
        {
            // Création du nouvel indicateur de comparaison
            I_DispersionPA indicCompare = new I_DispersionPA(fusionUsers(this, i), fusionOrdres(this, i), fusionPa(this, i), fusionS(this, i), fusionGroupes(this, i));

            // On cherche à comparer les deux dictionnaires
            Dictionary<ImageExp, List<double>> dico = new Dictionary<ImageExp, List<double>>();

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
            return indicCompare.Data;
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
                    dicoCompare.Add(i, (dico[i][0] + dico[i][1]) / 2);
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
