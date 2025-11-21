using System.Collections.Concurrent;
using Tp1;

public class Program
{
    static int NB_TAXIS = 5;
    static int NB_TELEPHONISTES = 2;
    static int NB_VOYAGES = 20;
    static Zone zone = new Zone();
    static int MILLE = 1000;
    static int DEUX_MILLES = 2000;

    static int DEUX = 2;

    static List<Taxi> taxis = new List<Taxi>();
    
    // Les voyages
    static ConcurrentQueue<Voyage> voyages = new ConcurrentQueue<Voyage>();

    static object LOCK_VOYAGES = new object();
    static object LOCK_TAXIS = new object();

    static Boolean coursesIndisponibles = false;

    static void Main(string[] args)
    {
        Thread[] thTelephonistes = new Thread[NB_TELEPHONISTES];
        Thread thRepartiteurs;
        Thread[] thTaxis = new Thread[NB_TAXIS];

        // 1- Créer et démarrer Thread des téléphonistes
        for (int i = 0; i < thTelephonistes.Length; i++)
        {
            thTelephonistes[i] = new Thread(new ThreadStart(CreerVoyage));
        }

        foreach(Thread th in thTelephonistes)
        {
            th.Start();
        
        }

        // 3- Créer et demarrer thread du répartiteur
        thRepartiteurs = new Thread(() => AssignerVoyages(thTelephonistes));
        thRepartiteurs.Start();   

        // 2- Créer et démarrer Thread des taxis
        for (int i = 0; i < thTaxis.Length; i++)
        {
            thTaxis[i] = new Thread(new ThreadStart(EffectuerService));
        }
        foreach (Thread th in thTaxis)
        {
            th.Start();
        }
    }

    private static void CreerVoyage()
    {
        Random rnd = new Random();
        Telephoniste telephoniste = new Telephoniste(zone);
        int cmpt = 0;

        while (cmpt < NB_VOYAGES)
        {
            int delai = rnd.Next(MILLE, DEUX_MILLES);

            lock (LOCK_VOYAGES)
            {
                Voyage newVoyage = telephoniste.CreerVoyage();
                voyages.Enqueue(newVoyage);
                Console.WriteLine($"Creation voyage {newVoyage.VoyageID} Depart:  {newVoyage.Depart.ToString()} Arrivee: {newVoyage.Arrivee.ToString()}");
            }

            Thread.Sleep(delai);
            cmpt++;
        }
    }

    private static void EffectuerService()
    {
        Taxi newTaxi = new Taxi();
        newTaxi.Position = SetPositionDepart();
        AddTaxiToList(newTaxi);
        DoTask(newTaxi);
    }

    private static void DoTask(Taxi taxi)
    {
        while (!coursesIndisponibles)
        {
            taxi.FaireVoyage();
        }
    }

    private static void AddTaxiToList(Taxi newTaxi)
    {
        lock (LOCK_TAXIS)
        {
            taxis.Add(newTaxi);
            Console.WriteLine($"Le taxi {newTaxi.TaxiID} est en fonction à {newTaxi.Position.ToString()}");
        }
    }

    private static Point SetPositionDepart()
    {
        Random rnd = new Random();
        Point point = new Point();
        point.X = rnd.Next(zone.ZONE_SIZE);
        point.Y = rnd.Next(zone.ZONE_SIZE);
        return point;
    }
    

    private static void AssignerVoyages(Thread[] thTelephonistes)
    {
        Random rnd = new Random();

        while (TelephonistsTasksAlive(thTelephonistes) || voyages.Count > 0)
        {
            lock (LOCK_VOYAGES)
            {
                Voyage? result;
                Taxi? taxiPlusProche;
                bool voyage = voyages.TryPeek(out result);
                if (voyage)
                {
                    taxiPlusProche = TaxisEnAttenteLePlusProche(result);
                    if (taxiPlusProche != null)
                    {
                        taxiPlusProche.AssignerVoyage(result);
                        voyages.TryDequeue(out result);
                    }
                }

            }
            Thread.Sleep(rnd.Next(1000, 2000));
        
        }
        coursesIndisponibles = true;
    }
    
    // --  Cette version a une faille : si aucun véhicule n'est disponible, le voyage est enlevé de la liste et n'a plus de chance d'être assigné après --
    // private static void AssignerVoyages(Thread[] thTelephonistes)
    // {
    //     Random rnd = new Random();

    //     while (telephonistsTasksAlive(thTelephonistes) || voyages.Count > 0)
    //     {
    //         lock (LOCK)
    //         {
    //             Voyage result;
    //             Taxi taxiPlusProche;
    //             bool voyage = voyages.TryDequeue(out result);
    //             if (voyage)
    //             {
    //                 taxiPlusProche = TaxisEnAttenteLePlusProche(result);
    //                 taxiPlusProche.AssignerVoyage(result);
    //             }

    //         }

    //         Thread.Sleep(rnd.Next(1000, 2000));
        
    //     }
    // }

    private static Taxi? TaxisEnAttenteLePlusProche(Voyage voyage)
    {
        Double plusCourte = Double.MaxValue;
        Taxi? taxiPlusProche = null;
        
        if (taxis.Count > 0)
        {
            foreach(Taxi taxi in taxis)
            {
                if (taxi.Etat == EtatTaxis.EN_ATTENTE)
                {
                    Double distance = CalculerDistance(voyage, taxi);
                    if (distance < plusCourte)
                    {
                        plusCourte = distance;
                        taxiPlusProche = taxi;
                    }
                }
            }
        } else 
        {
            Console.WriteLine("Il n'y a aucun taxi disponible.");
            return null;
        }
        return taxiPlusProche;
    }

    private static double CalculerDistance(Voyage voyage, Taxi taxi)
    {
        Point pointDepartVoyage = voyage.Depart;
        Point positionTaxi = taxi.Position;

        return Math.Sqrt(Math.Pow(pointDepartVoyage.X - positionTaxi.X, DEUX) + Math.Pow(pointDepartVoyage.Y - positionTaxi.Y, DEUX));
    }

    private static bool TelephonistsTasksAlive(Thread[] thTelephonistes)
    {
        foreach (Thread th in thTelephonistes)
        {
            if (!th.IsAlive) return false;
            
        }
        return true;
    }    
}