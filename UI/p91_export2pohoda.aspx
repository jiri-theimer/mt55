<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_export2pohoda.aspx.vb" Inherits="UI.p91_export2pohoda" %>
<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="periodcombo" Src="~/periodcombo.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="div6">
        <asp:Label ID="lblIC" runat="server" CssClass="lbl" Text="IČ účetní jednotky:"></asp:Label>
        <asp:textbox ID="txtIC" runat="server"></asp:textbox>
    </div>
    <div class="div6">
        <asp:Label ID="Label1" runat="server" CssClass="lbl" Text="Text faktury omezovat na maximálně:"></asp:Label>
        <asp:TextBox ID="txtMaxTextSize" runat="server" Text="240"></asp:TextBox>
        <span>znaků</span>
    </div>
    <div class="div6">
        U položek faktury doplňovat předkontaci:
        <asp:TextBox ID="txtPredkontace" runat="server" Text="1Fv"></asp:TextBox>
    </div>
    
    <fieldset>
        <legend>Hromadné generování faktur podle období zdanitelného plnění</legend>

        <div class="div6">
            <uc:periodcombo ID="period1" runat="server" Width="300px"></uc:periodcombo>
        </div>
        <div class="div6">
            <span>Vystavovatel faktury:</span>
            <asp:DropDownList ID="p93ID" runat="server" DataValueField="pid" DataTextField="p93Name"></asp:DropDownList>
        </div>
        <p>Z hromadného generování jsou automaticky vyloučeny DRAFT faktury.</p>
        <div class="div6">
            <asp:Button ID="cmdGenerateBatch" runat="server" Text="Vygenerovat hromadně pro zvolené období a vystavovatele" CssClass="cmd" />
        </div>
    </fieldset>
    


</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
