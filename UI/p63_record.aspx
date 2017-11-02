<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p63_record.aspx.vb" Inherits="UI.p63_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název přirážky:"></asp:Label></td>
            <td>
                <asp:TextBox ID="p63Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
      
        <tr>
            <td>
                <asp:Label ID="lblP32ID" Text="Peněžní aktivita (položka faktury):" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="p32ID" runat="server" AutoPostBack="false" DataTextField="p32Name" DataValueField="pid" Width="400px" IsFirstEmptyRow="true"></uc:datacombo>


            </td>
        </tr>
       
        <tr>
            <td>
                <asp:Label ID="lblRate" Text="Procento přirážky (nebo slevy):" runat="server" CssClass="lbl" AssociatedControlID="p63PercentRate"></asp:Label>

            </td>
            <td>

                <telerik:RadNumericTextBox ID="p63PercentRate" runat="server" NumberFormat-DecimalDigits="1" Width="70px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                %
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p63IsIncludeTime" runat="server" Text="Do výpočtu přirážky zahrnout honoráře fakturovaných časových úkonů" Checked="true" />
                
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p63IsIncludeFees" runat="server" Text="Do výpočtu přirážky zahrnout pevné/paušální odměny" Checked="true" />
              
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="p63IsIncludeExpense" runat="server" Text="Do výpočtu přirážky zahrnout peněžní výdaje" Checked="false" />
               
            </td>
        </tr>
    </table>

    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

