<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="freefields.ascx.vb" Inherits="UI.freefields" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
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

                    <td style="width: 150px;">
                        <asp:HiddenField ID="x28IsRequired" runat="server" />

                        <asp:Label ID="lblFF" runat="server" CssClass="lbl"></asp:Label>
                        <asp:HyperLink ID="clue_help" runat="server" CssClass="reczoom" Text="?" Visible="false" ToolTip="Nápověda"></asp:HyperLink>
                    </td>
                    <td>

                        <asp:TextBox ID="txtFF_Text" runat="server"></asp:TextBox>
                        <telerik:RadNumericTextBox ID="txtFF_Number" runat="server"></telerik:RadNumericTextBox>
                        <asp:CheckBox ID="chkFF" runat="server" ForeColor="Black" />
                        <telerik:RadDateInput ID="txtFF_Date" runat="server" DateFormat="dd.MM.yyyy"></telerik:RadDateInput>

                        <telerik:RadComboBox ID="cbxFF" runat="server" ShowToggleImage="false" ShowDropDownOnTextboxClick="true" MarkFirstMatch="true" Width="400px"></telerik:RadComboBox>

                        <asp:HiddenField runat="server" ID="hidField" />
                        <asp:HiddenField runat="server" ID="hidType" />
                        <asp:HiddenField ID="hidX28ID" runat="server" />
                        <asp:HiddenField ID="hidX23ID" runat="server" />

                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr>
                    <td style="width: 150px;">
                        <asp:Label ID="x18Name" runat="server" CssClass="lbl"></asp:Label>
                        <asp:HiddenField ID="x20ID" runat="server" />
                        <asp:HiddenField ID="x18ID" runat="server" />
                        <asp:HiddenField ID="x20IsMultiselect" runat="server" />
                    </td>
                    <td>
                        <uc:datacombo ID="o23IDs_Tag" runat="server" DataTextField="NameWithCode" DataValueField="pid" AllowCheckboxes="true" Filter="Contains" Width="400px"></uc:datacombo>
                        <button type="button" id="cmdX18_Items" runat="server" class="button-link" title="Nastavit položky kategorií">
                            <img src="Images/settings.png" />
                        </button>

                        
                        <asp:Repeater ID="rpO23IDs_Doc" runat="server">
                            <ItemTemplate>
                                <a class="reczoom" rel="clue_o23_record.aspx?pid=<%#Eval("pid")%>" title="Detail dokumentu">i</a>
                                <a style="margin-right: 20px;" href="javascript:o23_record_x18(<%#Eval("pid")%>,<%#Eval("x18ID")%>)"><%#Eval("o23Code")%> - <%#Eval("o23Name")%></a>

                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:ImageButton ID="cmdO23_Clear" runat="server" ImageUrl="Images/delete.png" ToolTip="Vyčistit vazbu" CommandName="clear" CssClass="button-link" />
                        <button type="button" id="cmdO23_Create" runat="server">Nový</button>
                        <button type="button" id="cmdO23_Find" runat="server">Najít</button>
                        <asp:HiddenField ID="hidO23IDs_Doc" runat="server" />
                    </td>

                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

</asp:Panel>

<asp:HiddenField ID="hidDataTable" runat="server" />
<asp:HiddenField ID="hidJ03ID" runat="server" />
<asp:HiddenField ID="hidDataPID" runat="server" />


<script type="text/javascript">
    function <%=Me.ClientID%>_OnClientItemsRequesting(sender, eventArgs) {

        var context = eventArgs.get_context();
        context["filterstring"] = eventArgs.get_text();
        context["j03id"] = document.getElementById("<%=Me.hidJ03ID.ClientID%>").value;

        context["x23id"] = sender.get_attributes().getAttribute("x23id");

    }

    function x18_items(x18id) {

        dialog_master("x18_items.aspx?pid=" + x18id, true)

    }
    function o23_create_x18(x18id) {
        dialog_master("o23_record.aspx?pid=0&x18id=" + x18id, true)
    }
    function o23_find_x18(x18id,x20id) {
        dialog_master("o23_queue.aspx?x18id=" + x18id+"&x20id="+x20id, true)
    }
    function o23_record_x18(o23id, x18id) {
        dialog_master("o23_record.aspx?pid=" + o23id + "&x18id=" + x18id, true)
    }
</script>
