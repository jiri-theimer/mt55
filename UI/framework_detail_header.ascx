<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="framework_detail_header.ascx.vb" Inherits="UI.framework_detail_header" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<div style="height: 40px;">
    <table cellpadding="6">
        <tr>
            <td>
                <asp:Image ID="imgEntity" runat="server" ImageUrl="Images/project_32.png" />
            </td>
            <td>
                <asp:HyperLink ID="cmdLevel1" runat="server" Text="Klient" CssClass="page_header_span"></asp:HyperLink>
            </td>
            <td style="width: 20px; text-align: center;">->
            </td>
            <td>
                <asp:HyperLink ID="cmdLevel2" runat="server" Text="Projekt" CssClass="page_header_span"></asp:HyperLink>
            </td>

        </tr>
    </table>

</div>
<div style="height: 3px; page-break-after: always"></div>
<telerik:RadTabStrip ID="tabs1" runat="server" ShowBaseLine="true" Width="100%" Skin="Metro">
    <Tabs>
        <telerik:RadTab Text="Pracovní plocha" Value="detail"></telerik:RadTab>
        <telerik:RadTab Text="Schvalování (příprava k fakturaci)" Value="approve" Selected="true"></telerik:RadTab>
        <telerik:RadTab Text="Časová osa" Value="timeline"></telerik:RadTab>
        <telerik:RadTab Text="Úkoly a termíny" Value="tasks"></telerik:RadTab>
    </Tabs>
</telerik:RadTabStrip>

