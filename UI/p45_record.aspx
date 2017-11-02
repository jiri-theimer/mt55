<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="p45_record.aspx.vb" Inherits="UI.p45_record" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panCreateClone" runat="server" CssClass="content-box2" Visible="false">
        <div class="title">
            Zkopírovat tuto verzi do nového rozpočtu            
        </div>
        <div class="content">
            
            <div>
                <asp:CheckBox ID="chkCloneP46" runat="server" Text="Zkopírovat limity hodin" Checked="true" />
            </div>
            <div>
                <asp:CheckBox ID="chkCloneP47" runat="server" Text="Zkopírovat kapacitní plán" Checked="true" />
            </div>
            <div>
                <asp:CheckBox ID="chkCloneP49" runat="server" Text="Zkopírovat finanční rozpočet" Checked="true" />
            </div>
        </div>
    </asp:Panel>
    <div style="padding:10px;">
        <asp:CheckBox ID="chkMakeCurrentAsFirstVersion" runat="server" Text="Nastavit tuto verzi jako [Aktuální]" />
    </div>
    <div style="padding:10px;">
        <asp:Label ID="lblVersionIndex" runat="server" Text="Verze rozpočtu:"></asp:Label>
        #
        <asp:Label ID="p45VersionIndex" runat="server" CssClass="valboldblue"></asp:Label>
       
    </div>
    <div style="padding: 10px;">
        <asp:Label ID="lblFrom" Text="Plánované zahájení:" runat="server" CssClass="lbl" Width="140px"></asp:Label>
        <telerik:RadDatePicker ID="p45PlanFrom" runat="server" Width="120px" SharedCalendarID="SharedCalendar">
            <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>            
        </telerik:RadDatePicker>
    </div>
    <div style="padding: 10px;">
        <asp:Label ID="lblUntil" Text="Plánované dokončení:" runat="server" CssClass="lbl" Width="140px"></asp:Label>
        <telerik:RadDatePicker ID="p45PlanUntil" runat="server" Width="120px" SharedCalendarID="SharedCalendar" MaxDate="1.1.3000">
            <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>            
        </telerik:RadDatePicker>
    </div>
    <div style="padding: 10px;">
        <asp:Label ID="Label2" Text="Název/popis:" runat="server" CssClass="lbl" Width="140px"></asp:Label>
        <asp:TextBox ID="p45Name" runat="server" Width="400px"></asp:TextBox>
    </div>
    
    <asp:HiddenField ID="hidClonePID" runat="server" />
    <asp:HiddenField ID="hidP41ID" runat="server" />
    <telerik:RadCalendar ID="SharedCalendar" runat="server" EnableMultiSelect="False" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False">

        <SpecialDays>
            <telerik:RadCalendarDay Repeatable="Today" ItemStyle-BackColor="SkyBlue"></telerik:RadCalendarDay>
        </SpecialDays>
    </telerik:RadCalendar>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
