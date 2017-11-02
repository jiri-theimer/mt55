<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="dbupdate_reports.aspx.vb" Inherits="UI.dbupdate_reports" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="padding: 20px; background-color: white;">
        <div class="div6">
            <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Aktualizace šablon výchozích tiskových sestav"></asp:Label>
        </div>
        <div class="div6">
            <span class="infoInForm">Aktualizace naimportuje všechny tiskové šablony sestav, které obsahuje výchozí distribuce systému
            </span>
        </div>
        <div class="div6">
            <asp:Label ID="lblError" runat="server" CssClass="failureNotification"></asp:Label>
        </div>
        <div class="div6">
            <asp:Button ID="cmdGo" runat="server" CssClass="cmd" Text="Zahájit aktualizaci" />

        </div>
        <div class="div6" style="margin-top: 10px;">
            <asp:Button ID="cmdDefPage" runat="server" CssClass="cmd" Text="Spustit MARKTIME" Visible="false" />
        </div>

        <div class="content-box2" style="max-height:200px;overflow:auto;">
            <div class="title">Seznam výchozích tiskových šablon</div>
            <div class="content">
                <table cellpadding="10">
                    <asp:Repeater ID="rp1" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# Container.DataItem %>
                                </td>

                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </div>

    <asp:Panel ID="panMultiDbs" runat="server" CssClass="content-box2" Visible="false">
        <div class="title">MULTIPLE report UPDATE</div>
        <div class="content">
            <asp:DropDownList ID="dbs" runat="server" AutoPostBack="true"></asp:DropDownList>
            
            <asp:Button ID="cmdGoDbs" runat="server" CssClass="cmd" Text="Spustit aktualizaci" />
            
            <asp:Label ID="lblDbsMessage" runat="server" CssClass="infoNotificationRed"></asp:Label>
        </div>

    </asp:Panel>
</asp:Content>
