<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="p41_framework_rec_budget.aspx.vb" Inherits="UI.p41_framework_rec_budget" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="entity_menu" Src="~/entity_menu.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="vysledovka" Src="~/p45_vysledovka.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        function hardrefresh(pid, flag) {

            if (flag == "p41-create") {
                parent.window.location.replace("p41_framework.aspx?pid=" + pid);
                return;
            }
            if (flag == "p41-delete") {
                parent.window.location.replace("p41_framework.aspx");
                return;
            }


            location.replace("p41_framework_rec_budget.aspx?pid=<%=master.datapid%>");

        }
        function p45_record(pid, is_clone) {
            if (pid == -1)
                pid = document.getElementById("<%=Me.p45ID.ClientID%>").value;

            var clone = "0";
            if (is_clone == true) {
                <%If Me.p45ID.Items.Count > 0 Then%>
                pid = document.getElementById("<%=Me.p45ID.ClientID%>").value;
                clone = "1";
                <%End If%>
            }

            sw_decide("p45_record.aspx?clone=" + clone + "&p41id=<%=master.datapid%>&pid=" + pid, "Images/budget_32.png", true);

        }
        function p45_p46() {
            var p45id = document.getElementById("<%=Me.p45ID.ClientID%>").value;
            sw_decide("p45_p46.aspx?pid=" + p45id, "Images/budget_32.png", true);

        }
        function RowSelected_budget(sender, args) {
            document.getElementById("<%=hidBudgetPID.clientid%>").value = args.getDataKeyValue("pid");
        }

        function RowDoubleClick_budget(sender, args) {
            budget_edit();

        }
        function budget_edit() {
            var pid = document.getElementById("<%=hidBudgetPID.clientid%>").value;
            <%If Me.cmdBudgetP46.Checked Then%>
                p45_detail();
                <%End If%>
            <%If Me.cmdBudgetP49.Checked Then%>
                p49_record(pid);
            <%End If%>
            }
        function p49_record(pid) {
            var p45id = document.getElementById("<%=Me.p45ID.ClientID%>").value;
            sw_decide("p49_record.aspx?pid=" + pid + "&p45id=" + p45id, "Images/budget_32.png");
        }
        function p49_to_p31() {
            var p49id = document.getElementById("<%=hidBudgetPID.clientid%>").value;
            if (p49id == "" || p49id == null) {
                alert("Musíte vybrat položku rozpočtu.");
                return
            }

            sw_decide("p31_record.aspx?pid=0&p41id=<%=Master.DataPID%>&p49id=" + p49id, "Images/worksheet_32.png", true);
            return (false);
        }

        function p47_plan() {
            window.open("p47_project.aspx?pid=<%=Master.DataPID%>&p45id=<%=Me.p45ID.SelectedValue%>", "_blank");
            return;
            sw_decide("p47_project.aspx?pid=<%=Master.DataPID%>&p45id=<%=Me.p45ID.SelectedValue%>", "Images/plan_32.png", true);
            }

            function budgetprefix_change(prefix) {
                location.replace("p41_framework_rec_budget.aspx?pid=<%=Master.DataPID%>&budgetprefix=" + prefix);
        }

        function report_local() {
            var p45id = document.getElementById("<%=Me.p45ID.ClientID%>").value;
            sw_decide("report_modal.aspx?prefix=p45&pid=" + p45id, "Images/reporting.png", true);

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc:entity_menu id="menu1" runat="server"></uc:entity_menu>
    <div class="div6">
        <div style="float: left;">
            <asp:DropDownList ID="p45ID" runat="server" AutoPostBack="true" DataValueField="pid" DataTextField="VersionWithName" BackColor="yellow"></asp:DropDownList>
        </div>
        <div style="float: left;">
            <telerik:RadMenu ID="recmenu1" Skin="Metro" runat="server" ClickToOpen="true" Style="z-index: 2000;" RenderMode="Auto" ExpandDelay="0" ExpandAnimation-Type="None">
                <Items>
                    <telerik:RadMenuItem Text="Rozpočet" ImageUrl="Images/menuarrow.png">
                        <Items>
                            <telerik:RadMenuItem Text="Hlavička rozpočtu" Value="edit" NavigateUrl="javascript:p45_record(-1,false)"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Tisková sestava" Value="report" NavigateUrl="javascript:report_local()"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Nastavení časového rozpočtu" Value="p46" NavigateUrl="javascript:p45_p46()"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Kapacitní plán" Value="p47" NavigateUrl="javascript:p47_plan()"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Přidat položku finančního rozpočtu" Value="new_p49" NavigateUrl="javascript:p49_record(0)"></telerik:RadMenuItem>
                            <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Nový rozpočet" Value="new" NavigateUrl="javascript:p45_record(0,false)"></telerik:RadMenuItem>
                            <telerik:RadMenuItem Text="Zkopírovat rozpočet do nové verze" Value="clone" NavigateUrl="javascript:p45_record(0,true)"></telerik:RadMenuItem>
                        </Items>
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadMenu>



        </div>
        <div style="float: left;">

            <asp:RadioButton ID="cmdBudgetP46" Text="Časový rozpočet" AutoPostBack="false" runat="server" onclick="budgetprefix_change('p46')" />
            <asp:RadioButton ID="cmdBudgetP49" Text="Finanční rozpočet" AutoPostBack="false" runat="server" onclick="budgetprefix_change('p49')" />

            <asp:HyperLink ID="linkP47" runat="server" Text="Kapacitní plán" NavigateUrl="javascript:p47_plan()" Visible="false"></asp:HyperLink>

            <asp:HyperLink ID="linkNewP49" runat="server" NavigateUrl="javascript:p49_record(0)" Visible="false" Text="Přidat" ToolTip="Zapsat novou položku do finančního rozpočtu"></asp:HyperLink>
            <asp:HyperLink ID="linkConvert2P31" runat="server" NavigateUrl="javascript:p49_to_p31()" Visible="false" Text="Překlopit" ToolTip="Překlopit plánovaný výdaj/příjem do worksheet úkonu"></asp:HyperLink>


        </div>
    </div>
    <div style="clear: both;"></div>
    <uc:datagrid ID="gridBudget" runat="server" OnRowSelected="RowSelected_budget" OnRowDblClick="RowDoubleClick_budget"></uc:datagrid>

    <asp:Panel ID="panP49Setting" runat="server">
        <asp:CheckBox ID="chkP49GroupByP34" runat="server" Text="Mezisoučty za sešity" AutoPostBack="true" Checked="true" />
        <asp:CheckBox ID="chkP49GroupByP32" runat="server" Text="Mezisoučty za aktivity" AutoPostBack="true" />
    </asp:Panel>

    <asp:CheckBox ID="chkVysledovka" runat="server" Text="Zobrazovat rychlou výsledovku rozpočtu" AutoPostBack="true" Checked="true" />

    <uc:vysledovka id="stat1" runat="server"></uc:vysledovka>


    <asp:HiddenField ID="hidBudgetPID" runat="server" />
</asp:Content>
