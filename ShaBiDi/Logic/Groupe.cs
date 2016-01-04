using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaBiDi.Logic
{
    /// <summary>
    /// Groupe - classe correspondant aux groupes de l'expérience
    /// </summary>
    public class Groupe
    {
        #region Attributs et propriétés

        /// <summary>
        /// Identifiant du groupe
        /// </summary>
        public int Identifiant { get; private set; }
        /// <summary>
        /// Ordre des modalités de l'expérience du groupe (SPA ou PAS)
        /// </summary>
        public OrdreGroupe Ordre { get; private set; }
        /// <summary>
        /// Sujets du groupe ayant fait l'expérience
        /// </summary>
        public List<Sujet> MesSujets { get; private set; }

        #endregion


        #region Constructeur

        /// <summary>
        /// Constructeur du groupe
        /// </summary>
        /// <param name="identifiant">Identifiant du groupe</param>
        /// <param name="ordre">Ordre des modalités</param>
        public Groupe(int identifiant, OrdreGroupe ordre)
        {
            Identifiant = identifiant;
            Ordre = ordre;

            MesSujets = new List<Sujet>();
        }

        #endregion


        #region Autres méthodes

        /// <summary>
        /// Ajout de sujet
        /// </summary>
        /// <param name="s">Sujet à ajouter</param>
        public void AddSujet(Sujet s)
        {
            MesSujets.Add(s);
        }

        /// <summary>
        /// Ajout d'observation au sujet
        /// </summary>
        /// <param name="obs">Observation à ajouter</param>
        /// <param name="mod">Modalité (S ou PA) à ajouter</param>
        /// <param name="index">Index de la liste sujet dans lequel on veut ajouter l'observation</param>
        public void AddObservation(Observation obs, Modalite mod, int index)
        {
            MesSujets[index].AddObservation(obs, mod);
        }

        public override string ToString()
        {
            return "Groupe " + Identifiant;
        }

        #endregion

    }
}
