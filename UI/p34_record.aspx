<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p34_record.aspx.vb" Inherits="UI.p34_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:label runat="server" ID="lblP33ID" Text="Formát vstupních dat:" CssClass="lblReq"></asp:label>
            </td>
            <td>
                <uc:datacombo ID="p33ID" runat="server" AutoPostBack="true" Width="200px" DataValueField="pid" DataTextField="p33Name"></uc:datacombo>

                

            </td>
        </tr>
        
        <tr>
            <td>
                <asp:label runat="server" ID="lblP34Name" Text="Název sešitu:" CssClass="lblReq"></asp:label>
            </td>
            <td>
                <asp:TextBox ID="p34name" runat="server" Style="width: 400px;"></asp:TextBox>
                <asp:label runat="server" ID="lblp34Code" Text="Kód sešitu:"></asp:label>
                <asp:TextBox ID="p34Code" runat="server" Style="width: 100px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:label runat="server" ID="Label2" Text="Index pořadí:" ></asp:label>
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p34Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>

            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:label runat="server" ID="Label1" Text="Barva:" ></asp:label>
            </td>
            <td>
                <telerik:RadColorPicker ID="p34Color" runat="server" CurrentColorText="Vybraná barva" NoColorText="Bez barvy" ShowIcon="true" Preset="Default" >
                </telerik:RadColorPicker>
                <span class="infoInForm">Barva pro odlišení sešitu v DAYLINE zobrazení a v operativním plánování.</span>
            </td>
        </tr>
    </table>
    <div style="padding: 6px;">
        <asp:DropDownList ID="p34ActivityEntryFlag" runat="server" AutoPostBack="true">            
            <asp:ListItem Text="V rozhraní worksheet úkonu uživatel aktivitu zadává" Value="3" Selected="True"></asp:ListItem>
            <asp:ListItem Text="V rozhraní worksheet úkonu uživatel aktivitu nezadává" Value="1"></asp:ListItem>            
        </asp:DropDownList>

        <asp:DropDownList ID="p34IncomeStatementFlag" runat="server" style="margin-left:80px;">
            <asp:ListItem Text="Pro reporting má povahu nákladů" Value="1" Selected="True"></asp:ListItem>
            <asp:ListItem Text="Pro reporting má povahu výnosů" Value="2"></asp:ListItem>
        </asp:DropDownList>
    </div>
    <asp:Panel ID="panDefaultActivity" runat="server">
        <span class="infoInForm">Pokud uživatelé nemají zadávat aktivitu do worksheet úkonu, technicky je nutné nějakou aktivitu do úkonu uložit!</span>
        <div>
            <span>Technicky vložená aktivita do worksheet úkonu:</span>
            <uc:datacombo ID="p32ID" runat="server" Width="400px" DataValueField="pid" DataTextField="p32Name" IsFirstEmptyRow="true"></uc:datacombo>
        </div>
    </asp:Panel>

    <div class="content-box2">
        <div class="title">
            <asp:label ID="ph1" runat="server" Text="Překlad sešitu do ostatních fakturačních jazyků" />
        </div>
    
    <asp:Panel ID="panLang" runat="server" CssClass="content">
        <table cellpadding="3" cellspacing="2">
            <tr>
                <td>
                    <asp:label runat="server" ID="lblLang1" Text="Název (Jazyk 1):"></asp:label>
                </td>
                <td>
                    <asp:TextBox ID="p34Name_BillingLang1" runat="server" Style="width: 400px;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:label runat="server" ID="lblLang2" Text="Název (Jazyk 2):"></asp:label>
                </td>
                <td>
                    <asp:TextBox ID="p34Name_BillingLang2" runat="server" Style="width: 400px;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:label runat="server" ID="lblLang3" Text="Název (Jazyk 3):"></asp:label>
                </td>
                <td>
                    <asp:TextBox ID="p34Name_BillingLang3" runat="server" Style="width: 400px;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:label runat="server" ID="lblLang4" Text="Název (Jazyk 4):"></asp:label>
                </td>
                <td>
                    <asp:TextBox ID="p34Name_BillingLang4" runat="server" Style="width: 400px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
