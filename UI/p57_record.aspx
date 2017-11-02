<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p57_record.aspx.vb" Inherits="UI.p57_record" %>

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
                <asp:TextBox ID="p57Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX38ID" Text="Číselná řada úkolu:" runat="server" CssClass="lblReq"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="x38ID" runat="server" DataTextField="x38Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblB01ID" Text="Workflow šablona:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="b01ID" runat="server" AutoPostBack="false" DataTextField="b01Name" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>

                <asp:CheckBox ID="p57IsHelpdesk" runat="server" Text="Tento typ slouží k zapisování helpdesk požadavků" CssClass="chk" Visible="false" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Datumy v úkolu:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="p57PlanDatesEntryFlag" runat="server">
                    <asp:ListItem Text="Termín nepovinný, plánované zahájení skryté" Value="" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Termín povinný, plánované zahájení nepovinné" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Termín nepovinný, plánované zahájení nepovinné" Value="2"></asp:ListItem>
                    <asp:ListItem Text="Termín povinný, plánované zahájení povinné" Value="3"></asp:ListItem>
                    <asp:ListItem Text="Termín povinný, plánované zahájení povinné, v kalendáři zobrazovat jako událost od-do" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p57IsEntry_Receiver" runat="server" Text="Ve formuláři úkolu zadávat okruh příjemců (řešitelů) úkolu" CssClass="chk" Checked="true"/>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p57IsEntry_Budget" runat="server" Text="Ve formuláři úkolu možnost zadat plán/limit hodin a výdajů" CssClass="chk" Checked="true"/>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p57IsEntry_Priority" runat="server" Text="Ve formuláři úkolu možnost zadat [Priorita zadavatele]" CssClass="chk" Checked="true"/>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p57IsEntry_CompletePercent" runat="server" Text="Ve formuláři úkolu možnost zadat [Hotovo %]" CssClass="chk" Checked="true"/>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblp57Caption_PlanUntil" runat="server" CssClass="lbl" Text="Přejmenovat [Termín splnění] na:"></asp:Label>

            </td>
            <td>
                <asp:TextBox ID="p57Caption_PlanUntil" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblp57Caption_PlanFrom" runat="server" CssClass="lbl" Text="Přejmenovat [Plánované zahájení] na:"></asp:Label>

            </td>
            <td>
                <asp:TextBox ID="p57Caption_PlanFrom" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="p57Ordinary"></asp:Label></td>
            <td>

                <telerik:RadNumericTextBox ID="p57Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

