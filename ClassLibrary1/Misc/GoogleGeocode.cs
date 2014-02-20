using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.Web.UI;
using Newtonsoft.Json;


namespace GoogleGeocoder
{
    public interface ISpatialCoordinate
    {
        decimal Latitude { get; set; }
        decimal Longitude { get; set; }
    }

    /// <summary>
    /// Coordiate structure. Holds Latitude and Longitude.
    /// </summary>
    public struct Coordinate : ISpatialCoordinate
    {
        private decimal _latitude;
        private decimal _longitude;

        public Coordinate(decimal latitude, decimal longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }

        #region ISpatialCoordinate Members

        public decimal Latitude
        {
            get
            {
                return _latitude;
            }
            set
            {
                this._latitude = value;
            }
        }

        public decimal Longitude
        {
            get
            {
                return _longitude;
            }
            set
            {
                this._longitude = value;
            }
        }

        #endregion
    }

    public class Geocode
    {
        private const string _googleUri = "https://maps.googleapis.com/maps/api/geocode/json"; 
        private const string _googleKey = "ABQIAAAAxJm82XBjR9SRcFpZJCVO6BRRVfaJZ8QztGdpa7Rq78iMlA85jRR9ITKlRPrnzARoUPhqrbkt_dMQ6Q"; // Attached to Jessie.K.Liu@gmail.com NOTE: We no longer need this for up to 2500 per day.
        // private const string _googleKey = "ABQIAAAAs-ZODfxj8f9LNfbR_FZHTRT2yXp_ZAY8_ufC3CFXhHIE1NvwkxSOqvFJau5XR-PEzbSn1R2d2qm26A";

        public class GoogleGeoCodeResponse
        {

            public string status { get; set; }
            public results[] results { get; set; }

        }

        public class results
        {
            public string formatted_address { get; set; }
            public geometry geometry { get; set; }
            public string[] types { get; set; }
            public address_component[] address_components { get; set; }
        }

        public class geometry
        {
            public string location_type { get; set; }
            public location location { get; set; }
        }

        public class location
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class address_component
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }

        private static Uri GetGeocodeUri(string address)
        {
            address = HttpUtility.UrlEncode(address);
            return new Uri(String.Format("{0}?address={1}&sensor=false", _googleUri, address));
        }

        /// <summary>
        /// Gets a Coordinate from a address.
        /// </summary>
        /// <param name="address">An address.
        /// <remarks>
        /// <example>1600 Amphitheatre Parkway Mountain View, CA 94043</example>
        /// </remarks>
        /// </param>
        /// <returns>A spatial coordinate that contains the latitude and longitude of the address.</returns>
        public static Coordinate GetCoordinates(string address)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            WebClient client = new WebClient();
            Uri uri = GetGeocodeUri(address);


            /* The first number is the status code, 
            * the second is the accuracy, 
            * the third is the latitude, 
            * the fourth one is the longitude.
            */
            string geocodeInfo = client.DownloadString(uri);

            GoogleGeoCodeResponse gr = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(geocodeInfo);

            if (gr.results == null)
                return new Coordinate(0.0M, 0.0M);
            location loc = gr.results[0].geometry.location;

            return new Coordinate(Convert.ToDecimal(loc.lat), Convert.ToDecimal(loc.lng));
        }

        public static Coordinate GetCoordinatesAndReformatAddress(ref string address)
        {
            try
            {
                string xml = GMapGeocoder.Util.GetXml(address, _googleKey);
                GMapGeocoder.Generated.kml kml = GMapGeocoder.Util.DeserializeXml(xml);
                var someResult = kml.Response.Placemark;
                if (someResult == null)
                    return new Coordinate(0, 0);
                var theResult = someResult[0];
                // string fullAddress = kml.Response.Placemark[0].address;
                if (theResult == null)
                {
                    return new Coordinate(0, 0);
                }
                else
                {
                    string[] theCoordinates = theResult.Point.coordinates.Split(',');
                    if (theCoordinates == null || theCoordinates[0] == null || theCoordinates[1] == null)
                        return new Coordinate(0, 0);
                    decimal latitude;
                    decimal longitude;
                    longitude = Convert.ToDecimal(theCoordinates[0]);
                    latitude = Convert.ToDecimal(theCoordinates[1]);
                    var theAddressDetails = theResult.AddressDetails;
                    if (theAddressDetails.Country == null || theAddressDetails.Country.AdministrativeArea == null
                        || (theAddressDetails.Country.AdministrativeArea.Locality == null && theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea == null))
                        return new Coordinate(0, 0);
                    string localityName;
                    string thoroughfareName;
                    string postalCode = "";
                    if (theAddressDetails.Country.AdministrativeArea.Locality != null)
                    {
                        localityName = theAddressDetails.Country.AdministrativeArea.Locality.LocalityName;
                        if (theAddressDetails.Country.AdministrativeArea.Locality.Thoroughfare == null)
                            thoroughfareName = "";
                        else
                            thoroughfareName = theAddressDetails.Country.AdministrativeArea.Locality.Thoroughfare.ThoroughfareName;
                        if (theAddressDetails.Country.AdministrativeArea.Locality.PostalCode != null)
                            postalCode = theAddressDetails.Country.AdministrativeArea.Locality.PostalCode.PostalCodeNumber;
                    }
                    else
                    {
                        if (theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality == null)
                        {
                            localityName = theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.SubAdministrativeAreaName;
                        }
                        else
                        {
                            localityName = theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.LocalityName;

                            if (theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.PostalCode != null)
                                postalCode = theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.PostalCode.PostalCodeNumber;
                        }
                        if (theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality == null || theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.Thoroughfare == null)
                            thoroughfareName = "";
                        else
                            thoroughfareName = theAddressDetails.Country.AdministrativeArea.SubAdministrativeArea.Locality.Thoroughfare.ThoroughfareName;

                    }
                    string stateCode = theAddressDetails.Country.AdministrativeArea.AdministrativeAreaName;
                    string countryCode = theAddressDetails.Country.CountryNameCode;
                    string newLine = "\n";
                    if (countryCode == "US")
                        address = thoroughfareName + newLine + localityName + ", " + stateCode + " " + postalCode;
                    else
                        address = thoroughfareName + newLine + localityName + ", " + stateCode + " " + postalCode + newLine + countryCode;

                    return new Coordinate(latitude, longitude);
                }
            }
            catch
            {
                return new Coordinate(0, 0);
            }
        }

    }
}