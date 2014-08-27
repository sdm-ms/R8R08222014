<%@ Page language="c#" Codebehind="addpost.aspx.cs" EnableEventValidation="false" ValidateRequest="false" AutoEventWireup="True" Inherits="aspnetforum.addpost" MasterPageFile="AspNetForumMaster.Master" %>

<asp:Content ID="ContentHead" runat="server" ContentPlaceHolderID="ContentPlaceHolderHEAD">
<link href="editor.css" rel="Stylesheet" type="text/css" />
</asp:Content>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" ID="AspNetForumContent" runat="server">
    <div class="location">
	<h2><asp:Label ID="lblNewPost" runat="server" EnableViewState="False" meta:resourcekey="lblNewPostResource1">Add new post</asp:Label></h2></div>
	
    <script src="jquery.min.js" type="text/javascript"></script>
    <script src="jquery.MultiFile.js" type="text/javascript"></script>
    <script src="editor.js" type="text/javascript"></script>
	<script type="text/javascript">
	function OpenSmilies() {
		OpenCloseDiv('divSmilies');
	}
	function OpenFiles() {
		OpenCloseDiv('divFilesContainer');
	}
	function OpenPolls() {
		OpenCloseDiv('divPollsContainer');
	}
	function OpenMoreSmilies() {
		window.open("smilies.htm", null, "height=200,width=400,status=yes,toolbar=no,menubar=no,location=no,scrollbars=yes");
	}
	function OpenCloseDiv(divname) {
		objDiv = document.getElementById(divname);
		if (objDiv.style.display == 'none')
			objDiv.style.display = '';
		else
			objDiv.style.display = 'none';
	}
    var numOptions=1;
    function AddOptionInput() {
        optionsDiv = document.getElementById("divOptions");
        opt = document.createElement("INPUT");
        opt.type = "text";
        opt.name = "PollOption" + numOptions;
        br = document.createElement("br");
        lbl = document.createTextNode("Poll option: ");
        numOptions++;
        optionsDiv.appendChild(br);
        optionsDiv.appendChild(lbl);
        optionsDiv.appendChild(opt);
    }
    function ClearPoll() {
        $("input[name^=PollOption]").val("");
        $("[id$='tbPollQuestion']").val("");
        OpenPolls();
    }
    //processes ctrl+enter
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
    //processes TAB
    function ProcessSubjectKeyPress(e) {
        //if TAB pressed - move focus to editor
        var evt = e ? e : window.event;
        var keyCode = evt.keyCode ? evt.keyCode : evt.which;
        //if TAB
        if (keyCode == 9) {
            try { document.getElementById('rte').contentWindow.focus(); } //wysiwyg
            catch (e) { document.getElementById("<%=tbMsg.ClientID%>").focus(); } //textarea
            
            //prevent processing
            evt.returnValue = false; // for IE
            if (evt.preventDefault) evt.preventDefault(); // for Mozilla
        }
    }
    window.setInterval("renewSession();", 9000);
    function renewSession()
    {
        try { document.images("renewSession").src = "renewsession.ashx?par1=" + Math.random() + "&par2=" + Math.random(); }
        catch(e) { }
    }
	</script>
	<asp:Label ID="lblDenied" Runat="server" Visible="False" Font-Bold="True" ForeColor="Red" meta:resourcekey="lblDeniedResource1">Posting to this forum is restricted</asp:Label>
	<div id="divMain" runat="server">
        <table style="width:100%" class="noouterborder" cellpadding="0" cellspacing="0">
			<tr>
			<td class="outerheader"><img src="images/menuleftedge.gif" alt="" /></td>
			<th style="white-space:nowrap"><asp:label id="lblSubject" runat="server" EnableViewState="False" meta:resourcekey="lblSubjectResource1">Subject:</asp:label></th>
			<th width="100%">
			<asp:Label ID="lblSubjectText" runat="server" EnableViewState="false"></asp:Label>
			<asp:textbox id="tbSubj" onkeydown="return ProcessSubjectKeyPress(event);" runat="server" Visible="False" MaxLength="50"></asp:textbox></th>
			<td class="outerheader"><img src="images/menurightedge.gif" alt="" /></td>
			</tr>			
		</table>
		<div class="richeditor">
		    <div class="editbar">
		        <button title="bold" onclick="doClick('bold');" type="button"><b>B</b></button>
			    <button title="italic" onclick="doClick('italic');" type="button"><i>I</i></button>
			    <button title="underline" onclick="doClick('underline');" type="button"><u>U</u></button>
			    <button title="hyperlink" onclick="doLink();" type="button" style="background-image:url('images/url.gif');"></button>
			    <button title="list" onclick="doClick('InsertUnorderedList');" type="button" style="background-image:url('images/icon_list.gif');"></button>
			    <button title="image" onclick="doImage();" type="button" style="background-image:url('images/img.gif');"></button>
			    <button title="color" onclick="showColorGrid2('none')" type="button" style="background-image:url('images/colors.gif');"></button><span id="colorpicker201" class="colorpicker201"></span>
			    <button title="smilies" onclick="OpenSmilies()" type="button" runat="server" id="btnSmilies" enableviewstate="false" style="background-image:url('images/smilies/smile.gif');"></button>
			    <span id="divSmilies" style="DISPLAY:none;position:absolute;border:1px solid slategray;background:#ffffff;padding: 5px 5px 5px 5px">
			        <img style="cursor:pointer" onclick="InsertSmile(':)')" src="images/smilies/smile.gif" alt=":)" />
			        <img style="cursor:pointer" onclick="InsertSmile(';)')" src="images/smilies/wink.gif" alt=";)" />
			        <img style="cursor:pointer" onclick="InsertSmile(':(')" src="images/smilies/upset.gif" alt=":(" />
			        <img style="cursor:pointer" onclick="InsertSmile(':cool:')" src="images/smilies/1cool.gif" alt="cool" />
			        <img style="cursor:pointer" onclick="InsertSmile(':\\:')" src="images/smilies/eek7.gif" alt="Whaaaaa?" />
			        <br />
			        <a href="javascript:OpenMoreSmilies()"><asp:Label ID="lblMoreSmilies" runat="server" EnableViewState="False" meta:resourcekey="lblMoreSmiliesResource1">more smilies</asp:Label></a>
		        </span>
		        <button title="quote" onclick="doQuote();" type="button" style="background-image:url('images/icon_quote.png');"></button>
		        <button title="youtube" onclick="InsertYoutube();" type="button" style="background-image:url('images/icon_youtube.gif');"></button>
		        <button title="switch to source" type="button" onclick="javascript:SwitchEditor()" style="background-image:url('images/icon_html.gif');"></button>
		    </div>
		    <asp:textbox id="tbMsg" runat="server" onkeydown="return ProcessKeyPress(event);" TextMode="MultiLine" height="100px"></asp:textbox>
		</div>
		<script type="text/javascript">
		    initEditor("<%=tbMsg.ClientID%>", <%= (!Request.Browser.IsMobileDevice).ToString().ToLower() %>);
		</script>
	</div>
		
	<div id="divCaptcha" runat="server" enableviewstate="false" visible="false">
	    <img alt="" src="captchaimage.ashx" /> :: <asp:textbox id="tbImgCode" autocomplete="off" runat="server"></asp:textbox>
	</div>
	<asp:Label ID="lblFileSizeError" runat="server" ForeColor="red" Visible="false" EnableViewState="false" meta:resourcekey="lblFileSizeErrorResource1">Max attachment size</asp:Label>
	<asp:Label ID="lblMaxSize" runat="server" ForeColor="red" Visible="false" EnableViewState="false"></asp:Label>
	<table style="width:100%;"class="noouterborder" cellpadding="0" cellspacing="0">
		<tr>
		    <td class="outerheader"><img src="images/menuleftedge.gif" alt="" /></td>
			<th>
			    <asp:checkbox id="cbSubscribe" runat="server" Text="Notify me when a reply is posted" Font-Bold="false" meta:resourcekey="cbSubscribeResource1"></asp:checkbox>
			    &nbsp;&nbsp;&nbsp;
				<asp:button CssClass="gradientbutton" id="btnSave" runat="server" Text="add message" Font-Bold="True" onclick="btnSave_Click" OnClientClick="doCheck();return true;" meta:resourcekey="btnSaveResource1"></asp:button>
				<span class="gray">(ctrl+enter)</span>
				&nbsp;&nbsp;&nbsp;
				<button type="button" class="gradientbutton" onclick="doCheck();ShowPreview(); return false;"><asp:Label ID="lblPreview" runat="server" EnableViewState="false" meta:resourcekey="btnPreviewResource1">preview</asp:Label></button>
				<script type="text/javascript">
				function ShowPreview()
				{
				    //jquery ajax post
				    $.post(
				        "addpostajax.ashx", //url
				        {messagetext: textboxelement.value, mode: "preview" }, //name-values to post
				        function(data) { $("#divPreview").get(0).innerHTML = data; }, //callback
				        "html");   //returned datatype
				}
				</script>
				<button class="gradientbutton" onclick="history.back()" type="button"><asp:Label ID="lblCancel" runat="server" EnableViewState="false" meta:resourcekey="lblCancelResource1">cancel</asp:Label></button>
			</th>
			<td class="outerheader"><img src="images/menurightedge.gif" alt="" /></td>
		</tr>
	</table>
	<asp:Repeater ID="rptExistingFiles" runat="server" EnableViewState="false" Visible="false">
    <HeaderTemplate>
		<br />
        <script type="text/javascript">
            function DelFile(ifileId) {
                if (!confirm('are you sure?')) return;
                //jquery ajax post
                $.post(
		        "addpostajax.ashx", //url
		        {FileID: ifileId, mode: "delfile" });
		        //remove element from page
		        $("#spanFileID" + ifileId).remove();
            }
		</script>
		<asp:Label ID="lblAttachments" runat="server" EnableViewState="False" meta:resourcekey="lblAttachmentsResource1">Attachments:</asp:Label><br />
    </HeaderTemplate>
    <ItemTemplate>
        <div id='spanFileID<%# Eval("FileID") %>'><a href='javascript:DelFile(<%# Eval("FileID") %>)' title="delete attachment" style="color:Red">x</a> <%# Eval("FileName") %>; </div>
    </ItemTemplate>
    </asp:Repeater>
	<div id="divFiles" runat="server">
	    <hr />
	    <a href="javascript:OpenFiles()"><asp:Label ID="lblAttach" runat="server" EnableViewState="False" meta:resourcekey="lblAttachResource1">attach files</asp:Label></a>
	    <div id="divFilesContainer" style="display:none;">
	        <asp:FileUpload id="fileUpload" runat="server" CssClass="multi" />
        </div>
	</div>
	<div id="divPolls" runat="server">
	    <hr />
	    <a href="javascript:OpenPolls()">
	        <asp:Label ID="lblCreatePoll" runat="server" EnableViewState="false" meta:resourcekey="lblCreatePollResource1">create a poll</asp:Label>
	    </a>
	    <div id="divPollsContainer" style="DISPLAY:none;">
	        <b><asp:Label ID="lblPollQuestion" runat="server" EnableViewState="false" meta:resourcekey="lblPollQuestionResource1">Poll Question</asp:Label></b>: <asp:TextBox ID="tbPollQuestion" runat="server"></asp:TextBox>
	        <br />
	        <div id="divOptions"><asp:Label ID="lblPollOption" runat="server" EnableViewState="false" meta:resourcekey="lblPollOptionResource1">Poll option</asp:Label>: <input type="text" name="PollOption0" /></div>
	        <button onclick="javascript:AddOptionInput();" type="button" class="gradientbutton"><asp:Label ID="lblAddOption" runat="server" EnableViewState="false" meta:resourcekey="lblAddOptionResource1">add an option...</asp:Label></button>
	        <button onclick="javascript:ClearPoll();" type="button" class="gradientbutton"><asp:Label ID="lblCancelOption" runat="server" EnableViewState="false" meta:resourcekey="lblCancelResource1">cancel</asp:Label></button>
	    </div>
	</div>
	<br />
	<table width="100%" cellpadding="0" cellpadding="0">
		<tr><td><div id="divPreview"></div></td></tr>
	</table>
	<asp:Repeater ID="rptMessages" runat="server">
	    <HeaderTemplate>
	        <br /><br />
            <asp:Label ID="lblPrevMsgs" runat="server" EnableViewState="False" meta:resourcekey="lblPrevMsgsResource1">Previous messages in the topic:</asp:Label>
            <table style="width:100%;" cellpadding="12">
	    </HeaderTemplate>
        <ItemTemplate>
            <tr><td valign="top" style="width:120px"><b><%# Eval("UserName") %></b><br /><%# Eval("CreationDate") %></td>
            <td><%# aspnetforum.Utils.Formatting.FormatMessageHTML(Eval("Body").ToString())%></td></tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
	</asp:Repeater>

	<img src="renewsession.ashx" id="renewSession" alt="" />
	<script type="text/javascript">
	//focus inputs onload:
	var tbSubjId = '<%= tbSubj.ClientID %>';
	try { document.getElementById(tbSubjId).focus(); }
	catch (err) {
	    try { document.getElementById('rte').focus(); }
	    catch(err1) { }
	}
	</script>
</asp:Content>