<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="contact.ascx.vb" Inherits="UI.contact" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadComboBox ID="cbx1"
runat="server" 
DropDownWidth="400" 
EnableTextSelection="true" 
MarkFirstMatch="true" 
EnableLoadOnDemand="true"
Width="250px">
<WebServiceSettings Method="LoadComboData" Path="~/Services/contact_service.asmx" UseHttpGet="false" />

</telerik:RadComboBox>

    
<asp:HiddenField ID="hidvalue" runat="server" />
<asp:HiddenField ID="hidj03id_system" runat="server" />
<asp:HiddenField ID="hidflag" runat="server" Value="" />

<script type="text/javascript">
    function <%=me.clientid%>_OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        var combo = $find("<%= cbx1.ClientID %>");

        if (combo.get_value() == "")
            context["filterstring"] = eventArgs.get_text();
        else
            context["filterstring"] = "";

        
        context["j03id"] = document.getElementById("<%=hidj03id_system.clientid%>").value;
        context["flag"] = document.getElementById("<%=hidflag.clientid%>").value;
        
       
    }

    function <%=me.clientid %>_get_value()
    {
    var combo = $find("<%= cbx1.ClientID %>");

    return(combo.get_value())
    }

        
</script>