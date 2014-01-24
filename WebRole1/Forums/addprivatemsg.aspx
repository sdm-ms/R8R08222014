<%@ Page language="c#" Codebehind="addprivatemsg.aspx.cs" EnableEventValidation="false" ValidateRequest="false" AutoEventWireup="True" Inherits="aspnetforum.addprivatemsg" MasterPageFile="AspNetForumMaster.Master" %>

<asp:Content ID="ContentHead" runat="server" ContentPlaceHolderID="ContentPlaceHolderHEAD">
<link href="editor.css" rel="Stylesheet" type="text/css" />
</asp:Content>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">

    <div class="location">
	<h2><asp:Label ID="lblPrivateMsg" runat="server" EnableViewState="False" meta:resourcekey="lblPrivateMsgResource1">Send a private message</asp:Label></h2></div>

    <script src="jquery.min.js" type="text/javascript"></script>
    <script src="jquery.MultiFile.js" type="text/javascript"></script>
    <script src="editor.js" type="text/javascript"></script>
	<script type="text/javascript">
	function OpenFiles() {
		OpenCloseDiv('divFilesContainer');
    }
    function OpenCloseDiv(divname) {
        objDiv = document.getElementById(divname);
        if (objDiv.style.display == 'none')
            objDiv.style.display = '';
        else
            objDiv.style.display = 'none';
    }
    function ProcessKeyPress(e) {
        var evt = e ? e : window.event;
        if (evt.ctrlKey) {
            var keyCode = evt.keyCode ? evt.keyCode : evt.which;
            if (keyCode == 13) {
                $("#<%=btnSave.ClientID%>").click();
                return false;
            }
            else
                return true;
        }
        else
            return true;
    }
	</script>
    <div id="divMain" runat="server">
        <div class="richeditor">
		    <div class="editbar">
			    <button title="bold" onclick="doClick('bold');" type="button"><b>B</b></button>
			    <button title="italic" onclick="doClick('italic');" type="button"><i>I</i></button> 
			    <button title="underline" onclick="doClick('underline');" type="button"><u>U</u></button>
			    <button title="hyperlink" onclick="doLink();" type="button" style="background-image:url('images/url.gif');"></button>
			    <button title="list" onclick="doClick('InsertUnorderedList');" type="button" style="background-image:url('images/icon_list.gif');"></button>
			    <button title="image" onclick="doImage();" type="button" style="background-image:url('images/img.gif');"></button>
			    <button title="color" onclick="showColorGrid2('none');" type="button" style="background-image:url('images/colors.gif');"></button><span id="colorpicker201" class="colorpicker201"></span>
			    <button title="quote" onclick="doQuote();" type="button" style="background-image:url('images/icon_quote.png');"></button>
			    <button title="switch to source" type="button" onclick="javascript:SwitchEditor()" style="background-image:url('images/icon_html.gif');"></button>
		    </div>
		    <asp:textbox id="tbMsg" runat="server" Width="100%" style="height:100px;display:none" TextMode="MultiLine"></asp:textbox>
		</div>		
		<script type="text/javascript">
		    initEditor("<%=tbMsg.ClientID%>", <%= (!Request.Browser.IsMobileDevice).ToString().ToLower() %>);
		</script>
		
		<asp:Label ID="lblFileSizeError" runat="server" ForeColor="red" Visible="false" EnableViewState="false">Max attachment size is </asp:Label>
		<asp:Label ID="lblMaxSize" runat="server" ForeColor="red" Visible="false" EnableViewState="false"></asp:Label>
		<table style="width:100%;">
			<tr>
				<th>
					<asp:button CssClass="gradientbutton" id="btnSave" runat="server" Text="send message" onclick="btnSave_Click" OnClientClick="doCheck();return true;" meta:resourcekey="btnSaveResource1" Font-Bold="true"></asp:button>
					<span class="gray">(ctrl+enter)</span>
				    &nbsp;&nbsp;&nbsp;
					<button type="button" class="gradientbutton" onclick="doCheck();ShowPreview(); return false;"><asp:Label ID="lblPreview" runat="server" EnableViewState="false" meta:resourcekey="btnPreviewResource1">preview</asp:Label></button>
					<script type="text/javascript">
					    function ShowPreview() {
					        //jquery ajax post
					        $.post(
					        "addpostajax.ashx", //url
					        {messagetext: textboxelement.value, mode: "preview" }, //name-values to post
					        function(data) { $("#divPreview").get(0).innerHTML = data; }, //callback
					        "html");   //returned datatype
					    }
					</script>
					<button type="button" onclick="history.back()" class="gradientbutton"><asp:Label ID="lblCancel" runat="server" EnableViewState="False" meta:resourcekey="lblCancelResource1">cancel</asp:Label></button>
				</th>
			</tr>
		</table>
		
		<div id="divFiles" runat="server">
		    <hr />
		    <a href="javascript:OpenFiles()"><asp:Label ID="lblAttach" runat="server" EnableViewState="False" meta:resourcekey="lblAttachResource1">attach files</asp:Label></a>
		    <div id="divFilesContainer" style="display:none;">
		        <asp:FileUpload id="fileUpload" runat="server" CssClass="multi" />
	        </div>
		</div>
		
		<br />
		<table width="100%">
			<tr><td><div id="divPreview"></div></td></tr>
		</table>
    </div>
    <asp:Label ID="lblError" runat="server" ForeColor="red" Font-Bold="true" Visible="false" EnableViewState="false" meta:resourcekey="lblErrorResource1" Text="User not found or you are not logged in"/>
</asp:Content>