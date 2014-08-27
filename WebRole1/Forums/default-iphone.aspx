<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default-iphone.aspx.cs" MasterPageFile="IPhone.Master" Inherits="aspnetforum.default_iphone" %>

<asp:Content ContentPlaceHolderID="AspNetForumContentPlaceHolder" runat="server">

<asp:Repeater ID="rptGroupsList" Runat="server" EnableViewState="False" OnItemDataBound="rptGroupsList_ItemDataBound">
	<ItemTemplate>
		<span class="graytitle"><%# Eval("GroupName") %></span>
		<asp:repeater id="rptForumsList" runat="server" EnableViewState="False">
		    <HeaderTemplate><ul class="pageitem"></HeaderTemplate>
			<ItemTemplate>
			    <li class="menu">
			        <a href='<%# aspnetforum.Utils.Various.GetForumURL(Eval("ForumID"), Eval("Title")) %>'>
			            <span class="name"><%# Eval("Title") %></span><span class="arrow"></span></a>
			    </li>
					<%--<%# aspnetforum.Utils.Various.GetForumTopicCount(Convert.ToInt32(Eval("ForumID")), Cmd) %>
					    <%# aspnetforum.Utils.Message.GetMsgInfoByID(Eval("LatestMessageID"), Cmd)%>--%>
			</ItemTemplate>
			<FooterTemplate></ul></FooterTemplate>
		</asp:repeater>
	</ItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater>

</asp:Content>