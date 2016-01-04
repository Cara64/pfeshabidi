using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using ShaBiDi.Views;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// IndicateurTauxRecouvrement - Classe de l'indicateur concernant le taux de recouvrement
    /// </summary>
    public class IndicateurTauxRecouvrement : Indicateur
    {

        #region Attributs et propriétés

        /// <summary>
        /// Dictionnaire permettant de récolter les données pour chaque image
        /// </summary>
        private Dictionary<ImageExp, double> data;
        public Dictionary<ImageExp, double> Data
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
        private List<Dictionary<ImageExp, double>> dataComparaison;
        public List<Dictionary<ImageExp, double>> DataComparaison
        {
            get { return dataComparaison; }
            set { dataComparaison = value; }
        }

        /// <summary>
        /// Indicateur avec lequel on effectue la comparaison
        /// </summary>
        private IndicateurTauxRecouvrement indicCompare;
        public IndicateurTauxRecouvrement IndicCompare
        {
            get { return indicCompare; }
            set { indicCompare = value; }
        }

        #endregion


        #region Constructeurs

        /// <summary>
        /// Constructeur de la classe IndicateurTauxRecouvrement
        /// </summary>
        /// <param name="mesUsers">Positions sélectionnées pour l'indicateur</param>
        /// <param name="ordres">Ordres sélectionnés pour l'indicateur</param>
        /// <param name="pa">Modalité PA ou non</param>
        /// <param name="s">Modalité S ou non</param>
        /// <param name="groupes">Groupes sélectionnés pour l'indicateur</param>
        public IndicateurTauxRecouvrement(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            Data = new Dictionary<ImageExp, double>();
            DataComparaison = new List<Dictionary<ImageExp, double>>();
        }

        #endregion


        #region Méthodes de gestion des threads

        /// <summary>
        /// Méthode de thread qui calcule pour chaque image une liste de taux
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<ImageExp, List<Observation>> dictionary = e.Argument as Dictionary<ImageExp, List<Observation>>;
            Dictionary<ImageExp, List<double>> dictionaryTaux = new Dictionary<ImageExp, List<double>>();
            
            int counterTauxImage = 0;
            foreach (ImageExp i in dictionary.Keys)
            {
                counterTauxImage++;
                (sender as BackgroundWorker).ReportProgress(counterTauxImage, dictionary.Count());
                // La méthode range les taux dans le dictionnaire
                calculeTaux(i, dictionaryTaux, dictionary[i]);        
            }

            e.Result = dictionaryTaux;
        }

        /// <summary>
        /// Méthode qui transmet les infos de chargement à la fenêtre de chargement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int total = (int) e.UserState;
            wait.lblProgression.Content = "Calcul de l'image " + e.ProgressPercentage.ToString() + " sur " + total.ToString() ;
        }

        /// <summary>
        /// Méthode exécuté une fois le calcul en thread terminé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wait.Close(); // fermeture de la fenêtre d'attente

            Dictionary<ImageExp, List<double>> dictionaryTaux = e.Result as Dictionary<ImageExp, List<double>>;
            
            // Les taux de tous les groupes sont mentionnés dans dictionaryTaux, ne reste plus qu'à faire la moyenne
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<ImageExp, double> tauxParImage = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dictionaryTaux.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                tauxParImage.Add(i, AppData.calculeMoyenne(dictionaryTaux[i]));
            }
            
            Data = tauxParImage;

            // On génère la fenêtre de taux de recouvrement 
            TauxRecouvrementUC tr = new TauxRecouvrementUC();
            
        }

        #endregion


        #region Algorithmes de calcul, de comparaison et d'extraction

        /// <summary>
        /// Méthode qui permet de calculer le taux de recouvrement d'une image
        /// </summary>
        /// <param name="i">Image de l'expérience dont on veut calculer le taux de recouvrement</param>
        /// <param name="dico">Dictionnaire dans lequel on veut stocker l'ensemble des taux pour l'image</param>
        /// <param name="listeObs">Liste des observations</param>
        private void calculeTaux(ImageExp i, Dictionary<ImageExp, List<double>> dico, List<Observation> listeObs)
        {
            // On fait le choix d'une grille qui fait la taille de l'image
            bool[,] pixelsImage = new bool[ImageExp.DIM_IMAGE_ROW, ImageExp.DIM_IMAGE_COL];

            for (int j = 0; j < ImageExp.DIM_IMAGE_ROW; j++)
            {
                for (int k = 0; k < ImageExp.DIM_IMAGE_COL; k++)
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
                        enCours.contributionTauxEllipse(ref pixelsImage);

                        // On change le PA en cours
                        enCours = pa;
                    }

                    // Si on est face au dernier élément de la liste, il faut faire le traitement quand même
                    if (pa == o.PointsAttentions[o.PointsAttentions.Count - 1])
                    {
                        enCours.contributionTauxEllipse(ref pixelsImage);
                    }
                    
                }
            }

            // On trouve le nombre de pixels "true"
            int somme = 0;
            for (int j = 0; j < ImageExp.DIM_IMAGE_ROW; j++)
            {
                for (int k = 0; k < ImageExp.DIM_IMAGE_COL; k++)
                {
                    if (pixelsImage[j, k])
                    {
                        somme++;
                    }
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

        /// <summary>
        /// Méthode qui permet d'obtenir la moyenne des taux de recouvrement pour chaque image
        /// </summary>
        public void determineTaux()
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
                    if (modPA) obsParGr = obsParGr.Concat(s.ObservationsPA).ToList() ;
                    if (modS) obsParGr = obsParGr.Concat(s.ObservationsS).ToList();
                }

                // Ensuite on va trier les observations par images en les regroupant grâce à leur numéro
                // On nettoie le dictionnaire du groupe précédent
                dictionary.Clear();

                foreach (Observation o in obsParGr)
                {
                    if (!(dictionary.ContainsKey(o.Image)))
                    {
                        dictionary.Add(o.Image, new List<Observation>());
                    }
                    
                    dictionary[o.Image].Add(o);
  
                }

                // Maintenant, toutes les observations sont triées par image, on va alors déterminer le taux par image que l'on va mettre dans la liste des taux
                // On laisse la place au BackgroundWorker pour effectuer le calcul et afficher le résultat
                
                // Paramétrage et affichage de la fenêtre de chargement
                wait.pbProgression.IsIndeterminate = true; 
                wait.Show();

                bw.RunWorkerAsync(dictionary);
            }
        }

        /// <summary>
        /// Méthode qui permet de comparer les taux de deux indicateurs
        /// </summary>
        /// <param name="type">Type de comparaison</param>
        /// <param name="i">Indicateur avec lequel comparé</param>
        /// <returns>Un dictionnaire contenant le résultat de la comparaison</returns>
        public void compareTaux(TypeComp type)
        {
            // Création du nouvel indicateur de comparaison
            IndicateurTauxRecouvrement resultat = new IndicateurTauxRecouvrement(fusionUsers(this, IndicCompare), fusionOrdres(this, IndicCompare), fusionPA(this, IndicCompare), fusionS(this, IndicCompare), fusionGroupes(this, IndicCompare));

            // On cherche à comparer les deux dictionnaires
            Dictionary<ImageExp, List<double>> dico = new Dictionary<ImageExp,List<double>>();

            // On remplit le dictionnaire avec les données du premier indicateur
            foreach (ImageExp img in this.Data.Keys)
            {
                if (!(dico.ContainsKey(img)))
                {
                    dico.Add(img, new List<double>());
                }
                   
                dico[img].Add(this.Data[img]);
            }

            // On remplit le dictionnaire avec les données du second indicateur
            foreach (ImageExp img in IndicCompare.Data.Keys)
            {
                if (!(dico.ContainsKey(img)))
                {
                    dico.Add(img, new List<double>());
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
        private Dictionary<ImageExp, double> additionner(Dictionary<ImageExp, List<double>> dico)
        {
            Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dico.Keys)
            {
                // On fait la différence entre les deux éléments de la liste que l'on stocke dans une variable
                // Il faut prévoir le cas où il n'y a qu'une composante
                dicoCompare.Add(i, (dico[i].Count == 2) ? dico[i][0] + dico[i][1] : dico[i][0]);   
            }

            return dicoCompare;
        }

        /// <summary>
        /// Méthode de comparaison par soustraction
        /// </summary>
        /// <param name="dico">Dictionnaire de données de l'indicateur à comparé</param>
        /// <returns>Dictionnaire de données de comparaison par soustraction</returns>
        private Dictionary<ImageExp, double> soustraire(Dictionary<ImageExp, List<double>> dico)
        {
            Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dico.Keys)
            {
                // On fait la différence entre les deux éléments de la liste que l'on stocke dans une variable
                // Il faut prévoir le cas où il n'y a qu'une composante
                dicoCompare.Add(i, (dico[i].Count == 2) ? Math.Abs(dico[i][0] - dico[i][1]) : dico[i][0]);
            }

            return dicoCompare;
        }

        /// <summary>
        /// Méthode de comparaison par moyenne
        /// </summary>
        /// <param name="dico">Dictionnaire de données de l'indicateur à comparé</param>
        /// <returns>Dictionnaire de données de comparaison par moyenne</returns>
        private Dictionary<ImageExp, double> moyenner(Dictionary<ImageExp, List<double>> dico)
        {
            Dictionary<ImageExp, double> dicoCompare = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dico.Keys)
            {
                // On fait la différence entre les deux éléments de la liste que l'on stocke dans une variable
                // Il faut prévoir le cas où il n'y a qu'une composante
                dicoCompare.Add(i, (dico[i].Count == 2) ? (dico[i][0] + dico[i][1])/2 : dico[i][0]);
            }

            return dicoCompare;
        }

        /// <summary>
        /// Méthode d'extraction des données résultant de l'indicateur dans un fichier CSV
        /// </summary>
        /// <param name="currentDir">Répertoire dans lequel on génère le CSV</param>
        public void extractOutputTauxToCSV(string currentDir)
        {
            // On prépare les données
            StringBuilder csv = new StringBuilder();
            string delimiter = ";";
            string filePath = currentDir + "/" + Title + "_OUTPUT.csv";
            var mesures = Data.Keys.OrderBy(o => o.Numero).ToList();

            // On ajoute les lignes du CSV
            csv.AppendLine(string.Join(delimiter, "Image", "Taux de recouvrement"));

            foreach (var key in mesures)
            {
                var id = key.Numero.ToString();
                var value = Data[key].ToString();
                var newLine = string.Join(delimiter, id, value);
                csv.AppendLine(newLine);
            }

            // Ecriture finale dans le CSV
            System.IO.File.WriteAllText(filePath, csv.ToString());
        }

        #endregion


        #region Divers

        protected override void setTitle()
        {
            string res = "TauxRecouvrement_GR";
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
