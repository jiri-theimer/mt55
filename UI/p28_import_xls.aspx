<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="p28_import_xls.aspx.vb" Inherits="UI.p28_import_xls" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="fileupload" Src="~/fileupload.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="panStep1" runat="server">
        <table>
            <tr valign="top">
                <td>
                    <uc:fileupload ID="upload1" runat="server" EntityX29ID="o23Notepad" InitialFileInputsCount="1" AllowedFileExtensions="xls,xlsx"  MaxFileInputsCount="1"></uc:fileupload>
                </td>
                <td>
                    <uc:fileupload_list id="upl1" runat="server"></uc:fileupload_list>
                </td>
            </tr>
        </table>
    </asp:Panel>

</asp:Content>
