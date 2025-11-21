using System;

namespace Tp1;

public class Telephoniste
{
    private Random rnd = new Random();

    public Zone Zone {
        set;
        get;
    }
    
    public Voyage CreerVoyage()
    {
        Voyage voyage = new Voyage();
        Point[] pointsVoyage = GenererPointsVoyage();
        voyage.SetPointsVoyage(pointsVoyage);

        return voyage;
    }

    private Point GenererPoint ()
    {
        Point point = new Point();
        point.X = rnd.Next(0, Zone.ZONE_SIZE);
        point.Y = rnd.Next(0, Zone.ZONE_SIZE);

        return point;
    }

    public Telephoniste(Zone zone)
    {
        Zone = zone;
    }

    private Point[] GenererPointsVoyage ()
    {
        Point depart = GenererPoint();
        Point arrivee = GenererPoint();
        
        while (MemePoint(depart, arrivee)) {
            arrivee = GenererPoint();
        }

        return new Point[] {depart, arrivee};
    }

    private bool MemePoint(Point depart, Point arrivee)
    {
        return depart.X == arrivee.X && arrivee.Y == depart.Y;
    }
}
