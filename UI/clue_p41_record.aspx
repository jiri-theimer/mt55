<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p41_record.aspx.vb" Inherits="UI.clue_p41_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register TagPrefix="uc" TagName="contactpersons" Src="~/contactpersons.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
            <img src="Images/project_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
            <uc:mytags ID="tags1" ModeUi="2" Prefix="p41" runat="server" />
        </div>

        <div class="content-box2">
            <div class="title">
                Záznam projektu
                <asp:Label ID="b02Name" runat="server" CssClass="valboldred" style="margin-left:10px;"></asp:Label>
                <asp:Image ID="imgFavourite" runat="server" Visible="false" />
                
            </div>
            <div class="content">
                <asp:Label ID="p41Name" runat="server" CssClass="valboldblue"></asp:Label>
                <table cellpadding="6">
                    <tr>
                        <td>Typ projektu:</td>
                        <td>
                            <asp:Label ID="p42Name" runat="server" CssClass="valbold"></asp:Label></td>
                        <td>Středisko:</td>
                        <td>
                            <asp:Label ID="j18Name" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                        
                    </tr>
                    <tr>
                        <td>Klient:</td>
                        <td colspan="3">
                            <asp:Label ID="Client" runat="server" CssClass="valbold"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>Fakturační ceník:</td>
                        <td>
                            <asp:Label ID="p51Name_Billing" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                        <td>Nadřízený projekt:</td>
                        <td>
                           
                            <asp:HyperLink ID="ParentProject" runat="server" Target="_top"></asp:HyperLink>
                        </td>
                    </tr>
                    <tr id="trDates" runat="server">
                        <td>Plánované zahájení:</td>
                        <td>
                            <asp:Label ID="p41PlanFrom" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                        <td>Plánované dokončení:</td>
                        <td>
                            <asp:Label ID="p41PlanUntil" runat="server" CssClass="valbold"></asp:Label>

                        </td>
                    </tr>

                </table>
                
            </div>
            
        </div>
        <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>
        <div class="content-box2">
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

        
     
    </div>
</asp:Content>
