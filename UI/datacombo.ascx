<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="datacombo.ascx.vb" Inherits="UI.datacombo" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadComboBox ID="cbx1" runat="server" DataValueField="pid" >
    <Localization AllItemsCheckedString="Všechny položky zaškrtnuty" ItemsCheckedString="x zaškrtnuto" />
</telerik:RadComboBox>
<asp:HyperLink ID="cm1" runat="server" CssClass="pp3" Visible="false"></asp:HyperLink>
<asp:HiddenField ID="hidIsEmptyFirstRow" runat="server" />



<asp:HiddenField ID="hidRemoteList" runat="server" Value="" />

<%If hidRemoteList.Value <> "" Then%>
<script type="text/javascript">
    function <%=Me.ClientID%>_OnClientItemsRequesting(sender, eventArgs) {
        var combo = $find("<%= cbx1.ClientID %>");

        if (sender.get_items().get_count() > 0) {
            eventArgs.set_cancel(true);
            return;
        }
        
        
        var context = eventArgs.get_context();                
        context["prefix"] = document.getElementById("<%=hidRemoteList.ClientID%>").value;
        
        
    }

    function <%=Me.ClientID%>_RCM(ctl) {
        var prefix = document.getElementById("<%=hidRemoteList.ClientID%>").value;

        var combo = $find("<%= cbx1.ClientID %>");
        var pid=combo.get_value();
        
        RCM(prefix, pid, ctl);
    }

</script>
<%end if%>