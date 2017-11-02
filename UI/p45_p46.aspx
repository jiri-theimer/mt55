<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p45_p46.aspx.vb" Inherits="UI.p45_p46" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="vysledovka" Src="~/p45_vysledovka.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });


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

            show_totals();

            save_cellvalue(row.attributes["p85id"].value, columnUniqueName, editorValue);


        }

        function show_needsave_message() {
            master_show_message("Změny se uloží až tlačítkem [Uložit změny].")
        }

        function show_totals() {
            grid = $find("<%=grid1.ClientID%>");

            var total_fa = 0;
            var total_nefa = 0;
            var total_total = 0;
            var total_fee_fa = 0;
            var total_fee_nefa = 0;

            var MasterTable = grid.get_masterTableView();

            var Rows = MasterTable.get_dataItems();

            for (var i = 0; i < Rows.length; i++) {
                var total_row = 0;
                var rate_billing = 0;
                var rate_cost = 0;
                var hours_fa = 0;
                var hours_nefa = 0;
                var row = Rows[i];
                var cell = row.get_element().cells[1].textContent;
                if (IsNumeric(cell)) {
                    hours_fa = parseFloat(cell);
                    total_fa += hours_fa;
                    total_row += hours_fa;
                }
                cell = row.get_element().cells[2].textContent;
                if (IsNumeric(cell)) {
                    hours_nefa = parseFloat(cell);
                    total_nefa += hours_nefa;
                    total_row += hours_nefa;
                }
                row.get_element().cells[3].textContent = total_row;
                total_total = total_total + total_row;

                cell = row.get_element().cells[4].textContent;
                if (IsNumeric(cell)) {
                    rate_billing = parseFloat(cell);
                }

                cell = row.get_element().cells[5];
                cell.textContent = hours_fa * rate_billing;
                total_fee_fa += hours_fa * rate_billing;

                cell = row.get_element().cells[6].textContent;
                if (IsNumeric(cell)) {
                    rate_cost = parseFloat(cell);
                }

                cell = row.get_element().cells[7];
                cell.textContent = (hours_nefa + hours_fa) * rate_cost;
                total_fee_nefa += (hours_nefa + hours_fa) * rate_cost;


            }
            $('tr.rgFooter').each(function () {
                $(this).find('td').eq(1).text(total_fa);
                $(this).find('td').eq(2).text(total_nefa);
                $(this).find('td').eq(3).text(total_total);
                $(this).find('td').eq(5).text(total_fee_fa);
                $(this).find('td').eq(7).text(total_fee_nefa);
            });




        }

        function IsNumeric(val) {
            return Number(parseFloat(val)) == val;
        }

        function save_cellvalue(p85id, field, cellValue) {
            var guid = "<%=viewstate("guid")%>";

            $.post("Handler/handler_p45_project.ashx", { guid: guid, p85id: p85id, value: cellValue, field: field, oper: "p46_value" }, function (data) {
                if (data == ' ') {
                    return;
                }

                show_needsave_message();
            });
        }

        
        function hardrefresh(pid, flag) {


            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">


    <div style="margin-top: 10px;">
        <button type="button" class="show_hide1" id="cmdAddPerson" runat="server">Okruh osob v časovém rozpočtu<img src="Images/arrow_down.gif" /></button>
    </div>


    <div class="slidingDiv1">
        <div class="content-box2">
            <div class="title">
                <asp:Label ID="lblInsertPersonsHeader" runat="server" Text=""></asp:Label>
                <asp:LinkButton ID="cmdInsertPersons" runat="server" CommandName="add" Text="Přidat zaškrtlé osoby do rozpočtu<img src='Images/arrow_down.gif' />"></asp:LinkButton>
            </div>
            <div class="content">
                <asp:Repeater ID="rpJ02" runat="server">
                    <ItemTemplate>
                        <div style="padding: 10px;">
                            <asp:HyperLink ID="clue_person" runat="server" CssClass="reczoom" Text="i"></asp:HyperLink>

                            <asp:CheckBox ID="Person" runat="server"></asp:CheckBox>

                            <asp:HiddenField ID="hidJ02ID" runat="server" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </div>




    <telerik:RadGrid ID="grid1" AutoGenerateColumns="false" AutoGenerateHierarchy="false" AutoGenerateEditColumn="false" EnableViewState="true" RenderMode="Lightweight" runat="server" PageSize="500" ShowFooter="true" AllowPaging="false" AllowSorting="true" Skin="Metro">
        <MasterTableView EditMode="Batch" ClientDataKeyNames="PID" NoMasterRecordsText="Zatím žádné osoby v rozpočtu">
            <ColumnGroups>
                <telerik:GridColumnGroup Name="Hodiny" HeaderText="Hodiny">
                    <HeaderStyle HorizontalAlign="Center" ForeColor="black" Font-Bold="true" BackColor="silver" />
                </telerik:GridColumnGroup>
                <telerik:GridColumnGroup Name="Billing" HeaderText="Fakturační cena">
                    <HeaderStyle HorizontalAlign="Center" ForeColor="black" Font-Bold="true" BackColor="silver" />
                </telerik:GridColumnGroup>
                <telerik:GridColumnGroup Name="Cost" HeaderText="Nákladová cena">
                    <HeaderStyle HorizontalAlign="Center" ForeColor="black" Font-Bold="true" BackColor="silver" />
                </telerik:GridColumnGroup>
            </ColumnGroups>

            <Columns>
                <telerik:GridBoundColumn HeaderText="Osoba" DataField="p85FreeText01" ReadOnly="true" AllowSorting="true"></telerik:GridBoundColumn>
                <telerik:GridNumericColumn HeaderText="Fa" DataField="p85FreeFloat01" ColumnEditorID="gridnumber1" ColumnGroupName="Hodiny">
                    <ItemStyle ForeColor="Green" />
                </telerik:GridNumericColumn>
                <telerik:GridNumericColumn HeaderText="NeFa" DataField="p85FreeFloat02" ColumnEditorID="gridnumber1" ColumnGroupName="Hodiny">
                    <ItemStyle ForeColor="Red" />
                </telerik:GridNumericColumn>
                <telerik:GridBoundColumn HeaderText="Fa+Nefa" DataField="p85FreeFloat03" ReadOnly="true" AllowSorting="true" ColumnGroupName="Hodiny">
                    <ItemStyle Font-Bold="true" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Sazba" DataField="p85FreeNumber01" ReadOnly="true" AllowSorting="true" ColumnGroupName="Billing">
                    <ItemStyle ForeColor="green" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Honorář" DataField="p85FreeNumber03" ReadOnly="true" AllowSorting="true" ColumnGroupName="Billing">
                    <ItemStyle ForeColor="green" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Sazba" DataField="p85FreeNumber02" ReadOnly="true" AllowSorting="true" ColumnGroupName="Cost">
                    <ItemStyle ForeColor="red" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Honorář" DataField="p85FreeNumber04" ReadOnly="true" AllowSorting="true" ColumnGroupName="Cost">
                    <ItemStyle ForeColor="red" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="p85OtherKey2" UniqueName="p85OtherKey2">
                    <ItemTemplate>
                        <asp:DropDownList ID="combo1" runat="server">
                            <asp:ListItem Value="1" Text="Zákaz vykázat přes Fa i přes Nefa"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Zákaz vykázat přes součet"></asp:ListItem>
                            <asp:ListItem Value="3" Text="Zákaz překročit Fa, lze překročit Nefa"></asp:ListItem>
                            <asp:ListItem Value="4" Text="Zákaz překročit Nefa, lze překročit Fa"></asp:ListItem>
                            <asp:ListItem Value="5" Text="Bez omezení vykazovat hodiny"></asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn UniqueName="delete" ButtonType="ImageButton" ImageUrl="Images/delete.png" ButtonCssClass="button-link" ItemStyle-Width="20px" CommandName="delete"></telerik:GridButtonColumn>
            </Columns>
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


    <div>
    <asp:Button ID="cmdRefreshStatement" runat="server" CssClass="cmd" Text="Přepočítat" />
    </div>
    <uc:vysledovka id="stat1" runat="server"></uc:vysledovka>

    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidP41ID" runat="server" />

    <script type="text/javascript">
        
        
        
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

