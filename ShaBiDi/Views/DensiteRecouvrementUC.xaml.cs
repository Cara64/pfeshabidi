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
    /// </summary>
    public partial class DensiteRecouvrementUC : UserControl
    {

        #region Attributs
        
        private Dictionary<ShaBiDi.Logic.ImageExp, double[,]> data;
        private I_DensiteRecouvrement indic;

        private List<int> positions;
        private List<OrdreGroupe> ordres;
        private List<Groupe> groupes;
        private bool modS;
        private bool modPA;

        private string mode;                            // transparence ou couleurs
        private List<ShaBiDi.Logic.ImageExp> mesImages;    // images de l'indicateur
        private int index;                              // ordre de l'image
        private ShaBiDi.Logic.ImageExp imageEnCours;   
        private WriteableBitmap imageBmp;

        #endregion


        # region Propriétés

        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        public List<ShaBiDi.Logic.ImageExp> MesImages
        {
            get { return mesImages; }
            set { mesImages = value; }
        }
        public int Index
        {
            get { return index; }
            set { index = value; }
        }
        public ShaBiDi.Logic.ImageExp ImageEnCours
        {
            get { return imageEnCours; }
            set { imageEnCours = value; }
        }
        public WriteableBitmap ImageBmp
        {
            get { return imageBmp; }
            set { imageBmp = value; }
        }

        public Dictionary<ShaBiDi.Logic.ImageExp, double[,]> Data
        {
            get { return data; }
            set { data = value; }
        }
        public ShaBiDi.Logic.I_DensiteRecouvrement Indic
        {
            get { return indic; }
            set { indic = value; }
        }

        public List<int> Positions
        {
            get { return positions; }
            set { positions = value; }
        }
        public List<OrdreGroupe> Ordres
        {
            get { return ordres; }
            set { ordres = value; }
        }
        public List<Groupe> Groupes
        {
            get { return groupes; }
            set { groupes = value; }
        }
        public bool ModS
        {
            get { return modS; }
            set { modS = value; }
        }
        public bool ModPA
        {
            get { return modPA; }
            set { modPA = value; }
        }

        #endregion

        public DensiteRecouvrementUC(string mode)
        {
            InitializeComponent();
            Mode = mode;

            GetData();
            LoadData();
            SetUpModel();
        }

        private void GetData()
        {
            Data = new Dictionary<ShaBiDi.Logic.ImageExp, double[,]>();

            Positions = CreateIndicWindow.Positions;
            Groupes = CreateIndicWindow.Groupes;
            Ordres = CreateIndicWindow.Ordres;
            ModS = CreateIndicWindow.ModS;
            ModPA = CreateIndicWindow.ModPA;

            Indic = new I_DensiteRecouvrement(Positions, Ordres, ModPA, ModS, Groupes);
            Data = Indic.determineDensite();
        }

        private void LoadData()
        {
            MesImages = new List<ShaBiDi.Logic.ImageExp>();
            MesImages = Data.Keys.ToList().OrderBy(o => o.Numero).ToList();
        }

        private void SetUpModel()
        {
            imageEnCours = MesImages[0];
            index = 1;
            lblNumImage.Content = "Image " + ImageEnCours.Numero + " (" + Index + "e image sur " + MesImages.Count() + ")";
            lblTitleIndicateur.Content = this.ToString();
            GenerateMask(Mode, ImageEnCours);
        }

        // Prendra l'image et les booleens en parametres
        // Le mode peut être "gris" ou "couleur"
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

        public override string ToString()
        {
            string res = "DensiteRecouvrement";
            res += (this.Mode.Equals("gris")) ? "Transparent_GR" : "Couleur_GR";
            foreach (Groupe groupe in Groupes)
                res += (!groupe.Equals(Groupes.Last())) ? groupe.Identifiant + "-" : groupe.Identifiant + "_U";
            foreach (int pos in Positions)
                res += (!pos.Equals(Positions.Last())) ? pos + "-" : pos + "_ORD";
            foreach (OrdreGroupe ordre in Ordres)
                res += (!ordre.Equals(Ordres.Last())) ? ordre.ToString() + "-" : ordre.ToString() + "_MOD";
            if (ModS && ModPA)
                res += "S-PA";
            else
                if (ModS) res += "S";
                else res += "PA";

            return res;
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            int nb = ImageEnCours.Numero - 1;

            if (Index <= 1) { }
            else
            {
                ImageEnCours = AppData.ImagesExp[nb - 1];
                Index--;
                GenerateMask(Mode, ImageEnCours);
                lblNumImage.Content = "Image " + nb + " (" + Index + "e image sur " + MesImages.Count() + ")";
            }
        }

        private void btnSuivant_Click(object sender, RoutedEventArgs e)
        {
            int nb = ImageEnCours.Numero + 1;

            if (Index >= Data.Keys.Count())
            {
                int a = 0;
            }
            else
            {
                ImageEnCours = AppData.ImagesExp[nb - 1];
                Index++;
                GenerateMask(Mode, ImageEnCours);
                lblNumImage.Content = "Image " + nb + " (" + index + "e image sur " + MesImages.Count() + ")";
            }
        }


    }
}
