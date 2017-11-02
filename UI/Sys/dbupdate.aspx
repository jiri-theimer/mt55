<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="dbupdate.aspx.vb" Inherits="UI.dbupdate" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 20px; background-color: white;">
        <div class="div6">
            <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Aktualizace aplikační databáze"></asp:Label>
            <asp:Button ID="cmdSP" runat="server" Text="Aktualizovat uložené procedury" CssClass="cmd" style="margin-left:30px;" />
            <asp:Label ID="lblLastSpLog" runat="server"></asp:Label>
        </div>
        <div class="div6">
            <asp:Label ID="lblError" runat="server" CssClass="failureNotification"></asp:Label>
        </div>
        <div class="div6">
            <asp:Button ID="cmdGo" runat="server" CssClass="cmd" Text="Otestovat změny ve struktuře aplikační databáze" />
            <asp:Label ID="lblDbVersion" runat="server"></asp:Label>
            
            <div>
                <asp:Label ID="lblLastRunDifferenceLog" runat="server"></asp:Label>
            </div>
            <div>
                <asp:Button ID="cmdRunResult" runat="server" CssClass="cmd" Text="Spustit aktualizaci změn v db struktuře" Visible="false" Font-Bold="true" />
            </div>
        </div>
        <asp:TextBox ID="txtScript" runat="server" Width="100%" Height="200px" TextMode="MultiLine" ReadOnly="true"></asp:TextBox>

        <asp:panel ID="panMultiDbs" runat="server" CssClass="content-box2" Visible="false">
            <div class="title">MULTIPLE db UPDATE</div>
            <div class="content">
                <asp:DropDownList ID="dbs" runat="server" AutoPostBack="true"></asp:DropDownList>
                <asp:Button ID="cmdCheckDbs" runat="server" CssClass="cmd" Text="Otestovat" />
                <asp:Button ID="cmdRunDbs" runat="server" CssClass="cmd" Text="Spustit aktualizaci" Visible="false" />
                <asp:Button ID="cmdRunSpDbs" runat="server" CssClass="cmd" Text="Uložené procedury" Visible="true" />
                <asp:Label ID="lblDbsMessage" runat="server" CssClass="infoNotificationRed"></asp:Label>
            </div>
           
        </asp:panel>
    </div>

</asp:Content>
