<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SubForm.Master" CodeBehind="o23_framework_detail.aspx.vb" Inherits="UI.o23_framework_detail" %>

<%@ MasterType VirtualPath="~/SubForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="b07_list" Src="~/b07_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="entityrole_assign_inline" Src="~/entityrole_assign_inline.ascx" %>
<%@ Register TagPrefix="uc" TagName="o23_record_readonly" Src="~/o23_record_readonly.ascx" %>

<%@ Register TagPrefix="uc" TagName="mytags" Src="~/mytags.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">

        function hardrefresh(pid, flag) {              
            <%If hidSource.Value <> "" Then%>
            var url="o23_framework.aspx?source=<%=hidSource.Value%>";
            <%Else%>
            var url="o23_fixwork.aspx?x18id=<%=me.CurrentX18ID%>";
            <%End If%>
            if (flag == "o23-delete") {
                window.open(url,"_top");
                return;
            }
            if (flag == "o23-save") {                
                window.open(url+"&pid="+pid,"_top");
                return;
            }
            
            if (flag != "o23-save") {
                pid=<%=Master.DataPID%>;
                
            }

            location.replace("o23_framework_detail.aspx?x18id=<%=me.CurrentX18ID%>&pid="+pid);

        }


        function b07_reaction(b07id) {
            sw_everywhere("b07_create.aspx?parentpid=" + b07id + "&masterprefix=o23&masterpid=<%=Master.DataPID%>", "Images/comment.png", true)

        }
        function b07_delete(b07id, flag) {
            sw_everywhere("b07_delete.aspx?pid=" + b07id, "Images/delete.png", true)

        }

        function menu_b07_record() {
            sw_everywhere("b07_create.aspx?masterprefix=o23&masterpid=<%=Master.DataPID%>", "Images/comment.png", true);
        }

        function record_edit() {
            window.parent.sw_everywhere("o23_record.aspx?x18id=<%=Me.CurrentX18ID%>&pid=<%=Master.DataPID%>", "Images/report.png", true);
        }
        function record_create() {
            window.parent.sw_everywhere("o23_record.aspx?x18id=<%=Me.CurrentX18ID%>&pid=0", "Images/report.png", true);
        }
        function record_clone() {
            window.parent.sw_everywhere("o23_record.aspx?clone=1&x18id=<%=Me.CurrentX18ID%>&pid=<%=Master.DataPID%>", "Images/report.png", true);
        }

        function report() {            

            window.parent.sw_everywhere("report_modal.aspx?prefix=o23&pid=<%=Master.DataPID%>", "Images/report.png", true);

        }
        function b07_create() {           
            sw_everywhere("b07_create.aspx?masterprefix=o23&masterpid=<%=Master.DataPID%>", "Images/comment.png", true);

        }
        function b07_create_upload() {           
            sw_everywhere("b07_create.aspx?masterprefix=o23&masterpid=<%=Master.DataPID%>&forceupload=1", "Images/comment.png", true);

        }
        function workflow() {         
            sw_everywhere("workflow_dialog.aspx?prefix=o23&pid=<%=Master.DataPID%>", "Images/workflow.png", true);
        }
        function sendmail() {
            sw_everywhere("sendmail.aspx?prefix=o23&pid=<%=Master.DataPID%>", "Images/email.png")


        }
        function plugin() {
            
            sw_everywhere("plugin_modal.aspx?prefix=o23&pid=<%=Master.DataPID%>&x18id=<%=hidX18ID.Value%>","Images/plugin.png",true);

        }
        function menu_fullscreen(){
            <%If hidSource.Value = "3" Then%>
            location.replace("o23_framework.aspx?pid=<%=Master.DataPID%>");
            <%Else%>
            window.open("o23_framework_detail.aspx?pid=<%=Master.DataPID%>&saw=1","_blank");
            <%End If%>
            
        }
        function barcode() {
            sw_everywhere("barcode.aspx?prefix=o23&pid=<%=master.datapid%>", "Images/barcode.png", true);
        }
        function file_preview(prefix,pid) {
            ///náhled na soubor            
            sw_everywhere("fileupload_preview.aspx?prefix="+prefix+"&pid="+pid,"Images/attachment.png",true);
            
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Panel ID="panPM1" runat="server" CssClass="entity_menu_cm">
        <table style="padding: 0px; width: 100%;">
            <tr>
                <td style="width: 40px;">
                    <asp:HyperLink ID="pm1" runat="server" CssClass="pp2"></asp:HyperLink>
                </td>
                <td>
                    <asp:HyperLink ID="linkPM" runat="server" CssClass="entity_menu_header" NavigateUrl="#"></asp:HyperLink>
                </td>

                <td style="float: right; width: 40px;">

                    <asp:Image ID="imgIcon32" runat="server" ImageUrl="Images/notepad_32.png" />
                </td>
            </tr>
        </table>
    </asp:Panel>







    <div style="clear: both;"></div>
    <asp:Panel ID="panEncrypted" runat="server" CssClass="div6" Visible="false">
        <p class="infoNotificationRed">Obsah dokumentu je zašifrován.</p>
        <span class="lbl">Zadejte heslo:</span>
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"></asp:TextBox>
        <asp:Button ID="cmdDecrypt" runat="server" CssClass="cmd" Text="Odšifrovat" />
    </asp:Panel>

    <div class="div6">
        <uc:o23_record_readonly ID="rec1" runat="server" />
    </div>
    <div class="div6" style="border-top: dashed 1px silver;">
        <uc:entityrole_assign_inline ID="roles1" runat="server" EntityX29ID="o23Doc" NoDataText=""></uc:entityrole_assign_inline>

        <uc:mytags ID="tags1" Prefix="o23" runat="server" />
    </div>


    <asp:Panel ID="panUpload" runat="server" CssClass="innerform_light">

        <img src="Images/attachment.png" style="margin-right: 10px;" />
        <asp:HyperLink ID="filesPreview" runat="server" Text="Přílohy dokumentu"></asp:HyperLink>
        <button type="button" onclick="b07_create_upload()" runat="server" id="cmdUpload">Nahrát přílohy</button>
        <asp:Button ID="cmdLockUnlock" runat="server" Text="Uzamknout přístup k přílohám" CssClass="cmd" Style="display: none;" />
    </asp:Panel>

    <div style="clear: both; margin-top: 20px;">
        <uc:b07_list ID="comments1" runat="server" JS_Create="b07_create()" JS_Reaction="b07_reaction" ShowInsertButton="false" />
    </div>



    <asp:HiddenField ID="hidX18ID" runat="server" />
    <asp:HiddenField ID="hidB01ID" runat="server" />
    <asp:HiddenField ID="hidSource" runat="server" />


</asp:Content>
