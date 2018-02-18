<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_navigator.aspx.vb" Inherits="UI.clue_navigator" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
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


        function rw(pid, prefix) {

            var url = prefix + "_framework_detail.aspx?source=3&pid=" + pid;

            window.open(url, "_top");

        }
        function hardrefresh(pid, flag) {

            //nic


        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:DropDownList ID="cbxPath" runat="server" AutoPostBack="true" ToolTip="Úrovně stromu navigátora">
        <asp:ListItem Text="Klient->Projekt" Value="p28-p41"></asp:ListItem>
        <asp:ListItem Text="Klient->Faktura" Value="p28-p91"></asp:ListItem>
        <asp:ListItem Text="Klient->Projekt->Faktura" Value="p28-p41-p91"></asp:ListItem>
        <asp:ListItem Text="Středisko->Projekt" Value="j18-p41"></asp:ListItem>
        <asp:ListItem Text="Projekt->Úkol" Value="p41-p56"></asp:ListItem>
        <asp:ListItem Text="Osoba->Úkol" Value="j02-p56"></asp:ListItem>
        <asp:ListItem Text="Stav projektu->Projekt" Value="b02-p41"></asp:ListItem>
        <asp:ListItem Text="Středisko->Stav projektu->Projekt" Value="j18-b02-p41"></asp:ListItem>
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


</asp:Content>
