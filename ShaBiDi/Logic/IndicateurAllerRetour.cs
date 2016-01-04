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
    /// IndicateurAllerRetour - Classe de l'indicateur concernant le nombre d'aller retour bandeau / image
    /// </summary>
    public class IndicateurAllerRetour : Indicateur
    {

        #region Attributs et propriétés

        /// <summary>
        /// Dictionnaire permettant de récolter les données de chaque image
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
        private IndicateurAllerRetour indicCompare;
        public IndicateurAllerRetour IndicCompare
        {
            get { return indicCompare; }
            set { indicCompare = value; }
        }

        #endregion


        #region Constructeurs

        /// <summary>
        /// Constructeur de la classe IndicateurAllerRetour
        /// </summary>
        /// <param name="mesUsers">Positions sélectionnées pour l'indicateur</param>
        /// <param name="ordres">Ordres sélectionnés pour l'indicateur</param>
        /// <param name="pa">Modalité PA ou non</param>
        /// <param name="s">Modalité S ou non</param>
        /// <param name="groupes">Groupes sélectionnés pour l'indicateur</param>
        public IndicateurAllerRetour(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
            : base(mesUsers, ordres, pa, s, groupes)
        {
            Data = new Dictionary<ImageExp, double>();
            DataComparaison = new List<Dictionary<ImageExp, double>>();
        }

        #endregion


        #region Méthodes de gestion des threads

        /// <summary>
        /// Méthode de thread qui calcule pour chaque image une liste de nombre d'aller-retour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<ImageExp, List<Observation>> dictionary = e.Argument as Dictionary<ImageExp, List<Observation>>;

            // On crée la liste où on va stocker tous les taux obtenus par image. Elle sera enrichie par chaque groupe
            Dictionary<ImageExp, List<double>> dictionaryAllerRetour = new Dictionary<ImageExp, List<double>>();

            int counterTauxImage = 0;
            foreach (ImageExp i in dictionary.Keys)
            {
                counterTauxImage++;
                (sender as BackgroundWorker).ReportProgress(counterTauxImage, dictionary.Count());
                // La méthode range les taux dans le dictionnaire
                calculeAllerRetour(i, dictionaryAllerRetour, dictionary[i]);
            }

            e.Result = dictionaryAllerRetour;
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
            wait.Close();   // fermeture de la fenêtre de chargement

            Dictionary<ImageExp, List<double>> dictionaryAllerRetour = e.Result as Dictionary<ImageExp, List<double>>;

            // Les taux de tous les groupes sont mentionnés dans dictionaryTaux, ne reste plus qu'à faire la moyenne
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<ImageExp, double> allerRetourParImage = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dictionaryAllerRetour.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                allerRetourParImage.Add(i, AppData.calculeMoyenne(dictionaryAllerRetour[i]));
            }

            Data = allerRetourParImage;

            // On génère la fenêtre de taux de recouvrement 
            AllerRetourUC allerRetour = new AllerRetourUC();
            
        }

        #endregion


        #region Algorithmes de calcul et de comparaison

        /// <summary>
        /// Méthode qui permet de calculer le nombre d'aller-retour dans une image
        /// </summary>
        /// <param name="i">Image de l'expérience dont on veut calculer le nombre d'aller-retour</param>
        /// <param name="dico">Dictionnaire dans lequel on veut stocker l'ensemble des nombre d'aller-retour pour l'image</param>
        /// <param name="listeObs">Liste des observations</param>
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

        /// <summary>
        /// Méthode qui permet d'obtenir la moyenne des aller-retour pour chaque image
        /// </summary>
        public void determineAllerRetour()
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
            foreach (Groupe g in mesGroupes)
            {
                sujParGr.Clear();
                sujParGr = trouveSujets(g);

                // On nettoie la liste des observations avant dans la remplir, on va y mettre toutes les observations des users concernés du groupe
                obsParGr.Clear();

                // Sélection des bonnes observations. Elles peuvent être mises en vrac puisque chaque observation connait son image
                foreach (Sujet s in sujParGr)
                {
                    if (modPA)
                    {
                        obsParGr = obsParGr.Concat(s.ObservationsPA).ToList();
                    }
                    if (modS)
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
                // Lancement du BackgroundWorker

                wait.pbProgression.IsIndeterminate = true;
                wait.Show();

                bw.RunWorkerAsync(dictionary);

            }
        }

        /// <summary>
        /// Méthode qui permet de comparer les taux de deux indicateurs
        /// </summary>
        /// <param name="type">Type de comparaison</param>
        public void compareAllerRetour(TypeComp type)
        {
            // Création du nouvel indicateur de comparaison
            IndicateurAllerRetour resultat = new IndicateurAllerRetour(fusionUsers(this, IndicCompare), fusionOrdres(this, IndicCompare), fusionPA(this, IndicCompare), fusionS(this, IndicCompare), fusionGroupes(this, IndicCompare));

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
            foreach (ImageExp img in IndicCompare.Data.Keys)
                {
                    if (dico.ContainsKey(img))
                    {
                        dico[img].Add(IndicCompare.Data[img]);
                    }
                    else
                    {
                        dico.Add(img, new List<double>());
                        dico[img].Add(IndicCompare.Data[img]);

                    }
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

        /// <summary>
        /// Méthode d'extraction des données résultant de l'indicateur dans un fichier CSV
        /// </summary>
        /// <param name="currentDir">Répertoire dans lequel on génère le CSV</param>
        public void extractOutputAllerRetourToCSV(string currentDir)
        {
            // On prépare les données
            StringBuilder csv = new StringBuilder();
            string delimiter = ";";
            string filePath = currentDir + "/" + Title + "_OUTPUT.csv";
            var mesures = Data.Keys.OrderBy(o => o.Numero).ToList();

            // On ajoute les lignes du CSV
            csv.AppendLine(string.Join(delimiter, "Image", "Nb A/R bandeau/image"));

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
            string res = "AllerRetour_GR";
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
