<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_j02_month.aspx.vb" Inherits="UI.clue_j02_month" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodmonth" Src="~/periodmonth.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugins/Plugin.css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panContainer" runat="server" Style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <asp:Panel ID="panHeader" runat="server">
            <img src="Images/person_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
            <asp:Label ID="Mesic" runat="server" CssClass="valboldblue" style="padding-left:60px;"></asp:Label>
        </asp:Panel>
        <table>
            <tr>
                <td>
                    <asp:RadioButtonList ID="opgView" runat="server" RepeatDirection="Horizontal" CellPadding="3" AutoPostBack="true">
                        <asp:ListItem Text="Klient" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Klient->Projekt" Selected="true" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Projekt->Aktivita" Value="3"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
             
                <td>
                   <uc:periodmonth ID="period1" runat="server"></uc:periodmonth>
                </td>
               
            </tr>
        </table>




        <uc:plugin_datatable ID="plug1" TableID="tab1" runat="server"
            ColHeaders="Klient|Projekt|Vykázané hodiny|Z toho fakturovatelné"
            ColHideRepeatedValues="1" ColTypes="S|S|N|N" ColFlexSubtotals="1|0|11|11"
            TableCaption="Vykázané hodiny v měsíci" />




    </asp:Panel>

</asp:Content>
