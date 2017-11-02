<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="period.ascx.vb" Inherits="UI.period" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Label ID="lblCaption" runat="server" Text="Období:" CssClass="lbl"></asp:Label>
<asp:DropDownList ID="cbxX21ID" runat="server" AutoPostBack="true" DataTextField="x21Name" DataValueField="x21ID" style="width:130px;"></asp:DropDownList>
<asp:Label ID="lblFrom" Text="Od:" runat="server" CssClass="lbl"></asp:Label>
<telerik:RadDatePicker ID="datFrom" runat="server" RenderMode="Lightweight" Width="120px">
    <DateInput ID="DateInput1" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
</telerik:RadDatePicker>
<asp:Label ID="lblUntil" Text="Do:" runat="server" CssClass="lbl"></asp:Label>
<telerik:RadDatePicker ID="datUntil" runat="server" RenderMode="Lightweight" Width="120px">
    <DateInput ID="DateInput2" DisplayDateFormat="d.M.yyyy ddd" runat="server"></DateInput>
</telerik:RadDatePicker>