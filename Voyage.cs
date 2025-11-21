using System;

namespace Tp1;

public enum EtatVoyage 
{
    EN_COURS,
    TERMINE
}

public class Voyage
{
    private static int NB_VOYAGES = 0;
    public int VoyageID 
    {
        set;
        get;
    }
    public Point Depart {
        set;
        get;
    }
    public Point Arrivee {
        set;
        get;
    }

    public EtatVoyage Etat {
        set;
        get;
    }

    public void SetPointsVoyage(Point[] points)
    {
        Depart = points[0];
        Arrivee = points[1];
    }

    public Voyage ()
    {
        NB_VOYAGES++;
        VoyageID = NB_VOYAGES;
        Etat = EtatVoyage.EN_COURS;
    }
}
