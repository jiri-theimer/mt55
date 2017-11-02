<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Anonym.Master" CodeBehind="kickoff.aspx.vb" Inherits="UI.kickoff" %>

<%@ MasterType VirtualPath="~/Anonym.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="První spuštění systému!"></asp:Label>
    </div>
    <div>
        <asp:Label ID="lblMessage" runat="server" CssClass="infoNotification" Text="Zatím nebyl založen ani jeden uživatel, budeš tak prvním."></asp:Label>
    </div>
    <asp:Panel ID="panKickOffReady" runat="server">

        <div class="content-box2">
            <div class="title">Založit prvního uživatele systému s administrátorským oprávněním</div>
            <div class="content">
                <table cellpadding="10">
                    <tr>
                        <td>
                            <asp:Label ID="lblLogin" runat="server" Text="Uživatelské jméno:" CssClass="lblReq"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtLogin" runat="server" Style="width: 300px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label1" runat="server" Text="Aplikační role:" CssClass="lblReq"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="j04ID" DataTextField="j04Name" runat="server" DataValueField="pid" Width="300px"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblPassword" runat="server" Text="Přístupové heslo:" CssClass="lblReq"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtPassword" runat="server" Style="width: 300px;" TextMode="SingleLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblVerify" runat="server" Text="Ověření hesla:" CssClass="lblReq"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="txtVerify" runat="server" Style="width: 300px;" TextMode="SingleLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Jméno:" CssClass="lblReq"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02FirstName" runat="server" Style="width: 300px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Příjmení:" CssClass="lblReq"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02Lastname" runat="server" Style="width: 300px;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="E-mail:" CssClass="lblReq"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="j02Email" runat="server" Style="width: 300px;"></asp:TextBox>
                        </td>
                    </tr>
                </table>

            </div>
            <div class="div6">
                <asp:Button ID="cmdGo" runat="server" CssClass="cmd" Text="Založit účet a pokračovat ->" />
                <asp:Label ID="lblError" runat="server" CssClass="failureNotification"></asp:Label>
            </div>

        </div>
    </asp:Panel>
</asp:Content>
