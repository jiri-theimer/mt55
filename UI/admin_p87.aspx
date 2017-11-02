<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="admin_p87.aspx.vb" Inherits="UI.admin_p87" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datalabel" src="~/datalabel.ascx"%>
<%@ Register TagPrefix="uc" TagName="pageheader" src="~/pageheader.ascx"%>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

<asp:Panel ID="panRec" runat="server">
<table cellpadding="4" cellspacing="2">
    
    <tr>
    <td>Další fakturační jazyk #1:</td>
    <td style="width:150px;"><asp:TextBox ID="p87lang1" runat="server" style="width:100px;"></asp:TextBox></td>
    <td><asp:DropDownList ID="p87icon1" runat="server" AutoPostBack="true"></asp:DropDownList></td>
    <td><asp:Image ID="img1" runat="server" /></td>
    </tr>
    <tr>
    <td>Další fakturační jazyk #2:</td>
    <td><asp:TextBox ID="p87lang2" runat="server" style="width:100px;"></asp:TextBox></td>
    <td><asp:DropDownList ID="p87icon2" runat="server" AutoPostBack="true"></asp:DropDownList></td>
    <td><asp:Image ID="img2" runat="server" /></td>
    </tr>
    <tr>
    <td>Další fakturační jazyk #3:</td>
    <td><asp:TextBox ID="p87lang3" runat="server" style="width:100px;"></asp:TextBox></td>
    <td><asp:DropDownList ID="p87icon3" runat="server" AutoPostBack="true"></asp:DropDownList></td>
    <td><asp:Image ID="img3" runat="server" /></td>
    </tr>
    <tr>
    <td>Další fakturační jazyk #4:</td>
    <td><asp:TextBox ID="p87lang4" runat="server" style="width:100px;"></asp:TextBox></td>
    <td><asp:DropDownList ID="p87icon4" runat="server" AutoPostBack="true"></asp:DropDownList></td>
    <td><asp:Image ID="img4" runat="server" /></td>
    </tr>
</table>
</asp:Panel>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
