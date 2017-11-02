<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="datagrid.ascx.vb" Inherits="UI.datagrid" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div id="gridContainer">
<telerik:RadGrid ID="grid1" AutoGenerateColumns="false" runat="server" ShowFooter="true" EnableViewState="true" AllowPaging="true" AllowSorting="true" Skin="Default" EnableLinqExpressions="false">
    <MasterTableView Width="100%"/>    
    <ExportSettings ExportOnlyData="true" OpenInNewWindow="true" FileName="marktime_export" UseItemStyles="true">
        <Excel Format="Biff" />
        <Pdf BorderStyle="Thin" BorderType="AllBorders" DefaultFontFamily="Calibri" PageBottomMargin="20" PageTopMargin="20" PageLeftMargin="30" PageRightMargin="20"></Pdf>
    </ExportSettings>
    <GroupingSettings CaseSensitive="false" />
    
    <ClientSettings>
        <Selecting AllowRowSelect="true" />
        <ClientEvents OnRowContextMenu="ContextSelect" OnGridCreated="GridCreated" OnCommand="OnGridCommand" />        
    </ClientSettings>
    <PagerStyle Position="Top" AlwaysVisible="false" Mode="NextPrevAndNumeric" Wrap="false" />
    
    <FooterStyle BackColor="#bcc7d8" HorizontalAlign="Right" />
    
</telerik:RadGrid>
</div>

<telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" RenderMode="Lightweight" Transparency="30" BackColor="#E0E0E0">
    <div style="float:none;padding-top:80px;">
    <img src="Images/loading.gif" />
    <h2>LOADING...</h2>
    </div>
</telerik:RadAjaxLoadingPanel>

<asp:HiddenField ID="hidAutoScrollHashID" runat="server" Value="" />

<script type="text/javascript">
    function OnGridCommand(sender, args) {
        //alert(args.get_commandName());
        var loadingPanel = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
        loadingPanel.show(sender.get_id());
    }


    if (document.getElementById("<%=hidAutoScrollHashID.ClientID%>").value != "") {
        //window.location.hash = document.getElementById("<%=hidAutoScrollHashID.ClientID%>").value;
    }

    function ContextSelect(sender, args) {

        var row = args.get_itemIndexHierarchical();
        if (row >= 0) {
            var masterTable = $find("<%=grid1.ClientID%>").get_masterTableView();

            masterTable.clearSelectedItems();
            var dataItem = masterTable.get_dataItems()[row];
            dataItem.set_selected(true);

        }
    }

    function GridCreated(sender, eventArgs) {        
        
        var ss = self.document.getElementById("gridContainer");
        var offset = $(ss).offset();

        <%If grid1.ClientSettings.Scrolling.AllowScroll Then%>

        var hx1 = new Number;

        hx1 = $(window).height();
        var scrollArea = sender.GridDataDiv;


        hx1 = hx1 - offset.top;
        hx1 = hx1 - 5;
        ss.style.height = hx1 + "px";

        var scrollArea = sender.GridDataDiv;
        var parent = $get("gridContainer");
        var gridHeader = sender.GridHeaderDiv;

        scrollArea.style.height = hx1 + "px";
        <%end if%>
        <%If grid1.ClientSettings.Scrolling.UseStaticHeaders Then%>
        hx1 = parent.clientHeight - gridHeader.clientHeight - 20;        
        <%if grid1.ShowFooter then%>
        hx1 = hx1 - 40;
        
        <%End If%>
        scrollArea.style.height = hx1 + "px";
        <%end if%>        
        
    }

    function <%=Me.ClientID%>_SetScrollingHeight_Explicit(h) {
        var grid = $find("<%=grid1.ClientID%>");



        <%If grid1.ClientSettings.Scrolling.UseStaticHeaders = True Then%>
        var gridHeader = grid.GridHeaderDiv;
        var scrollArea = grid.GridDataDiv;

        h = h - gridHeader.clientHeight - 50;
        <%if grid1.ShowFooter then%>
        h = h - 40;
        <%End If%>

        scrollArea.style.height = h + "px";
        <%end if%>



    }

    function <%=Me.ClientID%>_Scroll2SelectedRow(containerHeight) {        
        if (containerHeight == null)
            containerHeight = $(window).height();

        var grid = $find("<%=grid1.ClientID%>");
        
        var row = grid.get_masterTableView().get_selectedItems()[0];
        
        //if the position of the selected row is below the viewable grid area  
        if (row) {            
            <%If grid1.ClientSettings.Scrolling.UseStaticHeaders = False Then%>
            var rowPos1 = row.get_element().offsetTop + row.get_element().offsetHeight + 20;

            if (rowPos1 > containerHeight) {

                window.location.hash = row.get_element().getAttribute("id");

            }



            <%else%>
            var scrollArea = grid.GridDataDiv;
            if ((row.get_element().offsetTop - scrollArea.scrollTop) + row.get_element().offsetHeight + 20 > scrollArea.offsetHeight) {
                //scroll down to selected row  
                scrollArea.scrollTop = scrollArea.scrollTop + ((row.get_element().offsetTop - scrollArea.scrollTop) +
                row.get_element().offsetHeight - scrollArea.offsetHeight) + row.get_element().offsetHeight;
            }
                //if the position of the the selected row is above the viewable grid area  
            else if ((row.get_element().offsetTop - scrollArea.scrollTop) < 0) {
                //scroll the selected row to the top  
                scrollArea.scrollTop = row.get_element().offsetTop;
            }
            <%end if%>
        }

    }

</script>
