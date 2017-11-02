<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="membership_framework.aspx.vb" Inherits="UI.membership_framework" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Obnova membership účtů"></asp:Label>
    </div>
    <div class="div6">
        <asp:Label ID="lblError" runat="server" CssClass="failureNotification"></asp:Label>
    </div>
    <asp:panel ID="panVerify" runat="server" CssClass="div6">
        <span>Zadejte speciální heslo:</span>
        <asp:TextBox ID="txt1" runat="server" TextMode="Password">
        </asp:TextBox>
        <asp:Button ID="cmdVerify" runat="server" CssClass="cmd" Text="Vstoupit" />
    </asp:panel>

    <asp:Panel ID="panUI" runat="server" Visible="false">
        <table cellpadding="10">
            <asp:Repeater ID="rp1" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <%#Eval("IsClosed") %>
                        </td>
                        <td>
                            <%#Eval("j03Login")%>
                        </td>
                        <td>
                            <%#Eval("j04Name")%>
                        </td>
                        <td>
                            <asp:Button ID="cmdRecoveryAccount" runat="server" Text="Obnovit membership účet" CommandName="recovery"  CssClass="cmd"/>
                        </td>
                        <td>
                            <asp:Label ID="lblNewPassword" runat="server"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <asp:TextBox ID="txtReport" runat="server" Width="99%" Height="200px" TextMode="MultiLine"></asp:TextBox>
    </asp:Panel>
</asp:Content>
