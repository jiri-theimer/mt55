<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="email_receiver.ascx.vb" Inherits="UI.email_receiver" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<telerik:RadComboBox ID="cbx1" 
runat="server"
DropDownWidth="400" 
EnableTextSelection="true" 
MarkFirstMatch="true" 
EnableLoadOnDemand="true"
AutoCompleteSeparator=","
ShowToggleImage="false" MinFilterLength="1"
AllowCustomText="true"
Width="250px">
<WebServiceSettings Method="LoadComboData" Path="~/Services/person_service.asmx" UseHttpGet="false" />

</telerik:RadComboBox>

    
<asp:HiddenField ID="hidvalue" runat="server" />
<asp:HiddenField ID="hidj03id_system" runat="server" />

<script type="text/javascript">
    function <%=me.clientid%>_OnClientItemsRequesting(sender, eventArgs) {
        var context = eventArgs.get_context();
        context["filterstring"] = eventArgs.get_text();
        context["j03id"] = document.getElementById("<%=hidj03id_system.clientid%>").value;
        context["flag"] ="email";
       
    }

   

        
</script>
