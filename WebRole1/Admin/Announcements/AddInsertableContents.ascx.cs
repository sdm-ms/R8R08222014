using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Nonmodel_Code;


public partial class Admin_Announcements_AddInsertableContents : System.Web.UI.UserControl
{
    ActionProcessor Obj = new ActionProcessor();
    public static int NumTable = 0;
    public Guid? TopicId = null;
    public Guid? SubtopicId = null;
    public Guid? TableId = null;
    public Guid? announce = null;

   
   
    protected void Page_Load(object sender, EventArgs e)
    {
        Buttons.okHandler += new CommonControl_PageButtons.OnButtonClick(implementannouncement);
        Buttons.cnlHandler += new CommonControl_PageButtons.OnButtonCancle(Cancelannouncement);

       
       
    }
    public void SetupInitial()
    {
        
    }
    public void SetupInsertableContent(Guid? domainID, Guid? pointsManagerID, Guid? TblID, Guid? AnnounceId)
    {
        TopicId = domainID;
        SubtopicId = pointsManagerID;
        TableId = TblID;
        announce = AnnounceId;
      
        if (domainID != null)
        {
            TopicId = domainID;
            RButtonEveryWhere.Visible = true;
        }
        if (pointsManagerID != null)
        {
             SubtopicId = pointsManagerID;
             RButtonEveryWhere.Visible =false;
             TableId = null;

        }
        if (TblID != null)
        {
            TableId = TblID;
            RButtonEveryWhere.Visible = false;
            TopicId = null;
            SubtopicId = null;
        }

        if (AnnounceId != null)
        {
            announce = AnnounceId;
            Tdannouc.InnerHtml = "Change new Announcements";

            Guid userID = (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");

            var ObjInsertableContent = Obj.DataAccess.GetInsertableContents((Guid)AnnounceId);
                TxtName.Text = ObjInsertableContent.Name;
                TxtContent.Text = ObjInsertableContent.Content;
                ChkIncludeHtml.Checked = !ObjInsertableContent.IsTextOnly;
                ChkOverridable.Checked = ObjInsertableContent.Overridable;
                if (ObjInsertableContent.Status == (byte)StatusOfObject.Active)
                {
                    ChkActivated.Checked = true;
                }
                else
                {
                    ChkActivated.Checked = false;
                }

                if (ObjInsertableContent.DomainID == null && ObjInsertableContent.PointsManagerID == null && ObjInsertableContent.TblID == null)
                {
                    RButtonEveryWhere.Checked = true;
                    RButtonLocation.Checked = false;
                    ListRow.Visible = false;
                }

            }
                  
        }


    protected void RButtonEveryWhere_CheckedChanged(object sender, EventArgs e)
    {
        if (RButtonEveryWhere.Checked)
        {
            RButtonLocation.Checked = false;
         }
       else
        {
            RButtonLocation.Checked = true;
          
        }
    }
    protected void RButtonLocation_CheckedChanged(object sender, EventArgs e)
    {
        if (RButtonLocation.Checked)
        {
            RButtonEveryWhere.Checked = false;
            
        }
        else
        {
            RButtonEveryWhere.Checked = true;
           
        }
    }
    protected void implementannouncement()
    {

        Guid UserId = (Guid)ClassLibrary1.Nonmodel_Code.UserProfileCollection.GetCurrentUser().GetProperty("UserID");

        Guid? ChangeGroupId = null;
        string Name = TxtName.Text;
        string Content = TxtContent.Text;
        bool IsTextOnly = !ChkIncludeHtml.Checked;
        bool IsActivate = ChkActivated.Checked;
        bool IsOverridable = ChkOverridable.Checked;
        InsertableLocation Location = InsertableLocation.TopOfViewTblContent;
        User_Control_ModalPopUp PoPUp = (User_Control_ModalPopUp)Buttons.FindControl("PoPUp");
       
        if (announce != null)
        {


            Guid InsertableContentID = (Guid)announce;
            Obj.InsertableContentChange(InsertableContentID, Name, Content, IsTextOnly, IsOverridable, Location, IsActivate, UserId, ChangeGroupId);
            User_Control_ModalPopUp PopUp = (User_Control_ModalPopUp)Buttons.FindControl("PopUp");
            PopUp.MsgString = StringConstants.StringAnounceChange;
            PopUp.Show();
        }
        else
        {
            if (RButtonEveryWhere.Checked == false)
            {
                Obj.InsertableContentCreate(TopicId, SubtopicId, TableId, Name, Content, IsTextOnly, IsOverridable, Location, IsActivate, UserId, ChangeGroupId);
                PoPUp.MsgString = StringConstants.StringAnounceCreated;
                PoPUp.Show();
            }
            else if (RButtonEveryWhere.Checked == true)
            {
                Obj.InsertableContentCreate(null, null, null, Name, Content, IsTextOnly, IsOverridable, Location, IsActivate, UserId, ChangeGroupId);
                PoPUp.MsgString = StringConstants.StringAnounceCreated;
                PoPUp.Show();
            }
        }
        if (TableId != null)
        {

            PoPUp.MsgString = StringConstants.StringAnounceCreated;
            PoPUp.Show();
            Tbl theTbl = Obj.DataContext.GetTable<Tbl>().FirstOrDefault(x => x.TblID == TableId);
            Routing.Redirect(Response, new RoutingInfoMainContent(theTbl, null, null, false, false, false, false, false, true, false));
           
        }
        else if (SubtopicId != null)
        {
            
            PoPUp.MsgString = StringConstants.StringAnounceCreated;
            PoPUp.Show();
            Tbl theTbl = Obj.DataContext.GetTable<Tbl>().FirstOrDefault(x => x.PointsManagerID == SubtopicId && x.Name != "Changes");
            Routing.Redirect(Response, new RoutingInfoMainContent(theTbl, null, null, false, false, false, false, false, false, true));
                      
        }
        else if (TopicId != null)
        {
            PoPUp.MsgString = StringConstants.StringAnounceCreated;
            PoPUp.Show();
            //Response.Redirect("~/admin/domain/changeDoWebForms/Main.aspx?Domainid=" + TopicId, false);
        }


      

    }
    
    public void ShowUserControl()
    {
        AnnouncePopupContent.Visible = true;
        EmptyButton.Visible = true;
        AnnounceExtender.Show();
    }

    public void HideUserControl()
    {
        AnnouncePopupContent.Visible = false;
        EmptyButton.Visible = false;
        AnnounceExtender.Hide();
    }
    protected void Cancelannouncement()
    {
        HideUserControl();

    }
}
    

   

