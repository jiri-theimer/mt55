<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="report_framework.aspx.vb" Inherits="UI.report_framework" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="panelmenu" Src="~/panelmenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var _initResizing = "1";

        $(document).ready(function () {

            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });



        })

        function loadSplitter(sender) {
            var h1 = new Number;
            var h2 = new Number;
            var h3 = new Number;

            h1 = $(window).height();

            var ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            h3 = h1 - h2;

            sender.set_height(h3);

        }

        function AfterPaneResized(sender, args) {
            if (_initResizing == "1") {
                _initResizing = "0";
                return;
            }

            var w = sender.get_width();

            $.post("Handler/handler_userparam.ashx", { x36value: w, x36key: "report_framework-navigationPane_width", oper: "set" }, function (data) {
                if (data == ' ') {
                    return;
                }


            });


        }


        function sr(x31id, format) {

            document.getElementById("<%=hidx31id.ClientID%>").value = x31id;
            document.getElementById("<%=hidformat.ClientID%>").value = format;

            var splitter = $find("<%= RadSplitter1.ClientID %>");
            var pane = splitter.getPaneById("<%=contentPane.ClientID%>");

          

            if (format == "1") {
                pane.set_contentUrl("report_framework_detail1.aspx?x31id=" + x31id);
            }
            if (format == "3") {
                pane.set_contentUrl("report_framework_detail3.aspx?x31id=" + x31id);
            }
            if (format == "4") {
                pane.set_contentUrl("report_framework_detail4.aspx?x31id=" + x31id);
            }

        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="offsetY"></div>
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Width="100%" ResizeMode="Proportional" OnClientLoaded="loadSplitter" PanesBorderSize="0" Skin="Metro">
        <telerik:RadPane ID="navigationPane" runat="server" Width="350px" OnClientResized="AfterPaneResized" MaxWidth="1000" MinWidth="50">
            <table width="98%">
                <tr>
                    <td style="width: 35px;">
                        <img src="Images/reporting_32.png" />

                    </td>
                    <td>
                        <span class="framework_header_span" style="font-size:150%;">Sestavy a pluginy</span>
                    </td>
                    <td>
                        <asp:ImageButton ID="cmdExpandAll" runat="server" ImageUrl="Images/expand.png" ToolTip="Rozbalit vše" CssClass="button-link" />
                        <asp:ImageButton ID="cmdCollapseAll" runat="server" ImageUrl="Images/collapse.png" ToolTip="Sbalit vše" CssClass="button-link" />
                    </td>
                    <td style="display:none;">
                        <button type="button" id="cmdSetting" class="show_hide1" style="float: right;padding:0px;">
                                <img src="Images/setting.png" />
                                <img src="Images/arrow_down.gif" alt="Nastavení" />
                        </button>
                    </td>
                </tr>
            </table>

            <div class="slidingDiv1">
                <div class="div6">


                    <div class="div6" style="display: none;">
                        <fieldset style="border: dotted black 1px;">
                            <legend>Struktura přehledu sestav</legend>
                            <asp:RadioButtonList ID="opgStructure" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                                <asp:ListItem Text="Strom podle kategorie" Value="1" Selected="true"></asp:ListItem>
                                <asp:ListItem Text="Strom podle formátu" Value="2"></asp:ListItem>

                            </asp:RadioButtonList>
                        </fieldset>
                    </div>
                </div>
            </div>

            <uc:panelmenu ID="tree1" runat="server" width="100%"></uc:panelmenu>

            <asp:HiddenField ID="hidx31id" runat="server" />
            <asp:HiddenField ID="hidformat" runat="server" />
        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitbar1" runat="server" CollapseMode="Forward">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="contentPane" runat="server" ContentUrl="entity_framework_detail_missing.aspx?prefix=x31">
            Výstup sestavy
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
