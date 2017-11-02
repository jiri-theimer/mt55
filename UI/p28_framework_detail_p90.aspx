<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="p28_framework_detail_p90.aspx.vb" Inherits="UI.p28_framework_detail_p90" %>
<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function RowSelected_p90(sender, args) {
            document.getElementById("<%=hiddatapid_p90.ClientID%>").value = args.getDataKeyValue("pid");
        }

        function RowDoubleClick_p90(sender, args) {
            p90_edit();
           
        }
        function p90_edit() {
            var pid = document.getElementById("<%=hiddatapid_p90.ClientID%>").value;
            if (pid == "" || pid == null) {
                alert("Není vybrán záznam.");
                return
            }
            sw_decide("p90_record.aspx?pid=" + pid, "Images/proforma_32.png", true);

        }

        

        function p90_new() {
            sw_decide("p90_record.aspx?pid=0&p28id=<%=master.datapid%>", "Images/proforma_32.png", true);


        }
        
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <uc:entity_menu id="menu1" runat="server" DataPrefix="p28"></uc:entity_menu>

    <div class="commandcell">
    <img src="Images/invoice.png" alt="Faktury" />
    <asp:Label ID="lblHeaderP91" CssClass="framework_header_span" runat="server" Text="Zálohové faktury"></asp:Label>
</div>
<div class="commandcell" style="margin-left: 10px;">
    
    
</div>
<div class="commandcell" style="margin-left: 10px;">
    <telerik:RadMenu ID="recmenu1" Skin="Metro" runat="server" EnableRoundedCorners="false" EnableShadows="false" ClickToOpen="true" style="z-index:2000;" RenderMode="Auto" ExpandDelay="0" ExpandAnimation-Type="None" EnableAutoScroll="true">
                <Items>

                   <telerik:RadMenuItem Value="cmdNew" Text="Nová záloha" NavigateUrl="javascript:p90_new();" ImageUrl="Images/new4menu.png"></telerik:RadMenuItem>

                    
                </Items>
            </telerik:RadMenu>
</div>



<div style="clear: both; width: 100%;"></div>

<asp:HiddenField ID="hiddatapid_p90" runat="server" />


<uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected_p90" OnRowDblClick="RowDoubleClick_p90"></uc:datagrid>

</asp:Content>
