<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="j62_record.aspx.vb" Inherits="UI.j62_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="6" cellspacing="2">
         <tr>
            <td>Menu šablona:
            </td>
            <td>
                <asp:Label ID="j60Name" CssClass="valboldblue" runat="server"></asp:Label>
                <asp:HiddenField ID="hidJ60ID" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="width: 120px;">
                <asp:Label ID="lblX29ID" Text="Modul:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x29ID" runat="server" DataTextField="x29Name" DataValueField="x29ID" AutoPostBack="true">                    
                    <asp:ListItem Text="Osobní (přístupné všem)" Value="103"></asp:ListItem>
                    <asp:ListItem Text="Worksheet" Value="331"></asp:ListItem>
                    <asp:ListItem Text="Projekty" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Klienti" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Faktury" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Zálohové faktury" Value="390"></asp:ListItem>
                    <asp:ListItem Text="Lidé" Value="102"></asp:ListItem>
                    <asp:ListItem Text="Tiskové sestavy" Value="931"></asp:ListItem>
                    <asp:ListItem Text="Úkoly" Value="356"></asp:ListItem>
                    <asp:ListItem Text="Dokumenty" Value="223"></asp:ListItem>
                    <asp:ListItem Text="Ceníky sazeb" Value="351"></asp:ListItem>
                    <asp:ListItem Text="Nástěnka" Value="210"></asp:ListItem>
                    <asp:ListItem Text="Operativní plánování" Value="348"></asp:ListItem>
                    <asp:ListItem Text="Administrace" Value="1"></asp:ListItem>
                </asp:DropDownList>

                <asp:Label ID="lblParentID" runat="server" CssClass="lbl" Text="Nadřízená menu položka:"></asp:Label>
                <uc:datacombo ID="j62ParentID" runat="server" DataTextField="TreeMenuItem" DataValueField="pid" IsFirstEmptyRow="true" Width="300px"></uc:datacombo>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="j62Name" runat="server" Style="width: 250px;"></asp:TextBox>

                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl"></asp:Label>
                <telerik:RadNumericTextBox ID="j62Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
                <asp:Label ID="Label2" runat="server" CssClass="lbl" Text="English:"></asp:Label>
                <asp:TextBox ID="j62Name_ENG" runat="server" Style="width: 250px;"></asp:TextBox>
                <asp:CheckBox ID="j62IsSeparator" runat="server" AutoPostBack="true" Text="Pouze oddělovač" />
            </td>
        </tr>


    </table>
    <table id="tabLink" runat="server" cellpadding="6" cellspacing="2">
       
        <tr style="vertical-align: top;">
            <td style="width: 120px;">
                <asp:Label ID="lblUrl" runat="server" CssClass="lblReq" Text="Odkaz (URL):"></asp:Label>
            </td>
            <td>

                <asp:DropDownList ID="cbxUrls" runat="server" AutoPostBack="true" Style="width: 300px;" DataTextField="x31Name" DataValueField="PersonalPageValue"></asp:DropDownList>
                <b>-></b>
                <asp:TextBox ID="j62Url" runat="server" Style="width: 300px;"></asp:TextBox>




            </td>
        </tr>
        <tr>
            <td>Tag:</td>
            <td>
                <asp:TextBox ID="j62Tag" runat="server"></asp:TextBox>
            </td>
        </tr>
        
        <tr>
            <td>
                <asp:Label ID="lblTarget" runat="server" CssClass="lbl" Text="Cíl odkazu:"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="j62Target" runat="server">
                    <asp:ListItem Text="Aktuální stránka" Value=""></asp:ListItem>
                    <asp:ListItem Text="Nová záložka" Value="_blank"></asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="lblImageUrl" runat="server" CssClass="lbl" Text="Grafická ikona (URL):"></asp:Label>
                <asp:TextBox ID="j62ImageUrl" runat="server" Style="width: 200px;"></asp:TextBox>

            </td>
        </tr>

    </table>
    <div class="content-box2">
        <div class="title">Pouze pro datový přehled</div>
        <div class="content">
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblJ70ID" runat="server" CssClass="lbl" Text="Pojmenovaný přehled:"></asp:Label>
                    </td>
                    <td>
                        <uc:datacombo ID="j70ID" runat="server" DataTextField="NameWithCreator" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
                    </td>
                </tr>
                
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Souhrny podle:"></asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="j62GridGroupBy" runat="server" DataTextField="ColumnHeader" DataValueField="ColumnField">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <asp:panel ID="panPerm" runat="server" cssclass="content-box2" Visible="false">
        <div class="title">
            <img src="Images/projectrole.png" width="16px" height="16px" />
            <asp:Label ID="ph1" runat="server" Text="Dodatečná oprávnění (navíc k modulu)"></asp:Label>
            <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
        </div>
        <div class="content">
            <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="j62MenuHome"></uc:entityrole_assign>

        </div>
    </asp:panel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
