<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p92_record.aspx.vb" Inherits="UI.p92_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6" cellspacing="2">
        <tr>
            <td style="width: 180px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název typu faktury:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p92Name" runat="server" Style="width: 350px;"></asp:TextBox>
                <span>Index pořadí:</span>
                <telerik:RadNumericTextBox ID="p92Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblp92InvoiceType" Text="Typ dokladu:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:RadioButtonList ID="p92InvoiceType" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                    <asp:ListItem Text="Klientská faktura" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Opravný doklad (dobropis)" Value="2"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX38ID" Text="Číselná řada dokladu:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x38ID" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="260px"></uc:datacombo>
                <asp:Label ID="Label2" Text="Číslování DRAFT dokladu:" runat="server" CssClass="lbl"></asp:Label>
                <uc:datacombo ID="x38ID_Draft" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="150px"></uc:datacombo>
            </td>
        </tr>
       
        <tr valign="top">
            <td>
                <asp:Label ID="lblJ27ID" Text="Výchozí měna faktury:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j27ID" runat="server" AutoPostBack="false" DataTextField="j27Code" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>
                <span class="infoInForm">Měnu lze později měnit i ve faktuře.</span>

            </td>
        </tr>
        <tr valign="top" id="trX15ID" runat="server">
            <td>
                <asp:Label ID="lblX15ID" Text="Cílová DPH sazba faktury:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x15ID" runat="server" AutoPostBack="false" DataTextField="x15Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                <span class="infoInForm">Pokud je vyplněno, všechny worksheet úkony vstupující do faktury se automaticky převedou na cílovou sazbu DPH. Ve faktuře lze DPH následně měnit.</span>

            </td>
        </tr>


        <tr valign="top">
            <td>
                <asp:Label ID="lblP93ID" Text="Hlavička vystavovatele faktury:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p93ID" runat="server" AutoPostBack="false" DataTextField="p93Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>

            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="lblx31ID_Invoice" Text="Výchozí sestava faktury:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x31ID_Invoice" runat="server" AutoPostBack="false" DataTextField="NameWithCode" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div>
        <span class="lbl">Do sestavy faktury vkládat text:</span>
    </div>
    <div>
        <asp:TextBox ID="p92ReportConstantText" runat="server" TextMode="MultiLine" Style="width: 650px; height: 50px;"></asp:TextBox>
    </div>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="Label1" Text="Výchozí sestava přílohy faktury:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x31ID_Attachment" runat="server" AutoPostBack="false" DataTextField="NameWithCode" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                <span class="infoInForm">Sestavu přílohy je možné pořizovat i přes ostatní šablony fakturačních sestav.</span>
            </td>
        </tr>
         <tr valign="top">
            <td>
                <asp:Label ID="Label4" Text="Výchozí průvodní dopis k faktuře:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x31ID_Letter" runat="server" AutoPostBack="false" DataTextField="NameWithCode" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                
            </td>
        </tr>
        <tr valign="top" id="trJ17ID" runat="server">
            <td>
                <asp:Label ID="lblJ17ID" Text="Výchozí DPH region faktury:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j17ID" runat="server" AutoPostBack="false" DataTextField="j17Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                <span class="infoInForm">DPH region lze později změnit v samotné faktuře.</span>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="Label3" Text="Výchozí zaokrouhlovací pravidlo faktury:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p98ID" runat="server" AutoPostBack="false" DataTextField="p98Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                <span class="infoInForm">Zaokrouhlovací pravidlo lze později změnit v samotné faktuře.</span>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="Label5" Text="Výchozí struktura cenového rozpisu:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p80ID" runat="server" AutoPostBack="false" DataTextField="p80Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblB01ID" Text="Workflow šablona:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="b01ID" runat="server" AutoPostBack="false" DataTextField="b01Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>


            </td>
        </tr>

    </table>
    
    <div>
        <asp:Label ID="lblp92InvoiceDefaultText1" runat="server" Text="Výchozí text faktury:" CssClass="lbl"></asp:Label>
    </div>
    <div>
        <asp:TextBox ID="p92InvoiceDefaultText1" runat="server" TextMode="MultiLine" Style="width: 650px; height: 50px;"></asp:TextBox>
    </div>
    <div>
        <asp:Label ID="lblp92InvoiceDefaultText2" runat="server" Text="Výchozí technický text faktury:" CssClass="lbl"></asp:Label>
    </div>
    <div>
        <asp:TextBox ID="p92InvoiceDefaultText2" runat="server" TextMode="MultiLine" Style="width: 650px; height: 30px;"></asp:TextBox>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>



