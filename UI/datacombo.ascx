<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="datacombo.ascx.vb" Inherits="UI.datacombo" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadComboBox ID="cbx1" runat="server" DataValueField="pid" >
    <Localization AllItemsCheckedString="Všechny položky zaškrtnuty" ItemsCheckedString="x zaškrtnuto" />
</telerik:RadComboBox>
<asp:HiddenField ID="hidIsEmptyFirstRow" runat="server" />
<asp:HiddenField ID="hidSelectedValue" runat="server" />


<asp:HiddenField ID="hidRemoteList" runat="server" Value="" />

<script type="text/javascript">
    function <%=Me.ClientID%>_OnClientItemsRequesting(sender, eventArgs) {
        var combo = $find("<%= cbx1.ClientID %>");

        //if (combo.get_value() != "")
            //alert(combo.get_value());
        

        if (sender.get_items().get_count() > 0) {
            eventArgs.set_cancel(true);
            return;
        }
        
        
        var context = eventArgs.get_context();                
        context["prefix"] = document.getElementById("<%=hidRemoteList.ClientID%>").value;
        
        

    }

    function <%=me.clientid %>_get_value() {
        var combo = $find("<%= cbx1.ClientID %>");

        return (combo.get_value())
    }

  


</script>