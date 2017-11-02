<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p47_project.aspx.vb" Inherits="UI.p47_project" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {




        });

        $(window).load(function (e) {
            show_totals();
        });


        function BatchEditCellValueChanged(sender, args) {
            var row = args.get_row();
            var cell = args.get_cell();
            var tableView = args.get_tableView();
            var column = args.get_column();
            var columnUniqueName = args.get_columnUniqueName();
            var editorValue = args.get_editorValue();
            var cellValue = args.get_cellValue();

            save_cellvalue(row.attributes["p46id"].value, columnUniqueName, editorValue)

        }

        function save_cellvalue(p46id, colName, cellValue) {
            var guid = "<%=viewstate("guid")%>";
            var d1 = document.getElementById("<%=hidLimD1.clientid%>").value;
            var d2 = document.getElementById("<%=hidLimD2.ClientID%>").value;

            $.post("Handler/handler_capacity_plan.ashx", { guid: guid, d1: d1, d2: d2, p46id: p46id, value: cellValue, colName: colName, oper: "capacity_plan_value" }, function (data) {
                if (data == ' ') {
                    return;
                }


                show_totals();

            });
        }

        function show_totals() {
            grid = $find("<%=grid1.ClientID%>");

            var total = 0;
            var cols = $('tr.rgFooter').find('td').length;

            var MasterTable = grid.get_masterTableView();

            var Rows = MasterTable.get_dataItems();

            for (var col = 2; col < cols; col++) {
                total = 0;

                for (var i = 0; i < Rows.length; i++) {
                    var row = Rows[i];
                    var cell = row.get_element().cells[col].textContent;

                    if (IsNumeric(cell))
                        total += parseFloat(cell);

                }
                $('tr.rgFooter').each(function () {
                    $(this).find('td').eq(col).text(total);
                });

            }

            var total_total = 0;
            for (var i = 0; i < Rows.length; i++) {
                var row = Rows[i];
                total = 0;

                for (var col = 2; col < cols; col++) {
                    var cell = row.get_element().cells[col].textContent;
                    if (IsNumeric(cell)) {
                        total += parseFloat(cell);
                        total_total += parseFloat(cell);

                    }

                }

                var cell = row.get_element().cells[1].textContent = total;
            }

            $('tr.rgFooter').find('td').eq(1).text(total_total);


        }

        function IsNumeric(val) {
            return Number(parseFloat(val)) == val;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
   
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">


    <div style="float: left; padding: 6px;">
        <asp:Label ID="lblHeader" runat="server" CssClass="valboldblue"></asp:Label>
        <asp:Label ID="lblBudget" runat="server" CssClass="valboldblue" style="float:right;"></asp:Label>
    </div>
    <div style="clear: both;"></div>
    <div style="float: left; padding: 6px;">
        <asp:DropDownList ID="m1" runat="server" AutoPostBack="true"></asp:DropDownList>
        ->
       <asp:DropDownList ID="m2" runat="server" AutoPostBack="true"></asp:DropDownList>
    </div>
    <div style="clear: both;"></div>



    <telerik:RadGrid ID="grid1" AutoGenerateColumns="false" AutoGenerateHierarchy="false" AutoGenerateEditColumn="false" EnableViewState="true" RenderMode="Lightweight" runat="server" PageSize="500" ShowFooter="true" AllowPaging="false" AllowSorting="true" Skin="Metro">
        <MasterTableView EditMode="Batch" ClientDataKeyNames="PID">
            <FooterStyle HorizontalAlign="left" BackColor="silver" Font-Bold="true" />
            <BatchEditingSettings EditType="Cell" OpenEditingEvent="Click" />
            <ItemStyle Height="35px" />
            <AlternatingItemStyle Height="35px" />
        </MasterTableView>
        <ExportSettings ExportOnlyData="true" OpenInNewWindow="true" FileName="marktime_export" UseItemStyles="false">
            <Excel Format="Biff" />
        </ExportSettings>

        <ClientSettings AllowKeyboardNavigation="true" EnableAlternatingItems="true">
            <Scrolling AllowScroll="false" FrozenColumnsCount="1" UseStaticHeaders="false" />
            <Selecting CellSelectionMode="SingleCell" AllowRowSelect="false" />
            <ClientEvents OnBatchEditCellValueChanged="BatchEditCellValueChanged" />
            <KeyboardNavigationSettings EnableKeyboardShortcuts="false" />
        </ClientSettings>
        <PagerStyle Position="TopAndBottom" AlwaysVisible="false" />
        <FooterStyle BackColor="#bcc7d8" HorizontalAlign="Right" />

    </telerik:RadGrid>

    <telerik:GridNumericColumnEditor ID="gridnumber1" runat="server" NumericTextBox-Width="30px" NumericTextBox-BackColor="LightGoldenrodYellow">
        <NumericTextBox NumberFormat-DecimalDigits="0" IncrementSettings-InterceptArrowKeys="false"></NumericTextBox>
    </telerik:GridNumericColumnEditor>
    <div class="div6">
        <asp:Button ID="cmdClear" runat="server" Text="Komplet vyčistit" CssClass="cmd" />
    </div>

    <div class="content-box2" style="display:none;">
        <div class="title">
            Výjimky z kapacitního plánu
            <asp:Button ID="cmdAddP44" runat="server" text="Přidat" CssClass="cmd" />
        </div>
        <div class="content">
            <table>
                <asp:Repeater ID="rpP44" runat="server">
                    <ItemTemplate>
                    <tr>
                        <td>
                            <asp:DropDownList ID="p44DateFrom" runat="server"></asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="p44DateUntil" runat="server"></asp:DropDownList>
                        </td>
                        <td>
                            <asp:DropDownList ID="p44ExceptionFlag" runat="server">
                                <asp:ListItem Value="1" Text="Hodiny se načtou z operativního plánu"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" ToolTip="Odstranit položku" />
                            <asp:HiddenField ID="p85ID" runat="server" />
                        </td>
                    </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
    </div>

    <asp:HiddenField ID="hidLimD1" runat="server" />
    <asp:HiddenField ID="hidLimD2" runat="server" />
    <asp:HiddenField ID="hidP45ID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
