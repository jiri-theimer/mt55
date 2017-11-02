<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="p28_address.ascx.vb" Inherits="UI.p28_address" %>

    <table cellpadding="5" cellspacing="2">
        <asp:Repeater ID="rpO37" runat="server">
            <ItemTemplate>
                <tr valign="top">
                    <td>
                        <b><%# Eval("o36Name")%></b>
                    </td>
                    <td>
                        <%# Eval("o38Street")%>
                    </td>
                    <td>
                        <%# Eval("o38City")%>
                        <div><%# Eval("o38Country")%></div>
                    </td>
                  
                    <td>
                        <i><%# Eval("o38Description")%></i>

                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

