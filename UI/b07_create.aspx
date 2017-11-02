<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="b07_create.aspx.vb" Inherits="UI.b07_create" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="fileupload" Src="~/fileupload.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="person_or_team" Src="~/person_or_team.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function b07_delete(pid) {
            location.replace("b07_delete.aspx?pid=" + pid);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">


    <table>
        <tr>
            <td>
                <uc:fileupload ID="upload1" runat="server" MaxFileInputsCount="1" EntityX29ID="b07Comment" />
            </td>
            <td>
                <uc:fileupload_list ID="uploadlist1" runat="server" />
            </td>
        </tr>
    </table>
    <p></p>
    <asp:TextBox ID="b07Value" runat="server" TextMode="MultiLine" Style="width: 100%; height: 100px; font-family: 'Courier New';" ToolTip="Zapsaný komentář"></asp:TextBox>

    <div class="content-box2">
        <div class="title">
            <img src="Images/email.png" />
            Komu poslat notifikaci
            <asp:Button ID="cmdAddReceiver" runat="server" CssClass="cmd" Text="Přidat příjemce" />
        </div>
        <div class="content">
            <uc:person_or_team id="receiver1" runat="server"></uc:person_or_team>
        </div>
    </div>
   


    <uc:b07_list ID="history1" runat="server" ShowInsertButton="false" IsClueTipInfo="true" />

    <asp:HiddenField ID="hidPrefix" runat="server" />
    <asp:HiddenField ID="hidRecordPID" runat="server" />
    <asp:HiddenField ID="hidParentID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
