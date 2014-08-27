<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CommonControl_HeadTextBackgroundEffect" Codebehind="HeadTextBackgroundEffect.ascx.cs" %>
<div id="mydiv" runat="server" style="position: absolute; width: 100%; overflow:hidden;">
    <table width="100%" border="0" cellspacing="0" cellpadding="0">
        <tr>
            <td colspan="3" style="height: 8px;">
            </td>
        </tr>
        <tr>
            <td style="width: 14px;" valign="top">
                <img id="Img1" runat="server" src="~/images/left-corner.jpg" alt="left-corner" width="14"
                    height="62" />
            </td>
            <td valign="top" class="topbg" style="width: 100%;">
            </td>
            <td style="width: 14;" valign="top">
                <img id="Img2" runat="server" src="~/images/right-corner.jpg" alt="right-corner"
                    width="14" height="62" />
            </td>
        </tr>
    </table>
</div>
<table>
    <tr>
        <td style="height: 27px;">
            &nbsp;
        </td>
    </tr>
</table>
