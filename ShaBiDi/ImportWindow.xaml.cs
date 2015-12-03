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

namespace ShaBiDi
{
    /// <summary>
    /// Logique d'interaction pour ImportWindow.xaml
    /// </summary>
    /// TODO : Implémenter affichage des infos fichiers (sélection, num groupe et ordre)
    public partial class ImportWindow : Window
    {
        public static List<String> ImportedFiles;     // fichiers importés
        public static List<Image> ImagesExp;          // images de l'expérience
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
            ImagesExp = new List<Image>();
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
            //ajouterFichiers();
            //remplirClasses();
        }


        private void btnDeleteFiles_Click(object sender, RoutedEventArgs e)
        {
            // TODO : Corriger bug de suppression
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
            // Variables nécessaires de l'entête
            double tpsEcoule;       // colonne 00 de l'entête
            double tpsPrec;
            double tpsSuiv;
            Groupe groupe;          // colonne 01 de l'entête
            Modalite modalite;      // colonne 02 de l'entête
            int image;              // colonne 03 de l'entête  
            double x1;              // colonne 10 de l'entête
            double y1;              // colonne 11 de l'entête
            double z1;              
            double x2;              // colonne 18 de l'entête
            double y2;              // colonne 19 de l'entête
            double z2;
            double x3;              // colonne 26 de l'entête
            double y3;              // colonne 27 de l'entête
            double z3;

            int counterFiles = 0;

            // On crée les images leur nombre est fixe
            // On pourra mettre cette valeur dans une variable
            GroupesExp.Clear();
            ImagesExp.Clear();
            for(int i = 1; i<=30;i++)
            {
                ImagesExp.Add(new Image(i));
            }

            // Il faut penser à "nettoyer" les sujets à chaque début de groupe
            Sujet user1 = new Sujet(1);
            Sujet user2 = new Sujet(2);
            Sujet user3 = new Sujet(3);



            foreach (string file in ImportedFiles)
            {

                Console.WriteLine("Importation en cours...");
                // On "nettoie" tous les sujets
                user1.ObservationsPA.Clear();
                user1.ObservationsS.Clear();
                user2.ObservationsPA.Clear();
                user2.ObservationsS.Clear();
                user3.ObservationsPA.Clear();
                user3.ObservationsS.Clear();


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

                // Toutes les données sont rangées dans le tableau. On peut alors créer les classes
                // On réalise une boucle while pour faire défiler les lignes
                // l est le nuéro de la ligne
                int l = 0;

                while (l < lignes.Length)
                {
                    modalite = convert(donneesGroupe[l, 2]);
                    image = int.Parse(donneesGroupe[l,3]);

                    // On initialise la nouvelle observation chez chaque sujet du groupe, puisqu'il y en a une par image
                    user1.AddObservation(new Observation(ImagesExp[image-1]), modalite);
                    user2.AddObservation(new Observation(ImagesExp[image-1]), modalite);
                    user3.AddObservation(new Observation(ImagesExp[image-1]), modalite);

                    // On remplit la même liste d'observations tant qu'on ne change ps d'image
                    // Donc on commence par vérifier le numéro de l'image (on convertit la donnée du tableau)
                    while ((l < lignes.Length) && (int.Parse(donneesGroupe[l, 3]) == image))
                    {
                        tpsEcoule = double.Parse(donneesGroupe[l, 0]);

                        // Recherche des temps suivant et précédent
                        tpsPrec = (l!=0) ? double.Parse(donneesGroupe[l - 1, 0]) : 0;
                        tpsSuiv = (l != lignes.Length-1) ? double.Parse(donneesGroupe[l + 1, 0]) : double.Parse(donneesGroupe[l, 0]) ;
                       
                        image = int.Parse(donneesGroupe[l, 3]);
                        x1 = double.Parse(donneesGroupe[l, 10]);
                        y1 = double.Parse(donneesGroupe[l, 11]);
                        z1 = double.Parse(donneesGroupe[l, 6]);
                        x2 = double.Parse(donneesGroupe[l, 18]);
                        y2 = double.Parse(donneesGroupe[l, 19]);
                        z2 = double.Parse(donneesGroupe[l, 14]);
                        x3 = double.Parse(donneesGroupe[l, 26]);
                        y3 = double.Parse(donneesGroupe[l, 27]);
                        z3 = double.Parse(donneesGroupe[l, 22]);

                        user1.AddPA(image, modalite, x1, y1, z1, tpsEcoule, tpsPrec, tpsSuiv);
                        user2.AddPA(image, modalite, x2, y2, z2, tpsEcoule, tpsPrec, tpsSuiv);
                        user3.AddPA(image, modalite, x3, y3, z3, tpsEcoule, tpsPrec, tpsSuiv);

                        l++;
                    } // Fin boucle while, changement d'image

                } // Fin boucle for, observations remplies

                // Création du groupe associé

                int numGr = int.Parse(donneesGroupe[0, 1]);
                if (donneesGroupe[0, 2] == "PA")
                {
                    groupe = new Groupe(numGr, OrdreGroupe.PAS);
                }
                else
                {
                    groupe = new Groupe(numGr, OrdreGroupe.SPA);
                }

                //Les sujets sont remplis, on peut alors les ajouter au groupe
                groupe.AddSujet(user1);
                groupe.AddSujet(user2);
                groupe.AddSujet(user3);

                // Puis on ajoute le groupe à la liste
                GroupesExp.Add(groupe);

                counterFiles++;
                (sender as BackgroundWorker).ReportProgress(counterFiles);
                
            } // Fin foreach, changement de fichier (donc de groupe)

            MessageBox.Show("Importation réussie", "Fin de l'importation", MessageBoxButton.OK, MessageBoxImage.Information);
        
        }

        private void lbImportedFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedFiles = ImportedFiles[lbImportedFiles.SelectedIndex];
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
