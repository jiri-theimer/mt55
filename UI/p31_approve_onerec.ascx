<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="p31_approve_onerec.ascx.vb" Inherits="UI.p31_approve_onerec" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div class="content-box2">
    <div class="title">

        <asp:Label ID="lblHeader" runat="server"></asp:Label>
        <asp:Button ID="cmdUlozitSchvalovani" runat="server" Text="Uložit změny" CssClass="cmd" />
        <asp:Button ID="cmdZrusitSchvalovani" runat="server" Text="Zrušit" CssClass="cmd" />
        

        <img src="Images/approve.png" style="float: right;" />
    </div>
    <div class="content">
        <div class="div6">
            <asp:RadioButtonList ID="p71id" runat="server" AutoPostBack="true" RepeatDirection="Vertical" RepeatColumns="2">

                <asp:ListItem Text="Schváleno" Value="1"></asp:ListItem>
                <asp:ListItem Text="Nerozhodnuto (zůstane rozpracované)" Value="0"></asp:ListItem>
                <asp:ListItem Text="Neschváleno" Value="2"></asp:ListItem>

            </asp:RadioButtonList>
        </div>
        <table cellpadding="0">
            <tr style="vertical-align: baseline;">

                <td style="min-width: 180px;">
                    <asp:RadioButtonList ID="p72id" runat="server" AutoPostBack="true" RepeatDirection="Vertical" RepeatColumns="1" CellPadding="3">
                        <asp:ListItem Text="<img src='Images/a14.gif'/>Fakturovat" Value="4"></asp:ListItem>
                        <asp:ListItem Text="<img src='Images/a16.gif'/>Zahrnout do paušálu" Value="6"></asp:ListItem>
                        <asp:ListItem Text="<img src='Images/a12.gif'/>Viditelný odpis" Value="2"></asp:ListItem>
                        <asp:ListItem Text="<img src='Images/a13.gif'/>Skrytý odpis" Value="3"></asp:ListItem>
                        <asp:ListItem Text="<img src='Images/a17.gif'/>Fakturovat později" Value="7"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <asp:PlaceHolder ID="place1" runat="server"></asp:PlaceHolder>
                <td>


                    <table cellpadding="5">
                        <tr>
                            <td style="width: 190px;">
                                <asp:Label ID="lblFakturovat" runat="server" Text="Hodnota pro fakturaci:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="value_approved" runat="server" Style="width: 80px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblRate_Billing_Approved" runat="server" Text="Fakturační sazba:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>

                                <telerik:RadNumericTextBox ID="Rate_Billing_Approved" runat="server" NumberFormat-DecimalDigits="2" Width="80px" Value="0">
                                </telerik:RadNumericTextBox>
                                <telerik:RadNumericTextBox ID="ManualFee" runat="server" NumberFormat-DecimalDigits="2" Width="120px" Visible="false">
                                </telerik:RadNumericTextBox>
                                <asp:Label ID="j27Code" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblVatRate_Approved" runat="server" Text="Sazba DPH:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>

                                <telerik:RadNumericTextBox ID="VatRate_Approved" runat="server" NumberFormat-DecimalDigits="2" Width="50px" Value="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblValue_FixPrice" runat="server" Text="Zahrnuto v paušálu:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="value_fixprice" runat="server" Style="width: 80px;"></asp:TextBox>
                            </td>
                        </tr>
                    </table>


                    <asp:Panel ID="panInternalContainer" runat="server" Visible="false">
                        <asp:Panel ID="panInternal" runat="server">
                            <fieldset style="border: dotted 1px black;">
                                <legend style="font-weight: normal; font-size: 90%;">Vnitropodnikové schvalování</legend>
                                <table cellpadding="5">
                                    <tr>
                                        <td>
                                            <asp:Label ID="lblInternal" runat="server" Text="Hodnota pro interní schvalování:" CssClass="lbl"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="value_approved_internal" runat="server" Style="width: 80px;"></asp:TextBox>

                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 190px;">
                                            <asp:Label ID="lblRate_Internal_Approved" runat="server" Text="Interní (nákladová) sazba:" CssClass="lbl"></asp:Label>
                                        </td>
                                        <td>
                                            <telerik:RadNumericTextBox ID="Rate_Internal_Approved" runat="server" NumberFormat-DecimalDigits="2" Width="80px" Value="0">
                                            </telerik:RadNumericTextBox>
                                        </td>
                                    </tr>


                                </table>
                            </fieldset>
                        </asp:Panel>
                    </asp:Panel>
                </td>

            </tr>
        </table>
        <div>
            <asp:Label ID="lblP31Text" runat="server" Text="Podrobný popis úkonu:" CssClass="lbl"></asp:Label>
            <input id="search7" style="width: 200px;border:solid 1px white;color:#696969;font-family:'Segoe UI';margin-left:50px;" value="Našeptávač textů" onfocus="search7Focus()" onblur="search7Blur()" title="Hledání podle částečné shody textu úkonu, názvu klienta, názvu nebo kódu projektu a názvu aktivity" />
        </div>
        <asp:TextBox ID="p31Text" runat="server" TextMode="MultiLine" Style="width: 97%; height: 50px;"></asp:TextBox>
        <div>
             <asp:RadioButtonList ID="p31ApprovingLevel" runat="server" RepeatDirection="Horizontal" ToolTip="Úroveň schvalování">
                <asp:ListItem value="0" Text="#0" Selected="true"></asp:ListItem>
                <asp:ListItem value="1" Text="#1"></asp:ListItem>
                <asp:ListItem value="2" Text="#2"></asp:ListItem>
            </asp:RadioButtonList>
            <span>Datum úkonu:</span>
            <telerik:RadDatePicker ID="p31Date" runat="server" Width="120px">
                <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server" EmptyMessage="Povinné pole"></DateInput>
            </telerik:RadDatePicker>
           
            
            <span>Zařazeno do billing dávky:</span>
            <telerik:RadComboBox ID="p31ApprovingSet" Visible="true" runat="server" ShowToggleImage="false" ShowDropDownOnTextboxClick="true" MarkFirstMatch="true" Width="150px" AllowCustomText="true"></telerik:RadComboBox>
        </div>
    </div>
</div>
<asp:HiddenField ID="p33id" runat="server" />
<asp:HiddenField ID="j03id" runat="server" />
<asp:HiddenField ID="p31id" runat="server" />
<asp:HiddenField ID="isbillable" runat="server" />
<asp:HiddenField ID="p32ManualFeeFlag" runat="server" Value="0" />
<asp:HiddenField ID="hidIsVertical" runat="server" Value="0" />
<asp:HiddenField ID="hidGUID_TempData" runat="server" />
<asp:HiddenField ID="hidP41ID" runat="server" />
<asp:HiddenField ID="hidJ02ID" runat="server" />
<script type="text/javascript">
    
    $(function () {

        $("#search7").autocomplete({
            source: "Handler/handler_search_worksheet.ashx?p41id=<%=Me.hidP41ID.Value%>&j02id=<%=Me.hidJ02ID.Value%>",
            minLength: 2,
            select: function (event, ui) {
                if (ui.item) {

                    document.getElementById("<%=p31Text.ClientID%>").value = ui.item.p31Text;
                    return false;
                }
            },
            close: function (event, ui) {
                $('ul.ui-autocomplete')
                .hide();
            }



        }).data("ui-autocomplete")._renderItem = function (ul, item) {
            var s = "<div><a>";

            s = s + __highlight(item.p31Date + "<span style='color:silver;'>:" + item.Project + "</span><br><i>" + item.Text2Html + "</i>", item.FilterString);


            s = s + "</a>";



            s = s + "</div>";


            return $(s).appendTo(ul);


        };
    });

    function __highlight(s, t) {
        var matcher = new RegExp("(" + $.ui.autocomplete.escapeRegex(t) + ")", "ig");
        return s.replace(matcher, "<strong>$1</strong>");
    }

    function search7Focus() {
        document.getElementById("search7").value = "";
        document.getElementById("search7").style.background = "whitesmoke";
        document.getElementById("search7").style.border = "solid 1px silver";
    }
    function search7Blur() {

        document.getElementById("search7").style.background = "";
        document.getElementById("search7").style.border = "solid 1px white";
        document.getElementById("search7").value = "Našeptávač textů";
    }
</script>
