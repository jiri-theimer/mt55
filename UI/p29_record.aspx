<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p29_record.aspx.vb" Inherits="UI.p29_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p29Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX38ID" Text="Číselná řada klientů:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x38ID" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="250px"></uc:datacombo>
                 <asp:Label ID="Label1" Text="Číselná řada DRAFT klientů:" runat="server" CssClass="lbl"></asp:Label>
                <uc:datacombo ID="x38ID_Draft" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="200px"></uc:datacombo>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblB01ID" Text="Workflow šablona:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="b01ID" runat="server" AutoPostBack="false" DataTextField="b01Name" DataValueField="pid" Width="250px"></uc:datacombo>


            </td>
        </tr>
       
        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="p29Ordinary"></asp:Label>

            </td>
            <td>

                <telerik:RadNumericTextBox ID="p29Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p29IsDefault" runat="server" Text="Výchozí pro nové záznamy klientů" Visible="false" />
                <!-- //
                Výchozí klient se bere z naposledy uloženého klienta, volba p29IsDefault se již nevyužívá
                // -->
            </td>
        </tr>
    </table>

    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

