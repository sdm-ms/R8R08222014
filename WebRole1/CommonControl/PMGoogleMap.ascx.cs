using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using Subgurim.Controles;
using System.Collections.Generic;

public partial class PMGoogleMap : System.Web.UI.UserControl
{
    // GMap GM;
    //double latitudeSum = 0;
    //double longitudeSum = 0;
    //int numEntries = 0;
    GLatLng DefaultLocation = null;
    List<GLatLng> ListOfMarkers;

    public PMGoogleMap()
    {
        ListOfMarkers = new List<GLatLng>();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public void Hide()
    {
        GoogleMapTable.Visible = false;
    }

    public void Setup(double defaultLatitude, double defaultLongitude)
    {
        DefaultLocation = new GLatLng(defaultLatitude, defaultLongitude);
       GoogleMapTable.Visible = true;
       // GM = new GMap();
       // // GM.setCenter(new GLatLng(41, 3), 3);
       GM.Add(new GControl(GControl.preBuilt.SmallMapControl));
       // //GMarker m1 = new GMarker(new GLatLng(41, 3));
       // //MarkerManager mManager = new MarkerManager();
       // //List<GMarker> mks = new List<GMarker>();
       // //mManager.Add(m1, 2);
       // //mManager.Add(mks, 6, 8);
       // //GM.markerManager = mManager;
       // GmapInfoPlaceHolder.Controls.Add(GM);
    }

    // This function centers and zooms in the map based on a bunch of points.
    public static void GMAP_BoundingBox(List<GLatLng> latlong, GMap gMap_name, bool set_center, bool set_zoom)
    {
        StringBuilder sb = new StringBuilder();
        gMap_name.addCustomInsideJavascript("get_bounds_zoom();");
        sb.Append("function get_bounds_zoom()");
        sb.Append("{");
        sb.Append("var point = new GLatLng(" + latlong[0].lat.ToString() + "," + latlong[0].lng.ToString() + ");");
        sb.Append("var bounds = new GLatLngBounds(point);");

        foreach (GLatLng lls in latlong)
        {
            sb.Append("var point = new GLatLng(" + lls.lat.ToString() + "," + lls.lng.ToString() + ");");
            sb.Append("bounds.extend(point);");
        }

        sb.Append(gMap_name.GMap_Id + ".setCenter(bounds.getCenter());");

        if (set_zoom)
        {
            sb.Append(gMap_name.GMap_Id + ".setZoom(" + gMap_name.GMap_Id + ".getBoundsZoomLevel(bounds));");
        }

        sb.Append("}");
        gMap_name.Add(sb.ToString());
    }

    public void AddPushpin(double Latitude, double Longitude,string name)
    {
         GLatLng LatLong = new GLatLng((double)Latitude, (double)Longitude);
         GMarker marker = new GMarker(LatLong);
         GInfoWindow window = new GInfoWindow(marker,"<center><b>" + name + "</b></center>", false,GListener.Event.click);
         GM.addInfoWindow(window);
         ListOfMarkers.Add(LatLong);
     }

    public void AutoCenterZoomMap()
    {
        if (GoogleMapTable.Visible)
        {
            if (ListOfMarkers.Any())
                GMAP_BoundingBox(ListOfMarkers, GM, true, true);
            else
                GM.setCenter(DefaultLocation);
        }
    }

    public void setMap(double Latitude, double Longitude, string name)
    {
       
        GM.addControl(new GControl(GControl.preBuilt.MapTypeControl, new GControlPosition(GControlPosition.position.Top_Left)));
        GLatLng LatLong = new GLatLng((double)Latitude, (double)Longitude);
        GMarker marker = new GMarker(LatLong);
        GM.setCenter(LatLong);
    }
    

  


}
