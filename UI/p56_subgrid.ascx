<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="p56_subgrid.ascx.vb" Inherits="UI.p56_subgrid" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="mygrid" Src="~/mygrid.ascx" %>

<div class="commandcell">
    <img src="Images/task.png" alt="Úkoly" />
    <asp:Label ID="lblHeaderP56" CssClass="framework_header_span" runat="server" Text="Úkoly"></asp:Label>
</div>
<div class="commandcell" style="margin-left: 10px;">


    <asp:DropDownList ID="cbxP56Validity" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Otevřené i uzavřené úkoly" Value="1" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Pouze otevřené úkoly" Value="2"></asp:ListItem>
        <asp:ListItem Text="Pouze uzavřené úkoly" Value="3"></asp:ListItem>
    </asp:DropDownList>
</div>
<div class="commandcell">
        <uc:mygrid id="designer1" runat="server" prefix="p56" MasterPrefixFlag="2"></uc:mygrid>
    </div>
<div class="commandcell" style="margin-left: 10px;">
    <telerik:RadMenu ID="recmenu1" Skin="Metro" runat="server" ClickToOpen="true" EnableRoundedCorners="false" EnableShadows="false" Style="z-index: 2000;" RenderMode="Auto" ExpandDelay="0" ExpandAnimation-Type="None" EnableAutoScroll="true">
        <Items>
            <telerik:RadMenuItem Text="Nový úkol" Value="new" NavigateUrl="javascript:p56_record()" ImageUrl="Images/new4menu.png"></telerik:RadMenuItem>
          
            <telerik:RadMenuItem Text="Vybrané (zaškrtlé)" Value="akce" ImageUrl="Images/menuarrow.png">
                <Items>
                    
                    <telerik:RadMenuItem Text="Schválit úkony u označených úkolů" Value="cmdApprove" NavigateUrl="javascript:approving()"></telerik:RadMenuItem>
                    
                </Items>
            </telerik:RadMenuItem>
            <telerik:RadMenuItem Text="Další" ImageUrl="Images/menuarrow.png">
                <ContentTemplate>
                    <div class="content-box3">
                        <div class="title">
                            <img src="Images/griddesigner.png" />Nastavení přehledu
                        </div>
                        <div class="content">
                            <div class="div6">
                                <asp:DropDownList ID="cbxGroupBy" runat="server" AutoPostBack="true" ToolTip="Datové souhrny">
                                    <asp:ListItem Text="Bez souhrnů" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Typ úkolu" Value="p57Name"></asp:ListItem>
                                    <asp:ListItem Text="Klient" Value="Client"></asp:ListItem>
                                    <asp:ListItem Text="Projekt" Value="ProjectCodeAndName"></asp:ListItem>
                                    <asp:ListItem Text="Příjemce" Value="ReceiversInLine"></asp:ListItem>
                                    <asp:ListItem Text="Aktuální stav" Value="b02Name"></asp:ListItem>
                                    <asp:ListItem Text="Milník" Value="o22Name"></asp:ListItem>
                                    <asp:ListItem Text="Vlastník" Value="Owner"></asp:ListItem>
                                    <asp:ListItem Text="Uzavřeno" Value="IsClosed"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            

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
                        </div>
                    </div>



                    <asp:Panel ID="panExport" runat="server" CssClass="content-box3">
                        <div class="title">
                            <img src="Images/export.png" />
                            <span>Export záznamů aktuálního přehledu</span>
                        </div>
                        <div class="content">
                            
                            <asp:button ID="cmdExport" runat="server" Text="Export" CssClass="cmd" ToolTip="Export do MS EXCEL tabulky, plný počet záznamů" />

                           
                            <asp:button ID="cmdXLS" runat="server" Text="XLS" CssClass="cmd" ToolTip="Export do XLS vč. souhrnů s omezovačem na maximálně 2000 záznamů" />

                            
                            <asp:button ID="cmdPDF" runat="server" Text="PDF" CssClass="cmd" ToolTip="Export do PDF vč. souhrnů s omezovačem na maximálně 2000 záznamů" />

                            
                            <asp:button ID="cmdDOC" runat="server" Text="DOC" CssClass="cmd" ToolTip="Export do DOC vč. souhrnů s omezovačem na maximálně 2000 záznamů" />
                        </div>
                    </asp:Panel>
                    <div class="content-box3">
                        <div class="title"></div>
                        <div class="content">
                            <img src="Images/fullscreen.png" />
                            <asp:HyperLink ID="cmdFullScreen" runat="server" Text="Zobrazit přehled na celou stránku" NavigateUrl="javascript:p56_fullscreen()"></asp:HyperLink>
                        </div>
                    </div>

                </ContentTemplate>
            </telerik:RadMenuItem>
        </Items>
    </telerik:RadMenu>

</div>






<asp:HiddenField ID="hidReceiversInLine" runat="server" />
<asp:HiddenField ID="hidTasksWorksheetColumns" runat="server" />
<asp:HiddenField ID="hidDefaultSorting" runat="server" />
<asp:HiddenField ID="hidCols" runat="server" />
<asp:HiddenField ID="hidSumCols" runat="server" />
<asp:HiddenField ID="hidFrom" runat="server" />
<asp:HiddenField ID="hidFooterString" runat="server" />

<div style="clear: both; width: 100%;"></div>

<uc:datagrid ID="gridP56" runat="server" ClientDataKeyNames="pid" OnRowSelected="RowSelected_p56" OnRowDblClick="RowDoubleClick_p56"></uc:datagrid>

<script type="text/javascript">
    $(document).ready(function () {




    });

    function GetAllSelectedP56IDs() {

        var masterTable = $find("<%=gridP56.radGridOrig.ClientID%>").get_masterTableView();
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

    function approving() {
        var pids = GetAllSelectedP56IDs();
        if (pids == "") {
            alert("Není vybrán ani jeden záznam.");
            return;

        }
        p56_subgrid_approving(pids);



    }

    function p56_fullscreen() {
        window.open("p56_Framework.aspx?masterpid=<%=Me.MasterDataPID%>&masterprefix=<%=BO.BAS.GetDataPrefix(Me.x29ID)%>", "_top");

    }

    function go2workflow() {

        window.parent.sw_everywhere("workflow_dialog.aspx?prefix=p56&pid=<%=Me.MasterDataPID%>", "Images/task.png", true);

    }
</script>
