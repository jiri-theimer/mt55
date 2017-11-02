<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="stoppage.aspx.vb" Inherits="UI.stoppage" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:Image ImageUrl="~/Images/exclaim_32.png" ID="img1" runat="server" />


<asp:Label ID="lblMessage" runat="server" CssClass="failureNotification"></asp:Label>

<div style="padding-top:20px;">
<asp:Label ID="lblNeededPerms" runat="server" CssClass="failureNotification" ForeColor="blue"></asp:Label>
</div>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
