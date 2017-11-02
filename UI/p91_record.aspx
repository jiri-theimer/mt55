<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p91_record.aspx.vb" Inherits="UI.p91_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="freefields" Src="~/freefields.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function recordcode() {

            dialog_master("record_code.aspx?prefix=p91&pid=<%=Master.DataPID%>");

        }
        function hardrefresh(pid, flag, codeValue) {
            if (flag == "record-code") {
                document.getElementById("<%=Me.p91Code.ClientID%>").innerText = codeValue;
                $.alert("Změna čísla faktury byla uložena.")
                return;
            }



        }
        function changelog() {
            dialog_master("changelog.aspx?prefix=p91&pid=<%=Master.DataPID%>", true)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Vlastnosti" Selected="true" Value="core"></telerik:RadTab>
            <telerik:RadTab Text="Klient faktury" Value="client"></telerik:RadTab>
            <telerik:RadTab Text="Kategorie ({1}), uživatelská pole ({0})" Value="ff"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="core" runat="server" Selected="true">
            <table cellpadding="6" cellspacing="2">
                <tr>
                    <td style="width: 180px;">
                        <asp:Label ID="lblType" runat="server" CssClass="lblReq" Text="Typ faktury:"></asp:Label></td>
                    <td>
                        <uc:datacombo ID="p92ID" runat="server" DataTextField="p92Name" DataValueField="pid" AutoPostBack="true" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                        <asp:HyperLink ID="p91Code" runat="server" ToolTip="Číslo faktury"></asp:HyperLink>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp91DateSupply" Text="Datum zdanitelného plnění:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p91DateSupply" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp91Date" Text="Datum vystavení:" runat="server" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p91Date" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblp91DateMaturity" Text="Datum splatnosti:" runat="server" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p91DateMaturity" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput3" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblJ17ID" Text="DPH region:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="j17ID" runat="server" AutoPostBack="false" DataTextField="j17Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>

                    </td>
                </tr>
                <tr valign="top">
                    <td>
                        <asp:Label ID="Label5" Text="Zaokrouhlovací pravidlo:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p98ID" runat="server" AutoPostBack="false" DataTextField="p98Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label15" runat="server" Text="Struktura cenového rozpisu:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p80ID" runat="server" DataTextField="p80Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblP63ID" runat="server" Text="Režijní přirážka k faktuře:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="p63ID" runat="server" DataTextField="NameWithRate" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblOwner" runat="server" Text="Vlastník záznamu:" CssClass="lblReq"></asp:Label>

                    </td>
                    <td>
                        <uc:person ID="j02ID_Owner" runat="server" Width="300px" Flag="all" />

                    </td>
                </tr>

            </table>
            <div>Text faktury</div>
            <div>
                <asp:TextBox ID="p91text1" runat="server" TextMode="MultiLine" Style="height: 70px; width: 100%;"></asp:TextBox>
            </div>
            <div>Technický text faktury</div>
            <div>
                <asp:TextBox ID="p91text2" runat="server" TextMode="MultiLine" Style="height: 30px; width: 100%;"></asp:TextBox>
            </div>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="Label3" Text="Datum úkonů ve faktuře od:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p91Datep31_From" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput4" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                    <td>
                        <asp:Label ID="Label4" Text="do:" runat="server" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="p91Datep31_Until" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
                            <DateInput ID="DateInput5" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                        </telerik:RadDatePicker>

                    </td>
                </tr>
            </table>
            <asp:panel ID="panRoles" runat="server" cssclass="content-box2">
                <div class="title">
                    <img src="Images/projectrole.png" width="16px" height="16px" />
                    <asp:Label ID="ph1" runat="server" Text="Obsazení klientských rolí"></asp:Label>
                    <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
                </div>
                <div class="content">
                    <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="p91Invoice"></uc:entityrole_assign>
                </div>
            </asp:panel>
        </telerik:RadPageView>
        <telerik:RadPageView ID="tab2" runat="server">
            <table cellpadding="6" cellspacing="2">
                <tr valign="top">
                    <td>
                        <asp:Label ID="lblP28ID" runat="server" Text="Klient (odběratel):" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:contact ID="p28ID" runat="server" Width="400px" AutoPostBack="true" />

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblPerson" runat="server" Text="Kontaktní osoba klienta:" CssClass="lbl"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="j02ID_ContactPerson" runat="server" DataValueField="pid" DataTextField="FullNameDescWithEmail" IsFirstEmptyRow="true" Width="400px" />

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Fakturační adresa klienta:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:datacombo ID="o38ID_Primary" runat="server" AutoPostBack="false" DataTextField="FullAddress" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>

                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Poštovní adresa klienta:" CssClass="lbl"></asp:Label>

                    </td>
                    <td>
                        <uc:datacombo ID="o38ID_Delivery" runat="server" AutoPostBack="false" DataTextField="FullAddress" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>

                    </td>
                </tr>
            </table>

            <div class="content-box2">
                <div class="title">
                    <span>Údaje o klientovi, které historicky zůstávají ve faktuře (bez ohledu na aktualizaci klienta)</span>
                </div>
                <div class="content">
                    <table cellpadding="6" cellspacing="2">
                        <tr>
                            <td colspan="2">
                                <asp:Button ID="cmdLoadClient" runat="server" Text="Načíst klienta do faktury" CssClass="cmd" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label6" runat="server" Text="Název klienta:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="p91Client" runat="server" Style="width: 400px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label14" runat="server" Text="Kontaktní osoba klienta:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="p91ClientPerson" runat="server" Style="width: 200px;"></asp:TextBox>
                                                        <span>Oslovení:</span>
                                <asp:TextBox ID="p91ClientPerson_Salutation" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label7" runat="server" Text="IČ:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="p91Client_RegID" runat="server" Style="width: 150px;"></asp:TextBox>
                                <asp:Label ID="Label8" runat="server" Text="DIČ:" CssClass="lbl"></asp:Label>
                                <asp:TextBox ID="p91Client_VatID" runat="server" Style="width: 150px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label9" runat="server" Text="Ulice:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="p91ClientAddress1_Street" runat="server" TextMode="MultiLine" Style="width: 400px;height:40px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label10" runat="server" Text="Město:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="p91ClientAddress1_City" runat="server" Style="width: 400px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label11" runat="server" Text="PSČ:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="p91ClientAddress1_ZIP" runat="server" Style="width: 100px;"></asp:TextBox>
                                <asp:Label ID="Label12" runat="server" Text="Stát:" CssClass="lbl"></asp:Label>
                                <asp:TextBox ID="p91ClientAddress1_Country" runat="server" Style="width: 250px;"></asp:TextBox>
                            </td>
                        </tr>
                        <tr valign="top">
                            <td>
                                <asp:Label ID="Label13" runat="server" Text="Poštovní adresa:" CssClass="lbl"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="p91ClientAddress2" runat="server" TextMode="MultiLine" Style="width: 400px; height: 80px;"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
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




