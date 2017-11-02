<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p41_myworksheet.aspx.vb" Inherits="UI.clue_p41_myworksheet" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodmonth" Src="~/periodmonth.ascx" %>
<%@ Register TagPrefix="uc" TagName="contactpersons" Src="~/contactpersons.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="Plugins/Plugin.css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panContainer" runat="server" Style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <asp:Panel ID="panHeader" runat="server">
            <img src="Images/project_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
            
        </asp:Panel>
        <div class="title">
         
                <asp:Label ID="Me" runat="server" CssClass="valbold" Style="margin-left: 10px;"></asp:Label>
            <uc:periodmonth ID="period1" runat="server"></uc:periodmonth>
            
        </div>
        <table>
            <tr>
                <td>
                    <asp:RadioButtonList ID="opgView" runat="server" RepeatDirection="Horizontal" CellPadding="3" AutoPostBack="true">
                        <asp:ListItem Text="Sešit" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Sešit->Aktivita" Selected="true" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Úkol" Value="3"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>

                <td>
                    
                </td>

            </tr>
        </table>




        <uc:plugin_datatable ID="plug1" TableID="tab1" runat="server"
            ColHeaders="Sešit|Aktivita|Vykázané hodiny||Od|Do" NoDataMessage="Žádné časové úkony ve vybraném měsíci."
            ColHideRepeatedValues="1" ColTypes="S|S|N|N" ColFlexSubtotals="1|0|11|0|0|0"
            TableCaption="Vykázané hodiny v projektu za vybraný měsíc" />




        
        <div class="content-box2" style="margin-top:10px;">
            <div class="title">
                <img src="Images/projectrole.png" style="margin-right: 10px;" />
                Obsazení projektových rolí
            </div>
            <div class="content">
                <uc:entityrole_assign_inline ID="roles_project" runat="server" IsShowClueTip="false" EntityX29ID="p41Project" NoDataText="Projekt nemá přiřazené projektové role!"></uc:entityrole_assign_inline>
            </div>
        </div>
        <asp:Panel ID="panP30" runat="server" CssClass="content-box2">
            <div class="title">
                <img src="Images/person.png" style="margin-right: 10px;" />
                Kontaktní osoby projektu                        
            </div>
            <div class="content">
                <uc:contactpersons ID="persons1" runat="server" IsShowClueTip="false"></uc:contactpersons>
            </div>
        </asp:Panel>
    </asp:Panel>

</asp:Content>
