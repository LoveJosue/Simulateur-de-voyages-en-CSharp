using System;

namespace Tp1;

public enum EtatTaxis
{
    EN_ATTENTE,
    EN_MOUVEMENT
}

public class Taxi
{
    public static int NB_TAXIS = 0;
    private int tempsAttenteMin = 250;
    private int tempsAttenteMax = 500;

    private Random rnd = new Random();


    public int TaxiID
    {
        set;
        get;
    }
    public Point Position
    {
        set;
        get;
    }
    private Voyage? Voyage
    {
        set;
        get;
    }
    public EtatTaxis Etat
    {
        set;
        get;
    }

    public Taxi ()
    {
        NB_TAXIS++;
        TaxiID = NB_TAXIS;
        Etat = EtatTaxis.EN_ATTENTE;
    }

    public void AssignerVoyage(Voyage voyage)
    {
        if (Voyage != null)
        {
            Console.WriteLine("Attention! Ce taxis a déjà un voyage assigné!");
        }
        else
        {
            Voyage = voyage;
            Console.WriteLine($"Voyage {voyage.VoyageID} assigné au taxi {TaxiID}");

        }
    }

    public void FaireVoyage()
    {
        if (Voyage != null) {
            Etat = EtatTaxis.EN_MOUVEMENT;
            Voyage.Etat = EtatVoyage.EN_COURS;
            SeDeplacer();
            TerminerVoyage();
        }

    }

    private void SeDeplacer()
    {
        AllerPointDepart();
        AllerPointArrivee();
    }

    private void AllerPointArrivee()
    {
        int deltaX = Voyage.Arrivee.X - Position.X;
        int deltaY = Voyage.Arrivee.Y - Position.Y;
        int tempsAttente;

        // Déplacement Horizontal
        if (deltaX != 0)
        {
            for (int i = 0; i < Math.Abs(deltaX); i++)
            {
                Position.X += (deltaX / Math.Abs(deltaX));
                tempsAttente = rnd.Next(tempsAttenteMin, tempsAttenteMax);
                Thread.Sleep(tempsAttente);
            }
        }
    
        // Déplacement Vertical
        if (deltaY != 0)
        {
            for (int i = 0; i < Math.Abs(deltaY); i++)
            {
                Position.Y += (deltaY / Math.Abs(deltaY));
                tempsAttente = rnd.Next(tempsAttenteMin, tempsAttenteMax);
                Thread.Sleep(tempsAttente);
            }
        }
        Console.WriteLine($"Le client du voyage {Voyage.VoyageID} sort du taxi {TaxiID} à {Position.ToString()}");
    }

    private void AllerPointDepart()
    {
        int deltaX = Voyage.Depart.X - Position.X;
        int deltaY = Voyage.Depart.Y - Position.Y;
        int tempsAttente;

        // Déplacement Horizontal
        if (deltaX != 0)
        {
            for (int i = 0; i < Math.Abs(deltaX); i++)
            {
                Position.X += (deltaX / Math.Abs(deltaX));
                tempsAttente = rnd.Next(tempsAttenteMin, tempsAttenteMax);
                Thread.Sleep(tempsAttente);
            }
        }
        

        // Déplacement Vertical
        if (deltaY != 0)
        {
            for (int i = 0; i < Math.Abs(deltaY); i++)
            {
                Position.Y += (deltaY / Math.Abs(deltaY));
                tempsAttente = rnd.Next(tempsAttenteMin, tempsAttenteMax);
                Thread.Sleep(tempsAttente);
            }
        }
        Console.WriteLine($"Le client du voyage {Voyage.VoyageID} entre dans taxi {TaxiID} à {Position.ToString()}");
    }

    private void TerminerVoyage()
    {
        if (Etat == EtatTaxis.EN_MOUVEMENT)
        {
            Etat = EtatTaxis.EN_ATTENTE;
            Voyage.Etat = EtatVoyage.TERMINE;
            Voyage = null;
        }
    }
}
