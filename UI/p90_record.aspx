<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p90_record.aspx.vb" Inherits="UI.p90_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function recordcode() {

            dialog_master("record_code.aspx?prefix=p90&pid=<%=Master.DataPID%>");

        }
        function dppcode(p82id) {

            dialog_master("record_code.aspx?prefix=p82&pid="+p82id);

        }
        function hardrefresh(pid, flag, codeValue) {
            if (flag == "record-code") {
                location.replace("p90_record.aspx?pid=<%=master.DataPID%>");
            }



        }

        function report(x31id) {

            dialog_master("report_modal.aspx?prefix=p90&pid=<%=master.datapid%>&x31id="+x31id, true);

        }
        function report_dpp(x31id,p82id) {

            dialog_master("report_modal.aspx?prefix=p82&pid="+p82id+"&x31id=" + x31id, true);

        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti" Selected="true" Value="core"></telerik:RadTab>
            <telerik:RadTab Text="Štítky ({1}), uživatelská pole ({0})" Value="ff"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="core" runat="server" Selected="true">
            <table cellpadding="6" cellspacing="2">
                <tr>
                    <td style="width: 180px;">
                        <asp:Label ID="lblType" runat="server" CssClass="lblReq" Text="Typ zálohy:"></asp:Label></td>
                    <td>
                        <uc:datacombo ID="p89ID" runat="server" DataTextField="p89Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" AutoPostBack="true"></uc:datacombo>
                        <asp:HyperLink ID="p90Code" runat="server" ToolTip="Číslo zálohy"></asp:HyperLink>
                        <span style="padding-left: 30px;"></span>
                        <asp:HyperLink ID="link_x31id" runat="server" Text="Tisková sestava" NavigateUrl="javascript:report()"></asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblAmount" Text="Částka celkem:" runat="server" CssClass="lblReq"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p90Amount" runat="server" Width="100px"></telerik:RadNumericTextBox>
                        <uc:datacombo ID="j27ID" runat="server" DataTextField="j27Code" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="false" Width="100px"></uc:datacombo>

                        
                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="Label1" Text="Částka bez DPH:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="p90Amount_WithoutVat" runat="server" Width="100px"></telerik:RadNumericTextBox>
                        <asp:Label ID="Label2" Text="Sazba DPH:" runat="server" CssClass="lbl"></asp:Label>
                        <telerik:RadNumericTextBox ID="p90VatRate" runat="server" Width="30px" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                        <asp:Label ID="Label3" Text="Částka DPH:" runat="server" CssClass="lbl"></asp:Label>
                        <telerik:RadNumericTextBox ID="p90Amount_Vat" runat="server" Width="100px"></telerik:RadNumericTextBox>
                        
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button ID="cmdCalc1" runat="server" CssClass="cmd" Text="Dopočítat DPH z částky bez DPH" />
                        <asp:Button ID="cmdCalc2" runat="server" CssClass="cmd" Text="Dopočítat DPH z částky vč. DPH" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp90Date" Text="Datum vystavení:" runat="server" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p90Date" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp90DateMaturity" Text="Datum splatnosti:" runat="server" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p90DateMaturity" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput3" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblP28ID" runat="server" Text="Klient (odběratel):" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:contact ID="p28ID" runat="server" Width="400px" AutoPostBack="false" />

                    </td>
                </tr>


                
            </table>

            
            <div>Text zálohy</div>
            <div>
                <asp:TextBox ID="p90text1" runat="server" TextMode="MultiLine" Style="height: 70px; width: 100%;"></asp:TextBox>
            </div>
            <div>Technický text zálohy</div>
            <div>
                <asp:TextBox ID="p90text2" runat="server" TextMode="MultiLine" Style="height: 30px; width: 100%;"></asp:TextBox>
            </div>
            <uc:mytags ID="tags1" ModeUi="1" Prefix="p90" runat="server" />

            <div class="content-box2" style="margin-top:10px;">
                <div class="title">
                    Úhrada zálohy
                </div>
                <div class="content">

                    <asp:Button ID="cmdAddP82" runat="server" CssClass="cmd" Text="Přidat úhradu" />
                    <table cellpadding="8" cellspacing="2">
                        <tr>
                            <th>Datum úhrady</th>
                            <th>Částka úhrady</th>
                            <th>Text k dokladu o přijaté platbě</th>
                            <th></th>
                            <th></th>
                        </tr>
                    <asp:Repeater ID="rpP82" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <telerik:RadDatePicker ID="p82Date" runat="server" Width="120px" >
                                        <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td>
                                    <telerik:RadNumericTextBox ID="p82Amount" runat="server" Width="100px"></telerik:RadNumericTextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="p82Text" runat="server" style="width:300px;height:50px;" TextMode="MultiLine"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                    <asp:HiddenField ID="p85id" runat="server" />
                                </td>
                                <td>
                                    <asp:HyperLink ID="p82Code" runat="server" ToolTip="Číslo dokladu o přijaté platbě"></asp:HyperLink>
                    
                                    <asp:HyperLink ID="link_x31_dpp" runat="server" Text="Tisková sestava" NavigateUrl="javascript:report_dpp()"></asp:HyperLink>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    </table>
                </div>
            </div>

           <div class="div6">
               <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq"></asp:Label>
               <uc:person ID="j02ID_Owner" runat="server" Width="300px" Flag="all" />
           </div>
                        

                  

        </telerik:RadPageView>
        <telerik:RadPageView ID="ff" runat="server">

            <uc:freefields ID="ff1" runat="server" />

        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">
        <SpecialDays>
                    <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
                </SpecialDays>
    </telerik:RadCalendar>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
