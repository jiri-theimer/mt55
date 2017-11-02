<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="admin_robot.aspx.vb" Inherits="UI.admin_robot" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
   
    <div class="div6">
        <asp:CheckBox ID="chkUseRobot" runat="server" Text="Používat robota" Checked="true" AutoPostBack="true" />
    </div>
    <asp:Panel ID="panRec" runat="server">
        <table cellpadding="6" cellspacing="2">
           
            <tr valign="top">
                <td>
                    <asp:Label ID="lblrobot_host" runat="server" Text="Host URL:"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="robot_host" runat="server" Style="width: 400px;"></asp:TextBox>
                    <asp:Label ID="lblHostHelp" CssClass="infoInForm" runat="server"></asp:Label>
                </td>
            </tr>

           
            <tr>
                <td>
                    <asp:Label ID="lblrobot_cache_timeout" runat="server" Text="Period spouštění robota:"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="robot_cache_timeout" runat="server">
                        <asp:ListItem Text="30 sekund" Value="30"></asp:ListItem>
                        <asp:ListItem Text="60 sekund" Value="60"></asp:ListItem>
                        <asp:ListItem Text="120 sekund" Value="120"></asp:ListItem>
                        <asp:ListItem Text="180 sekund" Value="180"></asp:ListItem>
                        <asp:ListItem Text="240 sekund" Value="240"></asp:ListItem>
                        <asp:ListItem Text="300 sekund" Value="300" Selected="true"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>

        </table>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

