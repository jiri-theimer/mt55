<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_create_step1.aspx.vb" Inherits="UI.p91_create_step1" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="billingmemo" Src="~/billingmemo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });

        });


        function periodcombo_setting() {

            dialog_master("periodcombo_setting.aspx");
        }



        function hardrefresh(pid, flag) {


            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }


        function o23_record(pid) {

            dialog_master("o23_record.aspx?billing=1&masterprefix=<%=me.currentprefix%>&masterpid=<%=master.datapid%>&pid=" + pid, true);

        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panGateWay" runat="server">
        <table cellpadding="10" cellspacing="0">
            <tr>

                <td>
                    <fieldset>
                        <legend>Postup fakturace</legend>
                        <asp:RadioButtonList ID="opgPrefix" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                            <asp:ListItem Text="Vyfakturovat schválené úkony klienta" Value="p28" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Vyfakturovat klienta zrychleně bez schvalování" Value="p28-draft"></asp:ListItem>
                            <asp:ListItem Text="Vyfakturovat schválené úkony projektu" Value="p41"></asp:ListItem>
                            <asp:ListItem Text="Vyfakturovat projekt zrychleně bez schvalování" Value="p41-draft"></asp:ListItem>
                            <asp:ListItem Text="Vyfakturovat schválené úkony osoby" Value="j02"></asp:ListItem>
                            <asp:ListItem Text="Vyfakturovat schválené úkony úkolu" Value="p56"></asp:ListItem>
                            <asp:ListItem Text="Vyfakturovat ručně vybrané úkony" Value="p31"></asp:ListItem>
                            
                            <asp:ListItem Text="Nejrychlejší fakturace jednou částkou" Value="quick" style="color: blue;"></asp:ListItem>
                        </asp:RadioButtonList>
                    </fieldset>
                </td>


                <td style="padding-left: 20px;">
                    <asp:Label ID="lblFindEntity" runat="server"></asp:Label>
                </td>
                <td>
                    <uc:contact ID="p28id" runat="server" AutoPostBack="true" Width="300px" Visible="false" />
                    <uc:project ID="p41id" runat="server" AutoPostBack="true" Width="300px" Visible="false" />
                    <uc:person ID="j02id" runat="server" AutoPostBack="true" Width="300px" Visible="false" />
                </td>

            </tr>
        </table>

    </asp:Panel>
    <asp:Panel ID="panQuick" runat="server" Visible="false" CssClass="content-box2">
        <div class="title">
            Nejrychlejší způsob, jak vytvořit fakturu s jedinou částkou ...dodatečné úpravy už provedete ve faktuře (DPH sazby, další cenové položky apod.)
        </div>
        <div class="content">
            <table cellpadding="6">
                <tr>
                    <td>Datum:</td>
                    <td>
                        <telerik:RadDatePicker ID="p31Date_Quick" runat="server" Width="120px" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red">
                            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                            <Calendar runat="server">
                                <SpecialDays>
                                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
                                </SpecialDays>
                            </Calendar>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td>Projekt:</td>
                    <td>
                        <uc:project ID="p41ID_Quick" runat="server" AutoPostBack="true" Width="400px" Flag="createinvoice" />
                    </td>
                </tr>
                <tr>
                    <td>Sešit:</td>
                    <td>
                        <uc:datacombo ID="p34ID_Quick" runat="server" Width="400px" DataTextField="p34Name" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="true" Filter="Contains" />
                    </td>
                </tr>
                <tr>
                    <td>Aktivita:
                    </td>
                    <td>
                        <uc:datacombo ID="p32ID_Quick" runat="server" Width="400px" DataTextField="p32Name" DataValueField="pid" IsFirstEmptyRow="true" Filter="Contains" />
                    </td>
                </tr>
                <tr>
                    <td>Částka bez DPH:</td>
                    <td>
                        <telerik:RadNumericTextBox ID="p31Amount_WithoutVat_Orig" runat="server" Width="110px" NumberFormat-DecimalDigits="2" ShowSpinButtons="false"></telerik:RadNumericTextBox>
                        <uc:datacombo ID="j27ID" Width="60px" runat="server" DataTextField="j27Code" DataValueField="pid"></uc:datacombo>

                    </td>
                </tr>
                <tr>
                    <td>Sazba DPH (%):
                    </td>
                    <td>
                        <uc:datacombo ID="p31VatRate_Orig" Width="60px" runat="server" AllowCustomText="true" Filter="StartsWith" DataValueField="pid" DataTextField="p53Value"></uc:datacombo>
                    </td>
                </tr>
            </table>
            <asp:TextBox ID="p31Text" runat="server" Style="height: 90px; width: 98%;" TextMode="MultiLine"></asp:TextBox>

        </div>
    </asp:Panel>
    <asp:Panel ID="panSelectedEntity" runat="server">
        <table cellpadding="10">
            <tr valign="top">
                <td>
                    <asp:Image ID="imgEntity" runat="server" />
                </td>
                <td>
                    <asp:Label ID="lblEntityHeader" CssClass="framework_header_span" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:Label ID="lblPeriodCaption" runat="server" Text="Časové období:"></asp:Label>
                </td>
                <td>
                    <uc:periodcombo ID="period1" runat="server" Width="250px"></uc:periodcombo>
                </td>
               
                <td>
                    <span>K fakturaci bez DPH:</span>
                    <asp:Label ID="TotalAmount" runat="server" CssClass="valbold" ForeColor="Green"></asp:Label>                    
                </td>
             
            </tr>
        </table>
       
        <asp:RadioButtonList ID="opgGroupBy" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                        <asp:ListItem Text="Bez souhrnů" Value="" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Souhrny podle fakturačních oddílů" Value="p95"></asp:ListItem>
                        <asp:ListItem Text="Souhrny podle sešitů" Value="p34"></asp:ListItem>
                        <asp:ListItem Text="Souhrny podle aktivit" Value="p32"></asp:ListItem>
                        <asp:ListItem Text="Souhrny podle osob" Value="j02"></asp:ListItem>
                        <asp:ListItem Text="Podle projektů" Value="p41"></asp:ListItem>
                        <asp:ListItem Text="Souhrny podle měny" Value="j27"></asp:ListItem>
                    </asp:RadioButtonList>


    </asp:Panel>
    <uc:billingmemo ID="bm1" runat="server" />

    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>


    <asp:Panel ID="panComment" runat="server" CssClass="div6">
        <span class="infoInForm">Později můžete do faktury přidávat další úkony nebo je z faktury odebírat.</span>
    </asp:Panel>

    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hiddatapid_p31" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />


</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


