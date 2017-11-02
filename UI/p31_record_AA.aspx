<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p31_record_AA.aspx.vb" Inherits="UI.p31_record_AA" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="p31_approve_onerec" Src="~/p31_approve_onerec.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>

<%@ Register TagPrefix="uc" TagName="x18_readonly" Src="~/x18_readonly.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields_readonly" Src="~/freefields_readonly.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%If panApproving.Visible Then%>
     <link rel="stylesheet" href="Scripts/jqueryui/jquery-ui.min.css" />
     <script src="Scripts/jqueryui/jquery-ui.min.js" type="text/javascript"></script>
    <%End If%>
    <script type="text/javascript">
        $(document).ready(function () {

            $(".slidingDiv1").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".slidingDiv1").slideToggle();
            });




        });

        function hardrefresh(pid, flag) {
            location.replace("p31_record_AA.aspx?pid=<%=Master.datapid%>");       

        }

        function p31_comment_create() {
            sw_everywhere("b07_create.aspx?masterprefix=p31&masterpid=<%=master.datapid%>", true);

        }

        function p31_comment_reaction(b07id) {

            sw_everywhere("b07_create.aspx?parentpid=" + b07id + "&masterprefix=p31&masterpid=<%=master.datapid%>", true)

        }
        function o23_record(pid) {

            window.open("o23_framework.aspx?pid=" + pid, "_top");

        }

        function source_record() {
            sw_everywhere("p31_record.aspx?pid=<%=Master.DataPID%>&edit_approve=1","Images/worksheet.png", true);
        }
        function changelog() {
            location.replace("changelog.aspx?prefix=p31&pid=<%=Master.DataPID%>");
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

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
                            <asp:HyperLink ID="clue_person" runat="server" CssClass="reczoom" Text="i" title="Detail osoby"></asp:HyperLink>
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

                                <asp:Label ID="lblKorekceCaption" runat="server" Text="Korekce:" Visible="false" ></asp:Label>
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
                                <asp:Label ID="j27Code_Approved" runat="server" />
                            </td>
                        </tr>
                       
                        <tr id="trInternal" runat="server">
                            <td>
                                <span class="lbl">Interně schválené hodiny:</span>
                            </td>
                            <td>
                                <asp:Label ID="value_approved_internal" runat="server" CssClass="valbold"></asp:Label>
                                <asp:Image ID="imgKorekceInternal" runat="server" ImageUrl="./images/correction.png" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblTimestamp_Approve" runat="server" Style="color: gray;"></asp:Label>
                                , úroveň schvalování: <asp:Label ID="p31ApprovingLevel" runat="server"></asp:Label>
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
                                <asp:Label ID="p70name" runat="server" CssClass="valbold"></asp:Label>

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
    <uc:freefields_readonly ID="ff2" runat="server" />
    <uc:x18_readonly ID="labels1" runat="server"></uc:x18_readonly>



    <button type="button" id="cmdText" class="show_hide1" runat="server" visible="false">
        Upravit popis úkonu
        <img src="Images/arrow_down.gif" />
    </button>

    <asp:Button ID="cmdApprove" runat="server" Text="Pře-schválit worksheet úkon" CssClass="cmd" Visible="false" Font-Size="Large" />

    

    <asp:Label ID="lblLockedReasonMessage" runat="server" CssClass="failureNotification"></asp:Label>
    <div class="slidingDiv1">
        <asp:Panel ID="panUpdateText" runat="server" Style="padding: 6px;">
            <a class="show_hide1" href="#">
                <img alt="Close" border="0" src="Images/close.png" style="float: right;" />
            </a>
            <asp:Label CssClass="slidinHeader" ID="lblUPU" runat="server" Text="Upravit popis úkonu" />
            <asp:Button ID="cmdSaveText" runat="server" Text="Uložit změny" ImageUrl="Images/save.png" CssClass="cmd" />

            <asp:TextBox ID="update_p31text" runat="server" TextMode="MultiLine" Style="width: 99%; height: 100px;"></asp:TextBox>


        </asp:Panel>
    </div>

    <asp:Panel ID="panApproving" runat="server" Visible="false">
           

            <uc:p31_approve_onerec ID="approve1" runat="server" IsVertical="false" HeaderText="Schvalování worksheet úkonu" ShowCancelCommand="true"/>

            
       <button type="button" id="cmdSourceRecord" runat="server" onclick="javascript:source_record()">Upravit zdrojový úkon</button>
    </asp:Panel>

    <asp:Panel ID="panInvoicing" runat="server" CssClass="innerform" Visible="false">
        <div style="padding: 6px;">
            <span class="slidinHeader">Upravit fakturaci worksheet úkonu</span>

            <b>usercontrol pro fakturaci záznamu</b>
        </div>
        <div style="padding: 6px;">
            <asp:Label runat="server" ID="Datalabel4" Text="Datum úkonu:"></asp:Label>

            <telerik:RadDatePicker ID="update_p31date" runat="server" RenderMode="Lightweight" Width="120px">
                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
            </telerik:RadDatePicker>
        </div>

    </asp:Panel>



    <asp:panel ID="panP31Text" runat="server" CssClass="bigtext">
        <div class="div6">
            <asp:Label ID="p31text" runat="server" Style="color: Black;"></asp:Label>
        </div>
    </asp:panel>
    <div class="div6">
    <asp:HyperLink ID="linkTimestamp" runat="server" CssClass="wake_link" ToolTip="CHANGE-LOG" NavigateUrl="javascript:changelog()" ></asp:HyperLink>
    </div>
    
    
    
    <div style="padding-top:30px;clear:both;"></div>
    

    <uc:b07_list ID="comments1" runat="server" JS_Create="p31_comment_create()" JS_Reaction="p31_comment_reaction" />


   
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
