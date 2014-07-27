using System;
using System.Collections.Generic;

public static class GeocalcEnclosingSquare
{
    public static EarthCircle GetEnclosingSquare(double latitude, double longitude, double radius)
    {
        EarthCircle theCircle = new EarthCircle(latitude, longitude, radius);
        theCircle.CalculateEnclosingSquare(1);
        return theCircle;
    }
}

public class EarthCircle
{
    public double latitude;
    public double longitude;
    public double radius;
    public double eastLatitude;
    public double westLatitude;
    public double northLongitude;
    public double southLongitude;
    public bool enclosingSquareCalculated = false;

    public EarthCircle(double lat, double longi, double rad)
    {
        if (lat == 0 && longi == 0)
            throw new Exception("Cannot geocode from that location.");
        latitude = lat;
        longitude = longi;
        radius = rad;
    }

    internal double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / (double)180;
    }

    internal double RadiansToDegrees(double radians)
    {
        return radians * (double)180 / Math.PI;
    }

    internal EarthCircle GetDistanceFromCenter(double degrees, double distance)
    {
        const double R = 3959; // miles
        double latitudeInRadians = DegreesToRadians(latitude);
        double longitudeInRadians = DegreesToRadians(longitude);

        EarthCircle newCircle = new EarthCircle(latitude, longitude, radius);
        newCircle.latitude = Math.Asin(Math.Sin(latitudeInRadians) * Math.Cos(distance / R) +
              Math.Cos(latitudeInRadians) * Math.Sin(distance / R) * Math.Cos(degrees));
        newCircle.longitude = longitudeInRadians + Math.Atan2(Math.Sin(degrees) * Math.Sin(distance / R) * Math.Cos(latitudeInRadians),
                     Math.Cos(distance / R) - Math.Sin(latitudeInRadians) * Math.Sin(newCircle.latitude));
        newCircle.longitude = (newCircle.longitude + Math.PI) % (2 * Math.PI) - Math.PI;  // normalise to -180...+180 (in radians)
        newCircle.latitude = RadiansToDegrees(newCircle.latitude);
        newCircle.longitude = RadiansToDegrees(newCircle.longitude);

        return newCircle;
    }

    public void CalculateEnclosingSquare(double multiplyFactor)
    {
        if (!enclosingSquareCalculated)
        {
            eastLatitude = GetDistanceFromCenter(0, radius * multiplyFactor).latitude;
            westLatitude = GetDistanceFromCenter(180, radius * multiplyFactor).latitude;
            northLongitude = GetDistanceFromCenter(90, radius * multiplyFactor).longitude;
            southLongitude = GetDistanceFromCenter(270, radius * multiplyFactor).longitude;
            enclosingSquareCalculated = true;
        }
    }

    internal EarthCircle CalculateSmallerCircle(double multiplyFactor, double divideInto, double horizNum, double vertNum)
    {
        CalculateEnclosingSquare(multiplyFactor);
        EarthCircle newCircle = new EarthCircle(latitude, longitude, radius);
        newCircle.latitude = westLatitude + ((eastLatitude - westLatitude) / divideInto) * (horizNum - 0.5);
        newCircle.longitude = southLongitude + ((northLongitude - southLongitude) / divideInto) * (vertNum - 0.5);
        newCircle.radius = (radius / divideInto) * 1.1;
        return newCircle;
    }

    public List<EarthCircle> ReturnOverlappingCircles(int divideEachDirection, double multiplyFactor)
    {
        List<EarthCircle> theList = new List<EarthCircle>();
        for (int h = 1; h <= divideEachDirection; h++)
        {
            for (int v = 1; v <= divideEachDirection; v++)
            {
                theList.Add(CalculateSmallerCircle(multiplyFactor, divideEachDirection, h, v));
            }
        }
        return theList;
    }

    public bool IsInCircle(double pointLatitude, double pointLongitude)
    {
        return GeoCodeCalc.CalcDistance(latitude, longitude, pointLatitude, pointLongitude) <= radius;
    }

}

public static class GeoCodeCalc
{
    public const double EarthRadiusInMiles = 3956.0;
    public const double EarthRadiusInKilometers = 6367.0;
    public static double ToRadian(double val) { return val * (Math.PI / 180); }
    public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }
    /// <summary>
    /// Calculate the distance between two geocodes. Defaults to using Miles.
    /// </summary>
    public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
    {
        return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
    }
    /// <summary>
    /// Calculate the distance between two geocodes.
    /// </summary>
    public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
    {
        double radius = GeoCodeCalc.EarthRadiusInMiles;
        if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoCodeCalc.EarthRadiusInKilometers; }
        return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
    }
}
public enum GeoCodeCalcMeasurement : int
{
    Miles = 0,
    Kilometers = 1
}