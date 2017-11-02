<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="kurzy_cnb.aspx.vb" Inherits="UI.kurzy_cnb" %>
<%@ MasterType VirtualPath="~/Clue.Master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <asp:Label ID="Hlavicka" runat="server" CssClass="valbold"></asp:Label>
            <telerik:RadDatePicker ID="Datum" runat="server" RenderMode="Lightweight" Width="120px" AutoPostBack="true">
                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
            </telerik:RadDatePicker>
        <asp:textbox ID="txtKody" runat="server" Text="EUR,USD,GBP,CAD" ToolTip="Seznam sledovaných měn"></asp:textbox>
        <asp:ImageButton ID="cmdSave" ImageUrl="Images/save.png" runat="server" ToolTip="Uložit výběr sledovaných měn" />
    </div>


    <table class="tabulka" cellpadding="3" cellspacing="2">
        <tr>
            <th>Země</th>
            <th>Měna</th>
            <th>Kód</th>
            <th>Kurz</th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr class="trHover">
                    <td><%#Eval("Zeme")%></td>
                    <td><%#Eval("Mena")%></td>
                    <td><%#Eval("Kod")%></td>
                    <td align="right"><%#Eval("Kurz")%></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</asp:Content>
