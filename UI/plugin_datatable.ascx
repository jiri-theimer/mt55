<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="plugin_datatable.ascx.vb" Inherits="UI.plugin_datatable" %>
<asp:PlaceHolder ID="place1" runat="server"></asp:PlaceHolder>
<asp:HiddenField ID="hidColHeaders" runat="server" />
<asp:HiddenField ID="hidColTypes" runat="server" />
<asp:HiddenField ID="hidColFlexSubtotals" runat="server" />
<asp:HiddenField ID="hidIsShowGrandTotals" runat="server" Value="1" />
<asp:HiddenField ID="hidIsHideRepeatedGroupValues" runat="server" Value="1" />
<asp:HiddenField ID="hidColHideRepeatedValues" runat="server" Value="0" />
<asp:HiddenField ID="hidTableCaption" runat="server" />
<asp:HiddenField ID="hidTableID" runat="server" />
<asp:HiddenField ID="hidTableCssClass" runat="server" />
<asp:HiddenField ID="hidFormatDate" runat="server" />
<asp:HiddenField ID="hidFormatDateTime" runat="server" />
<asp:HiddenField ID="hidFormatNumber" runat="server" />
<asp:HiddenField ID="hidErrorMessage" runat="server" />
<asp:HiddenField ID="hidRowsCount" runat="server" Value="0" />
<asp:HiddenField ID="hidNoDataMessage" runat="server" value="Žádná data." />



