<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="mobile_p31_list.ascx.vb" Inherits="UI.mobile_p31_list" %>
<div class="panel panel-default" style="margin-top: 20px;">
    <!-- Default panel contents -->
    <div class="panel-heading">
        <asp:Label ID="lblListP31ListHeader" runat="server" Text="Zapsané úkony"></asp:Label>
        
    </div>

    <table class="table table-condensed">

        <asp:Repeater ID="rpP31" runat="server">
            <ItemTemplate>
                <tr style="background-color: whitesmoke;" id="trDate" runat="server">
                    <td colspan="3">
                        <asp:Label ID="p31Date" runat="server" Font-Bold="true"></asp:Label>

                        <asp:Label ID="Pocet" runat="server" Style="padding-left: 20px;"></asp:Label>

                        <asp:Label ID="Hodiny" runat="server" Style="padding-left: 20px;" class="badge1"></asp:Label>
                    </td>

                </tr>
                <tr>

                    <td>
                        <asp:Label ID="Project" runat="server"></asp:Label>

                    </td>
                    <td>
                        <asp:Label ID="p32Name" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: right;">
                        <asp:Label ID="p31Value_Orig" runat="server"></asp:Label>
                    </td>
                   
                </tr>
                <tr class="trTextRow">
                    <td style="font-size: 90%;" colspan="3">
                        <div>
                            <asp:Label ID="Task" runat="server" CssClass="task_in_table"></asp:Label>
                            <asp:Label ID="ContactPerson" runat="server" CssClass="person_in_table"></asp:Label>
                        </div>
                        <div>
                            <asp:HyperLink ID="cmdEdit" runat="server" Text="Upravit" CssClass="btn btn-primary btn-xs"></asp:HyperLink>
                        </div>
                        <asp:Label ID="p31Text" runat="server" Font-Italic="true"></asp:Label>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

</div>
