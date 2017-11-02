<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p42_record.aspx.vb" Inherits="UI.p42_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p42Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX38ID" Text="Číselná řada projektů:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x38ID" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
                <asp:Label ID="Label1" Text="Číselná řada DRAFT projektů:" runat="server" CssClass="lbl"></asp:Label>
                <uc:datacombo ID="x38ID_Draft" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="200px"></uc:datacombo>
            </td>
        </tr>
        
        
    </table>

    <div class="content-box2">
        <div class="title">
            Projekt pracuje s níže zaškrtlými moduly:
        </div>
        <div class="content">            
            <div class="div6">
                <asp:CheckBox ID="p42IsModule_p31" runat="server" Text="WORKSHEET" AutoPostBack="true" />
                <span class="infoInForm">Zapisování/schvalování/fakturace worksheet úkonů (hodiny/výdaje/pevné (paušální) odměny/kusovníkové úkony).</span>
            </div>
            <div class="div6">
                <asp:CheckBox ID="p42IsModule_p56" runat="server" Text="ÚKOLY" />
                <span class="infoInForm">Zakládání úkolů k řešení s termínem, jejich přidělování řešitelům (osoby/týmy). Možnost zapisovat k úkolům worksheet úkony, schvalování, fakturace.</span>
            </div>
            <div class="div6">
                <asp:CheckBox ID="p42IsModule_o22" runat="server" Text="Kalendářové UDÁLOSTI" />
                <span class="infoInForm">Zakládání termínů do kalendáře, jejich notifikace, synchronizace s externím kalendářem.</span>
            </div>
            <div class="div6">
                <asp:CheckBox ID="p42IsModule_o23" runat="server" Text="DOKUMENTY" AutoPostBack="true" />
                <asp:DropDownList ID="p42SubgridO23Flag" runat="server">
                    <asp:ListItem Text="Dokumenty projektu zobrazovat navíc i v boxu [Dokumenty]" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Dokumenty projektu pouze v samostatné záložce [Dokumenty]" Value="1"></asp:ListItem>                    
                </asp:DropDownList>
                <span class="infoInForm">Dokumentem může být např. fakturační poznámka, výdajový doklad, došlá faktura, došlá pošta apod.</span>
            </div>
            <div class="div6">
                <asp:CheckBox ID="p42IsModule_p48" runat="server" Text="Operativní plánování" />
                <span class="infoInForm">Operativní plánování hodin pracovníků v projektech do konkrétních dnů na nejbližší měsíc, maximálně 2 měsíce dopředu. Naplánované hodiny lze následně překlápět do reálných časových výkazů.</span>
            </div>
            <div class="div6">
                <asp:CheckBox ID="p42IsModule_p45" runat="server" Text="Projektové ROZPOČTY a kapacitní PLÁNY" />
                <span class="infoInForm">Verzované rozpočty odhadů/limitů hodin, výdajeů a příjmů. Kapacitní plánování na člověko-měsíce.</span>
            </div>
        </div>
    </div>

    <asp:panel ID="panP34IDs" runat="server" CssClass="content-box2">
        <div class="title">
            <asp:Label ID="ph1" runat="server" Text="Povolené sešity pro vykazování" />
        </div>
        <div class="content">
            <asp:CheckBoxList ID="p34ids" runat="server" AutoPostBack="false" DataValueField="pid" DataTextField="p34Name" RepeatColumns="3" CellPadding="8" CellSpacing="2"></asp:CheckBoxList>
        </div>
    </asp:panel>

    <table cellpadding="3" cellspacing="2">     
        <tr>
            <td>
                <asp:Label ID="lblF02ID" Text="V projektu založit složku:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="f02ID" runat="server" AutoPostBack="false" DataTextField="f02Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>


            </td>
        </tr>   
        <tr>
            <td>
                <asp:Label ID="lblB01ID" Text="Workflow šablona:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="b01ID" runat="server" AutoPostBack="false" DataTextField="b01Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>


            </td>
        </tr>   
        <tr>
            <td>
                <asp:Label ID="lblCode" Text="Kód:" runat="server" CssClass="lbl" AssociatedControlID="p42Code"></asp:Label></td>
            <td>
                <asp:TextBox ID="p42Code" runat="server" Style="width: 100px;"></asp:TextBox>
                <asp:CheckBox ID="p42IsDefault" runat="server" Text="Výchozí pro nové záznamy projektů" Visible="false" />
                <!-- //
                Výchozí projekt se bere z naposledy uloženého projektu, volba p42IsDefault se již nevyužívá
                // -->
            </td>
        </tr>
        
        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="p42Ordinary"></asp:Label>

            </td>
            <td>

                <telerik:RadNumericTextBox ID="p42Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="Ochrana proti archivování projektu:"></asp:Label></td>
            <td>
                <asp:DropDownList ID="p42ArchiveFlag" runat="server">
                    <asp:ListItem Text="Bez omezení" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Projekty nelze přesunout do archivu, pokud v něm existují nevyfakturované worksheet úkony" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Projekty nelze přesunout do archivu, pokud v něm existují rozpracované worksheet úkony" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label3" runat="server" CssClass="lbl" Text="Ochrana proti archivování úkonů v projektu:"></asp:Label></td>
            <td>
                <asp:DropDownList ID="p42ArchiveFlagP31" runat="server">
                    <asp:ListItem Text="Do archivu povoleno přesouvat rozpracované úkony" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Do archivu povoleno přesouvat rozpracované nebo schválené úkony" Value="2" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Zákaz archivovat jakékoliv úkony" Value="3"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
    </table>




</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

