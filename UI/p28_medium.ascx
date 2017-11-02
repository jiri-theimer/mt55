<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="p28_medium.ascx.vb" Inherits="UI.p28_medium" %>

    <table cellpadding="5" cellspacing="2">
        <asp:Repeater ID="rpO32" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <b><%# Eval("o33Name")%></b>
                    </td>
                    <td>
                        <asp:Label ID="o32Value" runat="server"></asp:Label>
                        <asp:HyperLink ID="aLink" runat="server" CssClass="wake_link"></asp:HyperLink>
                    </td>

                    <td>
                        <i><%# Eval("o32Description")%></i>

                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

