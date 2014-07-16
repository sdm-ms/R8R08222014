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
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;

public partial class MainAnnounceTab : System.Web.UI.UserControl
{
    ActionProcessor Obj = new ActionProcessor();
 
    Admin_Announcements_AddInsertableContents theInsertableContentsControl;
    public Guid? TableId = null;
    public Guid? SubtopicId = null;
    public Guid? TopicId = null;
    public Guid? AnnounceId = null;
     public bool createAnnounceVisible;
     public new Guid? changeAnnounceVisible;
  
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ViewState["createAnnounceVisible"] != null)
            createAnnounceVisible = (bool) ViewState["createAnnounceVisible"];
        if (ViewState["changeAnnounceVisible"] != null)
            changeAnnounceVisible = (Guid?)ViewState["changeAnnounceVisible"];
        DisplayModalPopupIfNecessary();
    }

    public void Setup(Guid? TblID, Guid? pointsManagerID, Guid? domainID)
    {
        TableId = TblID;
        SubtopicId = pointsManagerID;
        TopicId = domainID;

        if (TableId != null || SubtopicId != null)
        {

            var GetInsertableContents = Obj.DataContext.GetTable<InsertableContent>()
                                        .Where(x => x.PointsManagerID == SubtopicId || x.TblID == TableId)
                                        .Select(x => new { Name = x.Name, Id = x.InsertableContentID, Status = x.Status, TopicId = x.DomainID });
            if (GetInsertableContents.Count() > 0)
            {
                Table AnnouncementsTable = new Table();
                foreach (var Announce in GetInsertableContents)
                {


                    TableRow AnnounceRow = new TableRow();
                    TableCell AnnounceCell1 = new TableCell();
                    AnnounceCell1.CssClass = "arial11blue";

                    if (Announce.Status == (byte)StatusOfObject.Active)
                    {
                        AnnounceCell1.Text = Announce.Name;
                    }
                    else
                    {
                        AnnounceCell1.Text = Announce.Name + "[inactive]";

                    }


                    TableCell AnnounceCell2 = new TableCell();

                    Button BtnChangeAnnouncement = new Button();
                    BtnChangeAnnouncement.Text = "Change Announcement";
                    BtnChangeAnnouncement.CssClass = "Btn1";
                    BtnChangeAnnouncement.CausesValidation = false;
                    BtnChangeAnnouncement.ID = "BtnChangeAnnouncement" + Announce.Id.ToString();
                    BtnChangeAnnouncement.Click += new EventHandler(BtnChangeAnnouncement_Click);
                    BtnChangeAnnouncement.CommandArgument = Announce.Id.ToString();
                    AnnounceCell2.Controls.Add(BtnChangeAnnouncement);
                    TableCell AnnounceCell3 = new TableCell();
                    Image DotImage = new Image();
                    DotImage.ImageUrl = "~/images/dot.jpg";
                    AnnounceCell3.Controls.Add(DotImage);

                    AnnounceRow.Controls.Add(AnnounceCell3);
                    AnnounceRow.Cells.Add(AnnounceCell1);
                    AnnounceRow.Cells.Add(AnnounceCell2);
                    AnnouncementsTable.Rows.Add(AnnounceRow);
                }

                AnnouncePlaceHolder.Controls.Add(AnnouncementsTable);

            }
        }
   
        if (TopicId != null)
        {
           SubtopicId= null;
            TableId = null;
            var GetDomainInsertableContents = Obj.DataContext.GetTable<InsertableContent>()
                    .Where(x => x.DomainID == TopicId || (x.DomainID == null && x.PointsManagerID == null && x.TblID == null))
                    .Select(x => new { Name = x.Name, Id = x.InsertableContentID, Status = x.Status, TopicId = x.DomainID });
            if (GetDomainInsertableContents.Count() > 0)
            {
                Table AnnouncementsTable = new Table();
                foreach (var Announce in GetDomainInsertableContents)
                {
                    TableRow AnnounceRow = new TableRow();
                    TableCell AnnounceCell1 = new TableCell();
                    if (Announce.Status == (byte)StatusOfObject.Active)
                    {
                        if (Announce.TopicId == null)
                        {

                            AnnounceCell1.Text = Announce.Name + "[everywhere]";
                        }
                        else
                        {
                            AnnounceCell1.Text = Announce.Name + "[" + Obj.DataAccess.GetDomain((Guid)TopicId).Name + "]";
                        }
                    }
                    else
                    {
                        if (Announce.TopicId == null)
                        {
                            AnnounceCell1.Text = Announce.Name + "[everywhere][inactive]";
                        }
                        else
                        {
                            AnnounceCell1.Text = Announce.Name + "[" + Obj.DataAccess.GetDomain((Guid)TopicId).Name + "][inactive]";
                        }
                    }
                    TableCell AnnounceCell2 = new TableCell();
                    Button BtnChangeAnnouncement = new Button();
                    BtnChangeAnnouncement.Text = "Change Announcement";
                    BtnChangeAnnouncement.CssClass = "Btn1";
                    BtnChangeAnnouncement.CausesValidation = false;
                    BtnChangeAnnouncement.ID = "BtnChangeAnnouncement" + Announce.Id.ToString();
                    BtnChangeAnnouncement.Click += new EventHandler(BtnChangeAnnouncement_Click);
                    BtnChangeAnnouncement.CommandArgument = Announce.Id.ToString();
                    AnnounceCell2.Controls.Add(BtnChangeAnnouncement);
                    TableCell AnnounceCell3 = new TableCell();
                    Image DotImage = new Image();
                    DotImage.ImageUrl = "~/images/dot.jpg";
                    AnnounceCell3.Controls.Add(DotImage);

                    AnnounceRow.Controls.Add(AnnounceCell3);
                    AnnounceRow.Cells.Add(AnnounceCell1);
                    AnnounceRow.Cells.Add(AnnounceCell2);
              


                    AnnouncementsTable.Rows.Add(AnnounceRow);
                }
                AnnouncePlaceHolder.Controls.Add(AnnouncementsTable);
            }
            else
            {
                Label LblNone = new Label();
                LblNone.Text = "None";
                AnnouncePlaceHolder.Controls.Add(LblNone);
            }

        }


        CreateModalPopup();
    }
    
    void BtnChangeAnnouncement_Click(object sender, EventArgs e)
    {

        Button BtnChange = (Button)sender;
        Guid AnnounceId = new Guid(BtnChange.CommandArgument);
        createAnnounceVisible = false;
        changeAnnounceVisible = AnnounceId;
        ViewState["createAnnounceVisible"] = false;
        ViewState["changeAnnounceVisible"] = AnnounceId;
        DisplayModalPopupIfNecessary();
  
    }
    void CreateModalPopup()
    {
       
      theInsertableContentsControl = (Admin_Announcements_AddInsertableContents)LoadControl("~/Admin/Announcements/AddInsertableContents.ascx");
        AnnouncePopupPlaceHolder.Controls.Add(theInsertableContentsControl);
        theInsertableContentsControl.SetupInitial();
                
    }
    void DisplayModalPopupIfNecessary()
    {
        if (createAnnounceVisible)
        {
            theInsertableContentsControl.SetupInsertableContent(TopicId, SubtopicId, TableId, null);
            theInsertableContentsControl.ShowUserControl();
        }
        else if (changeAnnounceVisible != null)
        {
            theInsertableContentsControl.SetupInsertableContent(TopicId, SubtopicId, TableId, changeAnnounceVisible);
            theInsertableContentsControl.ShowUserControl();
        }
    }

    protected void BtnNewAnnouncement_Click(object sender, EventArgs e)
    {
        createAnnounceVisible = true;
        changeAnnounceVisible = null;
        ViewState["createAnnounceVisible"] = true;
        ViewState["changeAnnounceVisible"] = null;
        DisplayModalPopupIfNecessary();
    }

    
}
