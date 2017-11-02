<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="m62_record.aspx.vb" Inherits="UI.m62_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="6" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Typ kurzu:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="m62RateType" runat="server">
                    <asp:ListItem Text="Fakturační kurz" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Fixní kurz" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblDate" runat="server" CssClass="lblReq" Text="Datum:"></asp:Label></td>
            <td>
                <telerik:RadDatePicker ID="m62Date" runat="server" RenderMode="Lightweight" Width="120px" DateInput-EmptyMessage="Povinný údaj." DateInput-EmptyMessageStyle-ForeColor="red">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblRate" runat="server" CssClass="lblReq" Text="Kurz ve zdrojové měně:"></asp:Label></td>
            <td>
                <telerik:RadNumericTextBox ID="m62Rate" runat="server" Width="90px" NumberFormat-DecimalDigits="3" MinValue="0"></telerik:RadNumericTextBox>



            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="Množství cílové měny:"></asp:Label></td>
            <td>
                <telerik:RadNumericTextBox ID="m62Units" runat="server" Width="90px" NumberFormat-DecimalDigits="0" MinValue="1"></telerik:RadNumericTextBox>



            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblJ27Master" Text="Zdrojová měna:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j27ID_Master" runat="server" AutoPostBack="false" DataTextField="j27Code" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>


            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblJ27Slave" Text="Cílová měna:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j27ID_Slave" runat="server" AutoPostBack="false" DataTextField="j27Code" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>


            </td>
        </tr>




    </table>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>



