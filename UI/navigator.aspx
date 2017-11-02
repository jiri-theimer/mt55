<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="navigator.aspx.vb" Inherits="UI.navigator" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var _initResizing = "1";

        $(document).ready(function () {
            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });

            if (document.getElementById("<%=hidSettingIsActive.ClientID%>").value == "1") {
                $(".slidingDiv1").show();
            }

            document.getElementById("<%=hidSettingIsActive.ClientID%>").value = "0";
        });

        function loadSplitter(sender) {

            var h1 = new Number;
            var h2 = new Number;
            var h3 = new Number;

            h1 = $(window).height();

            var ss = self.document.getElementById("offsetY");
            var offset = $(ss).offset();

            h2 = offset.top;

            h3 = h1 - h2-5;

            sender.set_height(h3);

            var pane = sender.getPaneById("<%=contentPane.ClientID%>");
            document.getElementById("<%=Me.hidContentPaneWidth.ClientID%>").value = pane.get_width();
            //pane.set_contentUrl(document.getElementById("<%=Me.hidContentPaneDefUrl.ClientID%>").value + "&parentWidth=" + pane.get_width());



        }

        function SavePaneWidth(w) {
            $.post("Handler/handler_userparam.ashx", { x36value: w, x36key: "navigator-navigationPane_width", oper: "set" }, function (data) {
                if (data == ' ') {
                    return;
                }


            });
        }

        function AfterPaneResized(sender, args) {
            if (_initResizing == "1") {
                _initResizing = "0";
                return;
            }

            var w = sender.get_width();
            SavePaneWidth(w);

        }

        function AfterPaneCollapsed(pane) {
            var w = "-1";
            SavePaneWidth(w);
        }
        function AfterPaneExpanded(pane) {
            var w = pane.get_width();
            SavePaneWidth(w);
        }

        function rw(pid, prefix) {

            var splitter = $find("<%= RadSplitter1.ClientID %>");
            var pane = splitter.getPaneById("<%=contentPane.ClientID%>");

            pane.set_contentUrl(prefix + "_framework_detail.aspx?source=navigator&pid=" + pid + "&parentWidth=" + pane.get_width());


        }
        function hardrefresh(pid, flag) {

            //nic


        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="offsetY"></div>
    <telerik:RadSplitter ID="RadSplitter1" runat="server" Width="100%" ResizeMode="Proportional" OnClientLoad="loadSplitter" PanesBorderSize="0" Skin="Metro" RenderMode="Lightweight" Orientation="Vertical">
        <telerik:RadPane ID="navigationPane" runat="server" Width="350px" OnClientResized="AfterPaneResized" OnClientCollapsed="AfterPaneCollapsed" OnClientExpanded="AfterPaneExpanded" BackColor="white">
            <asp:DropDownList ID="cbxPath" runat="server" AutoPostBack="true" ToolTip="Úrovně stromu navigátora">
                <asp:ListItem Text="Klient->Projekt" Value="p28-p41"></asp:ListItem>                                
                <asp:ListItem Text="Klient->Faktura" Value="p28-p91"></asp:ListItem>
                <asp:ListItem Text="Klient->Projekt->Faktura" Value="p28-p41-p91"></asp:ListItem>
                <asp:ListItem Text="Středisko->Projekt" Value="j18-p41"></asp:ListItem>
                <asp:ListItem Text="Projekt->Úkol" Value="p41-p56"></asp:ListItem>           
                <asp:ListItem Text="Osoba->Úkol" Value="j02-p56"></asp:ListItem>                          
            </asp:DropDownList>

            <button type="button" id="cmdSetting" class="show_hide1" style="float: right; padding: 3px; border-radius: 4px; border-top: solid 1px silver; border-left: solid 1px silver; border-bottom: solid 1px gray; border-right: solid 1px gray; background: buttonface;" title="Nastavit si úrovně navigátora">
                <span>Nastavení</span>
                <img src="Images/arrow_down.gif" />
            </button>
            <div style="clear: both;"></div>
            <div class="slidingDiv1">

                <asp:HiddenField ID="hidLevel0" runat="server" />
                <asp:HiddenField ID="hidLevel1" runat="server" />
                <asp:HiddenField ID="hidLevel2" runat="server" />
                <asp:HiddenField ID="hidLevel3" runat="server" />

                <asp:Button ID="cmdRefresh" runat="server" CssClass="cmd" Text="Obnovit" />
                <div style="margin-top: 20px;">
                    <asp:RadioButtonList ID="opgBIN" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="Pouze otevřené záznamy" Value="0" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Otevřené i v archivu" Value="-1"></asp:ListItem>
                        <asp:ListItem Text="Pouze v archivu" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
                <hr />
            </div>

            <telerik:RadTreeView ID="tr1" runat="server" Skin="Default" ShowLineImages="false" SingleExpandPath="true">
            </telerik:RadTreeView>


            <asp:HiddenField ID="hiddatapid" runat="server" />
            <asp:HiddenField ID="hidDefaultSorting" runat="server" />

            <asp:HiddenField ID="hidX29ID" runat="server" Value="141" />
            <asp:HiddenField ID="hidPrefix" runat="server" Value="p41" />

            <asp:HiddenField ID="hidCols" runat="server" />
            <asp:HiddenField ID="hidSumCols" runat="server" />
            <asp:HiddenField ID="hidAdditionalFrom" runat="server" />
            <asp:HiddenField ID="hidContentPaneWidth" runat="server" />
            <asp:HiddenField ID="hidContentPaneDefUrl" runat="server" />
            <asp:HiddenField ID="hidSettingIsActive" runat="server" Value="" />

        </telerik:RadPane>
        <telerik:RadSplitBar ID="RadSplitbar1" runat="server" CollapseMode="Forward">
        </telerik:RadSplitBar>
        <telerik:RadPane ID="contentPane" runat="server" ShowContentDuringLoad="true" ContentUrl="blank.aspx">
            Detail projektu
        </telerik:RadPane>
    </telerik:RadSplitter>
</asp:Content>
