<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p47_framework.aspx.vb" Inherits="UI.p47_framework" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .RadGrid .rgRow td {
            border-left: solid 1px silver !important;
        }

        .RadGrid .rgAltRow td {
            border-left: solid 1px silver !important;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {


            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });



        });










        function RowSelected(sender, args) {

            document.getElementById("<%=hidCurP41ID.ClientID%>").value = args.getDataKeyValue("pid");

        }

        function RowDoubleClick(sender, args) {
            plan_edit();
        }

        function plan_edit() {
            var p41id = document.getElementById("<%=hidCurP41ID.clientid%>").value;
            if (p41id == "" || p41id == null) {
                alert("Není vybrán záznam.");
                return
            }
            sw_master("p47_project.aspx?year=<%=Me.CurrentYear%>&month=<%=Me.CurrentMonth%>&j02ids=<%=Me.hidJ02IDs.Value%>&pid=" + p41id,"Images/plan.png",true);

            

        }
        function plan_new() {
            var p41id = <%=Me.find_p41id.ClientID%>_get_value()
            if (p41id == "" || p41id == null) {
                alert("Není vybrán projekt.");
                return
            }

            sw_master("p47_project.aspx?year=<%=Me.CurrentYear%>&month=<%=Me.CurrentMonth%>&j02ids=<%=Me.hidJ02IDs.Value%>&pid=" + p41id,"Images/plan.png", true);

        }


        function hardrefresh(pid, flag) {
            document.getElementById("<%=hidCurP41ID.clientid%>").value = pid;
            document.getElementById("<%=hidHardRefreshFlag.ClientID%>").value = flag;
            
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefreshOnBehind, "", False)%>

        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="background-color: white; padding: 10px;">
        <div style="float: left;">
            <img src="Images/plan_32.png" />
            <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span" Text="Kapacitní plán"></asp:Label>
        </div>

        <div class="commandcell" style="padding-left: 20px;">
            <asp:DropDownList ID="query_year" runat="server" AutoPostBack="true"></asp:DropDownList>

            <asp:DropDownList ID="query_month" runat="server" AutoPostBack="true">
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

            <asp:ImageButton ID="cmdPrevMonth" runat="server" ImageUrl="Images/prevpage.png" ToolTip="Předchozí měsíc" />

            <asp:ImageButton ID="cmdNextMonth" runat="server" ImageUrl="Images/nextpage.png" ToolTip="Další měsíc" />
        </div>


        <div class="commandcell" style="padding-left:20px;">
            
            <button type="button" onclick="plan_new()">
                <img src="Images/new.png" />Zapsat plán do projektu:</button>
        </div>

        <div class="commandcell">
            <uc:project ID="find_p41id" runat="server" Width="300px" AutoPostBack="false" />
        </div>
        <div class="commandcell" style="padding-left:10px;">
            <asp:Button ID="cmdExport" runat="server" CssClass="cmd" Text="MS Excel" Style="height:26px;"/>
            
        </div>
        <div class="show_hide1" style="float: left; margin-top: 10px;">
            <button type="button">
                <img src="Images/arrow_down_menu.png" alt="Nastavení" />
                Nastavení

            </button>
        </div>
        <div style="clear: both; width: 100%;"></div>

        <div class="slidingDiv1">
            <div class="div6">
                <asp:DropDownList ID="cbxGroupBy" runat="server" AutoPostBack="true" ToolTip="Souhrny projektů podle" Visible="false">
                    <asp:ListItem Text="Bez souhrnů" Value=""></asp:ListItem>
                    <asp:ListItem Text="Souhrny podle klienta projektu" Value="Client"></asp:ListItem>
                    <asp:ListItem Text="Souhrny podle typu projektu" Value="p42Name"></asp:ListItem>
                    <asp:ListItem Text="Souhrny podle střediska projektu" Value="j18Name"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <asp:Panel ID="panPersonScope" runat="server" CssClass="content-box2">
                <div class="title">
                    Okruh zobrazených osob
                </div>
                <div class="content">
                    <table>
                        <tr>
                            <td>Osoba:
                            </td>
                            <td>
                                <uc:person ID="j02ID_Add" runat="server" Width="300px" />

                            </td>
                            <td rowspan="3" style="padding-left: 30px; text-align: center;">
                                <asp:LinkButton ID="cmdAppendJ02IDs" runat="server" CssClass="cmd" Text="Přidat" />
                                <p>nebo</p>
                                <asp:LinkButton ID="cmdReplaceJ02IDs" runat="server" CssClass="cmd" Text="Nahradit" />
                            </td>
                        </tr>
                        <tr>
                            <td>Tým osob:
                            </td>
                            <td>
                                <uc:datacombo ID="j11ID_Add" runat="server" DataTextField="j11Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>

                            </td>
                        </tr>
                        <tr>
                            <td>Pozice:
                            </td>
                            <td>
                                <uc:datacombo ID="j07ID_Add" runat="server" DataTextField="j07Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>

                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>

        </div>


        <div id="offsetY"></div>

        <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid" OnRowDblClick="RowDoubleClick" OnRowSelected="RowSelected"></uc:datagrid>


    </div>
    <asp:Button ID="cmdHardRefreshOnBehind" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidJ02IDs" runat="server" />
    <asp:HiddenField ID="hidCurP41ID" runat="server" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
</asp:Content>
