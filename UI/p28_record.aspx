<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p28_record.aspx.vb" Inherits="UI.p28_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Scripts/jqueryui/jquery-ui.min.css" />
    <script src="Scripts/jqueryui/jquery-ui.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        function recordcode() {

            dialog_master("record_code.aspx?prefix=p28&pid=<%=Master.DataPID%>");

        }
        function hardrefresh(pid, flag, codeValue) {
            if (flag == "record-code") {
                document.getElementById("<%=Me.p28Code.ClientID%>").innerText = codeValue;
                $.alert("Změna kódu záznamu byla uložena.")
            }

            document.getElementById("<%=HardRefreshPID.ClientID%>").value = pid;
            document.getElementById("<%=HardRefreshFlag.ClientID%>").value = flag;
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefresh, "", False)%>;
        }

        function p51_billing_add(customtailor) {

            dialog_master("p51_record.aspx?pid=0&prefix=p41&customtailor=" + customtailor, true)

        }

        function p51_edit(p51id) {

            dialog_master("p51_record.aspx?pid=" + p51id + "&prefix=p41", true)

        }
        function j02_record(j02id) {

            dialog_master("j02_record.aspx?iscontact=1&pid="+j02id, true)

        }
        function j02_delete(j02id) {           
            alert("V kartě osoby stiskněte tlačítko [Odstranit].")
            dialog_master("j02_record.aspx?pid=" + j02id, true)

        }
        
        function vat_info() {
            var dic = document.getElementById("<%=p28VatID.ClientID%>").value;
            if (dic == "") {
                alert("Musíte vyplnit DIČ.");
                return;
            }

            dialog_master("vat_registration.aspx?vat=" + dic, true);

        }
        function changelog() {
            dialog_master("changelog.aspx?prefix=p28&pid=<%=Master.DataPID%>", true)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti" Selected="true" Value="core"></telerik:RadTab>
            <telerik:RadTab Text="Fakturační nastavení" Value="billing"></telerik:RadTab>
            <telerik:RadTab Text="Kategorie ({1}), uživatelská pole ({0})" Value="ff"></telerik:RadTab>
            <telerik:RadTab Text="Ostatní" Value="other"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="core" runat="server" Selected="true">
            <div style="float: left; padding: 10px;">
                <asp:RadioButtonList ID="p28IsCompany" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                    <asp:ListItem Value="1" Text="Klient je právnická osoba (společnost)" Selected="True"></asp:ListItem>
                    <asp:ListItem Value="0" Text="Klient je fyzická osoba"></asp:ListItem>
                </asp:RadioButtonList>
            </div>
            <div style="float: left; padding: 10px;">

                <asp:HyperLink ID="p28Code" runat="server" ToolTip="Kód záznamu"></asp:HyperLink>
                <div>
                    <asp:CheckBox ID="p28IsDraft" runat="server" Text="DRAFT režim" Visible="false" />
                </div>
            </div>
            <div style="float: left; padding: 10px;">
                <asp:Label ID="lblp29ID" Text="Typ klienta:" runat="server" CssClass="lbl"></asp:Label>
                <uc:datacombo ID="p29ID" runat="server" DataTextField="p29Name" DataValueField="pid" AutoPostBack="true" IsFirstEmptyRow="true"></uc:datacombo>
            </div>
            <div style="float: right;">
                <asp:CheckBox ID="chkWhisper" runat="server" Text="Zapnutý našeptávač podobných klientů" AutoPostBack="true" Checked="true" />
            </div>
            <div style="clear: both;"></div>

            <asp:Panel ID="panCompany" runat="server">
                <table cellpadding="5" cellspacing="2">

                    <tr>
                        <td style="width: 80px;">
                            <asp:Label ID="lblp28CompanyName" runat="server" CssClass="lblReq" Text="Společnost:"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="p28CompanyName" runat="server" Style="width: 400px;"></asp:TextBox>


                        </td>
                    </tr>

                </table>
            </asp:Panel>
            <asp:Panel ID="panPerson" runat="server">
                <table cellpadding="5" cellspacing="2">
                    <tr>
                        <td style="width: 80px;">
                            <asp:Label ID="lblTitle" Text="Osoba:" runat="server" CssClass="lbl" AssociatedControlID="p28TitleBeforeName" meta:resourcekey="lblTitlex"></asp:Label>
                        </td>
                        <td>
                            <uc:datacombo ID="p28TitleBeforeName" runat="server" Width="70px" AllowCustomText="true" ShowToggleImage="false" Filter="Contains" DefaultValues="Bc.;BcA.;Ing.;Ing.arch.;MUDr.;MVDr.;MgA.;Mgr.;JUDr.;PhDr.;RNDr.;PharmDr.;ThLic.;ThDr.;Ph.D.;Th.D.;prof.;doc.;PaedDr.;Dr.;PhMr."></uc:datacombo>
                        </td>
                        <td>
                            <asp:Label ID="lblFirstName" Text="Jméno:" runat="server" CssClass="lbl" AssociatedControlID="p28FirstName" meta:resourcekey="lblFirstName"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="p28FirstName" runat="server" Style="width: 100px;"></asp:TextBox>
                        </td>
                        <td>
                            <asp:Label ID="lblLastName" Text="Příjmení:" runat="server" CssClass="lblReq" AssociatedControlID="p28LastName" meta:resourcekey="lblLastName"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ID="p28LastName" runat="server" Style="width: 160px;"></asp:TextBox>
                        </td>
                        <td>
                            <uc:datacombo ID="p28TitleAfterName" runat="server" Width="70px" AllowCustomText="true" ShowToggleImage="false" Filter="Contains" DefaultValues="CSc.;DrSc.;dr. h. c.;DiS."></uc:datacombo>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td style="width: 80px;">
                        <asp:Label ID="lblp28RegID" runat="server" Text="IČ:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <asp:TextBox ID="p28RegID" runat="server" Style="width: 80px;"></asp:TextBox>

                    </td>
                    <td>
                        <asp:LinkButton ID="cmdARES" runat="server" Text="ARES import" ToolTip="Podle IČ načte název, adresu a DIČ subjektu. Funguje pouze pro subjekty registrované v ARES databázi." />
                    </td>
                    <td>
                        <asp:Label ID="lblp28VatID" runat="server" Text="DIČ:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p28VatID" runat="server" Style="width: 130px;"></asp:TextBox>


                    </td>
                    <td>
                        <asp:LinkButton ID="cmdVIES" runat="server" Text="VIES import" ToolTip="Podle DIČ načte název a adresu subjektu z evropského registru VIES. Funguje pro subjekty ze všech zemí EU." />
                        <a href="javascript:vat_info()" style="margin-left: 20px;">Ověření v DPH registrech</a>

                    </td>

                </tr>
            </table>




            <div class="div6">
                <asp:RadioButtonList ID="p28SupplierFlag" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Pouze klient" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Pouze dodavatel" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Klient i dodavatel" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Ani klient ani dodavatel" Value="4"></asp:ListItem>
                </asp:RadioButtonList>
                <asp:Label ID="lblSupplierID" runat="server" Text="Kód dodavatele:"></asp:Label>
                <asp:TextBox ID="p28SupplierID" runat="server"></asp:TextBox>
            </div>
            <div id="search_dupl_result" style="position: relative; left: 0px; top: 0px; z-index: 1000;"></div>
            <uc:mytags ID="tags1" ModeUi="1" Prefix="p28" runat="server" />
            <div class="div6">
                <asp:CheckBox ID="chkDefineLimits" runat="server" AutoPostBack="true" Text="Definovat limity k upozornění" CssClass="chk" />
            </div>
            
            <asp:Panel ID="panLimits" runat="server">
                <div class="div6">
                    <asp:Label ID="lblLimitHours" runat="server" CssClass="lbl" Text="Limitní objem rozpracovaných hodin na projektech, po jehož překročení systém odešle notifikaci (upozornění):"></asp:Label>
                    <telerik:RadNumericTextBox ID="p28LimitHours_Notification" runat="server" MinValue="0" MaxValue="1000" NumberFormat-DecimalDigits="2" Value="0" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                </div>
                <div class="div6">
                    <asp:Label ID="lblLimitFee" runat="server" CssClass="lbl" Text="Limitní honorář na projektech (rozpracované hodiny x sazba), po jehož překročení systém odešle notifikaci (upozornění):"></asp:Label>
                    <telerik:RadNumericTextBox ID="p28LimitFee_Notification" runat="server" MinValue="0" MaxValue="9999999" NumberFormat-DecimalDigits="2" Value="0" Width="100px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                </div>
            </asp:Panel>



            <div class="content-box2">
                <div class="title">
                    <img src="Images/address.png" width="16px" height="16px" alt="Adresa" />
                    <asp:Label ID="phO37" runat="server" CssClass="framework_header_span" Text="Adresy" Style="display: inline-block; min-width: 150px;"></asp:Label>
                    <asp:Button ID="cmdAddO37" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <asp:Panel ID="panO37" runat="server" CssClass="content">
                    <table cellpadding="4">
                        <tr>
                            <th>Typ adresy</th>
                            <th>Ulice</th>
                            <th>Město</th>
                            <th>PSČ</th>
                            <th>Stát</th>
                            <th></th>
                        </tr>
                        <asp:Repeater ID="rpO37" runat="server">
                            <ItemTemplate>
                                <tr valign="top">
                                    <td>
                                        <asp:DropDownList ID="o36id" runat="server" Style="width: 140px;">
                                            <asp:ListItem Text="Fakturační adresa" Value="1" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="Poštovní adresa" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Jiné" Value="3"></asp:ListItem>
                                        </asp:DropDownList>
                                        <div>
                                            <asp:TextBox ID="o38name" runat="server" Style="width: 140px" ToolTip="Název"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="o38Street" runat="server" TextMode="MultiLine" Style="width: 200px; height: 50px;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="o38City" runat="server" TextMode="MultiLine" Style="width: 150px; height: 50px;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="o38ZIP" runat="server" Style="width: 50px;"></asp:TextBox>
                                    </td>
                                    <td>

                                        <uc:datacombo ID="o38country" runat="server" Width="170px" AllowCustomText="true" ShowToggleImage="false" Filter="Contains"></uc:datacombo>
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                        <asp:HiddenField ID="p85id" runat="server" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </asp:Panel>
            </div>
            

            <div class="content-box2" style="padding-top: 10px;">
                <div class="title">
                    <img src="Images/contactperson.png" width="16px" height="16px" alt="Kontaktní osoba" />
                    <asp:Label ID="Label2" runat="server" CssClass="framework_header_span" Text="Kontaktní osoby klienta" Style="display: inline-block; min-width: 150px;"></asp:Label>

                    <button type="button" onclick="j02_record(0)">Založit úplně novou osobu</button>
                    <span>nebo vložit již zavedenou z adresáře lidí:</span>
                    <uc:person ID="j02ID" runat="server" Flag="all2" Width="200px" AutoPostBack="true" />

                    
                </div>
                <div class="content">
                    <table cellpadding="6">
                    <asp:Repeater ID="rpP30" runat="server">
                        <ItemTemplate>
                            <tr class="trHover">
                                <td>
                                    <asp:DropDownList ID="p27id" runat="server" DataValueField="pid" DataTextField="p27Name" AutoPostBack="false" Visible="false"/>
                                    
                                </td>
                                <td>
                                    <asp:HyperLink ID="clue_j02" runat="server" CssClass="reczoom" Text="i" title="Detail"></asp:HyperLink>
                                    <asp:HyperLink ID="linkPerson" runat="server"></asp:HyperLink>
                                    <asp:HiddenField ID="p85id" runat="server" />
                                </td>
                                <td>
                                    <asp:label ID="j02Email" runat="server"></asp:label>
                                </td>
                                <td>
                                    <asp:Label ID="j02JobTitle" runat="server" Font-Italic="true"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="j02Mobile" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="j02Office" runat="server"></asp:Label>
                                </td>

                                <td>
                                    <button id="cmdDeletePerson" runat="server" type="button" title="Nenávratné odstranění osobního profilu vč. vazby na klienta" style="font-size:x-small;">Odstranit osobní profil</button>
                                    <asp:Button ID="del" CommandName="delelete" runat="server" Text="Odstranit pouze vazbu Osoba->Klient" CssClass="cmd" ToolTip="Po odstranění vazby zůstane osoba v adresáři lidí." Font-Size="X-Small" />
                                    
                                    
                                </td>

                            </tr>

                        </ItemTemplate>
                    </asp:Repeater>
                    </table>
                    <table id="tabDefaultPerson" runat="server">
                        <tr>
                            <td>Výchozí kontaktní osoba pro vykazování úkonů:</td>
                            <td>
                                <asp:DropDownList ID="j02ID_ContactPerson_DefaultInWorksheet" runat="server" DataValueField="p85OtherKey1" DataTextField="p85FreeText01"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>Kontaktní osoba na faktuře:</td>
                            <td>
                                <asp:DropDownList ID="j02ID_ContactPerson_DefaultInInvoice" runat="server" DataValueField="p85OtherKey1" DataTextField="p85FreeText01"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>

            <div class="content-box2" style="padding-top: 10px;">
                <div class="title">
                    <img src="Images/email.png" width="16px" height="16px" alt="E-mail" />
                    <asp:Label ID="phO32" runat="server" CssClass="framework_header_span" Text="Kontaktní média" Style="display: inline-block; min-width: 150px;"></asp:Label>
                    <asp:Button ID="cmdAddO32" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <asp:Panel ID="panO32" runat="server" CssClass="content">
                    <table cellpadding="4">
                        <tr>
                            <th>Typ</th>
                            <th>Číslo | Adresa</th>
                            <th>Poznámka</th>
                            <th></th>
                        </tr>
                        <asp:Repeater ID="rpO32" runat="server">
                            <ItemTemplate>
                                <tr valign="top">
                                    <td>
                                        <asp:DropDownList ID="o33id" runat="server" AutoPostBack="true">
                                            <asp:ListItem Text="TEL" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="E-MAIL" Value="2" Selected="true"></asp:ListItem>
                                            <asp:ListItem Text="URL" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="SKYPE" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="ICQ" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="FAX" Value="6"></asp:ListItem>
                                            <asp:ListItem Text="Jiné" Value="7"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="o32Value" runat="server" Style="width: 250px;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="o32Description" runat="server" Style="width: 250px;"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="o32IsDefaultInInvoice" runat="server" Text="Fakturační e-mail" />
                                    </td>
                                    <td>
                                        <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit položku" CssClass="button-link" />
                                        <asp:HiddenField ID="p85id" runat="server" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </asp:Panel>
            </div>

            <div style="margin-top: 30px;border-top:dashed 1px gray;">
                <br />
                <asp:Label ID="lblParentID" runat="server" Text="Nadřízený klient:" CssClass="lbl"></asp:Label>
                <uc:contact ID="p28ParentID" runat="server" Width="400px" Flag="client" />

            </div>



        </telerik:RadPageView>
        <telerik:RadPageView ID="billing" runat="server">
            <fieldset>
                <legend>Fakturační sazby (ceník)</legend>
                <asp:RadioButtonList ID="opgPriceList" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Bez ceníku sazeb" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Svázat klienta se zavedeným ceníkem" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Tento klient má sazby na míru" Value="3"></asp:ListItem>
                </asp:RadioButtonList>
                <div class="div6">
                    <asp:Label ID="lblP51ID_Billing" runat="server" Text="Ceník sazeb:" CssClass="lbl"></asp:Label>
                    <uc:datacombo ID="p51ID_Billing" runat="server" DataTextField="NameWithCurr" DataValueField="pid" AutoPostBack="true" IsFirstEmptyRow="true" Width="300px" />
                </div>
                <div class="div6">
                    <asp:HyperLink ID="cmdNewP51" runat="server" NavigateUrl="javascript:p51_billing_add()" Text="Založit nový ceník"></asp:HyperLink>
                    <asp:HyperLink ID="cmdEditP51" runat="server" NavigateUrl="javascript:p51_edit()" Text="Upravit ceník" Style="margin-left: 20px;"></asp:HyperLink>
                </div>
            </fieldset>


            <table cellpadding="5" cellspacing="2">

                <tr>
                    <td>
                        <asp:Label ID="lblP87ID" runat="server" Text="Fakturační jazyk klienta:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p87ID" runat="server" DataTextField="p87Name" DataValueField="pid" IsFirstEmptyRow="true" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp92ID" runat="server" Text="Výchozí typ faktury:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:datacombo ID="p92id" runat="server" Width="300px" DataTextField="p92Name" DataValueField="pid" IsFirstEmptyRow="true" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblMaturity" runat="server" Text="Výchozí počet dní splatnosti faktury:" CssClass="lbl"></asp:Label>


                        <telerik:RadNumericTextBox ID="p28InvoiceMaturityDays" runat="server" MinValue="0" MaxValue="200" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true">
                        </telerik:RadNumericTextBox>

                    </td>
                </tr>

                <tr>
                    <td>
                        <asp:Label ID="lblp28InvoiceDefaultText1" runat="server" Text="Výchozí text faktury:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <asp:TextBox ID="p28InvoiceDefaultText1" runat="server" TextMode="MultiLine" Style="width: 600px; height: 50px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp28InvoiceDefaultText2" runat="server" Text="Výchozí technický text faktury:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <asp:TextBox ID="p28InvoiceDefaultText2" runat="server" TextMode="MultiLine" Style="width: 600px; height: 30px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblJ61ID" runat="server" Text="Textová e-mail šablona:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="j61ID_Invoice" runat="server" DataTextField="j61Name" DataValueField="pid" Width="300px"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblP63ID" runat="server" Text="Režijní přirážka k faktuře:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p63ID" runat="server" DataTextField="NameWithRate" DataValueField="pid" IsFirstEmptyRow="true" Width="300px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp51ID_Internal" runat="server" Text="Nákladový ceník projektů klienta:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:datacombo ID="p51ID_Internal" runat="server" DataTextField="NameWithCurr" AutoPostBack="false" DataValueField="pid" IsFirstEmptyRow="true" Width="300px" />
                    </td>
                </tr>
            </table>
            <div>Fakturační poznámka klienta:</div>
            <asp:TextBox ID="p28BillingMemo" runat="server" Style="width: 99%; height: 60px;" TextMode="MultiLine"></asp:TextBox>
        </telerik:RadPageView>
        <telerik:RadPageView ID="ff" runat="server">

            <uc:freefields ID="ff1" runat="server" />

        </telerik:RadPageView>
        <telerik:RadPageView ID="other" runat="server">
            <table cellpadding="5" cellspacing="2">
                <tr>
                    <td>
                        <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq"></asp:Label>

                    </td>
                    <td>
                        <uc:person ID="j02ID_Owner" runat="server" Width="300px" Flag="all" />

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp28CompanyShortName" runat="server" Text="Zkrácený (preferovaný) název:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p28CompanyShortName" runat="server" Style="width: 100px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="infoInForm">Pro uživatele systém upřednostňuje zkrácený název před standardním názvem klienta.</span>
                    </td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Label ID="lblp28RobotAddress" runat="server" Text="Adresa pro IMAP robot:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p28RobotAddress" runat="server" Style="width: 200px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <span class="infoInForm">Adresa v kopii (CC/BCC) , podle které robot pozná, že načtená poštovní zpráva má vazbu k tomuto klientovi.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Externí kód:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p28ExternalPID" runat="server" Style="width: 200px;"></asp:TextBox>

                        <span class="infoInForm">Klíč záznamu z externího IS pro integraci s MT.</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="Kód DPH pro export faktur:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="p28Pohoda_VatCode" runat="server" Style="width: 200px;"></asp:TextBox>


                    </td>
                </tr>
            </table>
            <div class="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="ph1" runat="server" Text="Obsazení klientských rolí"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="p28Contact"></uc:entityrole_assign>
                </div>
            </div>
            <span class="infoInForm">V globální aplikační roli uživatele lze nastavit, aby uživatel disponoval přístupem ke všech klientům v databázi (to lze nastavit i pro vlastnické oprávnění).</span>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <asp:HiddenField ID="HardRefreshPID" runat="server" />
    <asp:HiddenField ID="hidP51ID_Tailor" runat="server" />
    <asp:HiddenField ID="HardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidDistinctCountries" runat="server" />
    <asp:Button ID="cmdHardRefresh" runat="server" Style="display: none;" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">


    <script type="text/javascript">
        <%If Me.chkWhisper.Checked Then%>
        $(function () {

            $("#<%=p28CompanyName.ClientID%>").autocomplete({
                source: "Handler/handler_search_contact.ashx?fo=p28Name",
                minLength: 1,
                select: function (event, ui) {
                    if (ui.item) {
                        if (ui.item.PID != null)
                            dialog_master("clue_p28_record.aspx?pid=" + ui.item.PID, false)

                        return false;
                    }
                },
                open: function (event, ui) {
                    $('ul.ui-autocomplete')
                       .removeAttr('style').hide()
                       .appendTo('#search_dupl_result').show();
                },
                close: function (event, ui) {
                    $('ul.ui-autocomplete')
                    .html("")
                    .hide();
                }


            }).data("ui-autocomplete")._renderItem = function (ul, item) {
                var s = "<div style='background-color:khaki;width:500px;'>";
                if (item.Closed == "1")
                    s = s + "<a style='text-decoration:line-through;'>";
                else
                    s = s + "<a>";

                s = s + __highlight(item.ItemText, item.FilterString);


                s = s + "</a>";


                if (item.Italic == "1")
                    s = "<i>" + s + "</i>"

                s = s + "</div>";


                return $(s).appendTo(ul);


            };
        });

        $(function () {

            $("#<%=p28RegID.ClientID%>").autocomplete({
                source: "Handler/handler_search_contact.ashx?fo=p28RegID",
                minLength: 1,
                select: function (event, ui) {
                    if (ui.item) {
                        if (ui.item.PID != null)
                            dialog_master("clue_p28_record.aspx?pid=" + ui.item.PID, false)

                        return false;
                    }
                },
                open: function (event, ui) {
                    $('ul.ui-autocomplete')
                       .removeAttr('style').hide()
                       .appendTo('#search_dupl_result').show();
                },
                close: function (event, ui) {
                    $('ul.ui-autocomplete')
                    .html("")
                    .hide();
                }


            }).data("ui-autocomplete")._renderItem = function (ul, item) {
                var s = "<div style='background-color:khaki;width:500px;'>";
                if (item.Closed == "1")
                    s = s + "<a style='text-decoration:line-through;'>";
                else
                    s = s + "<a>";

                s = s + __highlight(item.ItemText, item.FilterString);


                s = s + "</a>";


                if (item.Italic == "1")
                    s = "<i>" + s + "</i>"

                s = s + "</div>";


                return $(s).appendTo(ul);


            };
        });

        $(function () {

            $("#<%=p28VatID.ClientID%>").autocomplete({
                source: "Handler/handler_search_contact.ashx?fo=p28VatID",
                minLength: 1,
                select: function (event, ui) {
                    if (ui.item) {
                        if (ui.item.PID != null)
                            dialog_master("clue_p28_record.aspx?pid=" + ui.item.PID, false)

                        return false;
                    }
                },
                open: function (event, ui) {
                    $('ul.ui-autocomplete')
                       .removeAttr('style').hide()
                       .appendTo('#search_dupl_result').show();
                },
                close: function (event, ui) {
                    $('ul.ui-autocomplete')
                    .html("")
                    .hide();
                }


            }).data("ui-autocomplete")._renderItem = function (ul, item) {
                var s = "<div style='background-color:khaki;width:500px;'>";
                if (item.Closed == "1")
                    s = s + "<a style='text-decoration:line-through;'>";
                else
                    s = s + "<a>";

                s = s + __highlight(item.ItemText, item.FilterString);


                s = s + "</a>";


                if (item.Italic == "1")
                    s = "<i>" + s + "</i>"

                s = s + "</div>";


                return $(s).appendTo(ul);


            };
        });

        function __highlight(s, t) {
            var matcher = new RegExp("(" + $.ui.autocomplete.escapeRegex(t) + ")", "ig");
            return s.replace(matcher, "<strong>$1</strong>");
        }


        <%End If%>
    </script>
</asp:Content>
