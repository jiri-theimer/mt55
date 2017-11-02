<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="mytags.ascx.vb" Inherits="UI.mytags" %>
<asp:Repeater ID="rp1" runat="server">
    <ItemTemplate>
        <asp:panel ID="panItem" runat="server" CssClass="badge_tag">
            <asp:Label ID="o51Name" runat="server"></asp:Label>
            
        </asp:panel>
    </ItemTemplate>
</asp:Repeater>
<asp:HyperLink ID="cmdTags" runat="server" Text="[Zatím žádné štítky]" NavigateUrl="javascript:tags_assign()" CssClass="wake_link"></asp:HyperLink>
<asp:HiddenField ID="hidPrefix" runat="server" />
<asp:HiddenField ID="hidRecordPID" runat="server" Value="0" />
<asp:HiddenField ID="hidMode" runat="server" Value="0" />
<asp:HiddenField ID="hidO51IDs" runat="server" />
<asp:HiddenField ID="hidNeed2save" runat="server" Value="0" />
<asp:Button ID="cmdRefresh" runat="server" style="display:none;" />
<script type="text/javascript">
    
    function tags_assign() {
        var prefix = document.getElementById("<%=hidPrefix.ClientID%>").value;
        var recpid = document.getElementById("<%=hidRecordPID.ClientID%>").value;
        var mode = document.getElementById("<%=hidMode.ClientID%>").value;
        var o51ids = document.getElementById("<%=hidO51IDs.ClientID%>").value;

        sw_everywhere("tag_binding.aspx?prefix=" + prefix + "&pids="+recpid+"&mode="+mode+"&o51ids="+o51ids, "Images/tag.png", true);

        //sw_everywhere("tag_binding.aspx?prefix"+prefix+"&pids="+recpid, "", true);
    }

    function hardrefresh_mytags(o51ids) {
        document.getElementById("<%=hidO51IDs.ClientID%>").value = o51ids;
        var clickButton = document.getElementById("<%=cmdRefresh.ClientID%>");
        clickButton.click();
    }
</script>
