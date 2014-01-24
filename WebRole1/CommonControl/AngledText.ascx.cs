using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AngledText : System.Web.UI.UserControl
{
    public float TheFontSize { get; set; }
    public float  TheAngle { get; set; }
    public string TheText { get; set; }
    public string TheFontName { get; set; }
    public string TheHilite { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        GeneratedImage1.Parameters.Single(x => x.Name == "TheText").Value = TheText;
        GeneratedImage1.Parameters.Single(x => x.Name == "TheAngle").Value = TheAngle.ToString() ;
        GeneratedImage1.Parameters.Single(x => x.Name == "TheFontSize").Value = TheFontSize.ToString(); 
        GeneratedImage1.Parameters.Single(x => x.Name == "TheFontName").Value = TheFontName;
        GeneratedImage1.Parameters.Single(x => x.Name == "TheHilite").Value = TheHilite;
        
    }
}
