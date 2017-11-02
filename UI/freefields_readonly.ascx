<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="freefields_readonly.ascx.vb" Inherits="UI.freefields_readonly" %>
<asp:Panel ID="panContainer" runat="server">
    <table cellpadding="3" cellspacing="2" width="100%">
        <asp:Repeater ID="rpFF" runat="server">
            <ItemTemplate>
                <tr id="trHeader" runat="server" visible="false">
                    <td colspan="2">
                        <div style="width: 100%; border-bottom: solid 2px gray;">
                            <asp:Label ID="headerFF" runat="server" Style="font-weight: bold; padding-left: 10px;"></asp:Label>
                        </div>
                    </td>
                </tr>
                <tr style="vertical-align: top;">
                    <td style="width: 120px;">

                        <asp:Label ID="lblFF" runat="server" CssClass="lbl"></asp:Label>

                    </td>
                    <td>

                        <asp:Label ID="valFF" runat="server" CssClass="valbold"></asp:Label>

                        <asp:HiddenField runat="server" ID="hidField" />
                        <asp:HiddenField runat="server" ID="hidType" />
                        <asp:HiddenField ID="hidX28ID" runat="server" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <asp:HiddenField ID="hidDataTable" runat="server" />

    </table>
</asp:Panel>
