<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="periodmonth.ascx.vb" Inherits="UI.periodmonth" %>
<asp:DropDownList ID="query_year" runat="server" AutoPostBack="true"></asp:DropDownList>
<asp:DropDownList ID="query_month" runat="server" AutoPostBack="true">
    <asp:ListItem Text="Leden" Value="1"></asp:ListItem>
    <asp:ListItem Text="Únor" Value="2"></asp:ListItem>
    <asp:ListItem Text="Březen" Value="3"></asp:ListItem>
    <asp:ListItem Text="Duben" Value="4"></asp:ListItem>
    <asp:ListItem Text="Květen" Value="5"></asp:ListItem>
    <asp:ListItem Text="Červen" Value="6"></asp:ListItem>
    <asp:ListItem Text="Červenec" Value="7"></asp:ListItem>
    <asp:ListItem Text="Srpen" Value="8"></asp:ListItem>
    <asp:ListItem Text="Září" Value="9"></asp:ListItem>
    <asp:ListItem Text="Říjen" Value="10"></asp:ListItem>
    <asp:ListItem Text="Listopad" Value="11"></asp:ListItem>
    <asp:ListItem Text="Prosinec" Value="12"></asp:ListItem>
</asp:DropDownList>

