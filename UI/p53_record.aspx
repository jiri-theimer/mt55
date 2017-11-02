<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p53_record.aspx.vb" Inherits="UI.p53_record" %>

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
                <asp:Label ID="lblValue" runat="server" CssClass="lblReq" Text="Hodnota sazby (%):"></asp:Label></td>
            <td>
                <telerik:RadNumericTextBox ID="p53Value" runat="server" Width="90px"></telerik:RadNumericTextBox>


                <asp:Label ID="lblValidity" Text="Sazba platí pro období:" runat="server" CssClass="lbl"></asp:Label>
                
                <telerik:RadDatePicker ID="p53ValidFrom" runat="server" RenderMode="Lightweight" Width="120px">
                    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
                <telerik:RadDatePicker ID="p53ValidUntil" runat="server" RenderMode="Lightweight" Width="120px" MaxDate="1.1.3000">
                    <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
                </telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX15ID" Text="Hladina:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x15ID" runat="server" AutoPostBack="false" DataTextField="x15Name" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>


            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblJ27ID" Text="Měna:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j27ID" runat="server" AutoPostBack="false" DataTextField="j27Code" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>


            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="lblJ17ID" Text="DPH region:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j17ID" runat="server" AutoPostBack="false" DataTextField="j17Name" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>
                
            </td>
        </tr>
       
        

    </table>


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>


