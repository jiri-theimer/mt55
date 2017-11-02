<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p65_record.aspx.vb" Inherits="UI.p65_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datalabel" Src="~/datalabel.ascx" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:RadioButtonList ID="p65RecurFlag" runat="server" RepeatDirection="Vertical" AutoPostBack="true">        
        <asp:ListItem Value="3" Text="Měsíční opakování (rozhodné datum = 1.den v měsíci)" Selected="true"></asp:ListItem>
        <asp:ListItem Value="4" Text="Čtvrtletní opakování (rozhodné datum 1.den čtvrtletí)"></asp:ListItem>
        <asp:ListItem Value="5" Text="Roční opakování (první den v roce)"></asp:ListItem>
    </asp:RadioButtonList>

    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" Text="Název pravidla:" CssClass="lblReq" />
            </td>
            <td>
                <asp:TextBox ID="p65Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" Text="Čas generování bude o X dní před(-)/po(+) rozhodným dnem:" CssClass="lbl" />
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p65RecurGenToBase_D" runat="server" MinValue="-10" MaxValue="25" Value="-1" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label3" runat="server" Text="Čas generování bude o X měsíců před(-)/po(+) rozhodným dnem:" CssClass="lbl" />
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p65RecurGenToBase_M" runat="server" MinValue="0" MaxValue="18" Value="0" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p65IsPlanUntil" runat="server" Text="Pracovat s termínem plánovaného dokončení" Checked="true" AutoPostBack="true" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblp65RecurPlanUntilToBase_D" runat="server" Text="Plánované dokončení (termín) bude o X dní před(-)/po(+) rozhodným dnem:" CssClass="lbl" />
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p65RecurPlanUntilToBase_D" runat="server" MinValue="-10" MaxValue="30" Value="20" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblp65RecurPlanUntilToBase_M" runat="server" Text="Plánované dokončení (termín) bude o X měsíců před(-)/po(+) rozhodným dnem:" CssClass="lbl" />
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p65RecurPlanUntilToBase_M" runat="server" MinValue="0" MaxValue="18" Value="0" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p65IsPlanFrom" runat="server" Text="Pracovat s datumem plánovaného zahájení" AutoPostBack="true" Checked="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblp65RecurPlanFromToBase_D" runat="server" Text="Datum plánovaného zahájení bude o X dní před(-)/po(+) rozhodným dnem:" CssClass="lbl" />
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p65RecurPlanFromToBase_D" runat="server" MinValue="-10" MaxValue="25" NumberFormat-DecimalDigits="0" Value="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblp65RecurPlanFromToBase_M" runat="server" Text="Datum plánovaného zahájení bude o X měsíců před(-)/po(+) rozhodným dnem:" CssClass="lbl" />
            </td>
            <td>

                <telerik:RadNumericTextBox ID="p65RecurPlanFromToBase_M" runat="server" MinValue="0" MaxValue="18" NumberFormat-DecimalDigits="0" Value="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>

   
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


