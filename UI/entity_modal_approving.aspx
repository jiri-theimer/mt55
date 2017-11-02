<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="entity_modal_approving.aspx.vb" Inherits="UI.entity_modal_approving" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="plugin_datatable" Src="~/plugin_datatable.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="Plugins/Plugin.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        $(document).ready(function () {
            $(".slidingDiv1").hide();
            $(".show_hide1").show();


            $('.show_hide1').click(function () {

                $(".slidingDiv1").slideToggle();
            });

        });

        function sw_local(url, img, is_maximize) {
            dialog_master(url, is_maximize);
        }


        function periodcombo_setting() {

            sw_local("periodcombo_setting.aspx", "Images/settings_32.png");
        }




        function hardrefresh(pid, flag) {


            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }

        function get_ocas() {
            var s = "masterprefix=<%=Me.CurrentPrefix%>&masterpid=<%=master.DataPID%>&masterpids=<%=Me.CurrentInputPIDs%>&datefrom=<%=Format(period1.DateFrom, "dd.MM.yyyy")%>&dateuntil=<%=Format(period1.DateUntil,"dd.MM.yyyy")%>";
            s = s + "&approving_level=" + document.getElementById("<%=cbxApprovingLevel.ClientID%>").value;
            var aw = document.getElementById("<%=Me.hidMasterAW.ClientID%>").value;
            if (aw == "")
                return (s);

            return(s+"&aw="+encodeURI(aw));
        }

        function approve_all() {            
            location.replace("p31_approving_step1.aspx?"+get_ocas());
        }
        function approve_j02(j02id) {

            location.replace("p31_approving_step1.aspx?prefix=j02&pid=" + j02id+"&"+get_ocas());
        }
        function approve_p34(p34id) {

            location.replace("p31_approving_step1.aspx?prefix=p34&pid=" + p34id+"&"+get_ocas());
        }
        function approve_p41(p34id) {

            location.replace("p31_approving_step1.aspx?prefix=p41&pid=" + p41id+"&"+get_ocas())
        }
        function reapprove_all() {

            location.replace("p31_approving_step1.aspx?reapprove=1&"+get_ocas());
        }
        function clearapprove_all() {

            location.replace("p31_approving_step1.aspx?clearapprove=1&"+get_ocas());
        }


        
        function invoice() {            
            location.replace("p91_create_step1.aspx?nogateway=1&prefix=<%=Me.CurrentPrefix%>&pid=<%=master.DataPID%>&period=<%=Me.period1.SelectedValue%>&masterpids=<%=Me.CurrentInputPIDs%>");
        }
        function invoice_append() {

            location.replace("p91_add_worksheet_gateway.aspx?<%=Me.CurrentPrefix%>id=<%=master.DataPID%>&period=<%=Me.period1.SelectedValue%>");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="slidingDiv1" style="padding: 10px;">
        <fieldset style="padding: 10px;">
            <legend>Rozlišovat měny</legend>
            <asp:CheckBoxList ID="j27ids" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                <asp:ListItem Value="2" Text="CZK" Selected="true"></asp:ListItem>
                <asp:ListItem Value="3" Text="EUR" Selected="true"></asp:ListItem>
                <asp:ListItem Value="58" Text="USD"></asp:ListItem>
                <asp:ListItem Value="59" Text="GBP"></asp:ListItem>
            </asp:CheckBoxList>
        </fieldset>
        <fieldset style="padding: 10px;">
            <legend>Tlačítka pro dílčí schvalování</legend>
            <asp:CheckBox ID="chkCommandsP34" runat="server" Text="Za sešity" AutoPostBack="true" Checked="true" />
            <asp:CheckBox ID="chkCommandsJ02" runat="server" Text="Za osoby" AutoPostBack="true" />
            <asp:CheckBox ID="chkCommandsP41" runat="server" Text="Za projekty" AutoPostBack="true" Visible="false" />
        </fieldset>
    </div>


    <table cellpadding="10">
        <tr>


            <td>
                <span>Filtrovat období:</span>
            </td>
            <td>
                <uc:periodcombo ID="period1" runat="server" Width="220px"></uc:periodcombo>
            </td>
            <td>
                Úroveň schvalování:
                        <asp:DropDownList ID="cbxApprovingLevel" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="#0" Value="0"></asp:ListItem>
                            <asp:ListItem Text="#1" Value="1"></asp:ListItem>
                            <asp:ListItem Text="#2" Value="2"></asp:ListItem>
                        </asp:DropDownList>
            </td>
        </tr>
    </table>
    <telerik:RadTabStrip ID="tabs1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Rozpracované úkony - čeká na schvalování" Selected="true" Value="1" ImageUrl="Images/approve.png"></telerik:RadTab>
            <telerik:RadTab Text="Schválené úkony - čeká na fakturaci" Value="2" ImageUrl="Images/invoice.png"></telerik:RadTab>            
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="jedna" runat="server" Selected="true">


            <table cellpadding="6">
                <tr valign="bottom">
                    <td>
                        <span>Zobrazovat sloupce:</span>

                        <asp:DropDownList ID="col1" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Klient projektu" Value="p28"></asp:ListItem>
                            <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                            <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                            <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                            <asp:ListItem Text="Měsíc" Value="month"></asp:ListItem>
                            <asp:ListItem Text="Den" Value="day"></asp:ListItem>
                        </asp:DropDownList>
                        +
                        <asp:DropDownList ID="col2" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Klient projektu" Value="p28"></asp:ListItem>
                            <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                            <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                            <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                            <asp:ListItem Text="Měsíc" Value="month"></asp:ListItem>
                            <asp:ListItem Text="Den" Value="day"></asp:ListItem>
                        </asp:DropDownList>
                        +
                        <asp:DropDownList ID="col3" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Klient projektu" Value="p28"></asp:ListItem>
                            <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                            <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                            <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                            <asp:ListItem Text="Měsíc" Value="month"></asp:ListItem>
                            <asp:ListItem Text="Den" Value="day"></asp:ListItem>
                        </asp:DropDownList>

                    </td>

                </tr>
            </table>



            <div style="float: left;">
                <uc:plugin_datatable ID="plugin1" TableID="gridData" runat="server" ColHeaders="" ColHideRepeatedValues="1" ColTypes="" ColFlexSubtotals="" TableCaption="Worksheet rozpracovanost" NoDataMessage="Ani jeden rozpracovaný úkon." />
            </div>
            <div style="float: left;">
                
                <telerik:RadToolBar ID="tlb1" runat="server" Skin="Windows7" Orientation="Vertical" Width="200px" BorderStyle="None">
                    <Items>
                        <telerik:RadToolBarButton Value="all" Text="Schvalovat [vše]" NavigateUrl="javascript:approve_all()" ImageUrl="Images/approve.png"></telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
            </div>
            <div style="clear: both;"></div>


        </telerik:RadPageView>
        <telerik:RadPageView ID="dva" runat="server">



            <table cellpadding="6">
                <tr valign="bottom">
                    <td>
                        <span>Zobrazovat sloupce:</span>

                        <asp:DropDownList ID="col1x" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Billing dávka" Value="p31ApprovingSet"></asp:ListItem>
                            <asp:ListItem Text="Klient projektu" Value="p28"></asp:ListItem>
                            <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                            <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                            <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                            <asp:ListItem Text="Měsíc" Value="month"></asp:ListItem>
                            <asp:ListItem Text="Den" Value="day"></asp:ListItem>
                        </asp:DropDownList>
                        +
                        <asp:DropDownList ID="col2x" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Billing dávka" Value="p31ApprovingSet"></asp:ListItem>
                            <asp:ListItem Text="Klient projektu" Value="p28"></asp:ListItem>
                            <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                            <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                            <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                            <asp:ListItem Text="Měsíc" Value="month"></asp:ListItem>
                            <asp:ListItem Text="Den" Value="day"></asp:ListItem>
                        </asp:DropDownList>
                        +
                        <asp:DropDownList ID="col3x" runat="server" AutoPostBack="true">
                            <asp:ListItem Text="" Value=""></asp:ListItem>
                            <asp:ListItem Text="Billing dávka" Value="p31ApprovingSet"></asp:ListItem>
                            <asp:ListItem Text="Klient projektu" Value="p28"></asp:ListItem>
                            <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                            <asp:ListItem Text="Úkol" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                            <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                            <asp:ListItem Text="Měsíc" Value="month"></asp:ListItem>
                            <asp:ListItem Text="Den" Value="day"></asp:ListItem>
                        </asp:DropDownList>

                    </td>
                    
                </tr>
            </table>

            <div style="float: left;">
                <uc:plugin_datatable ID="plugin2" TableID="gridApproved" runat="server" ColHeaders="" ColHideRepeatedValues="1" ColTypes="" ColFlexSubtotals="" TableCaption="Prošlo schvalováním, čeká na fakturaci" NoDataMessage="Žádný schválený worksheet čekající na fakturaci." />
            </div>
            <div style="float: left;">
                <telerik:RadToolBar ID="tlb2" runat="server" Skin="Windows7" Orientation="Vertical" Width="200px" BorderStyle="None" >
                    <Items>
                        <telerik:RadToolBarButton Value="cmdCreateP91" Text="Vystavit fakturu" NavigateUrl="javascript:invoice()" ImageUrl="Images/invoice.png"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="cmdAppendP91" Text="Přidat do existující faktury" NavigateUrl="javascript:invoice_append()" ImageUrl="Images/invoice.png"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="cmdReApprove" Text="Pře-schválit již schválené" NavigateUrl="javascript:reapprove_all()" ImageUrl="Images/approve.png"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Value="cmdClearApprove" Text="Vyčistit schvalování" NavigateUrl="javascript:clearapprove_all()" ImageUrl="Images/break.png"></telerik:RadToolBarButton>
                        
                        

                    </Items>
                </telerik:RadToolBar>
            </div>


        </telerik:RadPageView>
      
    </telerik:RadMultiPage>

    <asp:HiddenField ID="hidX29ID" runat="server" Value="141" />
    <asp:HiddenField ID="hidPrefix" runat="server" Value="p41" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hidInputPIDS" runat="server" />
    <asp:HiddenField ID="hidMasterAW" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
