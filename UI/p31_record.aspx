<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p31_record.aspx.vb" Inherits="UI.p31_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Scripts/jqueryui/jquery-ui.min.css" />
    <style type="text/css">
        .ui-autocomplete {
     max-height: 300px;
     overflow-y: auto;
     width:80px;
     /* prevent horizontal scrollbar */
     overflow-x: hidden;
 }
    </style>
    <script src="Scripts/jqueryui/jquery-ui.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {            
            var hours_interval = [
            <%=ViewState("hours_offer")%>
            ];

            
            <%If Me.hidHoursEntryFlag.Value = "1" Or Me.hidHoursEntryFlag.Value = "2" Or Me.hidHoursEntryFlag.Value = "3" Then%>
            $("#<%=p31Value_Orig.ClientID%>").autocomplete({
                source: hours_interval,               
                minLength: 0,
                scroll: true,
                change: function (event, ui) {
                    handle_hours();                    
                    
                }
            }).focus(function () {                
                $(this).autocomplete("search", "")
                $(this).select();
            });
            <%End If%>


            var time_interval = [
      <%=ViewState("times_offer")%>
            ];



            $("#<%=Me.TimeFrom.ClientID%>").autocomplete({
                source: time_interval,
                minLength: 0,
                scroll: true,
                change: function (event, ui) {
                    
                    recalcduration();
                    
                }
            }).focus(function () {
                $(this).autocomplete("search", "")
                $(this).select();
            });

            $("#<%=Me.TimeUntil.ClientID%>").autocomplete({
                source: time_interval,
                minLength: 0,
                scroll: true,
                change: function (event, ui) {
                    recalcduration();
                }
            }).focus(function () {
                $(this).autocomplete("search", "")
                $(this).select();
            });
            
        });

        


        function RecalcWithVat() {
            <%If Not Me.p31Amount_WithVat_Orig.Visible Then%>
            return;
            <%End If%>

            var vatrate = new Number;
            var withoutvat = new Number;
            var withvat = new Number;
            var vat = new Number;

            withoutvat = $find("<%= p31Amount_WithoutVat_Orig.ClientID%>").get_value();

            var ss = $find("<%= p31VatRate_Orig.RadComboClientID%>").get_text();
            vatrate = ss.replace(",", ".");

            vat = withoutvat * vatrate / 100;
            withvat = vat + withoutvat;

            $find("<%= p31Amount_WithVat_Orig.ClientID%>").set_value(withvat);
            $find("<%= p31Amount_Vat_Orig.ClientID%>").set_value(vat);
        }

        function RecalcAmount_ByPieces() {
            var vatrate = new Number;
            var withoutvat = new Number;
            var withvat = new Number;
            var vat = new Number;

            pieces_count = $find("<%= p31Calc_Pieces.ClientID%>").get_value();
            pieces_amount = $find("<%= p31Calc_PieceAmount.ClientID%>").get_value();
            withoutvat = pieces_count * pieces_amount;

            $find("<%= p31Amount_WithoutVat_Orig.ClientID%>").set_value(withoutvat);

            <%If Not Me.p31Amount_WithVat_Orig.Visible Then%>
            return;
            <%End If%>

            var ss = $find("<%= p31VatRate_Orig.RadComboClientID%>").get_text();
            vatrate = ss.replace(",", ".");

            vat = withoutvat * vatrate / 100;
            withvat = vat + withoutvat;

            $find("<%= p31Amount_WithVat_Orig.ClientID%>").set_value(withvat);
            $find("<%= p31Amount_Vat_Orig.ClientID%>").set_value(vat);
        }

        function p31Amount_WithoutVat_Orig_OnChanged(sender, eventArgs) {
            RecalcWithVat();
        }

        function p31VatRate_Orig_OnChange() {

            RecalcWithVat();
        }


        function p31VatRate_Orig_OnChange(sender, eventArgs) {
            RecalcWithVat();
        }

        function Calc_OnChanged(sender, eventArgs) {
            RecalcAmount_ByPieces();
        }



        function p32id_OnClientSelectedIndexChanged(sender, eventArgs) {
            var item = eventArgs.get_item();
            var p32id = item.get_value();
            var p41id_pid = "<%=p41id.value%>";
            var j27id_pid = "";
            <%If panM.Visible then%>
            j27id_pid = "<%=j27ID_Orig.SelectedValue%>";
            <%end If%>
            
            $.post("Handler/handler_activity.ashx", { pid: p32id, p41id: p41id_pid, j27id: j27id_pid }, function (data) {
                if (data == null) {
                    alert("Neznámá chyba");
                    return;
                }
                if (data.ErrorMessage != "") {
                    alert(data.ErrorMessage);
                    return;
                }                
                
                
                <%If panT.Visible Then%>
                if (Number(data.Default_p31Value) != 0 && self.document.getElementById("<%=p31Value_Orig.ClientID%>").value == "") {                    
                    self.document.getElementById("<%=p31Value_Orig.ClientID%>").value = data.Default_p31Value_String;

                }
                if (data.p32ManualFeeFlag == 1) {
                    document.getElementById("<%=tdManulFee.ClientID%>").style.display = "block";
                    var ctl = $find("<%= ManualFee.ClientID%>");
                    if (data.p32ManualFeeDefAmount !=0)
                        ctl.set_value(data.p32ManualFeeDefAmount);
                }
                else {
                    document.getElementById("<%=tdManulFee.ClientID%>").style.display = "none";
                }
                <%End If%>
                <%If panU.Visible Then%>
                var ctl = $find("<%= Units_Orig.ClientID%>");
                if (data.Default_p31Value_String != "" && ctl.get_value() == "") {
                    ctl.set_value(data.Default_p31Value_String);
                }

                <%End If%>
                <%If panM.Visible Then%>
                if (Number(data.Default_p31Value) != 0) {
                    $find("<%= p31Calc_PieceAmount.ClientID%>").set_value(data.Default_p31Value);
                    $find("<%= p31Calc_Pieces.ClientID%>").set_value(1);                   
                    RecalcAmount_ByPieces();
                }
                <%End If%>
                if (data.Default_p31Text != "" && data.Default_p31Text !=null) {
                    
                    if (self.document.getElementById("<%=p31text.ClientID%>").value == "")
                        self.document.getElementById("<%=p31text.ClientID%>").value = data.Default_p31Text;
                    else {
                        <%If Master.DataPID = 0 Then%>
                        self.document.getElementById("<%=p31text.ClientID%>").value = data.Default_p31Text + "\n\r" + self.document.getElementById("<%=p31text.ClientID%>").value;
                        <%End If%>
                        //nebo nic 
                    }
                }

                if (data.IsTextRequired==true)
                    document.getElementById("<%=Me.lblP31Text.ClientID%>").className = "lblReq";
                else
                    document.getElementById("<%=Me.lblP31Text.ClientID%>").className = "lbl";
                
                if (data.IsDefaultVatRate==true) {
                    var combo = $find("<%= p31VatRate_Orig.RadCombo.ClientID%>");                     
                    combo.set_text(data.DefaultVatRate);
                }
                





            });
        }

        function recalcduration() {
            var t1 = self.document.getElementById("<%=Me.TimeFrom.ClientID%>").value;
            var t2 = self.document.getElementById("<%=Me.TimeUntil.ClientID%>").value;

            $.post("Handler/handler_time.ashx", { t1: t1, t2: t2, oper: "duration" }, function (data) {
                if (data == ' ') {
                    return;
                }

                var s = data.split("|");


                if (s.length <= 1) {
                    document.getElementById("<%=Me.HandlerMessage.ClientID%>").innerHTML = data;
                    return
                }


                self.document.getElementById("<%=Me.TimeFrom.ClientID%>").value = s[0];
                self.document.getElementById("<%=Me.TimeUntil.ClientID%>").value = s[1];

                self.document.getElementById("<%=p31Value_Orig.ClientID%>").value = s[2];
                document.getElementById("<%=Me.HandlerMessage.ClientID%>").innerHTML = s[3];

            });
        }

        function handle_hours() {            
            var h = document.getElementById("<%=Me.p31Value_Orig.ClientID%>").value;
            var hef = document.getElementById("<%=Me.hidHoursEntryFlag.ClientID%>").value;            
            var strOper = "hours";
            if (hef == "2")
                strOper = "minutes";

            $.post("Handler/handler_time.ashx", { hours: h, oper: strOper }, function (data) {
                if (data == ' ') {
                    return;
                }

                document.getElementById("<%=Me.HandlerMessage.ClientID%>").innerHTML = data;

            });
        }

        

        function setting() {
            var p33id = document.getElementById("<%=hidP33ID.ClientID%>").value;
            var hoursentryflag = document.getElementById("<%=me.hidHoursEntryFlag.ClientID%>").value;

            dialog_master("p31_setting.aspx?p33id=" + p33id + "&hoursentryflag=" + hoursentryflag);


        }

        function hardrefresh(pid, flag, par1) {
            document.getElementById("<%=Me.HardRefreshFlag.ClientID%>").value = flag;
            document.getElementById("<%=HardRefreshPID.ClientID%>").value = pid;

            if (par1 != null) {

                document.getElementById("<%=me.hidHoursEntryFlag.ClientID%>").value = par1;
            }
            

            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefresh, "", False)%>
            
            
          
            
        }

        function sw_local(url, img, is_maximize)  //je to zde kvůli zapisování komentářů přes info-bublinu
        {
            dialog_master(url, is_maximize);
        }

       
        function p49_bind() {
            var p41id = "<%=me.p41ID.value%>";
            var p34id = "<%=Me.p34ID.SelectedValue%>";
            dialog_master("p49_bind.aspx?p34id="+p34id+"&p41id="+p41id, true);
        }
        function changelog() {
            dialog_master("changelog.aspx?prefix=p31&pid=<%=Master.DataPID%>", true)
        }
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="5" cellspacing="2" id="responsive">
        <tr>
            <td style="width: 120px;">
                <asp:Label ID="lblJ02ID" runat="server" Text="Jméno:" CssClass="lblReq" meta:resourcekey="lblJ02ID"></asp:Label>
            </td>
            <td>
                <uc:person ID="j02ID" runat="server" Width="400px" AutoPostBack="true" Flag="p31_entry" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblP41ID" runat="server" Text="Projekt:" CssClass="lblReq" meta:resourcekey="lblP41ID"></asp:Label>
                
            </td>
            <td>
                <uc:project ID="p41ID" runat="server" Width="400px" AutoPostBack="true" Flag="p31_entry" />
                <asp:HyperLink ID="clue_project" runat="server" CssClass="reczoom" Text="i" Visible="false"></asp:HyperLink>
            </td>
        </tr>
        <tr style="height:30px;" id="trTask" runat="server" visible="false">
            <td>
                <asp:Label ID="lblP56ID" runat="server" Text="Úkol v projektu:" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p56ID" runat="server" Width="400px" DataTextField="NameWithTypeAndCode" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="false" Filter="Contains" BackgroundColor="#F0F8FF" />
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblP34ID" runat="server" Text="Sešit:" CssClass="lblReq" meta:resourcekey="lblP34ID"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p34ID" runat="server" Width="400px" DataTextField="p34Name" DataValueField="pid" IsFirstEmptyRow="true" AutoPostBack="true" Filter="Contains" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblP32ID" runat="server" Text="Aktivita:" CssClass="lblReq" meta:resourcekey="lblP32ID"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p32ID" runat="server" Width="400px" DataTextField="p32Name" DataValueField="pid" IsFirstEmptyRow="true" Filter="Contains" OnClientSelectedIndexChanged="p32id_OnClientSelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDate" runat="server" Text="Datum:" CssClass="lblReq" meta:resourcekey="lblDate"></asp:Label>
            </td>
            <td>
                <telerik:RadDatePicker ID="p31Date" runat="server" Width="120px" SharedCalendarID="SharedCalendar" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
                <asp:Label ID="MultiDateInput" runat="server" Visible="false" ForeColor="blue"></asp:Label>
            </td>
        </tr>

    </table>
    
    <asp:Panel ID="panT" runat="server" Visible="false">
        <table cellpadding="5" cellspacing="2">
            <tr>
                <td style="width: 120px;">
                    <asp:Label ID="lblHours" runat="server" Text="Hodiny:" CssClass="lblReq"></asp:Label>
                </td>
                <td>
                    
                    <asp:TextBox ID="p31Value_Orig" runat="server" Style="width: 50px;" onchange="handle_hours()"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblTimeFrom" runat="server" Text="Začátek:" CssClass="lbl" Visible="false" meta:resourcekey="lblTimeFrom"></asp:Label>
                </td>
                <td>

                    
                    <asp:TextBox ID="TimeFrom" runat="server" Style="width: 50px;"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblTimeUntil" runat="server" Text="Konec:" CssClass="lbl" Visible="false" meta:resourcekey="lblTimeUntil"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="TimeUntil" runat="server" Style="width: 50px;"></asp:TextBox>
                    
                </td>
                <td id="tdManulFee" runat="server" style="display:none;">
                    <span class="lblReq">Pevný honorář:</span>
                    <telerik:RadNumericTextBox ID="ManualFee" runat="server" Width="70px" NumberFormat-ZeroPattern="n"></telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:Label ID="HandlerMessage" runat="server" Style="color: navy; font-size: 90%;"></asp:Label>
                </td>
               
            </tr>
          
        </table>
    </asp:Panel>
    <asp:Panel ID="panU" runat="server" Visible="false">
        <table cellpadding="5" cellspacing="2">
            <tr>
                <td style="width: 120px;">
                    <asp:Label ID="lblUnits" runat="server" Text="Počet:" CssClass="lblReq"></asp:Label>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="Units_Orig" runat="server" Width="70px" NumberFormat-ZeroPattern="n"></telerik:RadNumericTextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="panM" runat="server" Visible="false">
        <table cellpadding="5" cellspacing="2">
            <tr>
                <td style="width: 120px;">
                    <asp:Label ID="lblp31Amount_WithoutVat_Orig" runat="server" Text="Částka bez DPH:" CssClass="lblReq" meta:resourcekey="lblp31Amount_WithoutVat_Orig"></asp:Label>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="p31Amount_WithoutVat_Orig" runat="server" Width="100px" NumberFormat-ZeroPattern="n">
                        <ClientEvents OnValueChanged="p31Amount_WithoutVat_Orig_OnChanged" />
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <uc:datacombo ID="j27ID_Orig" Width="70px" runat="server" DataTextField="j27Code" DataValueField="pid" AutoPostBack="true"></uc:datacombo>
                </td>
                <td>
                    <asp:Label ID="lblp31VatRate_Orig" runat="server" Text="Sazba DPH (%):" CssClass="lbl" meta:resourcekey="lblp31VatRate_Orig"></asp:Label>
                </td>
                <td>
                    <uc:datacombo ID="p31VatRate_Orig" Width="60px" runat="server" AllowCustomText="true" Filter="StartsWith" OnClientTextChange="p31VatRate_Orig_OnChange" OnClientSelectedIndexChanged="p31VatRate_Orig_OnChange" DataValueField="pid" DataTextField="p53Value"></uc:datacombo>
                </td>

            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblp31Amount_WithVat_Orig" runat="server" Text="Částka vč. DPH:" CssClass="lbl" meta:resourcekey="lblp31Amount_WithVat_Orig"></asp:Label>
                    
                   
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="p31Amount_WithVat_Orig" runat="server" Width="100px" NumberFormat-ZeroPattern="n" ></telerik:RadNumericTextBox>
                    
                </td>
                <td>
                    <asp:Label ID="lblp31Amount_Vat_Orig" runat="server" Text="Částka DPH:" CssClass="lbl" meta:resourcekey="lblp31Amount_Vat_Orig"></asp:Label>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="p31Amount_Vat_Orig" runat="server" Width="100px" NumberFormat-ZeroPattern="n"></telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:ImageButton ID="cmdRecalcVat1" runat="server" ImageUrl="Images/recalc.png" CssClass="button-link" style="display:block;" ToolTip="Dopočítat z celkové částky částku bez DPH a částku DPH"/>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblp31Calc_Pieces" runat="server" Text="Počet:" CssClass="lbl" meta:resourcekey="lblp31Calc_Pieces"></asp:Label>
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="p31Calc_Pieces" runat="server" Width="50px" NumberFormat-ZeroPattern="n">
                        <ClientEvents OnValueChanged="Calc_OnChanged" />
                    </telerik:RadNumericTextBox>
                    <uc:datacombo ID="p35ID" Width="60px" runat="server" AllowCustomText="false" Filter="StartsWith" DataValueField="pid" DataTextField="p35Code" IsFirstEmptyRow="true"></uc:datacombo>
                </td>
                <td>
                    <asp:Label ID="lblp31Calc_PieceAmount" runat="server" Text="Cena 1 ks:" CssClass="lbl" meta:resourcekey="lblp31Calc_PieceAmount"></asp:Label>                    
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="p31Calc_PieceAmount" runat="server" Width="100px" NumberFormat-ZeroPattern="n">
                        <ClientEvents OnValueChanged="Calc_OnChanged" />
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:DropDownList ID="j19ID" runat="server" DataValueField="pid" DataTextField="j19Name"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblSupplier" runat="server" CssClass="lbl" Text="Dodavatel:" meta:resourcekey="lblSupplier"></asp:Label>
                </td>
                <td colspan="4">
                    <uc:contact ID="p28ID_Supplier" runat="server" Width="250px" Flag="supplier" />
                    <asp:Label ID="lblCode" runat="server" CssClass="lbl" Text="Kód dokladu:"></asp:Label>
                    <asp:TextBox ID="p31Code" runat="server" Width="100px"></asp:TextBox>
                  
                </td>
            </tr>
           
        </table>
    </asp:Panel>
    <div class="div6">
        <div>
            <asp:Label ID="lblP31Text" runat="server" Text="Podrobný popis úkonu:" CssClass="lbl" meta:resourcekey="lblP31Text"></asp:Label>
            <asp:Image ID="imgFlag" runat="server" />
            <input id="search2" style="width: 400px;border:solid 1px white;color:#696969;font-family:'Segoe UI';margin-left:50px;" value="Našeptávač    ...stačí napsat 2 písmena" onfocus="search2Focus()" onblur="search2Blur()" title="Hledání podle částečné shody textu úkonu, názvu klienta, názvu nebo kódu projektu a názvu aktivity" />
            
        </div>
        <asp:TextBox ID="p31Text" runat="server" Style="height: 90px; width: 99%;" TextMode="MultiLine"></asp:TextBox>
        <uc:freefields ID="ff1" runat="server" />
    </div>
    
    <div class="content-box1" style="min-width:50px;">
        <div class="title">
            <img src="Images/contactperson.png" alt="Kontaktní osoba" />
            <asp:CheckBox ID="chkBindToContactPerson" runat="server" Text="Kontaktní osoba" AutoPostBack="true" meta:resourcekey="chkBindToContactPerson" />
        </div>
        <div class="content">
            <asp:DropDownList ID="j02ID_ContactPerson" runat="server" Visible="false" DataValueField="pid" DataTextField="FullNameDescWithEmail"></asp:DropDownList>
        </div>
    </div>
    

    <asp:panel ID="panP49" runat="server" cssclass="content-box1" style="min-width:170px;" Visible="false">
        <div class="title">
            <img src="Images/finplan.png" alt="Rozpočet" /><asp:Label ID="lblRozpocet" runat="server" Text="Rozpočet" meta:resourcekey="lblRozpocet"></asp:Label>
            <asp:HyperLink ID="cmdP49" runat="server" Text="Spárovat" NavigateUrl="javascript:p49_bind()"></asp:HyperLink>
        </div>
        <div class="content">
            <asp:Label ID="p49_record" runat="server" CssClass="valboldblue"></asp:Label>           
            <asp:Button ID="cmdClearP49ID" runat="server" Text="Vyčistit vazbu na rozpočet" CssClass="cmd" />
            <asp:HiddenField ID="p49ID" runat="server" />
        </div>
    </asp:panel>
    <asp:panel ID="panTrimming" runat="server" cssclass="content-box1" style="min-width:100px;" Visible="false">
        <div class="title">
            <img src="Images/correction_down.gif" alt="Korekce pro schvalování" />
            <img src="Images/correction_up.gif" alt="Korekce pro schvalování" />
            <asp:CheckBox ID="chkTrimming" runat="server" Text="Korekce" AutoPostBack="true" meta:resourcekey="chkTrimming" />
        </div>
        <div class="content">
            <asp:RadioButtonList ID="p72ID_AfterTrimming" runat="server" AutoPostBack="true" RepeatDirection="Vertical" Visible="false">
                <asp:ListItem Text="<%$ Resources:common, Fakturovat %>" Value="4"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:common, ZahrnoutDoPausalu %>" Value="6" Selected="true"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:common, SkrytyOdpis %>" Value="3"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:common, ViditelnyOdpis %>" Value="2"></asp:ListItem>
            </asp:RadioButtonList>
            <asp:Label ID="lblValueTrimmed" Text="Hodiny k fakturaci:" runat="server" Visible="false" meta:resourcekey="lblValueTrimmed"></asp:Label>
            <asp:TextBox ID="p31Value_Trimmed" runat="server" style="width:40px;text-align:left;" Visible="false"></asp:TextBox>
        </div>
    </asp:panel>

    <div style="clear: both;"></div>
    <div class="div6">
        <uc:mytags ID="tags1" ModeUi="1" Prefix="p31" runat="server" />
        <a href="javascript:setting()" style="text-align: right; float: right;"><%=resources.p31_record.Nastaveni %></a>
    </div>



    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" ShowRowHeaders="false">

        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>


    <asp:HiddenField ID="HardRefreshPID" runat="server" />
    <asp:HiddenField ID="HardRefreshFlag" runat="server" />
    <asp:Button ID="cmdHardRefresh" runat="server" Style="display: none;" />
    <asp:HiddenField ID="hidP33ID" runat="server" />
    <asp:HiddenField ID="hidHoursEntryFlag" runat="server" Value="1" />
    <asp:HiddenField ID="hidP91ID" runat="server" />
    <asp:HiddenField ID="hidP48ID" runat="server" />
    <asp:HiddenField ID="hidP85ID" runat="server" />
    <asp:HiddenField ID="hidP61ID" runat="server" />
    
    <asp:HiddenField ID="p31_default_HoursEntryFlag" runat="server" />
    <asp:HiddenField ID="hidCurIsScheduler" runat="server" Value="0" />
    <asp:HiddenField ID="hidCurPerson_J02ID" runat="server" />
    <asp:HiddenField ID="hidCurPerson_Name" runat="server" />
    <asp:HiddenField ID="hidDefaultP34ID" runat="server" />
    <asp:HiddenField ID="hidDefaultP32ID" runat="server" />
    <asp:HiddenField ID="hidDefaultP31Date" runat="server" />
    <asp:HiddenField ID="hidDefaultJ27ID" runat="server" />
    <asp:HiddenField ID="hidDefaultVatRate" runat="server" />
    <asp:HiddenField ID="hidGuidApprove" runat="server" />
    

    <script type="text/javascript">
        $(function () {

            $("#search2").autocomplete({
                source: "Handler/handler_search_worksheet.ashx?p41id=<%=Me.p41ID.Value%>&j02id=<%=Me.j02ID.Value%>",
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
                
                s = s + __highlight(item.p31Date+"<span style='color:silver;'>:"+item.Project + "</span><br><i>" + item.Text2Html+"</i>", item.FilterString);


                s = s + "</a>";

                

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
            document.getElementById("search2").style.background = "whitesmoke";
            document.getElementById("search2").style.border = "solid 1px silver";
        }
        function search2Blur() {

            document.getElementById("search2").style.background = "";
            document.getElementById("search2").style.border = "solid 1px white";
            document.getElementById("search2").value = "Našeptávač...";
        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
