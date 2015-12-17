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
    /// </summary>
    public partial class ImportWindow : Window
    {
        public static List<String> ImportedFiles;     // fichiers importés
        public static List<ShaBiDi.Logic.Image> ImagesExp;          // images de l'expérience
        public static List<Groupe> GroupesExp;        // totalité des groupes ayant passé l'expérience

        private string selectedFiles;                 // fichiers sélectionnés
        public string SelectedFiles
        {
            get { return selectedFiles; }
            set { selectedFiles = value; }
        }

        public ImportWindow()
        {
            InitializeComponent();

            ImportedFiles = new List<String>();
            GroupesExp = new List<Groupe>();
            ImagesExp = new List<ShaBiDi.Logic.Image>();
            SelectedFiles = "";
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            remplirClasses(sender);
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbImportFiles.Value = e.ProgressPercentage;
        }

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            ajouterFichiers();
            pbImportFiles.Maximum = ImportedFiles.Count();
            
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
        }


        private void btnDeleteFiles_Click(object sender, RoutedEventArgs e)
        {
            // TODO : Implémenter la suppression
            supprimerFichier();
            // viderClasses();
        }

        private Modalite convert(string mod)
        {
            return (mod.Equals("PA")) ? Modalite.PA : Modalite.S;
        }

        // On remplit les classes à partir des fichiers de données
        private void remplirClasses(object sender)
        {
            Console.WriteLine("Initialisation des variables");
            double tpsEcoule = 0.0;             // colonne 00 de l'entête
            double tpsPrec = 0.0;
            double tpsSuiv = 0.0 ;
            
            Groupe groupe;                      // colonne 01 de l'entête
            Modalite modalite;                  // colonne 02 de l'entête
            
            int image = 0;                      // colonne 03 de l'entête  
            double[] x = new double[3];         // colonnes 10, 18 et 26 de l'entête
            double[] y = new double[3];         // colonnes 11, 19 et 27 de l'entête
            double[] z = new double[3];         // colonnes 6, 14 et 22 de l'entête

            Sujet[] users = new Sujet[3];

            int counterFiles = 0;   // compteur pour la barre de progressiON

            // On crée les images leur nombre est fixe
            // On pourra mettre cette valeur dans une variable
            GroupesExp.Clear();
            ImagesExp.Clear();

            for(int i = 1; i<=30;i++) ImagesExp.Add(new ShaBiDi.Logic.Image(i));
            for (int i = 0; i < users.Length; i++) users[i] = new Sujet(i + 1);
           
            foreach (string file in ImportedFiles)
            {
                Console.WriteLine("Remplissage nouveau fichier");
                // On "nettoie" tous les sujets
                foreach (Sujet user in users)
                {
                    Console.WriteLine("Nettoyage sujets");
                    user.ObservationsPA.Clear();
                    user.ObservationsS.Clear();
                }

                string[] lignes = System.IO.File.ReadAllLines(file);
                
                // On va remplir le tableau avec chaque ligne
                string[,] donneesGroupe = new string[lignes.Length, 29];

                for (int i = 0; i < lignes.Length; i++) 
                {
                    string[] ligneDecoupee = lignes[i].Split(';');

                    for (int j = 0; j < ligneDecoupee.Length; j++)
                    {
                        donneesGroupe[i, j] = ligneDecoupee[j];
                    }
                }

                int l = 0;
                while (l < lignes.Length)
                {
                    modalite = convert(donneesGroupe[l, 2]);
                    image = int.Parse(donneesGroupe[l, 3]);

                    // On initialise la nouvelle observation chez chaque sujet du groupe, puisqu'il y en a une par image
                    foreach (Sujet user in users) user.AddObservation(new Observation(ImagesExp[image - 1]), modalite);

                    // On remplit la même liste d'observations tant qu'on ne change ps d'image
                    // Donc on commence par vérifier le numéro de l'image (on convertit la donnée du tableau)
                    while ((l < lignes.Length) && (int.Parse(donneesGroupe[l, 3]) == image))
                    {
                        tpsEcoule = double.Parse(donneesGroupe[l, 0]);
                        tpsPrec = (l != 0) ? double.Parse(donneesGroupe[l - 1, 0]) : 0;
                        tpsSuiv = (l != lignes.Length - 1) ? double.Parse(donneesGroupe[l + 1, 0]) : double.Parse(donneesGroupe[l, 0]);

                        image = int.Parse(donneesGroupe[l, 3]);
                        int[] colEntetes = { 10, 11, 6 };
                        for (int i = 0; i < x.Length; i++)
                        {
                            x[i] = double.Parse(donneesGroupe[l, colEntetes[0]]);
                            y[i] = double.Parse(donneesGroupe[l, colEntetes[1]]);
                            z[i] = double.Parse(donneesGroupe[l, colEntetes[2]]);
                            for (int j = 0; j < colEntetes.Length; j++) colEntetes[j] += 8;
                            users[i].AddPA(image, modalite, x[i], y[i], z[i], tpsEcoule, tpsPrec, tpsSuiv);    
                        }
                        l++;
                    } // Fin boucle while, changement d'image
                } // Fin boucle for, observations remplies

                // Création du groupe associé
                int numGr = int.Parse(donneesGroupe[0, 1]);
                groupe = new Groupe(numGr, (donneesGroupe[0, 2].Equals("PA")) ? OrdreGroupe.PAS : OrdreGroupe.SPA);

                foreach (Sujet user in users) groupe.AddSujet(user);

                GroupesExp.Add(groupe);
                
                counterFiles++;
                Console.WriteLine("Counter files = " + counterFiles);
                (sender as BackgroundWorker).ReportProgress(counterFiles);
                
            } // Fin foreach, changement de fichier (donc de groupe)

            MessageBox.Show("Importation réussie", "Fin de l'importation", MessageBoxButton.OK, MessageBoxImage.Information);
        
        }

        private void lbImportedFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO : Implémenter affichage infos fichiers
        }


        private void ajouterFichiers()
        {
            lbImportedFiles.ItemsSource = null;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Document texte|*.txt|Fichier CSV|*.csv";
            
            if (ofd.ShowDialog() == true)
            {  
                foreach(String file in ofd.FileNames)
                {
                    if (!ImportedFiles.Contains(file)) ImportedFiles.Add(file);
                }
            }

            lbImportedFiles.ItemsSource = ImportedFiles;
        }

        private void supprimerFichier()
        {
            lbImportedFiles.ItemsSource = null;
            Console.WriteLine("Suppression fichiers");
            ImportedFiles.Remove(SelectedFiles);
            lbImportedFiles.ItemsSource = ImportedFiles;
        }

        private int getNumeroGroupeFichier(string nameFile)
        {
            String[] nameFileItems = nameFile.Split('-');
            return Convert.ToInt32(nameFileItems.Last());
        }

    }
}
