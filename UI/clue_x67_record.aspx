<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_x67_record.aspx.vb" Inherits="UI.clue_x67_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">

        <div>
            <img src="Images/projectrole_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
        </div>
        <div>
            <asp:Label ID="Person" runat="server"></asp:Label>
        </div>
        <div>
            <asp:Label ID="Team" runat="server"></asp:Label>
        </div>
        <asp:Panel ID="panWorksheet" runat="server" CssClass="content-box2">
            <div class="title">
                Oprávnění projektové role k worksheet úkonům projektu
            </div>
            <div class="content">
                <table>
                    <tr>
                        <th>Sešit</th>
                        <th>Zapisovat úkony</th>
                        <th>Přístup k úkonům ostatních uživatelů</th>
                    </tr>
                    <asp:Repeater ID="rpO28" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Label ID="p34Name" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Image ID="img1" runat="server" src="Images/ok.png" />
                                </td>
                                <td>
                                    <asp:Label ID="PermFlag" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>

        </asp:Panel>
        <div class="content-box2">
            <div class="title">
                Výčet oprávnění této role
            </div>
            <div class="content">
                <ul>
                    <asp:Repeater ID="rpX53" runat="server">
                        <ItemTemplate>
                            <li>
                                <%# Eval("x53Name")%>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>

    </div>

</asp:Content>
