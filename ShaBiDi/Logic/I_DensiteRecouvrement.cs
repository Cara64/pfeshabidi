using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ShaBiDi.Logic
{
    public class I_DensiteRecouvrement : Indicateur
    {
        Dictionary<Image, double[,]> _monDico;

        public I_DensiteRecouvrement(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            _monDico = new Dictionary<Image, double[,]>();
        }

        // Permet de calculer la densité de recouvrement d'une image
        // Le dictionnaire des taux est en paramètre pour stocker le taux obtenu
        private void calculeDensite(Image i, Dictionary<Image, List<double[,]>> dico, List<Observation> listeObs)
        {

            // On fait le choix d'une grille qui fait la taille de l'image
            double[,] pixelsImage = new double[Image.dimensionsImageLignes, Image.dimensionsImageCol];

            for (int j = 0; j < Image.dimensionsImageLignes; j++)
            {
                for (int k = 0; k < Image.dimensionsImageCol; k++)
                {
                    pixelsImage[j, k] = 0;
                }
            }

            PointAttention enCours;
            double temps;

            // Pour chaque observations de l'mage
            foreach (Observation o in listeObs)
            {
                // et pour chaque point d'attention de l'observation
                // On stocke le premier point d'attention pour pouvoir comparer sa valeur
                enCours = o.PointsAttentions[0];
                temps = 0;

                foreach (PointAttention pa in o.PointsAttentions)
                {
                    // Si les coordonnées sont differentes alors on effectue le traitement sur tout le laps de temps concerné
                    if ((enCours.CoordPA.A != pa.CoordPA.A) || (enCours.CoordPA.A != pa.CoordPA.A))
                    {
                        temps = pa.TempsEcoule - enCours.TempsEcoule;
                        enCours.contributionDensite(ref pixelsImage, temps);

                        // On change le PA en cours
                        enCours = pa;

                        // On réinitialise le temps
                        temps = 0;
                    }
                    // Si on est face au dernier élément de la liste, il faut faire le traitement quand même
                    if (pa == o.PointsAttentions[o.PointsAttentions.Count - 1])
                    {
                        temps = pa.TempsEcoule - enCours.TempsEcoule;
                        enCours.contributionDensite(ref pixelsImage, temps);
                    }

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
            // le tableau doit faire la taille de l'image
            double[,] moyenne = new double[liste[0].GetLength(0),liste[0].GetLength(1)];

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
        public Dictionary<Image, double[,]> determineDensite()
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
                    calculeDensite(i, dictionaryDensite, dictionary[i]);
                    //break;
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
            _monDico = densiteParImage;

            return densiteParImage;

        }

        // Méthode de comparaison
        public I_DensiteRecouvrement compareDensite(TypeComp type, I_DensiteRecouvrement i)
        {
            // Création du nouvel indicateur de comparaison
            I_DensiteRecouvrement indicCompare = new I_DensiteRecouvrement(fusionUsers(this, i), fusionOrdres(this, i), fusionPa(this, i), fusionS(this, i), fusionGroupes(this, i));

            // On cherche à comparer les deux dictionnaires
            Dictionary<Image, List<double[,]>> dico = new Dictionary<Image, List<double[,]>>();

            // On remplit le dictionnaire avec les données du premier indicateur
            foreach (Image img in this._monDico.Keys)
            {
                if (dico.ContainsKey(img))
                {
                    dico[img].Add(this._monDico[img]);
                }
                else
                {
                    dico.Add(img, new List<double[,]>());
                    dico[img].Add(this._monDico[img]);

                }
            }
            // On remplit le dictionnaire avec les données du second indicateur
            foreach (Image img in i._monDico.Keys)
            {
                if (dico.ContainsKey(img))
                {
                    dico[img].Add(i._monDico[img]);
                }
                else
                {
                    dico.Add(img, new List<double[,]>());
                    dico[img].Add(i._monDico[img]);

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

       // travailler, il faudraf faire des foreach pour balayer toutes les valeurs du travaux
        private Dictionary<Image, double[,]> additionner(Dictionary<Image, List<double[,]>> dico)
        {
            Dictionary<Image, double[,]> dicoCompare = new Dictionary<Image, double[,]>();
            double[,] somme= new double[Image.dimensionsImageLignes, Image.dimensionsImageCol];

            foreach (Image i in dico.Keys)
            {

                if (dico[i].Count == 2)
                {
                    for (int l = 0; l < Image.dimensionsImageLignes; l++)
                    {
                        for (int c = 0; c < Image.dimensionsImageCol; c++)
                        {
                            somme[l, c] = dico[i][0][l, c] + dico[i][1][l, c];
                        }
                    }
                        dicoCompare.Add(i, somme);
                }
                else // Cas où il n'y a qu'un composant
                {
                    dicoCompare.Add(i, dico[i][0]);
                }

            }
            return dicoCompare;
        }
        private Dictionary<Image, double[,]> soustraire(Dictionary<Image, List<double[,]>> dico)
        {

            Dictionary<Image, double[,]> dicoCompare = new Dictionary<Image, double[,]>();
            double[,] dif = new double[Image.dimensionsImageLignes, Image.dimensionsImageCol];

            foreach (Image i in dico.Keys)
            {

                if (dico[i].Count == 2)
                {
                    for (int l = 0; l < Image.dimensionsImageLignes; l++)
                    {
                        for (int c = 0; c < Image.dimensionsImageCol; c++)
                        {
                            dif[l, c] = Math.Abs( dico[i][0][l, c] - dico[i][1][l, c]);
                        }
                    }
                        dicoCompare.Add(i, dif);
                }
                else // Cas où il n'y a qu'un composant
                {
                    dicoCompare.Add(i, dico[i][0]);
                }

            }
            return dicoCompare;
        }
        private Dictionary<Image, double[,]> moyenner(Dictionary<Image, List<double[,]>> dico)
        {
            Dictionary<Image, double[,]> dicoCompare = new Dictionary<Image, double[,]>();
            double[,] moy = new double[Image.dimensionsImageLignes, Image.dimensionsImageCol];

            foreach (Image i in dico.Keys)
            {

                if (dico[i].Count == 2)
                {
                    for (int l = 0; l < Image.dimensionsImageLignes; l++)
                    {
                        for (int c = 0; c < Image.dimensionsImageCol; c++)
                        {
                            moy[l, c] = (dico[i][0][l, c] + dico[i][1][l, c])/2;
                        }
                    }
                    dicoCompare.Add(i, moy);
                }
                else // Cas où il n'y a qu'un composant
                {
                    dicoCompare.Add(i, dico[i][0]);
                }

            }
            return dicoCompare;
        }

    }
}
