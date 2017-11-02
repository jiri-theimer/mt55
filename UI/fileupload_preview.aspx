<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="fileupload_preview.aspx.vb" Inherits="UI.fileupload_preview" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register TagPrefix="uc" TagName="fileupload_list" Src="~/fileupload_list.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
       


        $(document).ready(function () {
            document.getElementById("if1").src = "<%=ViewState("url")%>";
            
            
            
        });

        
        function download2(s) {

            location.replace("binaryfile.aspx?" + s);
            return (false);

        }

      
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <asp:panel ID="panList" runat="server" cssclass="content-box2" Visible="false">
        <div class="title">
            <img src="Images/attachment.png" style="margin-right: 10px;" />
            Přílohy dokumentu
        </div>
        <div class="content">
            <uc:fileupload_list ID="list1" runat="server" Target4Preview="if1" IsRepeatDirectionVerticaly="false" />
        </div>
    </asp:panel>
    <div id="offsetY"></div>
    <iframe id="if1" name="if1" width="99%" frameborder="0" style="position: absolute; height: <%=ViewState("if1_height")%>; border: none;"></iframe>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
