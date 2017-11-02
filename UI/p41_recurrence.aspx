<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p41_recurrence.aspx.vb" Inherits="UI.p41_recurrence" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <div class="content-box2" style="margin-top:20px;">
        <div class="title">            
            Matka (šablona), která opakovaně rodí instance tohoto projektu
        </div>
        <div class="content">
            <div class="div6">
                <span>Typ opakování:</span>
                <asp:DropDownList ID="p65ID" runat="server" DataTextField="NameWithFlag" DataValueField="pid" AutoPostBack="true"></asp:DropDownList>
                <asp:CheckBox ID="p41IsStopRecurrence" Text="Automatika pozastavena" runat="server" />
            </div>
            <asp:Panel ID="panRecurrence" runat="server">
                <div class="div6">
                    <span>Maska názvu nových projektů:</span>
                    <asp:TextBox ID="p41RecurNameMask" runat="server" Width="300px"></asp:TextBox>
                </div>
                <div class="div6">
                    <span>Rozhodné datum tohoto projektu:</span>
                    <telerik:RadDateInput ID="p41RecurBaseDate" runat="server" DisplayDateFormat="d.M.yyyy" DateFormat="d.M.yyyy" Width="100px"></telerik:RadDateInput>
                    <div>
                        <i>U měsíčního opakování musí být datum vždy první den v měsíci.</i>
                    </div>
                    <div>
                        <i>U čtvrtletního opakování musí být datum první den kvartálu, tedy jedno z následujících: 1.1., 1.4., 1.7., 1.9.</i>
                    </div>                    
                    <div>
                        <i>U ročního opakování musí být datum vždy první den v roce.</i>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>


</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
