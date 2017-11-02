<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x48_record.aspx.vb" Inherits="UI.x48_record" %>
<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <asp:RadioButtonList ID="x48TaskOutputFlag" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                    <asp:ListItem Text="Spustit SQL dotaz" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="SQL dotaz vrací ID záznamů" Value="2"></asp:ListItem>                    
                </asp:RadioButtonList>
            </td>
            <td>
                <asp:Label ID="lblX29ID" runat="server" CssClass="lbl" Text="Kontext:"></asp:Label>
                <asp:DropDownList ID="x29ID" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="" Value="" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>                                                                 
                </asp:DropDownList>
            </td>
            
        </tr>
     
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název úlohy:"></asp:Label>
            
                <asp:TextBox ID="x48Name" runat="server" Style="width: 400px;"></asp:TextBox>
            </td>
            <td>
                <asp:Label ID="lblOrdinary" Text="Index pořadí:" runat="server" CssClass="lbl" AssociatedControlID="x48Ordinary"></asp:Label>
                <telerik:RadNumericTextBox ID="x48Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblX31ID" runat="server" CssClass="lbl" Text="Tisková sestava:"></asp:Label>
                <asp:DropDownList ID="x31ID" runat="server" DataValueField="pid" DataTextField="x31Name"></asp:DropDownList>
            </td>
        </tr>
    </table>
    <div class="content-box2">
        <div class="title">Sql dotaz</div>
        <div class="content">
            <asp:TextBox ID="x48Sql" runat="server" Width="98%" TextMode="MultiLine" Height="300px"></asp:TextBox>
        </div>
    </div>
    <asp:Panel ID="panScheduling" runat="server" CssClass="content-box2">
        <div class="title">
            <img src="Images/calendar.png" />
            <span>Kdy spouštět úlohu</span>
        </div>
        <div class="content">
           <div class="div6">
               
               <asp:CheckBox ID="x48IsRunInDay1" runat="server" Text="Pondělí" />
                <asp:CheckBox ID="x48IsRunInDay2" runat="server" Text="Úterý" />
                <asp:CheckBox ID="x48IsRunInDay3" runat="server" Text="Středa" />
                <asp:CheckBox ID="x48IsRunInDay4" runat="server" Text="Čtvrtek" />
                <asp:CheckBox ID="x48IsRunInDay5" runat="server" Text="Pátek" />
                <asp:CheckBox ID="x48IsRunInDay6" runat="server" Text="Sobota" />
                <asp:CheckBox ID="x48IsRunInDay7" runat="server" Text="Neděle" />
           </div>
           <div class="div6">
               <span>Čas generování (HH:MM):</span>
               <asp:TextBox ID="x48RunInTime" runat="server" Width="50px"></asp:TextBox>
                              
           </div>
            <div class="div6">
                <span>Kdy naposledy byla sestava generována:</span>
                <asp:Label ID="x48LastScheduledRun" runat="server" CssClass="valbold"></asp:Label>
                <asp:Button ID="cmdClearLastScheduledRun" runat="server" CssClass="cmd" Text="Vyčistit čas posl.generování" Visible="false" />
            </div>
        </div>
    </asp:Panel>
    <div class="content-box2" style="margin-top:10px;">
    <div class="title">
        <img src="Images/email.png" />
        Notifikační e-mail zpráva
    </div>
    <div class="content">
    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                E-mail příjemci:
            </td>
            <td>
                <asp:TextBox ID="x48MailTo" runat="server" Width="500px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                Předmět zprávy:
            </td>
            <td>
                <asp:TextBox ID="x48MailSubject" runat="server" Width="500px"></asp:TextBox>
            </td>
        </tr>
        <tr valign="top">
            <td>
                Obsah zprávy:
            </td>
            <td>
                <asp:TextBox ID="x48MailBody" runat="server" Width="500px" TextMode="MultiLine" Height="100px"></asp:TextBox>
            </td>
        </tr>
    </table>
    </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
