<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="invoice.ascx.vb" Inherits="UI.invoice" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadComboBox ID="cbx1" 
runat="server"
DropDownWidth="400" 
EnableTextSelection="true" 
MarkFirstMatch="true" 
EnableLoadOnDemand="true"
Width="250px">
<WebServiceSettings Method="LoadComboData" Path="~/Services/invoice_service.asmx" UseHttpGet="false" />

</telerik:RadComboBox>

    
<asp:HiddenField ID="hidvalue" runat="server" />
<asp:HiddenField ID="hidj03id_system" runat="server" />
<asp:HiddenField ID="hidflag" runat="server" Value="" />

<script type="text/javascript">
    function <%=me.clientid%>_OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["filterstring"] = eventArgs.get_text();
        context["j03id"] = document.getElementById("<%=hidj03id_system.clientid%>").value;
        context["flag"] = document.getElementById("<%=hidflag.clientid%>").value;
        
       
    }

    function <%=me.clientid %>_get_value()
    {
    var combo = $find("<%= cbx1.ClientID %>");

    return(combo.get_value())
    }

        
</script>