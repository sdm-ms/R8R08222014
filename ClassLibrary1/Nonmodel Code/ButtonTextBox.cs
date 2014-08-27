//Author: © Luis Ramirez 2008
//Web site: http://www.sqlnetframework.com
//Creation date: Feb 29, 2008

using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace SqlNetFrameworkWebControls
{
    public class ButtonTextBox : TextBox, IPostBackEventHandler
    {
        private static readonly object EventCommand = new object();

        public ButtonTextBox()
        {
            base.AutoPostBack = true;
        }

        public string CommandArgument
        {
            get
            {
                string str = (string)this.ViewState["CommandArgument"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["CommandArgument"] = value;
            }
        }

        public string CommandName
        {
            get
            {
                string str = (string)this.ViewState["CommandName"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["CommandName"] = value;
            }
        }

        #region IPostBackEventHandler implementation
        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.CommandArgument = base.Text;
            this.RaisePostBackEvent(eventArgument);
        }
        #endregion

        protected virtual void OnCommand(CommandEventArgs e)
        {
            CommandEventHandler handler = (CommandEventHandler)base.Events[EventCommand];
            if (handler != null)
            {
                handler(this, e);
            }
            //It bubbles the event to the HandleEvent method of the GooglePagerField class.
            base.RaiseBubbleEvent(this, e);
        }

        protected virtual void RaisePostBackEvent(string eventArgument)
        {
            if (this.CausesValidation)
            {
                this.Page.Validate(this.ValidationGroup);
            }
            this.OnCommand(new CommandEventArgs(this.CommandName, this.CommandArgument));
        }
    }
}