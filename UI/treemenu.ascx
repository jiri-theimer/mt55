<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="treemenu.ascx.vb" Inherits="UI.TreeMenu" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadTreeView skin="Default" ID="treeMenu" runat="server" ShowLineImages="false" Style="white-space: normal"  SingleExpandPath="true">

</telerik:RadTreeView>



<script type="text/javascript">
    function toggle_onClientNodeClicking(sender, args) {
    
        if (args.get_node().get_nodes().get_count() == 0)
            return

        args.get_node().toggle();
    }

    
</script>