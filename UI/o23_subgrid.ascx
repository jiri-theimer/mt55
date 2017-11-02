<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="o23_subgrid.ascx.vb" Inherits="UI.o23_subgrid" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>

<div class="commandcell">
    <img src="Images/notepad.png" alt="Dokumenty" />
    <asp:Label ID="lblHeaderO23" CssClass="framework_header_span" runat="server" Text="Dokumenty"></asp:Label>
</div>
<div class="commandcell" style="margin-left: 10px;">
        <uc:mygrid id="designer1" runat="server" prefix="o23" MasterPrefixFlag="2"></uc:mygrid>
    </div>
<div class="commandcell" style="margin-left: 10px;">

    <asp:DropDownList ID="cbxGroupBy" runat="server" AutoPostBack="true" ToolTip="Datové souhrny" Visible="false">
        <asp:ListItem Text="Bez souhrnů" Value=""></asp:ListItem>
        <asp:ListItem Text="Typ dokumentu" Value="p57Name"></asp:ListItem>

    </asp:DropDownList>
</div>
<div class="commandcell" >
    <telerik:RadMenu ID="recmenu1" Skin="Metro" runat="server" EnableRoundedCorners="false" EnableShadows="false" ClickToOpen="true" style="z-index:2000;" RenderMode="Auto" ExpandDelay="0" ExpandAnimation-Type="None" EnableAutoScroll="true">
        <Items>
            <telerik:RadMenuItem Text="Nový" Value="new" NavigateUrl="javascript:o23_record(0,false)" ImageUrl="Images/new4menu.png"></telerik:RadMenuItem>
           
        
            <telerik:RadMenuItem Text="Další" ImageUrl="Images/menuarrow.png">
                <ContentTemplate>
                    <div style="padding: 20px; min-width: 200px;">
                      
                        <div>
                        <span>Stránkování:</span>
                        <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování">
                            <asp:ListItem Text="10"></asp:ListItem>
                            <asp:ListItem Text="20"></asp:ListItem>
                            <asp:ListItem Text="50" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="100"></asp:ListItem>
                            <asp:ListItem Text="200"></asp:ListItem>
                            <asp:ListItem Text="500"></asp:ListItem>
                        </asp:DropDownList>
                        </div>
                        <asp:hyperlink Text="Zobrazit přehled na celou stránku" id="cmdFullScreen" runat="server" NavigateUrl="javascript:o23_fullscreen()"></asp:hyperlink>
                    </div>
                </ContentTemplate>
            </telerik:RadMenuItem>
        </Items>
    </telerik:RadMenu>



</div>





<asp:HiddenField ID="hidDefaultSorting" runat="server" />
<asp:HiddenField ID="hidCols" runat="server" />
<asp:HiddenField ID="hidFrom" runat="server" />


<div style="clear: both; width: 100%;"></div>

<uc:datagrid ID="gridO23" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected_o23" OnRowDblClick="RowDoubleClick_o23"></uc:datagrid>

<script type="text/javascript">
    $(document).ready(function () {

      

    });

    function GetAllSelectedO23IDs() {

        var masterTable = $find("<%=gridO23.radGridOrig.ClientID%>").get_masterTableView();
        var sel = masterTable.get_selectedItems();
        var pids = "";

        for (i = 0; i < sel.length; i++) {
            if (pids == "")
                pids = sel[i].getDataKeyValue("pid");
            else
                pids = pids + "," + sel[i].getDataKeyValue("pid");
        }

        return (pids);
    }


    function o23_fullscreen() {
        window.open("o23_Framework.aspx?masterpid=<%=Me.MasterDataPID%>&masterprefix=<%=BO.BAS.GetDataPrefix(Me.x29ID)%>", "_top");
        
    }
</script>


