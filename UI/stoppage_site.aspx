<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="stoppage_site.aspx.vb" Inherits="UI.stoppage_site" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Image ImageUrl="~/Images/exclaim_32.png" ID="img1" runat="server" />


<asp:Label ID="lblMessage" runat="server" CssClass="failureNotification"></asp:Label>

</asp:Content>
