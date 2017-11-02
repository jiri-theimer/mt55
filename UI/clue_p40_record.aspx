<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p40_record.aspx.vb" Inherits="UI.clue_p40_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function p31_record(pid) {
            window.parent.p31_recurrence_record(pid);
            

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panContainer" runat="server" Style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
            <img src="Images/worksheet_recurrence_32.png" />
            <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
        </div>

        <table cellpadding="10" cellspacing="2">
            <tr>
                <td>Text:</td>
                <td>
                    <asp:Label ID="p40Text" runat="server" CssClass="valbold" Font-Italic="true"></asp:Label>

                </td>
            </tr>
            <tr>
                <td>Aktivita:</td>
                <td>
                    <asp:Label ID="p34Name" runat="server" CssClass="valbold"></asp:Label>
                    <span>- </span>
                    <asp:Label ID="p32Name" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Hodnota:</td>
                <td>
                    <asp:Label ID="p40Value" runat="server" CssClass="valbold" Style="color: red;"></asp:Label>
                    <asp:Label ID="j27Code" runat="server" CssClass="valbold"></asp:Label>
                </td>
            </tr>

        </table>


        <fieldset>
            <legend>Předpis generování</legend>
            <table cellpadding="5">
                <tr>
                    <th>Kdy generovat</th>
                    <th>Rozhodné datum</th>
                    <th>Text úkonu</th>
                    <th></th>
                    <th></th>
                </tr>
                <asp:Repeater ID="rpP39" runat="server">
                    <ItemTemplate>
                        <tr valign="top">

                            <td>
                                <asp:Label ID="p39DateCreate" runat="server"></asp:Label>

                            </td>
                            <td>
                                <asp:Label ID="p39Date" runat="server"></asp:Label>

                            </td>
                            <td>
                                <asp:Label ID="p39Text" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="p39ErrorMessage_NewInstance" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:HyperLink ID="cmdP31" runat="server"></asp:HyperLink>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </fieldset>


    </asp:Panel>
</asp:Content>
