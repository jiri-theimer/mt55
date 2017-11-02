<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="o23_record_readonly.ascx.vb" Inherits="UI.o23_record_readonly" %>


<table cellpadding="5" cellspacing="2" style="width: 100%;" id="responsive">
    <tr class="trHover">
        <td style="width: 140px;">
            <span class="lbl">
                <img src="Images/settings.png" />
                Typ:</span>
        </td>
        <td>
            <asp:Label ID="x18Name" runat="server" CssClass="valbold"></asp:Label>
        </td>
    </tr>
    <tr class="trHover" id="trB02Name" runat="server">
        <td>
            <span class="lbl">
                <img src="Images/workflow.png" />
                Aktuální stav:</span>
        </td>
        <td>
            <asp:Label ID="b02Name" runat="server" CssClass="valbold"></asp:Label>
            <button type="button" onclick="workflow()">Posunout/doplnit</button>

        </td>
    </tr>
    <tr class="trHover" id="trCode" runat="server">
        <td>
            <span class="lbl">
                <img src="Images/type_text.png" />
                Kód:</span>
        </td>
        <td>
            <asp:Label ID="o23Code" runat="server" CssClass="valbold"></asp:Label>
        </td>
    </tr>
    <tr class="trHover" id="trName" runat="server">
        <td>
            <span class="lbl">
                <img src="Images/type_text.png" />
                Název:</span>
        </td>
        <td>
            <asp:Label ID="o23Name" runat="server" CssClass="valbold"></asp:Label>
        </td>
    </tr>

    <asp:Repeater ID="rpFF" runat="server">
        <ItemTemplate>

            <tr class="trHover" style="vertical-align: top;">
                <td>

                    <asp:Label ID="x16Name" runat="server" CssClass="lbl"></asp:Label>

                </td>
                <td>

                    <asp:Label ID="valFF" runat="server" CssClass="valbold"></asp:Label>

                    <asp:HiddenField runat="server" ID="hidField" />
                    <asp:HiddenField runat="server" ID="hidType" />
                    <asp:HiddenField ID="hidX16ID" runat="server" />
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td colspan="2" style="border-top: dashed 1px silver;"></td>
    </tr>
    <asp:Repeater ID="rpX19" runat="server">
        <ItemTemplate>

            <tr class="trHover" style="vertical-align: top;">
                <td style="width: 140px;">

                    <asp:Label ID="BindName" runat="server" CssClass="lbl"></asp:Label>

                </td>
                <td>
                    <asp:HyperLink ID="pm1" runat="server" CssClass="pp1" NavigateUrl="#"></asp:HyperLink>
                    <asp:Label ID="BindValue" runat="server" CssClass="val"></asp:Label>
                    <asp:HyperLink ID="clue1" runat="server" CssClass="reczoom" Text="i" title="Náhled"></asp:HyperLink>

                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>


<asp:Panel ID="panWorksheetGrid" runat="server" CssClass="div6" Visible="false">
    <img src="Images/worksheet.png" />
    <a href="p31_grid.aspx?masterprefix=o23&masterpid=<%=hidPID.Value%>" target="_top">Přehled worksheet úkonů svázaných s dokumentem</a>
</asp:Panel>

<div class="div6">
    <asp:Label ID="Timestamp" runat="server" CssClass="timestamp"></asp:Label>
</div>
<asp:Panel ID="panHtml" runat="server" CssClass="div6" Style="border-top: dashed 1px silver;">
    <asp:PlaceHolder ID="place1" runat="server"></asp:PlaceHolder>
</asp:Panel>

<asp:HiddenField ID="hidX18ID" runat="server" />
<asp:HiddenField ID="hidPID" runat="server" />

<script type="text/javascript">
    function workflow() {

        if (parent == null) {
            sw_everywhere("workflow_dialog.aspx?prefix=o23&pid=<%=hidPID.Value%>", "Images/workflow.png", true);
        }
        else {
            window.parent.sw_everywhere("workflow_dialog.aspx?prefix=o23&pid=<%=hidPID.Value%>", "Images/workflow.png", true);
        }

    }
</script>

