using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class I_TauxRecouvrement : Indicateur
    {
        private Dictionary<ImageExp, double> data;

        public Dictionary<ImageExp, double> Data
        {
            get { return data; }
            set { data = value; }
        }

        public I_TauxRecouvrement(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            Data = new Dictionary<ImageExp, double>();
        }

        // Permet de calculer le taux de recouvrement d'une image
        // Le dictionnaire des taux est en paramètre pour stocker le taux obtenu
        private void calculeTaux(ImageExp i, Dictionary<ImageExp, List<double>> dico, List<Observation> listeObs)
        {

            // On fait le choix d'une grille qui fait la taille de l'image
            bool[,] pixelsImage = new bool[ImageExp.dimensionsImageLignes, ImageExp.dimensionsImageCol];

            for (int j = 0; j < ImageExp.dimensionsImageLignes; j++)
            {
                for (int k = 0; k < ImageExp.dimensionsImageCol; k++)
                {
                    pixelsImage[j, k] = false;
                }
            }

            PointAttention enCours;

            // Pour chaque observations de l'mage
            foreach (Observation o in listeObs)
            {
                // et pour chaque point d'attention de l'observation
                // On stocke le premier point d'attention pour pouvoir comparer sa valeur
                enCours = o.PointsAttentions[0];

                foreach (PointAttention pa in o.PointsAttentions)
                {
                    // Si les coordonnées sont diféfrentes alors on effectue le traitement sur tout le laps de temps concerné
                    if ((enCours.CoordPA.A != pa.CoordPA.A) || (enCours.CoordPA.B != pa.CoordPA.B))
                    {
                        enCours.contributionTaux2(ref pixelsImage);

                        // On change le PA en cours
                        enCours = pa;
                    }
                    // Si on est face au dernier élément de la liste, il faut faire le traitement quand même
                    if (pa == o.PointsAttentions[o.PointsAttentions.Count - 1])
                    {
                        enCours.contributionTaux2(ref pixelsImage);
                    }
                    
                }
            }

            // On trouve le nombre de pixels "true"
            int somme = 0;
            for (int j = 0; j < ImageExp.dimensionsImageLignes; j++)
            {
                for (int k = 0; k < ImageExp.dimensionsImageCol; k++)
                {
                    if (pixelsImage[j, k])
                    {
                        somme++;
                    }
                    else { }
                }
            }

            // Puis on calcule le taux
            double taux = somme * 100 / (pixelsImage.Length * 1.0);

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
        public Dictionary<ImageExp, double> determineTaux()
        {

            // On crée la liste provisoire des observations de chaque image (à réinitialiser pour chaque groupe)
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image (à réinitialiser pour chaque groupe)
            List<Sujet> sujParGr = new List<Sujet>();

            // Et la liste provisoire des observations associées à chaque image (par groupe)
            Dictionary<ImageExp, List<Observation>> dictionary = new Dictionary<ImageExp, List<Observation>>();

            // On crée la liste où on va stocker tous les taux obtenus par image. Elle sera enrichie par chaque groupe
            Dictionary<ImageExp, List<double>> dictionaryTaux = new Dictionary<ImageExp, List<double>>();

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
                    calculeTaux(i, dictionaryTaux, dictionary[i]);
                }

            }

            // Les taux de tous les groupes sont mentionnés dans dictionaryTaux, ne reste plus qu'à faire la moyenne
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<ImageExp, double> tauxParImage = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dictionaryTaux.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                tauxParImage.Add(i, calculeMoyenne(dictionaryTaux[i]));
            }
            Data = tauxParImage;
            return tauxParImage;

        }

        // Méthode de comparaison
        public Dictionary<ImageExp, double> compareTaux(TypeComp type, I_TauxRecouvrement i)
        {
            // Création du nouvel indicateur de comparaison
            I_TauxRecouvrement indicCompare = new I_TauxRecouvrement(fusionUsers(this, i), fusionOrdres(this, i), fusionPa(this, i), fusionS(this, i), fusionGroupes(this, i));

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
