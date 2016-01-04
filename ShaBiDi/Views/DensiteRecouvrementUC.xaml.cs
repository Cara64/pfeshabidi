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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShaBiDi.Logic;

namespace ShaBiDi.Views
{
    /// <summary>
    /// Logique d'interaction pour DensiteRecouvrementUC.xaml
    /// DensiteRecouvrementUC - Contrôle utilisateur pour l'affichage de l'indicateur de densité de recouvrement
    /// </summary>
    public partial class DensiteRecouvrementUC : UserControl
    {

        #region Attributs et propriétés
        
        /// <summary>
        /// Données liées à l'indicateur
        /// </summary>
        private Dictionary<ImageExp, double[,]> data;
        public Dictionary<ImageExp, double[,]> Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Mode transparence ou couleur
        /// </summary>
        private string mode;
        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        /// <summary>
        /// Images de l'indicateur
        /// </summary> 
        private List<ImageExp> mesImages;
        public List<ImageExp> MesImages
        {
            get { return mesImages; }
            set { mesImages = value; }
        }
       
        /// <summary>
        /// Ordre de l'image
        /// </summary>
        private int index;                              
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Image en cours
        /// </summary>
        private ImageExp imageEnCours;
        public ImageExp ImageEnCours
        {
            get { return imageEnCours; }
            set { imageEnCours = value; }
        }    

        /// <summary>
        /// Bitmap de l'image
        /// </summary>
        private WriteableBitmap imageBmp;
        public WriteableBitmap ImageBmp
        {
            get { return imageBmp; }
            set { imageBmp = value; }
        }

        private ResultWindow res;

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur de la classe DensiteRecouvrementUC
        /// </summary>
        /// <param name="mode">Type de heatmap (transparence ou couleur)</param>
        public DensiteRecouvrementUC(string mode)
        {
            GetData();
            LoadData();

            InitializeComponent();
            Mode = mode;

            Index = 1;
            lblNumImage.Content = "Image " + ImageEnCours.Numero + " (" + Index + "e image sur " + MesImages.Count() + ")";
            lblTitleIndicateur.Content = this.ToString();

            SetUpModel();

            res = new ResultWindow();
            res.Title = this.ToString();
            res.Content = this;
            res.Show();
        }

        #endregion


        #region Méthodes de mise en place des données

        /// <summary>
        /// Récupération des données
        /// </summary>
        private void GetData()
        {
            Data = new Dictionary<ImageExp, double[,]>();
            Data = AppData.IndicateursDensiteRecouvrement.Last().Data;
        }

        /// <summary>
        /// Mise en place des données
        /// </summary>
        private void LoadData()
        {
            MesImages = new List<ImageExp>();
            MesImages = Data.Keys.ToList().OrderBy(o => o.Numero).ToList();
            ImageEnCours = MesImages[0];
        }

        /// <summary>
        /// Mise en place dee l'UI
        /// </summary>
        private void SetUpModel()
        {
            GenerateMask(Mode, ImageEnCours);
        }

        #endregion


        #region Algorithmes de création des heatmaps
        
        /// <summary>
        /// Génération du masque de heatmap
        /// </summary>
        /// <param name="mode">Mode transparence ou couleur</param>
        /// <param name="img">Image sur laquelle on créé le masque</param>
        private void GenerateMask(string mode,ShaBiDi.Logic.ImageExp img)
        {
            ImageBmp = new WriteableBitmap(new BitmapImage(new Uri(img.Acces, UriKind.RelativeOrAbsolute)));
            imgBackground.Source = ImageBmp;

            // Taille de l'image d'origine
            //int width = imageBitmap.PixelWidth;
            //int height = imageBitmap.PixelHeight;
            int width = 1680;
            int height = 900;

            // Creation du masque a partir du bitmap d'origine
            WriteableBitmap bitmap = new WriteableBitmap(ImageBmp);

            // Pixels Destination (r,g,b,a)
            byte[, ,] pixels = new byte[height, width, 4];

            // Pixels Source
            int strideSource = ImageBmp.PixelWidth * 4;
            int sizeSource = ImageBmp.PixelHeight * strideSource;
            byte[] pixelSource = new byte[sizeSource];
            bitmap.CopyPixels(pixelSource, strideSource, 0);

            double max = Data[AppData.ImagesExp[img.Numero-1]].Cast<double>().Max();

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // le numéro de l'image à indiquer est un en dessous
                    double tps = Data[AppData.ImagesExp[img.Numero-1]][row, col];
                    
                        // Pixel d'origine
                        int indexSource = row * strideSource + 4 * col;

                        double coef = tps / max;

                        // 0 : bleu, 1 : vert, 2 : rouge
                        // Cas où on demande le gris
                        if (mode == "gris")
                        {
                            pixels[row, col, 0] = (byte)(pixelSource[indexSource] * coef);
                            pixels[row, col, 1] = (byte)(pixelSource[indexSource + 1] * coef);
                            pixels[row, col, 2] = (byte)((pixelSource[indexSource + 2]) * coef);
                        }
                            // Cas où l'utilisateur demande de la couleur
                        else
                        {
                            int x = (int)Math.Floor((1000 / max) * (max-tps)); 

                            if (x >= 0 && x < 255)
                            {
                                pixels[row, col, 2] = (byte)(pixelSource[indexSource+2]/2 + (255)/2);
                                pixels[row, col, 1] = (byte)(pixelSource[indexSource+1]/2 + (x)/2);
                                pixels[row, col, 0] = (byte)( pixelSource[indexSource]/2 + (0)/2);
                            }
                            if (x >= 255 && x < 510)
                            {
                                pixels[row, col, 2] = (byte)(pixelSource[indexSource + 2] / 2 + (510 - x) / 2);
                                pixels[row, col, 1] = (byte)(pixelSource[indexSource + 1] / 2 + (255) / 2);
                                pixels[row, col, 0] = (byte)(pixelSource[indexSource] / 2 + (0) / 2);

                            }
                            if (x >= 510 && x < 765)
                            {
                                pixels[row, col, 2] = (byte)(pixelSource[indexSource + 2] / 2 + (0) / 2);
                                pixels[row, col, 1] = (byte)(pixelSource[indexSource + 1] / 2 + (255) / 2);
                                pixels[row, col, 0] = (byte)(pixelSource[indexSource] / 2 + (x-510) / 2);
                            }
                            if (x >= 765 && x < 1020)
                            {
                                pixels[row, col, 2] = (byte)(pixelSource[indexSource + 2] / 2 + (0) / 2);
                                pixels[row, col, 1] = (byte)(pixelSource[indexSource + 1] / 2 + (1020-x) / 2);
                                pixels[row, col, 0] = (byte)(pixelSource[indexSource] / 2 + (255) / 2);
                            }
                            if (x >= 1020 && x < 1275)
                            {
                                pixels[row, col, 2] = (byte)(pixelSource[indexSource + 2] / 2 + (x-1020) / 2);
                                pixels[row, col, 1] = (byte)(pixelSource[indexSource + 1] / 2 + (0) / 2);
                                pixels[row, col, 0] = (byte)(pixelSource[indexSource] / 2 + (255) / 2);
                            }
                            if (x >= 1275 && x <= 1530)
                            {
                                pixels[row, col, 2] = (byte)(pixelSource[indexSource + 2] / 2 + (255) / 2);
                                pixels[row, col, 1] = (byte)(pixelSource[indexSource + 1] / 2 + (0) / 2);
                                pixels[row, col, 0] = (byte)(pixelSource[indexSource] / 2 + (1530-x) / 2);
                            }

                    }

                    // Alpha
                    pixels[row, col, 3] = 255;
                }
            }

            // Flat le tableau
            byte[] pixels1d = new byte[height * width * 4];
            int index = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    for (int i = 0; i < 4; i++)
                        pixels1d[index++] = pixels[row, col, i];
                }
            }

            Int32Rect rect = new Int32Rect(0, 0, width, height);
            int stride = 4 * width;
            bitmap.WritePixels(rect, pixels1d, stride, 0);

            imgMask.Source = bitmap;
        }

        #endregion


        #region Méthodes d'événements d'interface

        /// <summary>
        /// Méthode de clic sur le bouton précédent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            int nb = ImageEnCours.Numero - 1;

            if (Index > 1)
            {
                ImageEnCours = AppData.ImagesExp[nb - 1];
                Index--;
                GenerateMask(Mode, ImageEnCours);
                lblNumImage.Content = "Image " + nb + " (" + Index + "e image sur " + MesImages.Count() + ")";
            }
        }

        /// <summary>
        /// Méthode de clic sur le bouton suivant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSuivant_Click(object sender, RoutedEventArgs e)
        {
            int nb = ImageEnCours.Numero + 1;

            if (Index < Data.Keys.Count())
            {
                ImageEnCours = AppData.ImagesExp[nb - 1];
                Index++;
                GenerateMask(Mode, ImageEnCours);
                lblNumImage.Content = "Image " + nb + " (" + index + "e image sur " + MesImages.Count() + ")";
            }
        }

        #endregion

        /// <summary>
        /// Méthode pour le changement de type de heatmap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbTypeDensite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            ComboBoxItem valueItem = comboBox.SelectedItem as ComboBoxItem;
            string value = valueItem.Content as string;

            Mode = (value.Equals("Transparence")) ? "gris" : "couleur";
            GenerateMask(Mode, ImageEnCours);
     
        }


        #region Helpers

        public override string ToString()
        {
            return AppData.IndicateursDensiteRecouvrement.Last().ToString();
        }

        #endregion


    }
}
