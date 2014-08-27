using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.Web.UI;
using MQClientInterface;
using MQServers;
using GoogleGeocoder;

namespace MQServers
{
   class MQServerDef
   {
      public const System.String mqMapServerName         = "map.free.mapquest.com";
      public const System.String mqMapServerPath         = "mq";
      public const int           mqMapServerPort         = 80;
      public const System.String mqMapServerClientId     = "85121";
      public const System.String mqMapServerPassword = "pT7xN8kU";

      public const System.String mqGeocodeServerName     = "geocode.free.mapquest.com";
      public const System.String mqGeocodeServerPath     = "mq";
      public const int           mqGeocodeServerPort     = 80;
      public const System.String mqGeocodeServerClientId = "85121";
      public const System.String mqGeocodeServerPassword = "pT7xN8kU";

      public const System.String mqRouteServerName       = "route.free.mapquest.com";
      public const System.String mqRouteServerPath       = "mq";
      public const int           mqRouteServerPort       = 80;
      public const System.String mqRouteServerClientId   = "85121";
      public const System.String mqRouteServerPassword = "pT7xN8kU";

      public const System.String mqSpatialServerName     = "spatial.free.mapquest.com";
      public const System.String mqSpatialServerPath     = "mq";
      public const int           mqSpatialServerPort     = 80;
      public const System.String mqSpatialServerClientId = "85121";
      public const System.String mqSpatialServerPassword = "pT7xN8kU";
   }
}

namespace MapquestGeocoder
{


    public class MapquestGeocode
    {
        


        public static GoogleGeocoder.Coordinate GetCoordinatesAndReformatAddress(ref string submittedAddress)
        {
            try
            {
			    MQClientInterface.Exec geocodeClient = new MQClientInterface.Exec();

			    //Client.mqGeocodeServerName refers to the name of the server where the MapQuest server resides.
			    //Client.mqGeocodeServerPath refers to the virtual directory where the MapQuest server resides.
			    //Client.mqMapServerPort refers to the port the client uses to communicate with the MapQuest
			    geocodeClient.ServerName = MQServers.MQServerDef.mqGeocodeServerName;
			    geocodeClient.ServerPath = MQServers.MQServerDef.mqGeocodeServerPath;
			    geocodeClient.ServerPort = MQServers.MQServerDef.mqGeocodeServerPort;
			    geocodeClient.ClientId   = MQServers.MQServerDef.mqGeocodeServerClientId;
			    geocodeClient.Password   = MQServers.MQServerDef.mqGeocodeServerPassword;

			    /*Create an Address object to contain the location to be geocoded.*/
			    MQClientInterface.Address address = new MQClientInterface.Address();

			    /*
			    The GeocodeResults Tbl will contain the results of the geocode. A Tbl
			    is used so that multiple potential matches or ambiguities can be returned when an
			    exact match cannot be found.
			    */
			    MQClientInterface.LocationCollection geocodeResults = new MQClientInterface.LocationCollection();

			    address.Init();
			    address.Street     = submittedAddress;

				geocodeClient.Geocode(address, geocodeResults);

                if (geocodeResults.Size == 0)
                {
                    throw new Exception("Could not geocode.");
                }
                /*Location geocoded, so display the match(es).*/
                else
                {
                    MQClientInterface.GeoAddress geoAddress = (MQClientInterface.GeoAddress)geocodeResults.GetAt(0);
                    string newLine = "\n";
                    submittedAddress = geoAddress.Street + newLine + ((geoAddress.City == null || geoAddress.City == "") ? geoAddress.County : geoAddress.City) + ", " + geoAddress.State + " " + geoAddress.PostalCode;
                    return new GoogleGeocoder.Coordinate((decimal) geoAddress.LatLng.Latitude, (decimal) geoAddress.LatLng.Longitude);
                }
            }
            catch
            {
                return new GoogleGeocoder.Coordinate(0, 0);
            }
        }

        public static void BulkGetCoordinatesAndReformatAddress(List<string> submittedAddresses, out List<GoogleGeocoder.Coordinate> coordinatesList)
        {
            try
            {
                MQClientInterface.Exec geocodeClient = new MQClientInterface.Exec();

                //Client.mqGeocodeServerName refers to the name of the server where the MapQuest server resides.
                //Client.mqGeocodeServerPath refers to the virtual directory where the MapQuest server resides.
                //Client.mqMapServerPort refers to the port the client uses to communicate with the MapQuest
                geocodeClient.ServerName = MQServers.MQServerDef.mqGeocodeServerName;
                geocodeClient.ServerPath = MQServers.MQServerDef.mqGeocodeServerPath;
                geocodeClient.ServerPort = MQServers.MQServerDef.mqGeocodeServerPort;
                geocodeClient.ClientId = MQServers.MQServerDef.mqGeocodeServerClientId;
                geocodeClient.Password = MQServers.MQServerDef.mqGeocodeServerPassword;


                MQClientInterface.LocationCollection geocodeRequests = new LocationCollection();
                foreach (var submittedAddress in submittedAddresses)
                {
                    MQClientInterface.Address address = new MQClientInterface.Address();
                    address.Street = submittedAddress;
                    geocodeRequests.Add(address);
                 }

                MQClientInterface.LocationCollectionCollection geocodeResults = new MQClientInterface.LocationCollectionCollection();

                geocodeClient.batchGeocode(geocodeRequests, geocodeResults);

                if (geocodeResults.Size == 0)
                {
                    throw new Exception("Could not geocode.");
                }
                /*Location geocoded, so display the match(es).*/
                else
                {
                    coordinatesList = new List<Coordinate>();
                    for (int i = 0; i < submittedAddresses.Count; i++)
                    {
                        MQClientInterface.GeoAddress geoAddress = (MQClientInterface.GeoAddress)(geocodeResults.GetAt(i).GetAt(0));
                        string newLine = "\n";
                        string reformattedAddress = geoAddress.Street + newLine + ((geoAddress.City == null || geoAddress.City == "") ? geoAddress.County : geoAddress.City) + ", " + geoAddress.State + " " + geoAddress.PostalCode;
                        GoogleGeocoder.Coordinate theCoordinates = new GoogleGeocoder.Coordinate((decimal)geoAddress.LatLng.Latitude, (decimal)geoAddress.LatLng.Longitude);
                        submittedAddresses[i] = reformattedAddress;
                        coordinatesList.Add(theCoordinates);
                    }
                }
            }
            catch
            {
                coordinatesList = new List<Coordinate>();
                for (int i = 0; i < submittedAddresses.Count; i++)
                { 
                    coordinatesList.Add(new GoogleGeocoder.Coordinate(0, 0));
                }
            }
        }


    }
}