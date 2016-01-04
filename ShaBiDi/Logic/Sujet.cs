using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// Classe qui permet de modéliser le sujet de l'expérience
    /// </summary>
    public class Sujet
    {

        #region Attributs et données

        /// <summary>
        /// Position du sujet dans le groupe
        /// </summary>
        public int Position { get; private set; }
        /// <summary>
        /// Liste des observations enregistrées en mode PA
        /// </summary>
        public List<Observation> ObservationsPA { get; private set; }
        /// <summary>
        /// Liste des observations enregistrées en mode S
        /// </summary>
        public List<Observation> ObservationsS { get; private set; }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur de la classe Sujet
        /// </summary>
        /// <param name="position">Position du sujet</param>
        public Sujet(int position)
        {
            Position = position;
            ObservationsPA = new List<Observation>();
            ObservationsS = new List<Observation>();
        }

        #endregion


        #region Méthodes

        /// <summary>
        /// Permet d'ajouter une observation selon la modalité
        /// </summary>
        /// <param name="obs">Observation à ajouter</param>
        /// <param name="mod">Modalité</param>
        public void AddObservation(Observation obs, Modalite mod)
        {
            switch (mod)
            {
                case Modalite.PA:
                    ObservationsPA.Add(obs);
                    break;
                case Modalite.S:
                    ObservationsS.Add(obs);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Ajouter un point d'attention
        /// </summary>
        /// <param name="i">Numéro de l'image concernée</param>
        /// <param name="mod">Modalité</param>
        /// <param name="x">Coordonnées X du PA</param>
        /// <param name="y">Coordonnées Y du PA</param>
        /// <param name="tps">Temps de l'observation</param>
        public void AddPA(int i, Modalite mod, double x, double y, double tps)
        {
            // Il faut trouver le rang de l'observation concernée par l'image donnée
            int indice = 0;

            // Premier cas, l'observation est en modalité PA
            if (mod == Modalite.PA)
            {
                foreach (Observation o in ObservationsPA)
                {
                    if (o.Image.Numero == i)
                    {
                        indice = ObservationsPA.IndexOf(o);
                    }
                }

                ObservationsPA[indice].AddPA(x, y, tps);
            }
            // Deuxième cas, l'observation est en modalité S
            else
            {
                foreach (Observation o in ObservationsS)
                {
                    if (o.Image.Numero == i)
                    {
                        indice = ObservationsS.IndexOf(o);
                    }
                }

                ObservationsS[indice].AddPA(x, y, tps);
            }
        }

        #endregion
    }
}
