<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_search.aspx.vb" Inherits="UI.mobile_search" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="invoice" Src="~/invoice.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function p41id_search(sender, eventArgs) {
            //var item = eventArgs.get_item();
            var pid = <%=Me.p41id_search.ClientID%>_get_value();
            location.replace("mobile_p41_framework.aspx?pid=" + pid);
        }
        function p28id_search(sender, eventArgs) {
            var pid = <%=Me.p28id_search.ClientID%>_get_value();
            location.replace("mobile_p28_framework.aspx?pid=" + pid);
        }
        function j02id_search(sender, eventArgs) {
            var pid = <%=Me.j02id_search.ClientID%>_get_value();
            location.replace("mobile_report.aspx?prefix=j02&pid=" + pid);
        }
        function p91id_search(sender, eventArgs) {
            //var item = eventArgs.get_item();
            var pid = <%=Me.p91id_search.ClientID%>_get_value();
            location.replace("mobile_p91_framework.aspx?pid=" + pid);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well">
        <img src="Images/search_20.png" />
        <button type="button" data-toggle="collapse" data-target="#divSetting">
            <img src='Images/arrow_down.gif' />Nastavení</button>
        <button type="button" data-toggle="collapse" data-target="#divLast">
            <img src='Images/arrow_down.gif' />Naposledy navštívené</button>

        <div id="divSetting" class="collapse">
            Počet maximálně zobrazených položek a zda prohledávat i archivované záznamy si nastavte v základním rozhraní aplikace MARKTIME. Zde, v mobilním rozhraní se nastavení zohlední.
        </div>
        <div id="divLast" class="collapse" style="margin-top:10px;">
            <div>

                <asp:HyperLink ID="linkLastProject" runat="server" Style="display: none;" NavigateUrl="mobile_p41_framework.aspx"></asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="linkLastClient" runat="server" Style="display: none;" NavigateUrl="mobile_p28_framework.aspx"></asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="linkLastInvoice" runat="server" Style="display: none;" NavigateUrl="mobile_p91_framework.aspx"></asp:HyperLink>
            </div>
            <div>
                <asp:HyperLink ID="linkLastPerson" runat="server" Style="display: none;" NavigateUrl="mobile_j02_framework.aspx"></asp:HyperLink>
            </div>

        </div>
    </div>

    <asp:Panel ID="panP41" runat="server">
        <div>
            Projekt:
        </div>
        <div>
            <uc:project ID="p41id_search" runat="server" Width="99%" Flag="searchbox" AutoPostBack="false" OnClientSelectedIndexChanged="p41id_search" />
        </div>
    </asp:Panel>
    <asp:Panel ID="panP28" runat="server">
        <div>
            Klient:
        </div>
        <div>
            <uc:contact ID="p28id_search" runat="server" Width="99%" Flag="searchbox" AutoPostBack="false" />
        </div>
    </asp:Panel>
    <asp:Panel ID="panP91" runat="server">
        <div>
            Faktura:
        </div>
        <div>
            <uc:invoice ID="p91id_search" runat="server" Width="99%" Flag="searchbox" OnClientSelectedIndexChanged="p91id_search" />
        </div>
    </asp:Panel>
    <asp:Panel ID="panJ02" runat="server">
        <div>
            Osoba:
        </div>
        <div>
            <uc:person ID="j02id_search" runat="server" Width="99%" Flag="searchbox" AutoPostBack="false" OnClientSelectedIndexChanged="j02id_search" />
        </div>
    </asp:Panel>



</asp:Content>
