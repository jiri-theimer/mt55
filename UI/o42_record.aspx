<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o42_record.aspx.vb" Inherits="UI.o42_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register TagPrefix="uc" TagName="person" Src="~/person.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6" cellspacing="2">
        <tr>
            <td style="width:160px;">
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název pravidla:"></asp:Label></td>
            <td>
                <asp:TextBox ID="o42Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
            <td>
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblO41ID" runat="server" CssClass="lblReq" Text="IMAP účet:"></asp:Label></td>
            <td>
                <uc:datacombo ID="o41ID" runat="server" DataTextField="o41Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>

            </td>
            <td></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:RadioButtonList ID="opgTarget" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
                    <asp:ListItem Text="Cílem pravidla je založit z načtené poštovní zprávy úkol/požadavek" Value="p56" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Cílem pravidla je založit z načtené poštovní zprávy dokument" Value="o23"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblBind" runat="server" CssClass="lblReq" Text="Vazba:"></asp:Label>

            </td>
            <td>
                <uc:datacombo ID="p57ID" runat="server" DataTextField="p57Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" Visible="false"></uc:datacombo>
                <uc:datacombo ID="x18ID" runat="server" DataTextField="x18Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px" Visible="false"></uc:datacombo>

            </td>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblRole" runat="server" CssClass="lblReq" Text="Role pro CC/TO osoby:"></asp:Label>

            </td>
            <td>
                <uc:datacombo ID="x67ID" runat="server" DataTextField="x67Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                <span class="infoInForm">Příjemcem poštovní zprávy může být imap adresa konkrétní osoby nebo řešitelského týmu.</span>

            </td>
            <td></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:CheckBox ID="o42IsUse_CC" runat="server" Checked="true" Text="Rozpoznávat osoby/projekt/klient ze CC (v kopii)" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:CheckBox ID="o42IsUse_To" runat="server" Text="Rozpoznávat řešitele/projekt/klient z přímých příjemců zprávy" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblSenderAddress" runat="server" CssClass="lbl" Text="Filtrování podle odesílatele zprávy:"></asp:Label></td>
            <td>
                <asp:TextBox ID="o42SenderAddress" runat="server" Style="width: 400px;height:50px;" TextMode="MultiLine"></asp:TextBox>
            </td>
            <td>
                <span class="infoInForm">Může být celá e-mail adresa nebo pouze doména. Oddělovač je středník. Pokud není vyplněno, pravidlo zpracuje všechny příchozí zprávy.</span>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDefaultOwner" Text="Výchozí vlastník:" runat="server" CssClass="lbl"></asp:Label>

            </td>
            <td>
                <uc:person ID="j02ID_Owner_Default" runat="server" Width="400px" />

            </td>
            <td>
                <span class="infoInForm">Aplikuje se v případě, že se pro zakládaný úkol nebo dokument nenajde osoba vlastníka z adresy odesílatele poštovní zprávy.</span>
            </td>
        </tr>

        <tr id="trProject" runat="server">
            <td>
                <span class="lbl">Výchozí projekt:</span>
            </td>
            <td>
                <uc:project ID="p41ID_Default" runat="server" Width="400px" />
            </td>
            <td>
                <span class="infoInForm">Aplikuje se v případě, že se pro zakládaný úkol nebo dokument nenajde projekt z CC/TO adres</span>
            </td>
        </tr>


    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
