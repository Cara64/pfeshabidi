using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShaBiDi.Views;
using System.ComponentModel;
using System.Threading;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// Indicateur - Classe abstraite pour le calcul d'indicateur
    /// </summary>
    public abstract class Indicateur
    {
        #region Attributs et propriétés

        #region Paramètres sélectionnés par l'utilisateur
        
        /// <summary>
        /// Numéro des positions
        /// </summary>
        protected List<int> users;
        /// <summary>
        /// Ordre PA-S ou S-PA
        /// </summary>
        protected List<OrdreGroupe> ordres;
        /// <summary>
        /// Modalité PA
        /// </summary>
        protected bool modPA;
        /// <summary>
        /// Modalité S
        /// </summary>
        protected bool modS;

        #endregion

        /// <summary>
        /// Groupes importés
        /// </summary>
        protected List<Groupe> tousLesGroupes;
        
        /// <summary>
        /// Groupes pour l'indicateur
        /// </summary>
        protected List<Groupe> mesGroupes;

        /// <summary>
        /// Titre de l'indicateur
        /// </summary>
        protected string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// Fenêtre de chargement
        /// </summary>
        protected WaitWindow wait;
        /// <summary>
        /// Thread pour calcul de l'indicateur
        /// </summary>
        protected BackgroundWorker bw;

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur de la classe Indicateur
        /// </summary>
        /// <param name="mesUsers">Utilisateurs pour l'indicateur</param>
        /// <param name="mesOrdres">Ordres de modalité pour l'indicateur</param>
        /// <param name="pa">Modalité PA ou non</param>
        /// <param name="s">Modalité S ou non</param>
        /// <param name="groupes">Groupes sélectionnés</param>
        public Indicateur(List<int> mesUsers, List<OrdreGroupe> mesOrdres, bool pa, bool s, List<Groupe> groupes)
        {
            users = new List<int>();
            users = mesUsers;
            ordres = new List<OrdreGroupe>();
            ordres = mesOrdres;

            modPA = pa;
            modS = s;

            tousLesGroupes = groupes;
            mesGroupes = new List<Groupe>();
            mesGroupes = trouveGroupes();

            wait = new WaitWindow();
            bw = new BackgroundWorker();
            SetUpBackgroundWorker();

            setTitle();
        }

        #endregion


        #region Méthodes de gestion des threads

        /// <summary>
        /// Méthode pour paramétrer le BackgroundWorker
        /// </summary>
        protected void SetUpBackgroundWorker()
        {
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = false;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        /// <summary>
        /// Méthode lorsque le BackgroundWorker est terminé (update UI...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e);

        /// <summary>
        /// Méthode pour afficher la progression dans l'UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void bw_ProgressChanged(object sender, ProgressChangedEventArgs e);

        /// <summary>
        /// Méthode de thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void bw_DoWork(object sender, DoWorkEventArgs e);

        #endregion


        #region Autres méthodes

        /// <summary>
        /// Méthode qui retourne la liste des groupes concernés par l'indicateur
        /// </summary>
        /// <returns>Une liste de groupes concernés par l'indicateur</returns>
        private List<Groupe> trouveGroupes()
        {
            List<Groupe> liste = new List<Groupe>();
            foreach (Groupe g in tousLesGroupes)
            {
                if ((ordres.Contains(OrdreGroupe.PAS)) && (g.Ordre == OrdreGroupe.PAS))
                {
                    liste.Add(g);
                }
                else { }
                if ((ordres.Contains(OrdreGroupe.SPA)) && (g.Ordre == OrdreGroupe.SPA))
                {
                    liste.Add(g);
                }
                else { }
            }

            return liste;
        }

        /// <summary>
        /// Méthode qui retourne les bons sujets concernés par l'indicateur dans un groupe donnée
        /// </summary>
        /// <param name="g">Un groupe sélectionné par l'indicateur</param>
        /// <returns>La liste des sujets concernés par l'indicateur</returns>
        protected List<Sujet> trouveSujets(Groupe g)
        {
            List<Sujet> liste = new List<Sujet>();
            foreach (Sujet s in g.MesSujets)
            {
                if (users.Contains(s.Position))
                {
                    liste.Add(s);
                }
            }

            return liste;
        }

       /// <summary>
       /// Méthode pour fusionner les utilisateurs de deux indicateurs
       /// Nécessaire pour la création d'un indicateur à partir de deux autres indicateurs (pour les comparaisons)
       /// </summary>
       /// <param name="i1">Indicateur 1 à fusionner</param>
       /// <param name="i2">Indicateur 2 à fusionner</param>
       /// <returns>Liste de positions résultant de la fusion</returns>
        protected List<int> fusionUsers(Indicateur i1, Indicateur i2)
        {
            List<int> newlist = new List<int>();
            foreach (int i in i1.users)
            {
                if (!newlist.Contains(i))
                {
                    newlist.Add(i);
                }
            
            }
            return newlist;
        }
        
        /// <summary>
        /// Méthode pour fusionner les ordres de modalité de deux indicateurs
        /// Nécessaire pour la création d'un indicateur à partir de deux autres indicateurs (pour les comparaisons)
        /// </summary>
        /// <param name="i1">Indicateur 1 à fusionner</param>
        /// <param name="i2">Indicateur 2 à fusionner</param>
        /// <returns>Liste des ordres de modalité résultant de la fusion</returns>
        protected List<OrdreGroupe> fusionOrdres(Indicateur i1, Indicateur i2)
        {
            List<OrdreGroupe> newList = new List<OrdreGroupe>();
            foreach (OrdreGroupe o in i1.ordres)
            {
                if (!newList.Contains(o))
                {
                    newList.Add(o);
                }
              
            }
            return newList;
        }

        /// <summary>
        /// Méthode pour fusionner les groupes de deux indicateurs
        /// Nécessaire pour la création d'un indicateur à partir de deux autres indicateurs (pour les comparaisons)
        /// </summary>
        /// <param name="i1">Indicateur 1 à fusionner</param>
        /// <param name="i2">Indicateur 2 à fusionner</param>
        /// <returns>Liste de groupes résultant de la fusion</returns>
        protected List<Groupe> fusionGroupes(Indicateur i1, Indicateur i2)
        {
            List<Groupe> newList = new List<Groupe>();
            foreach (Groupe g in i1.mesGroupes)
            {
                if (!newList.Contains(g))
                {
                    newList.Add(g);
                }
            }

            return newList;
        }

        /// <summary>
        /// Méthode pour fusionner les modalités PA de deux indicateurs
        /// Nécessaire pour la création d'un indicateur à partir de deux autres indicateurs (pour les comparaisons)
        /// </summary>
        /// <param name="i1">Indicateur 1 à fusionner</param>
        /// <param name="i2">Indicateur 2 à fusionner</param>
        /// <returns>Booléen résultant de la fusion</returns>
        protected bool fusionPA(Indicateur i1, Indicateur i2)
        {
            return ((i1.modPA) || (i2.modPA));
        }

        /// <summary>
        /// Méthode pour fusionner les modalités S de deux indicateurs
        /// Nécessaire pour la création d'un indicateur à partir de deux autres indicateurs (pour les comparaisons)
        /// </summary>
        /// <param name="i1">Indicateur 1 à fusionner</param>
        /// <param name="i2">Indicateur 2 à fusionner</param>
        /// <returns>Booléen résultant de la fusion</returns>
        protected bool fusionS(Indicateur i1, Indicateur i2)
        {
            return ((i1.modS) || (i2.modS));
        }

        /// <summary>
        /// Méthode qui permet de définir le titre
        /// </summary>
        protected abstract void setTitle();

        /// <summary>
        /// Méthode qui permet de récupérer le nom de l'indicateur
        /// TODO: Bug dans la nomenclature à corriger
        /// </summary>
        /// <returns>Une chaîne de caractère contenant le nom de l'indicateur (NomInd_GRxx_Ux-x_ORDxxx_MODxxx)</returns>
        public override string ToString()
        {
            return Title;
        }

        #endregion
    }
}
