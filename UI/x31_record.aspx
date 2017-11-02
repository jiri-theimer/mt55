<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x31_record.aspx.vb" Inherits="UI.x31_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/pageheader.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload" Src="~/fileupload.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign" Src="~/entityrole_assign.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td>
                <uc:fileupload ID="upload1" runat="server" MaxFileUploadedCount="1" MaxFileInputsCount="1" InitialFileInputsCount="1" ButtonText_Add="Přidat" AllowedFileExtensions="trdx,rep,aspx,docx,xlsx" EntityX29ID="x31Report" />
            </td>
            <td>
                <uc:fileupload_list ID="uploadlist1" runat="server" />
            </td>
        </tr>
    </table>

    <div style="width: 100%; border-bottom: dashed 1px gray; margin-top: 6px; margin-bottom: 6px;"></div>

    <table cellpadding="5" cellspacing="2">
        <tr>
            <td colspan="2">
                <asp:RadioButtonList ID="x31FormatFlag" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" CellPadding="10">
                    <asp:ListItem Text="<img src='Images/report.png' style='margin-right:5px;'/>Tisková sestava [TRDX]" Value="1" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="<img src='Images/plugin.png' style='margin-right:5px;'/>Plugin [ASPX]" Value="3"></asp:ListItem>
                    <asp:ListItem Text="<img src='Images/doc.png' style='margin-right:5px;'/>Slučovací dokument [DOCX]" Value="2"></asp:ListItem>
                    <asp:ListItem Text="<img src='Images/xls.png' style='margin-right:5px;'/>XLS export [XLSX]" Value="4"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblX29ID" runat="server" CssClass="lbl" Text="Kontext sestavy (entita):"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x29ID" runat="server">
                    <asp:ListItem Text="Bez kontextu vybraného záznamu, v menu [DALŠÍ->Tiskové sestavy]" Value="" Selected="true"></asp:ListItem>
                    <asp:ListItem Text="Projekt" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Klient" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Osoba" Value="102"></asp:ListItem>
                    <asp:ListItem Text="Úkol" Value="356"></asp:ListItem>
                    <asp:ListItem Text="Daňová faktura" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Zálohová faktura" Value="390"></asp:ListItem>
                    <asp:ListItem Text="Úhrada zálohové faktury" Value="382"></asp:ListItem>
                    <asp:ListItem Text="Rozpočet projektu" Value="345"></asp:ListItem>
                    <asp:ListItem Text="Úhrada faktury" Value="394"></asp:ListItem>
                    <asp:ListItem Text="Worksheet úkon" Value="331"></asp:ListItem>                    
                    <asp:ListItem Text="Dokument" Value="223"></asp:ListItem>
                    <asp:ListItem Text="Schvalování" Value="999"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblName" runat="server" CssClass="lblReq" Text="Název šablony:"></asp:Label></td>
            <td>
                <asp:TextBox ID="x31Name" runat="server" Style="width: 400px;"></asp:TextBox>
                <asp:Label ID="lblOrdinary" Text="Index pořadí v rámci kategorie:" runat="server" CssClass="lbl" AssociatedControlID="x31Ordinary"></asp:Label>
                <telerik:RadNumericTextBox ID="x31Ordinary" runat="server" NumberFormat-DecimalDigits="0" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblCode" runat="server" CssClass="lblReq" Text="Kód šablony:"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x31Code" runat="server" Style="width: 200px;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblJ25ID" Text="Kategorie:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <uc:datacombo ID="j25ID" runat="server" DataTextField="j25Name" DataValueField="pid" IsFirstEmptyRow="true" Width="400px"></uc:datacombo>
            </td>
        </tr>   
        <tr>
            <td>
                <asp:Label ID="lblx31PluginFlag" Text="Typ pluginu:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x31PluginFlag" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="Plugin použitelný podobně jako tisková sestava"></asp:ListItem>
                    <asp:ListItem Text="Plugin pod menu na stránce projektu/klienta/osoby" Value="1"></asp:ListItem>
                    
                </asp:DropDownList>
                <telerik:RadNumericTextBox ID="x31PluginHeight" ToolTip="Výška v px." runat="server" IncrementSettings-Step="10" NumberFormat-DecimalDigits="0" Value="30" Width="50px" ShowSpinButtons="true"></telerik:RadNumericTextBox>
            </td>
        </tr>     
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="x31IsPeriodRequired" runat="server" Text="Sestava podporuje filtrování podle období" />
                
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Label1" Text="Vztah sestavy k pojmenovaným filtrům:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:DropDownList ID="x31QueryFlag" runat="server">
                    <asp:ListItem Text=""></asp:ListItem>
                    <asp:ListItem Text="Worksheet filtr" Value="331"></asp:ListItem>
                    <asp:ListItem Text="Filtr nad projekty" Value="141"></asp:ListItem>
                    <asp:ListItem Text="Filtr nad klienty" Value="328"></asp:ListItem>
                    <asp:ListItem Text="Filtr nad fakturami" Value="391"></asp:ListItem>
                    <asp:ListItem Text="Filtr nad úkoly" Value="356"></asp:ListItem>
                    <asp:ListItem Text="Filtr nad osobami" Value="102"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:CheckBox ID="x31IsUsableAsPersonalPage" runat="server" Text="Šablona je použitelná jako osobní (výchozí) stránka uživatele" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblx31ExportFileNameMask" Text="Maska export souboru:" runat="server" CssClass="lbl"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="x31ExportFileNameMask" runat="server" style="width:300px;"></asp:TextBox>
            </td>
        </tr>
    </table>

    <div class="content-box2">
        <div class="title">
            <img src="Images/projectrole.png" width="16px" height="16px" />
            <asp:Label ID="ph1" runat="server" Text="Přístupová práva k sestavě"></asp:Label>
            <asp:Button ID="cmdAddX69" runat="server" CssClass="cmd" Text="Přidat" />
        </div>
        <div class="content">
            <uc:entityrole_assign ID="roles1" runat="server" EntityX29ID="x31Report" EmptyDataMessage="K sestavě nejsou deffinována přístupová práva, proto bude přístupná pouze administrátorům."></uc:entityrole_assign>

        </div>
    </div>
    <asp:Panel ID="panDocFormat" runat="server" CssClass="content-box2">
        <div class="title">
            <img src="Images/doc_32.png" />
        </div>
        <div class="content">
            <span>Zdrojový SQL dotaz:</span>
            <asp:TextBox ID="x31DocSqlSource" runat="server" style="width:99%;height:200px;" TextMode="MultiLine"></asp:TextBox>
            <br />
            <span>SQL vnořených tabulek (Název oblasti|SQL dotaz + ENTER):</span>
            <asp:TextBox ID="x31DocSqlSourceTabs" runat="server" style="width:99%;height:100px;" TextMode="MultiLine"></asp:TextBox>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="panScheduling" runat="server" CssClass="content-box2">
        <div class="title">
            <img src="Images/calendar.png" />
            <asp:CheckBox ID="x31IsScheduling" runat="server" Text="Sestavu bude aplikace pravidelně odesílat mailem" AutoPostBack="true" />
        </div>
        <asp:panel ID="panIsScheduling" runat="server" CssClass="content">
           <div class="div6">
               <span>Den v týdnu:</span>
               <asp:CheckBox ID="x31IsRunInDay1" runat="server" Text="Pondělí" />
                <asp:CheckBox ID="x31IsRunInDay2" runat="server" Text="Úterý" />
                <asp:CheckBox ID="x31IsRunInDay3" runat="server" Text="Středa" />
                <asp:CheckBox ID="x31IsRunInDay4" runat="server" Text="Čtvrtek" />
                <asp:CheckBox ID="x31IsRunInDay5" runat="server" Text="Pátek" />
                <asp:CheckBox ID="x31IsRunInDay6" runat="server" Text="Sobota" />
                <asp:CheckBox ID="x31IsRunInDay7" runat="server" Text="Neděle" />
           </div>
           <div class="div6">
               <span>Čas generování (HH:MM):</span>
               <asp:TextBox ID="x31RunInTime" runat="server" Width="50px"></asp:TextBox>
               <span>Příjemci (e-mail) sestavy:</span>
               <asp:TextBox ID="x31SchedulingReceivers" runat="server" Width="500px"></asp:TextBox>
           </div>
            <div class="div6">
                <span>Kdy naposledy byla sestava generována:</span>
                <asp:Label ID="x31LastScheduledRun" runat="server" CssClass="valbold"></asp:Label>
                <asp:Button ID="cmdClearLastScheduledRun" runat="server" CssClass="cmd" Text="Vyčistit čas posl.generování" />
            </div>
        </asp:panel>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
