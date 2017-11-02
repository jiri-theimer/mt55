<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_search.aspx.vb" Inherits="UI.clue_search" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="invoice" Src="~/invoice.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function p41id_search(sender, eventArgs) {
            //var item = eventArgs.get_item();
            var pid = <%=Me.p41id_search.ClientID%>_get_value();
            window.open("p41_framework.aspx?pid=" + pid, "_top");
        }
        function p28id_search(sender, eventArgs) {
            var pid = <%=Me.p28id_search.ClientID%>_get_value();
            window.open("p28_framework.aspx?pid=" + pid, "_top");
        }
        function j02id_search(sender, eventArgs) {
            var pid = <%=Me.j02id_search.ClientID%>_get_value();
            window.open("j02_framework.aspx?pid=" + pid, "_top");
        }
        function p91id_search(sender, eventArgs) {
            //var item = eventArgs.get_item();
            var pid = <%=Me.p91id_search.ClientID%>_get_value();
            window.open("p91_framework.aspx?pid=" + pid, "_top");
        }
        function cbx1_OnClientSelectedIndexChanged(sender, eventArgs) {
            var combo = sender;
            var ret = combo.get_value();
            var a = ret.split("|");
            if (a[0] == "p31") {
                window.open("p31_grid.aspx?pid=" + a[1], "_top");
            }
            else {
                window.open(a[0] + "_framework.aspx?pid=" + a[1], "_top");
            }


        }
        function cbx1_OnClientItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            var combo = sender;

            if (combo.get_value() == "")
                context["filterstring"] = eventArgs.get_text();
            else
                context["filterstring"] = "";

            context["j03id"] = "<%=Master.Factory.SysUser.PID%>";


        }
        function cbx1_OnClientFocus(sender, args) {
            var combo = sender;
            var s = combo.get_text();
            if (s.indexOf("...") > 0)
                combo.set_text("");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panCommands" runat="server" CssClass="div6">
        <button type="button" onclick="window.close()">Zavřít</button>
    </asp:Panel>
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="RadMultiPage1" ShowBaseLine="true">
        <Tabs>
            <telerik:RadTab Text="Hledat" Selected="true" Value="search"></telerik:RadTab>
            <telerik:RadTab Text="Nastavení hledání" Value="fulltext"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server">
        <telerik:RadPageView ID="search" runat="server" Selected="true">
            <table>
                <tr id="trP41" runat="server">
                    <td>
                        <img src="Images/search_20.png" />
                    </td>
                    <td>Projekt:
                    </td>
                    <td>
                        <uc:project ID="p41id_search" runat="server" Width="690px" Flag="searchbox" AutoPostBack="false" OnClientSelectedIndexChanged="p41id_search" />
                    </td>


                </tr>
                <tr id="trP28" runat="server">
                    <td>
                        <img src="Images/search_20.png" />
                    </td>
                    <td>Klient:
                    </td>
                    <td>
                        <uc:contact ID="p28id_search" runat="server" Width="690px" Flag="searchbox" AutoPostBack="false" />
                    </td>


                </tr>
                <tr id="trP91" runat="server">
                    <td>
                        <img src="Images/search_20.png" />
                    </td>
                    <td>Faktura:
                    </td>
                    <td>
                        <uc:invoice ID="p91id_search" runat="server" Width="690px" Flag="searchbox" />

                    </td>

                </tr>
                <tr id="trJ02" runat="server">
                    <td>
                        <img src="Images/search_20.png" />
                    </td>
                    <td>Osoba:
                    </td>

                    <td>
                        <uc:person ID="j02id_search" runat="server" Width="690px" Flag="searchbox" AutoPostBack="false" />

                    </td>


                </tr>

            </table>
            <asp:Panel ID="panFulltext" runat="server" CssClass="content-box2">
                <div class="title">
                    FULL-TEXT hledání
                </div>
                <div class="content">
                    <img src="Images/search_20.png" />
                    <telerik:RadComboBox ID="cbx1" runat="server" RenderMode="Auto" DropDownWidth="700px" EnableTextSelection="true" MarkFirstMatch="true" EnableLoadOnDemand="true" ShowToggleImage="false" Text="Hledat..." Width="700px" OnClientFocus="cbx1_OnClientFocus" OnClientSelectedIndexChanged="cbx1_OnClientSelectedIndexChanged" OnClientItemsRequesting="cbx1_OnClientItemsRequesting" AutoPostBack="false">
                        <WebServiceSettings Method="LoadComboData" UseHttpGet="false" Path="~/Services/fulltext_service.asmx" />
                    </telerik:RadComboBox>

                    <div style="clear:both;">
                    <div style="float: left; padding: 10px;">
                        <asp:CheckBox ID="chkMain" runat="server" Checked="true" Text="Projekty, klienti, kontaktní osoby" AutoPostBack="true" />
                    </div>
                    <div style="float: left; padding: 10px;">
                        <asp:CheckBox ID="chkWorksheet" runat="server" Checked="true" Text="Worksheet (hodiny, výdaje, odměny)" AutoPostBack="true" />
                    </div>
                    <div style="float: left; padding: 10px;">
                        <asp:CheckBox ID="chkInvoice" runat="server" Checked="true" Text="Faktury, zálohové faktury" AutoPostBack="true" />
                    </div>
                    <div style="float: left; padding: 10px;">
                        <asp:CheckBox ID="chkTask" runat="server" Text="Úkoly" AutoPostBack="true" />
                    </div>
                    <div style="float: left; padding: 10px;">
                        <asp:CheckBox ID="chkDocument" runat="server" Text="Dokumenty" AutoPostBack="true" />
                    </div>
                    </div>
                </div>
            </asp:Panel>


        </telerik:RadPageView>
        <telerik:RadPageView ID="fulltext" runat="server">
            <fieldset style="padding: 6px;" id="fsP41" runat="server">
                <legend>Vyhledávání projektu</legend>
                <p>Částečná shoda v: Název projektu | Kód projektu | Zkrácený název projektu | Název klienta | Zkrácený název klienta</p>
                <div class="div6">
                    <span>Kolik maximálně zobrazit nalezených:</span>
                    <asp:DropDownList ID="cbxP41Top" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="20" Value="20"></asp:ListItem>
                        <asp:ListItem Text="50" Value="50" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="100" Value="100"></asp:ListItem>
                        <asp:ListItem Text="200" Value="200"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:CheckBox ID="chkP41Bin" runat="server" Text="Hledat i v archivu" AutoPostBack="true" CssClass="chk" />
                </div>
                <div class="div6">
                    <span>Maska ve vyhledávači projektu:</span>
                    <asp:DropDownList ID="j03ProjectMaskIndex" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="Klient+název projektu+kód projektu" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Strom cesta" Value="5"></asp:ListItem>
                        <asp:ListItem Text="Název projektu" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Název projektu+kód projektu" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Název projektu+klient" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Kód projektu" Value="4"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </fieldset>
            <fieldset style="padding: 6px;" id="fsP28" runat="server">
                <legend>Vyhledávání klienta</legend>
                <p>Částečná shoda v: Název | Kód | Zkrácený název | IČ | DIČ</p>

                <div class="div6">
                    <span>Kolik maximálně zobrazit nalezených:</span>
                    <asp:DropDownList ID="cbxP28Top" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="20" Value="20"></asp:ListItem>
                        <asp:ListItem Text="50" Value="50" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="100" Value="100"></asp:ListItem>
                        <asp:ListItem Text="200" Value="200"></asp:ListItem>
                    </asp:DropDownList>
                    <asp:CheckBox ID="chkP28Bin" runat="server" Text="Hledat i v archivu" AutoPostBack="true" CssClass="chk" />
                </div>
            </fieldset>
            <fieldset style="padding: 6px;" id="fsP91" runat="server">
                <legend>Vyhledávání faktury</legend>
                <p>Částečná shoda v: Číslo dokladu | Název klienta (odběratele faktury) | Text faktury | IČ klienta | DIČ klienta | Název fakturovaného projektu</p>
                <div class="div6">
                    <span>Kolik maximálně zobrazit nalezených:</span>
                    <asp:DropDownList ID="cbxP91Top" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="20" Value="20"></asp:ListItem>
                        <asp:ListItem Text="50" Value="50" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="100" Value="100"></asp:ListItem>
                        <asp:ListItem Text="200" Value="200"></asp:ListItem>
                    </asp:DropDownList>

                </div>
            </fieldset>
            <fieldset style="padding: 6px;" id="fsJ02" runat="server">
                <legend>Vyhledávání osoby</legend>
                <p>Částečná shoda v: Jméno | Příjmení | E-mail</p>
                <div class="div6">
                    <asp:CheckBox ID="chkJ02Bin" runat="server" Text="Hledat i v archivu" AutoPostBack="true" CssClass="chk" />
                </div>
            </fieldset>

         

        </telerik:RadPageView>
    </telerik:RadMultiPage>

</asp:Content>
