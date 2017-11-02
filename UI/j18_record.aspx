<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j18_record.aspx.vb" Inherits="UI.j18_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label></td>
            <td>
                <asp:TextBox ID="j18Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblCode" runat="server" CssClass="lblReq" Text="Kód:"></asp:Label></td>
            <td>
                <asp:TextBox ID="j18Code" runat="server" Style="width: 50px;"></asp:TextBox>
            </td>
        </tr>
        <tr valign="top">
            <td>
                <asp:Label ID="lblJ17ID" Text="DPH region:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j17ID" runat="server" AutoPostBack="false" DataTextField="j17Name" DataValueField="pid" IsFirstEmptyRow="true"></uc:datacombo>
                <span class="infoInForm">Má vliv na okruh DPH sazeb, které systém povolí vykázat na projektech ze střediska.</span>
            </td>
        </tr>

        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="j18Ordinary"></asp:Label></td>
            <td>

                <telerik:RadNumericTextBox ID="j18Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>

    <div class="content-box2">
        <div class="title">
            <img src="Images/projectrole.png" width="16px" height="16px" />
            <asp:Label ID="ph1" runat="server" Text="Obsazení projektových rolí | Oprávnění k projektům ve středisku"></asp:Label>
            <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
        </div>
        <div class="content">
            <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="j18Region"></uc:entityrole_assign>
        </div>
    </div>



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
