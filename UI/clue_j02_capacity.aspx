<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_j02_capacity.aspx.vb" Inherits="UI.clue_j02_capacity" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
            <img src="Images/person_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
        </div>
        <div>
            <asp:Label ID="Project" runat="server" CssClass="valboldblue"></asp:Label>
        </div>
        <table>
            <tr>
                <th>Měsíc</th>
                <th>Fond</th>
                <th>Kapacitní plán
                    <br />(mimo akt.projekt)</th>
                <th>Kapacitní plán
                    <br />(vč. akt.projektu)</th>
               
                <th>Vykázáno<br />
                    hodiny Fa</th>
                <th>Vykázáno<br />
                    hodiny Nefa</th>
                <th>Vykázáno<br />
                    celkem</th>
            </tr>

            <asp:Repeater ID="rp1" runat="server">
                <ItemTemplate>
                    <tr class="trHover">
                        <td>
                            <asp:Label ID="Mesic" runat="server"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Fond" runat="server"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="PlanMimoProjekt" runat="server"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="PlanVcProjektu" runat="server"></asp:Label>
                        </td>

                        <td align="right">
                            <asp:Label ID="HodinyFa" runat="server"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="HodinyNefa" runat="server"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="HodinyCelkem" runat="server"></asp:Label>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

        </table>

    </div>
</asp:Content>
