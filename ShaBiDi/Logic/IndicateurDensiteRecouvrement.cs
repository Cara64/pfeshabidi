using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using ShaBiDi.Views;


namespace ShaBiDi.Logic
{
    /// <summary>
    /// IndicateurDensiteRecouvrement - Classe de l'indicateur concernant la densité de recouvrement
    /// </summary>
    public class IndicateurDensiteRecouvrement : Indicateur
    {

        #region Attributs et propriétés

        /// <summary>
        /// Dictionnaire permettant de récolter les données pour chaque image
        /// </summary>
        private Dictionary<ImageExp, double[,]> data;
        public Dictionary<ImageExp, double[,]> Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Liste de dictionnaires de données pour les comparaisons
        /// Premier élément : données de la classe
        /// Deuxième élément : données de l'indicateur comparé
        /// Troisième élément : résultat de la comparaison
        /// </summary>
        private List<Dictionary<ImageExp, double[,]>> dataComparaison;
        public List<Dictionary<ImageExp, double[,]>> DataComparaison
        {
            get { return dataComparaison; }
            set { dataComparaison = value; }
        }


        /// <summary>
        /// Indicateur avec lequel on effectue la comparaison
        /// </summary>
        private IndicateurDensiteRecouvrement indicCompare;
        public IndicateurDensiteRecouvrement IndicCompare
        {
            get { return indicCompare; }
            set { indicCompare = value; }
        }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur de la classe IndicateurDensiteRecouvrement
        /// </summary>
        /// <param name="mesUsers">Positions sélectionnées pour l'indicateur</param>
        /// <param name="ordres">Ordres sélectionnés pour l'indicateur</param>
        /// <param name="pa">Modalité PA ou non</param>
        /// <param name="s">Modalité S ou non</param>
        /// <param name="groupes">Groupes sélectionnés pour l'indicateur</param>
        public IndicateurDensiteRecouvrement(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            Data = new Dictionary<ImageExp, double[,]>();
            DataComparaison = new List<Dictionary<ImageExp, double[,]>>();
        }

        #endregion


        #region Méthodes de gestion des threads

        /// <summary>
        /// Méthode de thread qui calcule pour chaque image une matrice de densité
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<ImageExp, List<Observation>> dictionary = e.Argument as Dictionary<ImageExp, List<Observation>>;
            Dictionary<ImageExp, List<double[,]>> dictionaryDensite = new Dictionary<ImageExp,List<double[,]>>();
            
            int counterTauxImage = 0;
            foreach (ImageExp i in dictionary.Keys)
            {
                counterTauxImage++;
                (sender as BackgroundWorker).ReportProgress(counterTauxImage, dictionary.Count());
                // La méthode range les taux dans le dictionnaire dictionaryDensite
                calculeDensite(i, dictionaryDensite, dictionary[i]);
            }

            e.Result = dictionaryDensite;
        }

        /// <summary>
        /// Méthode qui transmet les infos de chargement à la fenêtre de chargement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int total = (int)e.UserState;
            wait.lblProgression.Content = "Calcul de l'image " + e.ProgressPercentage.ToString() + " sur " + total.ToString();
        }

        /// <summary>
        /// Méthode exécuté une fois le calcul en thread terminé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wait.Close(); // fermeture de la fenêtre d'attente

            Dictionary<ImageExp, List<double[,]>> dictionaryDensite = e.Result as Dictionary<ImageExp, List<double[,]>>;

            // Les taux de tous les groupes sont mentionnés dans dictionaryDensite, ne reste plus qu'à faire la moyenne
            // On crée la liste des densité par image sous forme de dictionnaire
            Dictionary<ImageExp, double[,]> densiteParImage = new Dictionary<ImageExp, double[,]>();

            foreach (ImageExp i in dictionaryDensite.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                densiteParImage.Add(i, AppData.calculeMoyenne(dictionaryDensite[i]));
            }

            Data = densiteParImage;

            // On génère la fenêtre de taux de recouvrement 
            DensiteRecouvrementUC dr = new DensiteRecouvrementUC("gris");
            
        }

        #endregion


        #region Algorithmes de calcul et de comparaison

        /// <summary>
        /// Méthode qui permet de calculer la densité de recouvrement d'une image
        /// </summary>
        /// <param name="i">Image de l'expérience dont on veut calculer la densité de recouvrement</param>
        /// <param name="dico">Dictionnaire dans lequel on veut stocker l'ensemble des données</param>
        /// <param name="listeObs">Liste des observations</param>
        private void calculeDensite(ImageExp i, Dictionary<ImageExp, List<double[,]>> dico, List<Observation> listeObs)
        {

            // On fait le choix d'une grille qui fait la taille de l'image
            double[,] pixelsImage = new double[ImageExp.DIM_IMAGE_ROW, ImageExp.DIM_IMAGE_COL];

            for (int j = 0; j < ImageExp.DIM_IMAGE_ROW; j++)
            {
                for (int k = 0; k < ImageExp.DIM_IMAGE_COL; k++)
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
                    if ((enCours.CoordPA.A != pa.CoordPA.A) || (enCours.CoordPA.B != pa.CoordPA.B))
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
            if (!(dico.ContainsKey(i)))
            {
                dico.Add(i, new List<double[,]>());

            }
            
            dico[i].Add(pixelsImage);       
        }
        
        /// <summary>
        /// Obtenir une liste d'image avec les temps moyennés pour chaque pixel
        /// </summary>
        public void determineDensite()
        {
            // On crée la liste provisoire des observations de chaque image (à réinitialiser pour chaque groupe)
            List<Observation> obsParGr = new List<Observation>();

            // Ainsi que la liste provisoire des sujets de chaque image (à réinitialiser pour chaque groupe)
            List<Sujet> sujParGr = new List<Sujet>();

            // Et la liste provisoire des observations associées à chaque image (par groupe)
            Dictionary<ImageExp, List<Observation>> dictionary = new Dictionary<ImageExp, List<Observation>>();

            // Sélection des bons sujets
            foreach (Groupe g in mesGroupes)
            {
                sujParGr.Clear();
                sujParGr = trouveSujets(g);

                // On nettoie la liste des observations avant dans la remplir, on va y mettre toutes les observations des users concernés du groupe
                obsParGr.Clear();

                // Sélection des bonnes observations. Elles peuvent être mises en vrac puisque chaque observation connait son image
                foreach (Sujet s in sujParGr)
                {
                    if (modPA) obsParGr = obsParGr.Concat(s.ObservationsPA).ToList();
                    if (modS) obsParGr = obsParGr.Concat(s.ObservationsS).ToList();

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
                // On laisse la place au BackgroundWorker pour effectuer le calcul et afficher le résultat
                wait.pbProgression.IsIndeterminate = true;
                wait.Show();

                bw.RunWorkerAsync(dictionary);
            }
        }

        /// <summary>
        /// Méthode qui permet de comparer les taux de deux indicateurs
        /// </summary>
        /// <param name="type">Type de comparaison</param>
        public void compareDensite(TypeComp type)
        {
            // Création du nouvel indicateur de comparaison
            IndicateurDensiteRecouvrement resultat = new IndicateurDensiteRecouvrement(fusionUsers(this, IndicCompare), fusionOrdres(this, IndicCompare), fusionPA(this, IndicCompare), fusionS(this, IndicCompare), fusionGroupes(this, IndicCompare));

            // On cherche à comparer les deux dictionnaires
            Dictionary<ImageExp, List<double[,]>> dico = new Dictionary<ImageExp, List<double[,]>>();

            // On remplit le dictionnaire avec les données du premier indicateur
            foreach (ImageExp img in this.Data.Keys)
            {
                if (!(dico.ContainsKey(img)))
                {
                    dico.Add(img, new List<double[,]>());
                }

                dico[img].Add(this.Data[img]);
            }

            // On remplit le dictionnaire avec les données du second indicateur
            foreach (ImageExp img in IndicCompare.Data.Keys)
            {
                if (!(dico.ContainsKey(img)))
                {
                    dico.Add(img, new List<double[,]>());
                }
                
                dico[img].Add(IndicCompare.Data[img]);
            }

            //Puis on procède à la comparaison voulue
            // Ne pas oublier de remplir le dico de l'indicateur à chaque étape
            if (type == TypeComp.Add)
            {
                resultat.Data = additionner(dico);
            }
            else if (type == TypeComp.Sous)
            {
                resultat.Data = soustraire(dico);
            }
            else
            {
                resultat.Data = moyenner(dico);
            }

            DataComparaison.Add(this.Data);
            DataComparaison.Add(IndicCompare.Data);
            DataComparaison.Add(resultat.Data);
        }

        /// <summary>
        /// Méthode de comparaison par addition
        /// </summary>
        /// <param name="dico">Dictionnaire de données de l'indicateur à comparé</param>
        /// <returns>Dictionnaire de données de comparaison par addition</returns>
        private Dictionary<ImageExp, double[,]> additionner(Dictionary<ImageExp, List<double[,]>> dico)
        {
            Dictionary<ImageExp, double[,]> dicoCompare = new Dictionary<ImageExp, double[,]>();
            double[,] somme= new double[ImageExp.DIM_IMAGE_ROW, ImageExp.DIM_IMAGE_COL];

            foreach (ImageExp i in dico.Keys)
            {

                if (dico[i].Count == 2)
                {
                    for (int l = 0; l < ImageExp.DIM_IMAGE_ROW; l++)
                    {
                        for (int c = 0; c < ImageExp.DIM_IMAGE_COL; c++)
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

        /// <summary>
        /// Méthode de comparaison par soustraction
        /// </summary>
        /// <param name="dico">Dictionnaire de données de l'indicateur à comparé</param>
        /// <returns>Dictionnaire de données de comparaison par soustraction</returns>
        private Dictionary<ImageExp, double[,]> soustraire(Dictionary<ImageExp, List<double[,]>> dico)
        {
            Dictionary<ImageExp, double[,]> dicoCompare = new Dictionary<ImageExp, double[,]>();
            double[,] dif = new double[ImageExp.DIM_IMAGE_ROW, ImageExp.DIM_IMAGE_COL];

            foreach (ImageExp i in dico.Keys)
            {
                if (dico[i].Count == 2)
                {
                    for (int l = 0; l < ImageExp.DIM_IMAGE_ROW; l++)
                    {
                        for (int c = 0; c < ImageExp.DIM_IMAGE_COL; c++)
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

        /// <summary>
        /// Méthode de comparaison par moyenne
        /// </summary>
        /// <param name="dico">Dictionnaire de données de l'indicateur à comparé</param>
        /// <returns>Dictionnaire de données de comparaison par moyenne</returns>
        private Dictionary<ImageExp, double[,]> moyenner(Dictionary<ImageExp, List<double[,]>> dico)
        {
            Dictionary<ImageExp, double[,]> dicoCompare = new Dictionary<ImageExp, double[,]>();
            double[,] moy = new double[ImageExp.DIM_IMAGE_ROW, ImageExp.DIM_IMAGE_COL];

            foreach (ImageExp i in dico.Keys)
            {

                if (dico[i].Count == 2)
                {
                    for (int l = 0; l < ImageExp.DIM_IMAGE_ROW; l++)
                    {
                        for (int c = 0; c < ImageExp.DIM_IMAGE_COL; c++)
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

        /// <summary>
        /// Méthode d'extraction des données résultant de l'indicateur dans un fichier CSV
        /// </summary>
        /// <param name="currentDir">Répertoire dans lequel on génère le CSV</param>
        public void extractOutputDensiteRecouvrementToCSV(string currentDir)
        {
            // On prépare les données
            StringBuilder csv = new StringBuilder();
            string delimiter = ";";
            string filePath = currentDir + "/" + Title + "_OUTPUT.csv";
            var mesures = Data.Keys.OrderBy(o => o.Numero).ToList();


            // On ajoute les lignes du CSV
            // Pour déterminer le bandeau de titre
            int sizeCsvTitle = ShaBiDi.Logic.ImageExp.DIM_IMAGE_ROW * ShaBiDi.Logic.ImageExp.DIM_IMAGE_COL + 1;
            string[] csvTitle = new string[sizeCsvTitle];
            csvTitle[0] = "Image";
            int lignes = 0;
            int col = 0;
            for (int i = 1; i < csvTitle.Length; i++)
            {
                if (lignes > ShaBiDi.Logic.ImageExp.DIM_IMAGE_ROW) lignes = 0;
                if (col > ShaBiDi.Logic.ImageExp.DIM_IMAGE_COL)
                {
                    col = 0;
                    lignes++;
                }
                csvTitle[i] = "[" + lignes + "," + col + "]";
                col++;
            }

            csv.AppendLine(string.Join(delimiter, csvTitle));

            foreach (var key in mesures)
            {
                int k = 0;
                string[] value = new string[sizeCsvTitle];
                value[k] = key.Numero.ToString();

                for (int i = 0; i < Data[key].GetLength(0); i++)
                {
                    for (int j = 0; j < Data[key].GetLength(1); j++)
                    {
                        k++;
                        value[k] = Data[key][i, j].ToString();
                    }
                }

                var newLine = string.Join(delimiter, value);
                csv.AppendLine(newLine);
            }

            // Ecriture finale dans le CSV
            System.IO.File.WriteAllText(filePath, csv.ToString());
        }

        #endregion


        #region Helpers

        protected override void setTitle()
        {
            string res = "DensiteRecouvrement_GR";
            foreach (Groupe groupe in mesGroupes)
                res += (!groupe.Equals(mesGroupes.Last())) ? groupe.Identifiant + "-" : groupe.Identifiant + "_U";
            foreach (int pos in users)
                res += (!pos.Equals(users.Last())) ? pos + "-" : pos + "_ORD";
            foreach (OrdreGroupe ordre in ordres)
                res += (!ordre.Equals(ordres.Last())) ? ordre.ToString() + "-" : ordre.ToString() + "_MOD";
            if (modS && modPA)
                res += "S-PA";
            else
                if (modS) res += "S";
                else res += "PA";

            Title = res;
        }

        #endregion

    }
}
