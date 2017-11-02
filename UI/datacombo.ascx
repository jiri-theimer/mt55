<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="datacombo.ascx.vb" Inherits="UI.datacombo" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<telerik:RadComboBox ID="cbx1" runat="server" DataValueField="pid">
    <Localization AllItemsCheckedString="Všechny položky zaškrtnuty" ItemsCheckedString="x zaškrtnuto" />
</telerik:RadComboBox>
<asp:HiddenField ID="hidIsEmptyFirstRow" runat="server" />
