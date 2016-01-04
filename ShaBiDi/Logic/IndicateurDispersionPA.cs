using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaBiDi.Views;
using System.Threading;
using System.ComponentModel;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// IndicateurDispersionPA - Classe de l'indicateur concernant la dispersion du PA par image
    /// </summary>
    public class IndicateurDispersionPA : Indicateur
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
        /// Indicateur avec lequel on effectue la comparaison
        /// </summary>
        private IndicateurDispersionPA indicCompare;
        public IndicateurDispersionPA IndicCompare
        {
            get { return indicCompare; }
            set { indicCompare = value; }
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

        #endregion


        #region Constructeurs

        /// <summary>
        /// Constructeur de la classe IndicateurDispersionPA
        /// </summary>
        /// <param name="mesUsers">Positions sélectionnées pour l'indicateur</param>
        /// <param name="ordres">Ordres sélectionnés pour l'indicateur</param>
        /// <param name="pa">Modalité PA ou non</param>
        /// <param name="s">Modalité S ou non</param>
        /// <param name="groupes">Groupes sélectionnés pour l'indicateur</param>
        public IndicateurDispersionPA(List<int> mesUsers, List<OrdreGroupe> ordres, bool pa, bool s, List<Groupe> groupes)
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
            Dictionary<ImageExp, List<double>> dictionaryDispersion = new Dictionary<ImageExp, List<double>>();

            int counterTauxImage = 0;
            foreach (ImageExp i in dictionary.Keys)
            {
                counterTauxImage++;
                (sender as BackgroundWorker).ReportProgress(counterTauxImage, dictionary.Count());
                // La méthode range les taux dans le dictionnaire
                calculeDispersion(i, dictionaryDispersion, dictionary[i]);
            }

            e.Result = dictionaryDispersion;
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

            Dictionary<ImageExp, List<double>> dictionaryDispersion = e.Result as Dictionary<ImageExp, List<double>>;

            // Les taux de tous les groupes sont mentionnés dans dictionaryTaux, ne reste plus qu'à faire la moyenne
            // On crée la liste des taux par image sous forme de dictionnaire
            Dictionary<ImageExp, double> dispersionParImage = new Dictionary<ImageExp, double>();

            foreach (ImageExp i in dictionaryDispersion.Keys)
            {
                // Calcul de la moyenne de tous les taux de l'image
                dispersionParImage.Add(i, AppData.calculeMoyenne(dictionaryDispersion[i]));
            }

            Data = dispersionParImage;

            // On génère la fenêtre de taux de recouvrement 
            DispersionPAUC dispersionPA = new DispersionPAUC();
     
        }

        #endregion


        #region Algorithmes de calcul et de comparaison

        /// <summary>
        /// Méthode qui permet de calculer la dispersion des PA à chaque instant t obtenue
        /// </summary>
        /// <param name="i">Image de l'expérience dont on veut calculer la dispersion des PA</param>
        /// <param name="dico">Dictionnaire dans lequel on veut stocker l'ensemble des taux de dispersion des PA par image</param>
        /// <param name="listeObs">Liste des observations</param>
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

        /// <summary>
        /// Méthode pour obtenir la moyenne des taux de dispersion
        /// </summary>
        public void determineDispersion()
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

                // Maintenant, toutes les observations sont triées par image, on va alors déterminer la dispersion moyenne par image

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
        /// <param name="i">Indicateur avec lequel comparé</param>
        /// <returns>Un dictionnaire contenant le résultat de la comparaison</returns>
        public void compareDispersion(TypeComp type)
        {
            // Création du nouvel indicateur de comparaison
            IndicateurDispersionPA resultat = new IndicateurDispersionPA(fusionUsers(this, IndicCompare), fusionOrdres(this, IndicCompare), fusionPA(this, IndicCompare), fusionS(this, IndicCompare), fusionGroupes(this, IndicCompare));

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
                    dicoCompare.Add(i, (dico[i][0] + dico[i][1]) / 2);
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
        public void extractOutputDispersionToCSV(string currentDir)
        {
            // On prépare les données
            StringBuilder csv = new StringBuilder();
            string delimiter = ";";
            string filePath = currentDir + "/" + Title + "_OUTPUT.csv";
            var mesures = Data.Keys.OrderBy(o => o.Numero).ToList();

            // On ajoute les lignes du CSV
            csv.AppendLine(string.Join(delimiter, "Image", "Taux de dispersion du point d'attention"));

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
            string res = "DispersionPA_GR";
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
