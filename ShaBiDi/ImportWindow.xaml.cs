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

namespace ShaBiDi
{
    /// <summary>
    /// Logique d'interaction pour ImportWindow.xaml
    /// </summary>
    /// TODO : Implémenter affichage des infos fichiers (sélection, num groupe et ordre)
    public partial class ImportWindow : Window
    {
        public static List<String> _importFiles;
        
        // Liste des images de l'expérience
        public static List<Image> lesImages;
        // Liste des groupes de l'expérience
        public static List<Groupe> lesGroupes;

        public ImportWindow()
        {
            InitializeComponent();

            _importFiles = new List<String>();
            lesGroupes = new List<Groupe>();
            lesImages = new List<Image>();
        }

        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {

            lbImportedFiles.ItemsSource = null;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Document texte|*.txt|Fichier CSV|*.csv";
            
            if (ofd.ShowDialog() == true)
            {         
                foreach(String file in ofd.FileNames)
                {
                    if (!_importFiles.Contains(file)) _importFiles.Add(file);
                }
            }

            lbImportedFiles.ItemsSource = _importFiles;            
      }

        private void btnDeleteFiles_Click(object sender, RoutedEventArgs e)
        {
            // TODO : Implémenter suppression des fichiers
        }

        private Modalite convert(string mod)
        {
            if(mod == "PA")
            {
                return Modalite.PA;
            }
            else 
            {
                return Modalite.S;
            }
        }

        // On remplit les classes à partir des fichiers de données
        private void remplirClasses()
        {

            /* Récapitulatif entête :
             * 0 : temps écoulé
             * 1 : nO groupe
             * 2 : PA ou S
             * 3 : nO image
             * 10 : PAx user1
             * 11 : PAy user1
             * 18 : PAx user2
             * 19 : PAy user2
             * 26 : PAx user3
             * 27 : PAy user3*/

            // On crée les images leur nombre est fixe
            // On pourra mettre cette valeur dans une variable
            lesGroupes.Clear();
            lesImages.Clear();
            for(int i = 1; i<=30;i++)
            {
                lesImages.Add(new Image(i));
            }

            // Il faut penser à "nettoyer" les sujets à chaque début de groupe
            Groupe groupe;
            Sujet user1 = new Sujet(1);
            Sujet user2 = new Sujet(2);
            Sujet user3 = new Sujet(3);

            // On nomme les variables nécessaires pour instancier les objets
                    double tpsEcoule;
                    Modalite modalite;
                    int image;
                    double x1;
                    double y1;
                    double x2;
                    double y2;
                    double x3;
                    double y3;

            foreach (string file in _importFiles)
            {
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

                // On ralise une boucle while pour faire défiler les lignes
                // l est le nuéro de la ligne
                int l = 0;

                while (l < lignes.Length)
                {
                    modalite = convert(donneesGroupe[l, 2]);
                    image = int.Parse(donneesGroupe[l,3]);

                    // On initialise la nouvelle observation chez chaque sujet du groupe, puisqu'il y en a une par image
                    user1.AddObservation(new Observation(lesImages[image-1]), modalite);
                    user2.AddObservation(new Observation(lesImages[image-1]), modalite);
                    user3.AddObservation(new Observation(lesImages[image-1]), modalite);

                    // On remplit la même liste d'observations tant qu'on ne change ps d'image
                    // Donc on commence par vérifier le numéro de l'image (on convertit la donnée du tableau)
                    while ((l < lignes.Length) && (int.Parse(donneesGroupe[l, 3]) == image))
                    {
                        tpsEcoule = double.Parse(donneesGroupe[l, 0]);
                        image = int.Parse(donneesGroupe[l, 3]);
                        x1 = double.Parse(donneesGroupe[l, 10]);
                        y1 = double.Parse(donneesGroupe[l, 11]);
                        x2 = double.Parse(donneesGroupe[l, 18]);
                        y2 = double.Parse(donneesGroupe[l, 19]);
                        x3 = double.Parse(donneesGroupe[l, 26]);
                        y3 = double.Parse(donneesGroupe[l, 27]);

                        user1.AddPA(image, modalite, x1, y1, tpsEcoule);
                        user2.AddPA(image, modalite, x2, y2, tpsEcoule);
                        user3.AddPA(image, modalite, x3, y3, tpsEcoule);

                        l++;
                    } // Fin boucle while, changement d'image

                }// Fin boucle for, observations remplies

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
                lesGroupes.Add(groupe);
                
            } // Fin foreach, changement de fichier (donc de groupe)
        }


    }
}
