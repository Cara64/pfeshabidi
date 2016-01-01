using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Threading;
using System.ComponentModel;
using ShaBiDi.Logic;

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour ImportWindow.xaml
    /// ImportWindow - Fenêtre pour importer les fichiers dans les classes métiers
    /// </summary>
    public partial class ImportWindow : Window
    {

        #region Attributs

        /// <summary>
        /// Thread pour l'importation
        /// </summary>
        private BackgroundWorker importWorker; 
        /// <summary>
        /// Liste de chaînes de caractères contenant les fichiers à ajouter
        /// </summary>
        private List<String> addedFiles;        // fichiers ajoutés (non importés)

        #endregion


        #region Constructeurs

        /// <summary>
        /// Constructeur de la classe ImportWindow
        /// </summary>
        public ImportWindow()
        {
            InitializeComponent();

            addedFiles = new List<String>();
            importWorker = new BackgroundWorker();

            SetUpImportWorker();
        }

        #endregion


        #region Méthodes de gestion du thread

        /// <summary>
        /// Méthode pour paramétrer le thread d'import
        /// </summary>
        private void SetUpImportWorker()
        {
            importWorker.WorkerReportsProgress = true;
            importWorker.WorkerSupportsCancellation = true;

            importWorker.DoWork += importWorker_DoWork;
            importWorker.ProgressChanged += importWorker_ProgressChanged;
            importWorker.RunWorkerCompleted += importWorker_RunWorkerCompleted;
        }

        /// <summary>
        /// Méthode d'exécution du thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            remplirClasses(worker, e);
            
        }

        /// <summary>
        /// Méthode qui modifie le pourcentage affichée dans la fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int nbImported = e.ProgressPercentage;
            int nbToImport = addedFiles.Count();

            ListBoxItem selectedListBoxItem = lbImportedFiles.ItemContainerGenerator.ContainerFromIndex(nbImported-1) as ListBoxItem;
            selectedListBoxItem.Background = Brushes.LightGreen ;
            
            lblInfoImport.Content = nbImported + " / " + nbToImport + ((nbImported == 1) ? " fichier importé" : " fichiers importés");
        }
        
        /// <summary>
        /// Méthode déclenchée lorsque le thread se termine (succès, erreur ou annulation)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pbImportFiles.IsIndeterminate = false;

            if (e.Error != null)
            {
                MessageBox.Show("Error : " + e.Error.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (e.Cancelled == true)
            {
                MessageBox.Show("L'import a été interrompu", "Annulation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                MessageBox.Show("Importation terminée avec succès", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion


        #region Méthodes événements

        /// <summary>
        /// Méthode déclenchée lors du clic sur le bouton d'ajout de fichiers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            ajouterFichiers();

            int nbToImport = addedFiles.Count();
            lblInfoImport.Content = nbToImport + ((nbToImport == 1) ? " fichier à importer" : " fichiers à importer");

            if (addedFiles.Count() > 0)
            {
                btnImportFiles.IsEnabled = true;
            }


        }

        /// <summary>
        /// Méthode déclenchée lors du clic sur le bouton d'importation ou d'annulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportFiles_Click(object sender, RoutedEventArgs e)
        {
        
            // Si l'import n'est pas en cours : on affiche le bouton "Importer"
            if (importWorker.IsBusy != true)
            {
                pbImportFiles.IsIndeterminate = true;
                btnAddFiles.IsEnabled = false;
                btnImportFiles.Content = "Annuler";
                importWorker.RunWorkerAsync();
            }
            else if (importWorker.WorkerSupportsCancellation == true && importWorker.IsBusy == true)
            {
                pbImportFiles.IsIndeterminate = false;
                btnAddFiles.IsEnabled = true;
                btnImportFiles.Content = "Importer";
                importWorker.CancelAsync();
            }           
        }

        /// <summary>
        /// Méthode déclenchée lors de la fermeture de fenêtre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        #endregion


        #region Helpers et méthodes internes

        /// <summary>
        /// Méthode pour convertir un chaîne de caractère en une modalité (PA ou S)
        /// </summary>
        /// <param name="mod">Chaîne de caractère à convertir (PA ou S)</param>
        /// <returns>Une valeur de type Modalité (PA ou S)</returns>
        private Modalite convert(string mod)
        {
            return (mod.Equals("PA")) ? Modalite.PA : Modalite.S;
        }

        /// <summary>
        /// Méthode d'importation des données des fichiers dans les classes objets*
        /// TODO: Déplacer la méthode dans la classe AppData et la découper en sous-méthodes
        /// </summary>
        /// <param name="bw">Thread BackgroundWorker dans lequel s'effectue l'opération</param>
        /// <param name="e">Evénements de gestion du thread</param>
        private void remplirClasses(BackgroundWorker bw, DoWorkEventArgs e)
        {
            bool refFaite = false;
            double tpsEcoule = 0.0;             // colonne 00 de l'entête
            
            Groupe groupe;                      // colonne 01 de l'entête
            Modalite modalite;                  // colonne 02 de l'entête
            
            int image = 0;                      // colonne 03 de l'entête  
            double[] x = new double[3];         // colonnes 10, 18 et 26 de l'entête
            double[] y = new double[3];         // colonnes 11, 19 et 27 de l'entête

            Sujet[] users = new Sujet[3];

            int counterFiles = 0;   // compteur pour la barre de progressiON

            // On crée les images leur nombre est fixe
            // On pourra mettre cette valeur dans une variable
            AppData.GroupesExp.Clear();
            AppData.ImagesExp.Clear();

            string[] img = System.IO.Directory.GetFiles(@"..\..\Resources\ImagesExp");

            for (int i = 1; i <= img.Length; i++)
            {
                AppData.ImagesExp.Add(new ShaBiDi.Logic.ImageExp(i));
                AppData.ImagesExp[i - 1].Acces = img[i - 1];
            }
            
            for (int i = 0; i < users.Length; i++) users[i] = new Sujet(i + 1);

            foreach (string file in addedFiles)
            {
                // On "nettoie" tous les sujets
                foreach (Sujet user in users)
                {
                    user.ObservationsPA.Clear();
                    user.ObservationsS.Clear();
                }

                string[] lignes = System.IO.File.ReadAllLines(file);

                // On va remplir le tableau avec chaque ligne
                string[,] donneesGroupe = new string[lignes.Length, 29];

                for (int i = 0; i < lignes.Length; i++)
                {
                    string[] ligneDecoupee = lignes[i].Split(';');

                    for (int j = 0; j < ligneDecoupee.Length; j++) donneesGroupe[i, j] = ligneDecoupee[j];
                }

                int l = 0;
                while (l < lignes.Length)
                {
                    modalite = convert(donneesGroupe[l, 2]);
                    image = int.Parse(donneesGroupe[l, 3]);

                    // On initialise la nouvelle observation chez chaque sujet du groupe, puisqu'il y en a une par image
                    foreach (Sujet user in users) user.AddObservation(new Observation(AppData.ImagesExp[image - 1]), modalite);

                    // On remplit la même liste d'observations tant qu'on ne change ps d'image
                    // Donc on commence par vérifier le numéro de l'image (on convertit la donnée du tableau)
                    while ((l < lignes.Length) && (int.Parse(donneesGroupe[l, 3]) == image))
                    {
                        tpsEcoule = double.Parse(donneesGroupe[l, 0]);

                        image = int.Parse(donneesGroupe[l, 3]);
                        int[] colEntetes = { 10, 11 };
                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] = double.Parse(donneesGroupe[l, colEntetes[0]]);
                            y[i] = double.Parse(donneesGroupe[l, colEntetes[1]]);

                            if (!refFaite)
                            {
                                PointAttention PA1 = new PointAttention(new Vecteur2(x[i], y[i]), tpsEcoule);
                                PA1.pixelsEllipse(AppData.SCREEN_DISTANCE, AppData.LOGICAL_HEIGHT, AppData.LOGICAL_WIDTH, AppData.PHYSICAL_HEIGHT, AppData.PHYSICAL_WIDTH, y[i] = double.Parse(donneesGroupe[l, 6]));
                                refFaite = true;
                            }

                            for (int j = 0; j < colEntetes.Length; j++) colEntetes[j] += 8;
                            users[i].AddPA(image, modalite, x[i], y[i], tpsEcoule);
                        }
                        l++;
                    } // Fin boucle while, changement d'image
                } // Fin boucle for, observations remplies

                // Création du groupe associé
                int numGr = int.Parse(donneesGroupe[0, 1]);
                groupe = new Groupe(numGr, (donneesGroupe[0, 2].Equals("PA")) ? OrdreGroupe.PAS : OrdreGroupe.SPA);

                foreach (Sujet user in users) groupe.AddSujet(user);

                AppData.GroupesExp.Add(groupe);

                if (bw.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }



                counterFiles++;
                bw.ReportProgress(counterFiles);

            } // Fin foreach, changement de fichier (donc de groupe)
            
      
        }

        /// <summary>
        /// Méthode d'ajout des fichiers dans la liste
        /// </summary>
        private void ajouterFichiers()
        {
            addedFiles.Clear();
            lbImportedFiles.ItemsSource = null;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Document texte|*.txt|Fichier CSV|*.csv";
            
            if (ofd.ShowDialog() == true)
            {  
                foreach(String file in ofd.FileNames)
                {
                    if (!addedFiles.Contains(file)) addedFiles.Add(file);
                }
            }

            lbImportedFiles.ItemsSource = addedFiles;
        }

        #endregion

    }
}
