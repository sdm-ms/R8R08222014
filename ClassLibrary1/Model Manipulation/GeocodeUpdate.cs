

using System.Linq;
using System;
using GoogleGeocoder;
using MapquestGeocoder;
using System.Collections.Generic;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{
    public static class GeocodeUpdate
    {

        public static bool DoUpdate(IR8RDataContext theDataContext)
        {
            return false; 
            // for now, we are going to disable doing background worker geocoding. 
            // If we do ever decide to do this again and to use MapQuest as we do here, we
            // must update MapQuest Geocode account/code to conform to current API -- then uncomment the rest of this
            //DateTime recheckSince = TestableDateTime.Now - new TimeSpan(7, 0, 0, 0); // only check if it hasn't been checked in a week
            //var addressFieldsAndTblRowFieldDisplays = theDataContext.GetTable<AddressField>().Where(x => x.LastGeocode == null || (DateTime)x.LastGeocode < recheckSince).Take(100).Select(x => new { TheAddressField = x, TheTblRowFieldDisplay = x.Field.TblRow.TblRowFieldDisplay }).ToList();
            //List<AddressField> theAddressFieldsNeedingUpdating = addressFieldsAndTblRowFieldDisplays.Select(x => x.TheAddressField).ToList();
            //if (theAddressFieldsNeedingUpdating.Any())
            //{
            //    List<String> theAddresses = theAddressFieldsNeedingUpdating.Select(x => x.AddressString).ToList();
            //    List<Coordinate> theCoordinates;
            //    MapquestGeocode theGeocoder = new MapquestGeocode();
            //    MapquestGeocode.BulkGetCoordinatesAndReformatAddress(theAddresses, out theCoordinates);
            //    for (int i = 0; i < theAddresses.Count(); i++)
            //    {
            //        if (theCoordinates[i].Latitude != 0 || theCoordinates[i].Longitude != 0)
            //        {
            //            theAddressFieldsNeedingUpdating[i].Latitude = theCoordinates[i].Latitude;
            //            theAddressFieldsNeedingUpdating[i].Longitude = theCoordinates[i].Longitude;
            //            theAddressFieldsNeedingUpdating[i].AddressString = theAddresses[i];
            //            theAddressFieldsNeedingUpdating[i].LastGeocode = TestableDateTime.Now;
            //            addressFieldsAndTblRowFieldDisplays[i].TheTblRowFieldDisplay.ResetNeeded = true;
            //        }
            //    }
            //    return true;
            //}
            //return false;
        }

        public static bool DoUpdateOneAtATime(IR8RDataContext theDataContext)
        {
            DateTime recheckSince = TestableDateTime.Now - new TimeSpan(7, 0, 0, 0); // only check if it hasn't been checked in a week
            IQueryable<AddressField> theAddressFieldsNeedingUpdating = theDataContext.GetTable<AddressField>().Where(x => x.LastGeocode == null || (DateTime)x.LastGeocode < recheckSince).Take(100);
            if (theAddressFieldsNeedingUpdating.Any())
            {
                foreach (var theAddressField in theAddressFieldsNeedingUpdating)
                {
                    string theAddress = theAddressField.AddressString;
                    const bool useMapQuest = true; // can change this if we want to switch back to google.
                    Coordinate theCoordinate;
                    if (useMapQuest)
                    {
                        MapquestGeocode theGeocoder = new MapquestGeocode();
                        theCoordinate = MapquestGeocode.GetCoordinatesAndReformatAddress(ref theAddress);
                    }
                    //else
                    //{ // use google.
                    //    Geocode theGeocoder = new Geocode();
                    //    theCoordinate = Geocode.GetCoordinatesAndReformatAddress(ref theAddress);
                    //}
                    if (theCoordinate.Latitude != 0 || theCoordinate.Longitude != 0)
                    {
                        theAddressField.Latitude = theCoordinate.Latitude;
                        theAddressField.Longitude = theCoordinate.Longitude;
                        theAddressField.AddressString = theAddress;
                        theAddressField.LastGeocode = TestableDateTime.Now;
                    }
                }
                return true;
            }
            return false;
        }

    }
}