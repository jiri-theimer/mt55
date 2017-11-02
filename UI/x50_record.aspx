<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalDataRecord.Master" CodeBehind="x50_record.aspx.vb" Inherits="UI.x50_record" %>

<%@ MasterType VirtualPath="~/ModalDataRecord.Master" %>
<%@ Register TagPrefix="uc" TagName="pageheader" Src="~/PageHeader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="fileupload" Src="~/fileupload.ascx" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table cellpadding="3" cellspacing="2">
        <tr>
            <td>
                <asp:Label ID="lblX50Name" Text="Název nápovědy:" runat="server" CssClass="lblReq" AssociatedControlID="x50Name"></asp:Label></td>
            <td>
                <asp:TextBox ID="x50Name" runat="server" Style="width: 400px;"></asp:TextBox>

            </td>
            <td>
                <asp:Label ID="lblx50AspxPage" Text="Název ASPx stránky:" runat="server" CssClass="lblReq" AssociatedControlID="x50AspxPage"></asp:Label></td>
            <td>
                <asp:TextBox ID="x50AspxPage" runat="server" Style="width: 400px;"></asp:TextBox>

            </td>
        </tr>


    </table>



    <telerik:RadEditor ID="BodyHtml" ForeColor="Black" Font-Names="MS Sans Serif" Font-Size="10pt" runat="server" Skin="Metro" Width="98%" Height="800px" ToolbarMode="Default" Language="cs-CZ" ContentAreaMode="Div">

        <Content>
     Obsah nápovědy
        </Content>
        <Tools>
            <telerik:EditorToolGroup>
                <telerik:EditorTool Name="Print" />
                <telerik:EditorTool Name="ToggleScreenMode" />
                <telerik:EditorSeparator />

                <telerik:EditorTool Name="Undo" />
                <telerik:EditorTool Name="Redo" />
                <telerik:EditorSeparator />

                <telerik:EditorTool Name="Cut" />
                <telerik:EditorTool Name="Copy" />
                <telerik:EditorTool Name="Paste" />
                <telerik:EditorSeparator />

                <telerik:EditorTool Name="FontName" />
                <telerik:EditorTool Name="FontSize" />

                <telerik:EditorTool Name="Underline" />
                <telerik:EditorTool Name="Bold" />
                <telerik:EditorTool Name="Italic" />
                <telerik:EditorTool Name="ForeColor" />
                <telerik:EditorTool Name="BackColor" />
                <telerik:EditorSeparator />

                <telerik:EditorTool Name="InsertUnorderedList" />
                <telerik:EditorTool Name="InsertOrderedList" />

                <telerik:EditorTool Name="InsertParagraph" />
                <telerik:EditorTool Name="Indent" />
                <telerik:EditorTool Name="Outdent" />


                <telerik:EditorTool Name="InsertHorizontalRule" />

                <telerik:EditorTool Name="InsertTable" />
                <telerik:EditorTool Name="InsertLink" />
                <telerik:EditorTool Name="InsertImage"></telerik:EditorTool>

                <telerik:EditorSeparator />
                <telerik:EditorTool Name="PasteFromWord" />
                <telerik:EditorTool Name="PasteFromWordNoFontsNoSizes" />
                <telerik:EditorTool Name="PasteHtml" />


            </telerik:EditorToolGroup>
        </Tools>

    </telerik:RadEditor>

    <div class="div6">
        <asp:Label ID="lblx50ExternalURL" Text="URL na externí nápovědu:" runat="server" CssClass="lbl" AssociatedControlID="x50ExternalURL"></asp:Label>
        <asp:TextBox ID="x50ExternalURL" runat="server" Style="width: 600px;"></asp:TextBox>

    </div>

    <fieldset>
        <legend>Souborové přílohy</legend>
        <table cellpadding="5" cellspacing="2">
            <tr>
                <td>
                    <uc:fileupload ID="upload1" runat="server" ButtonText_Add="Přidat" InitialFileInputsCount="0" EntityX29ID="x50Help" />
                </td>
                <td>
                    <uc:fileupload_list ID="upl1" runat="server" />
                </td>
            </tr>
        </table>
    </fieldset>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>

