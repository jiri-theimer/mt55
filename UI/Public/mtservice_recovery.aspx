<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Anonym.Master" CodeBehind="mtservice_recovery.aspx.vb" Inherits="UI.mtservice_recovery" %>
<%@ MasterType VirtualPath="~/Anonym.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding:20px;">
        <asp:Button ID="cmd1" runat="server" Text="Obnovit účet mtservice" CssClass="cmd" />
        <div class="div6">
            <asp:Label ID="lblMessage" CssClass="infoNotification" runat="server"></asp:Label>
        </div>
    </div>
    
</asp:Content>
