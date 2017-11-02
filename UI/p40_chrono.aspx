<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p40_chrono.aspx.vb" Inherits="UI.p40_chrono" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="10" cellspacing="2">
        <tr>
            <td>Projekt:</td>
            <td colspan="3">
                <asp:Label ID="Project" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Název pravidla:</td>
            <td>
                <asp:Label ID="p40Name" runat="server" CssClass="valbold"></asp:Label>
            </td>
            <td>Text úkonu:</td>
            <td>
                <i><asp:Label ID="p40Text" runat="server" CssClass="valbold" Font-Italic="true"></asp:Label></i>

            </td>
        </tr>

        <tr>
            <td>Aktivita úkonu:</td>
            <td>
                <asp:Label ID="p34Name" runat="server" CssClass="valbold"></asp:Label>
                <span>- </span>
                <asp:Label ID="p32Name" runat="server" CssClass="valbold"></asp:Label>
            </td>

            <td>Hodnota:</td>
            <td>
                <asp:Label ID="p40Value" runat="server" CssClass="valbold" Style="color: red;"></asp:Label>
                <asp:Label ID="j27Code" runat="server" CssClass="valbold"></asp:Label>
            </td>
        </tr>

    </table>

    <div class="content-box2">
        <div class="title">
            Chronologický plán generování
        </div>
        <div class="content">
            <table cellpadding="6">
                <tr>
                    <th>Kdy generovat</th>
                    <th>Datum úkonu</th>
                    <th>Text úkonu</th>                    
                    <th></th>
                    <th></th>
                </tr>
                <asp:Repeater ID="rpP39" runat="server">
                    <ItemTemplate>
                        <tr valign="top" class="trHover">

                            <td>
                                <asp:Label ID="p39DateCreate" runat="server"></asp:Label>

                            </td>
                            <td>
                                <asp:Label ID="p39Date" runat="server"></asp:Label>

                            </td>
                            <td>
                                <i><asp:Label ID="p39Text" runat="server"></asp:Label></i>
                            </td>
                            
                            <td>
                                <asp:HyperLink ID="cmdP31" runat="server"></asp:HyperLink>
                                <asp:Label ID="lblMessage" runat="server" CssClass="infoNotification"></asp:Label>
                                <asp:Button ID="cmdGenerate" runat="server" Text="Vygenerovat nyní ručně" CssClass="cmd" Visible="false" CommandName="generate" />
                                
                            </td>
                            <td>
                                <asp:Label ID="lblError" runat="server" CssClass="infoNotificationRed"></asp:Label>
                                <asp:Button ID="cmdClear" runat="server" Text="Vyčistit stopu generování" CssClass="cmd" Visible="false" CommandName="clear" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
