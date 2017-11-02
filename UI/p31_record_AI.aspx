<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_record_AI.aspx.vb" Inherits="UI.p31_record_AI" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">


    <script type="text/javascript">
        $(document).ready(function () {

            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });




        });

        function hardrefresh(pid, flag) {
            location.replace("p31_record_AI.aspx?pid=<%=Master.datapid%>");

        }

        function p31_comment_create() {
            dialog_master("b07_create.aspx?masterprefix=p31&masterpid=<%=master.datapid%>", true);

        }
        function p31_comment_reaction(b07id) {

            dialog_master("b07_create.aspx?parentpid=" + b07id + "&masterprefix=p31&masterpid=<%=master.datapid%>", true)

        }
        function changelog() {
            location.replace("changelog.aspx?prefix=p31&pid=<%=Master.DataPID%>");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content-box2">
        <div class="title">Upravit položku faktury</div>
        <div class="content">
            <table cellpadding="6" cellspacing="2">
                <tr valign="top">
                    <td>
                        <asp:RadioButtonList ID="Edit_p70ID" runat="server" AutoPostBack="true" RepeatDirection="Vertical" CellPadding="3">
                            <asp:ListItem Text="<img src='Images/a14.gif'/>Fakturovat" Value="4"></asp:ListItem>
                            <asp:ListItem Text="<img src='Images/a16.gif'/>Zahrnout do paušálu" Value="6"></asp:ListItem>
                            <asp:ListItem Text="<img src='Images/a12.gif'/>Viditelný odpis" Value="2"></asp:ListItem>
                            <asp:ListItem Text="<img src='Images/a13.gif'/>Skrytý odpis" Value="3"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                    <td>
                        <asp:Panel ID="panEdit" runat="server">
                            <table>
                                <tr>

                                    <td>
                                        <asp:Label ID="lblEdit_Value" runat="server" Text="Fakturované hodiny:" CssClass="lbl"></asp:Label>
                                    </td>
                                    <td>
                                        
                                        <asp:TextBox ID="Edit_p31Value_Invoiced" runat="server" style="width:100px;"></asp:TextBox>
                                        <asp:Label ID="value_j27code" runat="server"></asp:Label>
                                    </td>

                                </tr>
                                <tr>

                                    <td>
                                        <asp:Label ID="lblEdit_Rate" runat="server" Text="Fakturační sazba:" CssClass="lbl"></asp:Label>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="Edit_p31Rate_Billing_Invoiced" runat="server" Width="90px" NumberFormat-DecimalDigits="2"></telerik:RadNumericTextBox>
                                        <telerik:RadNumericTextBox ID="Edit_ManualFee" runat="server" Width="120px" NumberFormat-DecimalDigits="2" Visible="false"></telerik:RadNumericTextBox>
                                        <asp:Label ID="rate_j27code" runat="server"></asp:Label>
                                    </td>

                                </tr>
                              
                                <tr>

                                    <td>
                                        <asp:Label ID="lblEdit_VatRate" runat="server" Text="DPH sazba (%):" CssClass="lbl"></asp:Label>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="Edit_p31VatRate_Invoiced" runat="server" Width="50px" NumberFormat-DecimalDigits="2"></telerik:RadNumericTextBox>
                                    </td>

                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <asp:RadioButtonList ID="p31IsInvoiceManual" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                                            <asp:ListItem Text="Částka bude vycházet ze schvalování s přepočtem podle měnového kurzu faktury" Value="0" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Částka se už dále nebude automaticky přepočítávat" Value="1"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </td>

                </tr>

            </table>
            <asp:panel ID="panEdit6" runat="server" cssclass="div6" Visible="false">
                <asp:Label ID="lblValue_FixPrice" runat="server" Text="Hodiny v paušálu:" CssClass="lbl"></asp:Label>
                <asp:TextBox ID="Edit_p31Value_FixPrice" runat="server" style="width:100px;"></asp:TextBox>
            </asp:panel>
            <div>
                <asp:Label ID="lblP31Text" runat="server" Text="Text:" CssClass="lbl"></asp:Label>
            </div>
            <asp:TextBox ID="Edit_p31Text" runat="server" TextMode="MultiLine" Style="width: 100%; height: 50px;"></asp:TextBox>
            <uc:freefields ID="ff1" runat="server" />
        </div>
        
    </div>
    


    <table>
        <tr valign="top">
            <td>
                <table cellpadding="5" cellspacing="2">
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblPerson" Text="Osoba:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="Person" runat="server" CssClass="valbold" />
                        </td>

                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblClient" Text="Klient:" CssClass="lbl"></asp:Label></td>
                        <td>
                            <asp:Label ID="Client" runat="server" CssClass="valbold" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblProject" Text="Projekt:" CssClass="lbl"></asp:Label>

                        </td>
                        <td>
                            <asp:Label ID="p41Name" runat="server" CssClass="valbold" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblP56" Text="Úkol:" CssClass="lbl"></asp:Label></td>
                        <td>
                            <asp:Label ID="Task" runat="server" CssClass="valbold" />
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblP34" Text="Sešit:" CssClass="lbl"></asp:Label></td>
                        <td>
                            <asp:Label ID="p34name" runat="server" CssClass="valbold" />
                        </td>
                    </tr>
                    <tr valign="top">
                        <td>
                            <asp:Label runat="server" ID="lblP32" Text="Aktivita:" CssClass="lbl"></asp:Label></td>
                        <td>
                            <asp:Label ID="p32name" runat="server" CssClass="valbold" />
                            <asp:Label ID="billable" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblDate" Text="Datum:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="p31date" runat="server" CssClass="valbold" />

                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblValueOrig" Text="Vykázaná hodnota:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="p31value_orig" runat="server" CssClass="valbold" />
                            <asp:Label ID="j27ident_orig" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label runat="server" ID="lblBillingRate_Orig" Text="Výchozí fakturační sazba:" CssClass="lbl"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="p31Rate_Billing_Orig" runat="server" CssClass="valbold" />
                            <asp:Label ID="rate_j27ident" runat="server" />
                        </td>
                    </tr>

                </table>
            </td>
            <td style="padding-left: 70px; padding-top: 5px;">

                <asp:Panel ID="panApproved" runat="server" Visible="false">
                    <table cellpadding="5" cellspacing="2">
                        <tr>
                            <td valign="top">
                                <asp:Label ID="Label1" runat="server" Text="Rozhodnutí schvalovatele:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="p71name" runat="server" CssClass="valbold"></asp:Label>
                                <span>-> </span>
                                <asp:Image ID="p72img" runat="server" />
                                <asp:Label ID="p72name" runat="server" Style="padding-left: 10px;" CssClass="valbold"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblvalue_approved" runat="server" Text="Schváleno pro fakturaci:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="value_approved_billing" runat="server" CssClass="valbold"></asp:Label>

                                <asp:Label ID="lblKorekceCaption" runat="server" Text="Korekce:" Visible="false"></asp:Label>
                                <asp:Image ID="imgKorekce" runat="server" ImageUrl="./images/correction.png" Visible="false" />
                                <asp:Label ID="value_korekce" runat="server" CssClass="valbold"></asp:Label>


                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblFakturacniSazba_Approved" runat="server" Text="Schválená fakturační sazba:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="rate_approved" runat="server" CssClass="valbold"></asp:Label>
                                <asp:Label ID="j07Code_Approved" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Datalabel3" runat="server" Text="Hodnota pro interní schvalování:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="value_approved_internal" runat="server" CssClass="valbold"></asp:Label>
                                <asp:Image ID="imgKorekceInternal" runat="server" ImageUrl="./images/correction.png" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblTimestamp_Approve" runat="server" Style="color: gray;"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="panInvoiced" runat="server">
                    <table cellpadding="5" cellspacing="2">
                        <tr>
                            <td>
                                <img src="Images/invoice.png">
                                <asp:Label ID="lblP70" runat="server" Text="Vyfakturováno statusem:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="p70name" runat="server" CssClass="valbold" ForeColor="white"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Datalabel1" runat="server" Text="ID faktury:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="p91ident" runat="server" CssClass="valbold"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Datalabel2" runat="server" Text="Vyfakturovaná částka:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="p31amount_withoutvat_invoiced" runat="server" CssClass="valbold"></asp:Label>
                                <asp:Label ID="j27ident_invoiced" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>

            </td>
        </tr>
    </table>
    <uc:x18_readonly ID="labels1" runat="server"></uc:x18_readonly>
    <asp:HyperLink ID="linkTimestamp" runat="server" CssClass="wake_link" ToolTip="CHANGE-LOG" NavigateUrl="javascript:changelog()" ></asp:HyperLink>
    


    <div style="padding-top: 30px;clear:both;"></div>
    <uc:b07_list ID="comments1" runat="server" JS_Create="p31_comment_create()" JS_Reaction="p31_comment_reaction" />
    <asp:HiddenField ID="hidP33ID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
