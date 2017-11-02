<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="barcode.aspx.vb" Inherits="UI.barcode" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Label ID="lblHeader" runat="server" CssClass="page_header_span"></asp:Label>
    </div>

    <table cellpadding="2" cellspacing="2">
        <tr>
            <td>Typ čárového kódu:
            </td>
            <td>
                <asp:DropDownList ID="cbxType" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="QRCode" Value="23" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Code11" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Code128" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Code128A" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Code128B" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Code128C" Value="4"></asp:ListItem>
                    <asp:ListItem Text="Code39" Value="5"></asp:ListItem>
                   
                    
                    
                    <asp:ListItem Text="CodaBar" Value="9"></asp:ListItem>
              
                    
                    <asp:ListItem Text="Code93" Value="16"></asp:ListItem>
                    <asp:ListItem Text="Code93Extended" Value="17"></asp:ListItem>
                   
                   
                    <asp:ListItem Text="PDF417" Value="24"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="trFormat" runat="server" style="display:none;">
            <td>Formát textu:
            </td>
            <td>
                <asp:DropDownList ID="cbxTextFormat" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="Kód + název projektu" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Klient + název projektu" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Text:
            </td>
            <td>
                <asp:TextBox ID="txt1" runat="server" Width="600px" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="chkShowTextBellow" runat="server" Text="Zobrazovat pod kódem text" Checked="false" AutoPostBack="true" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="chkShowChecksum" runat="server" Text="Zobrazovat za textem kontrolní součet" Checked="false" AutoPostBack="true" />
            </td>
        </tr>
    </table>
    <div class="div6">
        <asp:Button ID="cmdRefresh" runat="server" Text="Obnovit" CssClass="cmd" />
        <asp:Button ID="cmdSave" runat="server" Text="Uložit jako obrázek" CssClass="cmd" style="margin-left:50px;" />
    </div>
    <telerik:RadBarcode ID="bc1" runat="server" Type="Code11" OutputType="SVG_VML" Font-Size="12px" Height="100px">
        <PDF417Settings EncodingMode="Byte" />
    </telerik:RadBarcode>

   

    <asp:HiddenField ID="hidRecordPrefix" runat="server" />
    <asp:HiddenField ID="hidRecordPID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
