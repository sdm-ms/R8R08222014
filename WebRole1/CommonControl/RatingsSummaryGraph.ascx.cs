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

using System.Web.UI.DataVisualization.Charting;
using System.Globalization;
using System.Collections.Generic;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;


public class ColumnAndRating
{
    public string Column;
    public Guid ColumnID;
    public decimal Rating;
}

public partial class RatingsSummaryGraph : System.Web.UI.UserControl
{
    public List<ColumnAndRating> TheInfo { get; set; }

    internal R8RDataAccess DataAccess = new R8RDataAccess();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (TheInfo != null)
        {
            Manual_Setup(TheInfo);
        }
    }

    public void Manual_Setup(List<ColumnAndRating> theInfo)
    {
        Series series = new Series("Default");
        var xValues = theInfo.Select(x => x.Column).ToList();
        var yValues = theInfo.Select(y => y.Rating).ToList();
        series.Points.DataBindXY(xValues, yValues);

        // Add series into the chart's series Tbl
        Chart1.Series.Add(series);
        // Set series chart type
        Chart1.Series["Default"].ChartType = SeriesChartType.Column;

        // Set series point width
        Chart1.Series["Default"]["PointWidth"] = "0.6";

        // Show data points labels
        Chart1.Series["Default"].IsValueShownAsLabel = true;

        // Set data points label style
        Chart1.Series["Default"]["BarLabelStyle"] = "Center";

        // Show as 3D
        Chart1.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = false;

        Chart1.ChartAreas["ChartArea1"].AxisY.Minimum = (double)0;
        Chart1.ChartAreas["ChartArea1"].AxisY.Maximum = (double)10;

        if (Chart1.Legends.Any())
        {
            var theLegend = Chart1.Legends.First();
            Chart1.Legends.Remove(theLegend);
        }
    }


}
