using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi
{
    class I_DensiteRecouvrement : Indicateur
    {
        public I_DensiteRecouvrement(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes) : base(mesUsers, ordres, pa, s, groupes) { }

        // Permet de calculer la densité de recouvrement d'une image
        // Le dictionnaire des taux est en paramètre pour stocker le taux obtenu
        private void calculeTaux(Image i, Dictionary<Image, List<double[,]>> dico, List<Observation> listeObs)
        {

            // On fait le choix d'une grille qui fait la taille de l'image
            double[,] pixelsImage = new double[Image.dimensionsImageX, Image.dimensionsImageY];

            for (int j = 0; j < Image.dimensionsImageX; j++)
            {
                for (int k = 0; k < Image.dimensionsImageY; k++)
                {
                    pixelsImage[j, k] = 0;
                }
            }

            // Pour chaque observations de l'mage
            foreach (Observation o in listeObs)
            {
                // et pour chaque point d'attention de l'observation

                foreach (PointAttention pa in o.PointsAttentions)
                {
                    pa.contributionDensité(ref pixelsImage);
                }
            }

            // Ajout pour une image des temps sur tous ses pixels
            if (dico.ContainsKey(i))
            {
                dico[i].Add(pixelsImage);
            }
            else
            {
                dico.Add(i, new List<double[,]>());
                dico[i].Add(pixelsImage);
            }

        }

        private double[,] calculeMoyenne(List<double[,]> liste)
        {
            // le tableau doit faire la taille e l'image
            double[,] moyenne = new double[liste[0].GetLength(0),liste[0].GetLength(0)];

            // Faire la moyenne sur chaque image
            foreach (double[,] d in liste)
            {
                // et sur chaque pixel de l'image
                for (int i = 0; i < d.GetLength(0); i++)
                {
                    for (int j = 0; j < d.GetLength(1); j++)
                    {
                        moyenne[i, j] = moyenne[i, j] + d[i, j];
                    }
                }

            }

            for (int i = 0; i < moyenne.GetLength(0); i++)
            {
                for (int j = 0; j < moyenne.GetLength(1); j++)
                {
                    moyenne[i, j] = moyenne[i, j]/liste.Count();
                }
            }
            return moyenne;
  
        }

        // on obtient une liste d'image avec les temps moyennés pour chaque pixel
        // Là on obtient pour toutes les images concernées
        // Il faudrait permettre de choisir pour une seule image
        public Dictionary<Image, double[,]> determineTaux()
        {

            // On crée la liste provisoire des observations de chaque image (à réinitialiser pour chaque groupe)
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image (à réinitialiser pour chaque groupe)
            List<Sujet> sujParGr = new List<Sujet>();

            // Et la liste provisoire des observations associées à chaque image (par groupe)
            Dictionary<Image, List<Observation>> dictionary = new Dictionary<Image, List<Observation>>();

            // On crée la liste où on va stocker tous les taux obtenus par image. Elle sera enrichie par chaque groupe
            Dictionary<Image, List<double[,]>> dictionaryDensite = new Dictionary<Image, List<double[,]>>();

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
                    // La méthode range les taux dans le dictionnaire dictionaryDensite
                    calculeTaux(i, dictionaryDensite, dictionary[i]);
                }

            }

            // Les taux de tous les groupes sont mentionnés dans dictionaryDensite, ne reste plus qu'à faire la moyenne de chaque pixel pour chaque image
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<Image, double[,]> densiteParImage = new Dictionary<Image, double[,]>();

            foreach (Image i in dictionaryDensite.Keys)
            {
                // Calcul de la moyenne de tous les temps de chaque pixel de l'image
                densiteParImage.Add(i, calculeMoyenne(dictionaryDensite[i]));
            }
            return densiteParImage;

        }
    }
}
