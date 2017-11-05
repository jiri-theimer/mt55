<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="o21_record.aspx.vb" Inherits="UI.o21_record" %>

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
                <asp:Label ID="lblX29ID" Text="Entita:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x29ID" runat="server" DataTextField="x29Name" DataValueField="x29ID" Enabled="False">
                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                    <asp:ListItem Text="Daňová faktura" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>

                </asp:DropDownList>

            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblO25ID" Text="Výchozí kalendář:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="o25ID" runat="server" DataTextField="o25Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblColor" Text="Výchozí barva v kalendáři:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <telerik:RadComboBox ID="o21ColorID" runat="server" Filter="None" AutoPostBack="true">
                    <Items>
                        <telerik:RadComboBoxItem Text="" Value="" />
                        <telerik:RadComboBoxItem BackColor="#4986e7"  Text="9-výrazně modrá" Value="9" />    
                        <telerik:RadComboBoxItem BackColor="#9fc6e7"  Text="1-modrá" Value="1" />  
                        <telerik:RadComboBoxItem BackColor="#30d5c8"  Text="7-tyrkysová" Value="7" />                  
                        <telerik:RadComboBoxItem BackColor="#7ae7bf"  Text="2-zelená" Value="2" />
                        <telerik:RadComboBoxItem BackColor="#b3dc6c"  Text="10-výrazně zelená" Value="10" />
                        <telerik:RadComboBoxItem BackColor="#fbd75b"  Text="5-žlutá" Value="5" />
                        <telerik:RadComboBoxItem BackColor="#ffb878"  Text="6-oranžová" Value="6" />
                        <telerik:RadComboBoxItem BackColor="#ff887c"  Text="4-červená" Value="4" />
                        <telerik:RadComboBoxItem BackColor="#dc2127"  Text="11-výrazně červená" Value="11" />
                        <telerik:RadComboBoxItem BackColor="#dbadff"  Text="3-fialová" Value="3" />
                        <telerik:RadComboBoxItem BackColor="#e1e1e1"  Text="8-šedá" Value="8" />            
                    </Items>
                </telerik:RadComboBox>
            </td>
        </tr>
        
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="o21Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="o21Ordinary"></asp:Label>

            </td>
            <td>

                <telerik:RadNumericTextBox ID="o21Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="content-box2">
                    <div class="title">Událost | Milník</div>
                    <div class="content">
                        <asp:RadioButtonList ID="o21Flag" runat="server" CellPadding="5">
                            <asp:ListItem Text="<img src='Images/milestone.png'/>Pouze jedno datum (termín) [DO] - vhodné pro definování milníků, termínů, lhůt, výročí apod." Value="1" Selected="true"></asp:ListItem>
                            <asp:ListItem Text="<img src='Images/event.png'/>Časový úsek definovaný rozsahem [OD] - [DO] - vhodné pro definování událostí, rezervací apod." Value="2"></asp:ListItem>                            
                        </asp:RadioButtonList>
                    </div>
                </div>

            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

