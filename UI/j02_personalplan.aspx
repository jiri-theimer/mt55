<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="j02_personalplan.aspx.vb" Inherits="UI.j02_personalplan" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


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

            save_cellvalue(row.attributes["j02id"].value, columnUniqueName, editorValue)

        }

        function save_cellvalue(j02id, colName, cellValue) {
            var guid = document.getElementById("<%=Me.hidGUID.ClientID%>").value;
            var d1 = document.getElementById("<%=hidLimitD1.ClientID%>").value;
            var d2 = document.getElementById("<%=hidLimitD2.ClientID%>").value;

            $.post("Handler/handler_p66_plan.ashx", { guid: guid, d1: d1, d2: d2, j02id: j02id, value: cellValue, colName: colName, oper: "p66_plan_value" }, function (data) {
                if (data == ' ') {
                    return;
                }

                document.getElementById("<%=hidNeedSave.ClientID%>").value = "1"
                document.getElementById("<%=cbxField.ClientID%>").disabled = true;
                document.getElementById("<%=cbxJ11ID.ClientID%>").disabled = true;
                document.getElementById("<%=m1.ClientID%>").disabled = true;
                document.getElementById("<%=y1.ClientID%>").disabled = true;
                document.getElementById("lblNeedSave").style.display = "block";
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
    
    <table cellpadding="6">
        <tr>
            <td>
                <asp:DropDownList ID="cbxJ11ID" runat="server" DataTextField="j11Name" DataValueField="pid" ToolTip="Tým osob" AutoPostBack="true" style="max-width:250px;"></asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="cbxField" runat="server" AutoPostBack="true" BackColor="Yellow">
                    <asp:ListItem Text="Plán vyfakturovaných hodin" Value="p66HoursInvoiced"></asp:ListItem>
                    <asp:ListItem Text="Plán vykázaných fakturovatelných hodin" Value="p66HoursBillable"></asp:ListItem>
                    <asp:ListItem Text="Plán vykázaných nefakturovatelných hodin" Value="p66HoursNonBillable"></asp:ListItem>
                    <asp:ListItem Text="Plán celkově vykázaných hodin" Value="p66HoursTotal"></asp:ListItem>
                    <asp:ListItem Text="Volná veličina 1" Value="p66FreeNumber01"></asp:ListItem>
                    <asp:ListItem Text="Volná veličina 2" Value="p66FreeNumber02"></asp:ListItem>
                    <asp:ListItem Text="Volná veličina 3" Value="p66FreeNumber03"></asp:ListItem>
                    <asp:ListItem Text="Volná veličina 4" Value="p66FreeNumber04"></asp:ListItem>
                    <asp:ListItem Text="Volná veličina 5" Value="p66FreeNumber05"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <span>Rok:</span>
                <asp:DropDownList ID="y1" runat="server" AutoPostBack="true">                    
                </asp:DropDownList>
            </td>
            <td>
                <span>Začínat měsícem:</span>
                <asp:DropDownList ID="m1" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="Leden" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Únor" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Březen" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Duben" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Květen" Value="5"></asp:ListItem>
                    <asp:ListItem Text="Červen" Value="6"></asp:ListItem>
                    <asp:ListItem Text="Červenec" Value="7"></asp:ListItem>
                    <asp:ListItem Text="Srpen" Value="8"></asp:ListItem>
                    <asp:ListItem Text="Září" Value="9"></asp:ListItem>
                    <asp:ListItem Text="Říjen" Value="10"></asp:ListItem>
                    <asp:ListItem Text="Listopad" Value="11"></asp:ListItem>
                    <asp:ListItem Text="Prosinec" Value="12"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Label ID="lblPeriod" runat="server" CssClass="valboldblue"></asp:Label>
            </td>
            <td>
                <i style="color:red;display:none;" id="lblNeedSave">Změny se uloží až tlačítkem [Uložit změny] nebo [Uložit a zavřít].</i>
            </td>
        </tr>
    </table>

    <telerik:RadGrid ID="grid1" AutoGenerateColumns="false" AutoGenerateHierarchy="false" AutoGenerateEditColumn="false" EnableViewState="true" RenderMode="Lightweight" runat="server" PageSize="500" ShowFooter="true" AllowPaging="false" AllowSorting="true" Skin="Metro">
        <MasterTableView EditMode="Batch" ClientDataKeyNames="PID">
            <FooterStyle HorizontalAlign="left" BackColor="silver" Font-Bold="true" />
            <BatchEditingSettings EditType="Cell" OpenEditingEvent="Click" />
            <ItemStyle Height="35px" />
            <AlternatingItemStyle Height="35px" />
        </MasterTableView>
        <HeaderStyle Font-Bold="true" BackColor="LightBlue" />
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


    <asp:HiddenField ID="hidGUID" runat="server" />
    <asp:HiddenField ID="hidLimitD1" runat="server" />
    <asp:HiddenField ID="hidLimitD2" runat="server" />
    <asp:HiddenField ID="hidNeedSave" runat="server" Value="0" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
