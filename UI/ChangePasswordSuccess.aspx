<%@ Page Title="Change Password" Language="vb" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="ChangePasswordSuccess.aspx.vb" Inherits="UI.ChangePasswordSuccess" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div style="background-color: white; padding: 10px;">
        <table cellpadding="5">
            <tr>
                <td>
                    <img src="../Images/password_32.png" />
                </td>
                <td>
                    <asp:Label ID="lblHeader" runat="server" CssClass="framework_header_span" Style="font-size: 200%;" Text="Změna přihlašovacího hesla"></asp:Label>

                </td>

            </tr>
        </table>

        <h2>Přihlašovací heslo bylo úspěšně změněno.
        </h2>
    </div>
</asp:Content>
