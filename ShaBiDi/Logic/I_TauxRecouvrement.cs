using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    public class I_TauxRecouvrement : Indicateur
    {
        public Dictionary<Image, double> _monDico;

        public I_TauxRecouvrement(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            _monDico = new Dictionary<Image, double>();
        }

        // Permet de calculer le taux de recouvrement d'une image
        // Le dictionnaire des taux est en paramètre pour stocker le taux obtenu
        private void calculeTaux(Image i, Dictionary<Image, List<double>> dico, List<Observation> listeObs)
        {

            // On fait le choix d'une grille qui fait la taille de l'image
            bool[,] pixelsImage = new bool[Image.dimensionsImageX, Image.dimensionsImageY];

            for (int j = 0; j < Image.dimensionsImageX; j++)
            {
                for (int k = 0; k < Image.dimensionsImageY; k++)
                {
                    pixelsImage[j, k] = false;
                }
            }

            // Pour chaque observations de l'mage
            foreach (Observation o in listeObs)
            {
                // et pour chaque point d'attention de l'observation

                foreach (PointAttention pa in o.PointsAttentions)
                {
                    pa.contributionTaux1(ref pixelsImage);
                }
            }

            // On trouve le nombre de pixels "true"
            int somme = 0;
            for (int j = 0; j < Image.dimensionsImageX; j++)
            {
                for (int k = 0; k < Image.dimensionsImageY; k++)
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
        public Dictionary<Image, double> determineTaux()
        {

            // On crée la liste provisoire des observations de chaque image (à réinitialiser pour chaque groupe)
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image (à réinitialiser pour chaque groupe)
            List<Sujet> sujParGr = new List<Sujet>();

            // Et la liste provisoire des observations associées à chaque image (par groupe)
            Dictionary<Image, List<Observation>> dictionary = new Dictionary<Image, List<Observation>>();

            // On crée la liste où on va stocker tous les taux obtenus par image. Elle sera enrichie par chaque groupe
            Dictionary<Image, List<double>> dictionaryTaux = new Dictionary<Image, List<double>>();

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
                tauxParImage.Add(i, calculeMoyenne(dictionaryTaux[i]));
            }
            _monDico = tauxParImage;
            return tauxParImage;

        }

        // Méthode de comparaison
        public I_TauxRecouvrement compareTaux(TypeComp type, I_TauxRecouvrement i1, I_TauxRecouvrement i2)
        {
            // Création du nouvel indicateur de comparaison
            I_TauxRecouvrement indicCompare = new I_TauxRecouvrement(fusionUsers(i1, i2), fusionOrdres(i1, i2), fusionPa(i1, i2), fusionS(i1, i2), fusionGroupes(i1, i2));

            // On cherche à comparer les deux dictionnaires
            Dictionary<Image, List<double>> dico = new Dictionary<Image,List<double>>();

            // On remplit le dictionnaire avec les données du premier indicateur
            foreach (Image i in i1._monDico.Keys)
                {
                    if (dico.ContainsKey(i))
                    {
                        dico[i].Add(i1._monDico[i]);
                    }
                    else
                    {
                        dico.Add(i, new List<double>());
                        dico[i].Add(i1._monDico[i]);

                    }
                }
            // On remplit le dictionnaire avec les données du second indicateur
            foreach (Image i in i2._monDico.Keys)
                {
                    if (dico.ContainsKey(i))
                    {
                        dico[i].Add(i2._monDico[i]);
                    }
                    else
                    {
                        dico.Add(i, new List<double>());
                        dico[i].Add(i2._monDico[i]);

                    }
                }
            //Puis on procède à la comparaison voulue
            // Ne pas oublier de remplir le dico de l'indicateur à chaque étape
            if (type == TypeComp.add)
            {
                indicCompare._monDico = additionner(dico);
            }
            else if (type == TypeComp.sous)
            {
                indicCompare._monDico = soustraire(dico);
            }
            else
            {
                indicCompare._monDico = moyenner(dico);
            }
            return indicCompare;
        }

        private Dictionary<Image, double> additionner(Dictionary<Image, List<double>> dico)
        {
            Dictionary<Image, double> dicoCompare = new Dictionary<Image, double>();

            foreach (Image i in dico.Keys)
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
        private Dictionary<Image, double> soustraire(Dictionary<Image, List<double>> dico)
        {
            Dictionary<Image, double> dicoCompare = new Dictionary<Image, double>();

            foreach (Image i in dico.Keys)
            {

                // On fait la différence entre les deux éléments de la liste que l'on stocke dans une variable
                // Il faut prévoir le cas où il n'y a qu'une composante
                if (dico[i].Count == 2)
                {
                    dicoCompare.Add(i, dico[i][0] - dico[i][1]);
                }
                else
                {
                    dicoCompare.Add(i, dico[i][0]);
                }

            }
            return dicoCompare;
        }
        private Dictionary<Image, double> moyenner(Dictionary<Image, List<double>> dico)
        {
            Dictionary<Image, double> dicoCompare = new Dictionary<Image, double>();

            foreach (Image i in dico.Keys)
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
