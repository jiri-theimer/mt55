<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_create_step2.aspx.vb" Inherits="UI.p91_create_step2" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="billingmemo" Src="~/billingmemo.ascx" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Scripts/jqueryui/jquery-ui.min.css" />
    <script src="Scripts/jqueryui/jquery-ui.min.js" type="text/javascript"></script>

    <style type="text/css">
        .ui-autocomplete {
            width: 600px;
            height: 300px;
            overflow-y: auto;
            /* prevent horizontal scrollbar */
            overflow-x: hidden;
            font-family: 'Microsoft Sans Serif';
            z-index: 9900;
        }

        * html .ui-autocomplete {
            height: 300px;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {

           <%If Request.Browser.Browser = "IE" Or Request.Browser.Browser = "InternetExplorer" Then%>
            document.getElementById("search2").value = "";
            <%End If%>

        });





        function hardrefresh(pid, flag) {


            document.getElementById("<%=Me.hidHardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=Me.hidHardRefreshFlag.ClientID%>").value = flag;

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdRefresh, "", False)%>;

        }


        function o23_record(pid) {

            dialog_master("o23_record.aspx?billing=1&masterprefix=p28&masterpid=<%=Me.CurrentP28ID%>&pid=" + pid, true);

        }


        $(function () {

            $("#search2").autocomplete({
                <%If chkSearchByClientOnly.Checked Then%>
                source: "Handler/handler_search_invoice.ashx?p28id=<%=Me.p28id.Value%>",
                <%Else%>
                source: "Handler/handler_search_invoice.ashx",
                <%End If%>
                minLength: 1,
                select: function (event, ui) {
                    if (ui.item) {
                        //alert(ui.item.PID);

                        document.getElementById("<%=p91text1.clientid%>").value = ui.item.ItemComment;
                        return false;
                    }
                }



            }).data("ui-autocomplete")._renderItem = function (ul, item) {
                var s = "<div>";
                if (item.Closed == "1")
                    s = s + "<a style='text-decoration:line-through;'>";
                else
                    s = s + "<a>";

                s = s + __highlight(item.ItemText + "<br>" + item.ItemComment, item.FilterString);


                s = s + "</a>";

                if (item.Draft == "1")
                    s = s + "<img src='Images/draft.png' alt='DRAFT'/>"

                s = s + "</div>";


                return $(s).appendTo(ul);


            };
        });

        function __highlight(s, t) {
            var matcher = new RegExp("(" + $.ui.autocomplete.escapeRegex(t) + ")", "ig");
            return s.replace(matcher, "<strong>$1</strong>");
        }

        function search2Focus() {
            document.getElementById("search2").value = "";
            document.getElementById("search2").style.background = "yellow";
        }
        function search2Blur() {

            document.getElementById("search2").style.background = "";
            document.getElementById("search2").value = "Našeptávač fakturačního textu...";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panSelectedEntity" runat="server">
        <table cellpadding="10">
            <tr valign="top">

                <td>
                    <asp:Label ID="lblHeader" CssClass="framework_header_span" runat="server"></asp:Label>
                </td>
                <td>Klient (odběratel) faktury:
                </td>
                <td>
                    <uc:contact ID="p28id" runat="server" AutoPostBack="true" Width="300px" Flag="nondraft" />
                </td>

               
                <td>
                    <span>K fakturaci bez DPH:</span>
                    <asp:Label ID="TotalAmount" runat="server" CssClass="valbold" ForeColor="Green"></asp:Label>
                    <asp:Label ID="TotalHours" runat="server" CssClass="valboldblue" Style="margin-left: 20px;"></asp:Label>
                    <asp:Label ID="TotalCount" runat="server" CssClass="valbold" Style="margin-left: 20px;"></asp:Label>

                    
                </td>
            </tr>
        </table>
       

    </asp:Panel>
    <uc:billingmemo ID="bm1" runat="server" />

    <div class="content-box2">
        <div class="title">Výchozí nastavení dokladu faktury</div>
        <div class="content">
            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td style="width: 120px;">
                        <asp:Label ID="lblp92ID" Text="Typ faktury:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="p92ID" runat="server" AutoPostBack="true" DataTextField="p92Name" DataValueField="pid"></asp:DropDownList>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="opgCode" runat="server" RepeatDirection="Horizontal" ForeColor="blue">
                            <asp:ListItem Text="Vygenerovat DRAFT (koncept) faktury" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Vygenerovat oficiální číslo dokladu faktury" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblp91DateSupply" Text="Datum plnění:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p91DateSupply" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                    <td>
                        <asp:RadioButtonList ID="opgDateSupply" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Dnes" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Konec minulého měsíce" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Konec aktuálního měsíce" Value="3"></asp:ListItem>
                            <asp:ListItem Text="1.den příštího měsíce" Value="4"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblp91Date" Text="Datum vystavení:" runat="server" CssClass="lbl"></asp:Label></td>
                    <td>
                        <telerik:RadDatePicker ID="p91Date" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                    <td>
                        <asp:RadioButtonList ID="opgDate" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Dnes" Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="Konec minulého měsíce" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Konec aktuálního měsíce" Value="3"></asp:ListItem>
                            <asp:ListItem Text="1.den příštího měsíce" Value="4"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblp91DateMaturity" Text="Datum splatnosti:" runat="server" CssClass="lbl"></asp:Label></td>
                    <td>
                        <telerik:RadDatePicker ID="p91DateMaturity" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput3" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                    <td>
                        <span>Worksheet časový rámec faktury, začátek:</span>
                        <telerik:RadDatePicker ID="p91Datep31_From" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput4" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>
                        <span>Konec:</span>
                        <telerik:RadDatePicker ID="p91Datep31_Until" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput5" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
            </table>
            <div>
                <asp:CheckBox ID="chkRememberDates" runat="server" Text="V mé další faktuře nabízet ty samé datumy plnění a vystavení" />
                <asp:CheckBox ID="chkRememberMaturiy" runat="server" Text="V mé další faktuře nabízet to samé datum splatnosti" />
            </div>
        </div>
        <div class="div6">
            <span>Kontaktní osoba klienta:</span>
            <uc:datacombo ID="j02ID_ContactPerson" runat="server" DataValueField="pid" DataTextField="FullNameDescWithEmail" IsFirstEmptyRow="true" Width="400px" />
        </div>
        <div class="div6">
            Text faktury
            <input id="search2" style="width: 200px; margin-top: 7px;" value="Našeptávač fakturačního textu..." onfocus="search2Focus()" onblur="search2Blur()" />
            <asp:CheckBox ID="chkSearchByClientOnly" runat="server" AutoPostBack="true" Text="Hledat pouze ve fakturách klienta" />
            <br />
            <asp:TextBox ID="p91text1" runat="server" TextMode="MultiLine" Style="height: 50px; width: 90%;"></asp:TextBox>
        </div>
    </div>

    <table cellpadding="6">
        <tr>
            <td>
                <asp:DropDownList ID="cbxPaging" runat="server" AutoPostBack="true" ToolTip="Stránkování">
                    <asp:ListItem Text="20" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="50"></asp:ListItem>
                    <asp:ListItem Text="100"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:RadioButtonList ID="opgGroupBy" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                    <asp:ListItem Text="Bez souhrnů" Value="" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Fakturační oddíl" Value="p95"></asp:ListItem>
                    <asp:ListItem Text="Billing dávka" Value="p31ApprovingSet"></asp:ListItem>
                    <asp:ListItem Text="Sešit" Value="p34"></asp:ListItem>
                    <asp:ListItem Text="Aktivita" Value="p32"></asp:ListItem>
                    <asp:ListItem Text="Osoba" Value="j02"></asp:ListItem>
                    <asp:ListItem Text="Projekt" Value="p41"></asp:ListItem>
                    <asp:ListItem Text="Měna" Value="j27"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Button ID="cmdRemovePIDs" runat="server" Text="Označené (zaškrtlé) úkony vyjmout z fakturace" CssClass="cmd" Visible="false" />
            </td>
        </tr>
    </table>


    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>


    <asp:Panel ID="panComment" runat="server" CssClass="div6">
        <span class="infoInForm">Později můžete do již vytvořené faktury přidávat další úkony nebo je z faktury odebírat.</span>
    </asp:Panel>

    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidHardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidHardRefreshPID" runat="server" />
    <asp:HiddenField ID="hiddatapid_p31" runat="server" />
    <asp:HiddenField ID="hidIsDefTextFixed" runat="server" />
    <asp:Button ID="cmdRefresh" runat="server" Style="display: none;" />

    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
        <FastNavigationSettings EnableTodayButtonSelection="true"></FastNavigationSettings>
    </telerik:RadCalendar>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
