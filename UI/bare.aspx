<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="bare.aspx.vb" Inherits="UI.bare" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
    <asp:Repeater ID="rp1" runat="server">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:HyperLink ID="link1" runat="server"></asp:HyperLink>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    </table>
</asp:Content>
