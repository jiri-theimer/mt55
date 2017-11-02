<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="fileupload_list.ascx.vb" Inherits="UI.fileupload_list" %>
<asp:Label ID="lblListHeader" runat="server" CssClass="framework_header_span" Visible="false"></asp:Label>
<asp:DataList ID="rp1" runat="server" RepeatDirection="Vertical">
    <ItemTemplate>
        <asp:Image ID="img1" runat="server" AlternateText="File format" ImageUrl="Images/Files/other.png" Style="vertical-align: bottom;" />

        <asp:ImageButton ID="cmdDownload" runat="server" ImageUrl="Images/download.png" CssClass="button-link" ToolTip="Stáhnout soubor/dokument" />


        <asp:HyperLink ID="aPreview" runat="server" ToolTip="Náhled na soubor/dokument"></asp:HyperLink>
        <asp:HyperLink ID="cmdPdfExport" runat="server" Text="PDF" ToolTip="PDF export dokumentu" Target="_blank" ForeColor="red" Font-Bold="true"></asp:HyperLink>
        <asp:Label ID="FileSize" runat="server"></asp:Label>

        <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" ToolTip="Odstranit" CommandName="delete" CssClass="button-link" />
        <asp:Label ID="version" runat="server" CssClass="badge1" Text="1"></asp:Label>

        <asp:Label ID="UserInsert" runat="server" Style="color: gray;"></asp:Label>
        <asp:Label ID="DateInsert" runat="server" Style="color: Red;"></asp:Label>



        <i style="margin-left: 6px;">
            <asp:HyperLink ID="FileTitle" runat="server" Style="color: black;" Text="Bez názvu/popisu"></asp:HyperLink>

        </i>
        
    </ItemTemplate>
</asp:DataList>


<asp:HiddenField ID="hidAllowEdit" runat="server" Value="1" />
<asp:HiddenField ID="hidGUID" runat="server" Value="" />
<asp:LinkButton ID="cmdRefreshOnBehind" runat="server" Style="display: none;"></asp:LinkButton>
<asp:HiddenField ID="hidUpdateTitle" runat="server" Value="" />
<asp:HiddenField ID="hidShowDateInsert" runat="server" Value="1" />

<asp:HiddenField ID="hidTarget4DispositionInline" runat="server" Value="_blank" />
<asp:HiddenField ID="hidOnClientClickPreview" runat="server" />
<asp:HiddenField ID="hidLockFlag" runat="server" />
<script type="text/javascript">
    function edittitle(p85id, o27id, strDef) {

        var s = window.prompt("Zadejte nový název/popis přílohy", strDef);

        if (s === null) {
            return;
        }

        document.getElementById("<%=hidUpdateTitle.clientid%>").value = p85id + "||" + o27id + "||" + s;
        document.forms['form1'].submit();

    }

    function download(s) {

        location.replace("binaryfile.aspx?" + s);
        return (false);

    }
</script>
