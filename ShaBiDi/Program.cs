using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShabidiTaux
{
    class Program
    {
        static void Main(string[] args)
        {
            Groupe groupe1 = new Groupe(1, OrdreGroupe.PAS);
            Groupe groupe2 = new Groupe(2, OrdreGroupe.SPA);

            // g1
            groupe1.AddSujet(new Sujet(1));
            groupe1.AddSujet(new Sujet(2));
            
            // g2
            groupe2.AddSujet(new Sujet(1));
            groupe2.AddSujet(new Sujet(2));

            Image imgChien = new Image(1);
            Image imgLapin = new Image(2);
            Image imgPoisson = new Image(3);

            groupe1.AddObservation(new Observation(imgChien), Modalite.PA, 0);
            groupe1.AddObservation(new Observation(imgChien), Modalite.PA, 1);

            groupe1.AddObservation(new Observation(imgLapin), Modalite.PA, 0);
            groupe1.AddObservation(new Observation(imgLapin), Modalite.PA, 1);

            groupe1.AddObservation(new Observation(imgPoisson), Modalite.S, 0);
            groupe1.AddObservation(new Observation(imgPoisson), Modalite.S, 1);

            groupe2.AddObservation(new Observation(imgChien), Modalite.S, 0);
            groupe2.AddObservation(new Observation(imgChien), Modalite.S, 1);

            groupe2.AddObservation(new Observation(imgLapin), Modalite.S, 0);
            groupe2.AddObservation(new Observation(imgLapin), Modalite.S, 1);

            groupe2.AddObservation(new Observation(imgPoisson), Modalite.PA, 0);
            groupe2.AddObservation(new Observation(imgPoisson), Modalite.PA, 1);

            Indicateur indicateur1 = new Indicateur(new List<int>(){ 1, 2 }, new List<Groupe>(){ groupe1, groupe2 });
        }
    }
}
