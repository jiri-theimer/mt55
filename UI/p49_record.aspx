<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p49_record.aspx.vb" Inherits="UI.p49_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="periodmonth" Src="~/periodmonth.ascx" %>
<%@ Register TagPrefix="uc" TagName="contact" Src="~/contact.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6">
        <tr>
            <td>
                <asp:Label ID="lblP45ID" runat="server" CssClass="lblReq" Text="Rozpočet:"></asp:Label>
            </td>
            <td>
                <asp:Label ID="p45Name" runat="server" CssClass="valbold"></asp:Label>
                <asp:HiddenField ID="p45ID" runat="server" />
                <asp:HiddenField id="p41ID" runat="server"></asp:HiddenField>
            </td>
        </tr>
        <tr>
            <td><span class="lblReq">Sešit:</span></td>
            <td>
                <uc:datacombo ID="p34ID" runat="server" DataTextField="p34Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" AutoPostBack="true"></uc:datacombo>
            </td>

        </tr>
        <tr>
            <td><span class="lbl">Aktivita:</span></td>
            <td>
                <uc:datacombo ID="p32ID" runat="server" DataTextField="p32Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="lblSupplier" runat="server" Text="Dodavatel:" CssClass="lbl" Visible="false"></asp:Label>

            </td>
            <td>
                <uc:contact ID="p28ID_Supplier" runat="server" Width="400px" Flag="supplier" Visible="false" />


            </td>
        </tr>
        <tr>
            <td><span class="lbl">Osoba:</span></td>
          <td>
                <uc:person ID="j02ID" runat="server" Width="400px" />
            </td>
        </tr>
        <tr>
            <td><span class="lblReq">Částka (bez DPH):</span></td>
            <td>
                <telerik:RadNumericTextBox ID="p49Amount" runat="server" Width="110px" NumberFormat-DecimalDigits="2" ShowSpinButtons="false"></telerik:RadNumericTextBox>
                <uc:datacombo ID="j27ID" Width="60px" runat="server" DataTextField="j27Code" DataValueField="pid"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td><span class="lblReq">Měsíc:</span></td>
            <td>
                <uc:periodmonth ID="month1" runat="server" AutoPostback="false" />
            </td>
        </tr>
        <tr valign="top">
            <td>Text:</td>
            <td>
                <asp:TextBox ID="p49Text" runat="server" TextMode="MultiLine" Style="width: 400px; height: 50px;"></asp:TextBox>
            </td>
        </tr>
    </table>
    
     
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
