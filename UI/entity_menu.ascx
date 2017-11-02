<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="entity_menu.ascx.vb" Inherits="UI.entity_menu" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Panel ID="panPM1" runat="server" CssClass="entity_menu_cm">
    <table style="padding: 0px; width: 100%;">
        <tr>
            <td style="width: 40px;">
                <asp:HyperLink ID="pm1" runat="server" CssClass="pp2"></asp:HyperLink>
            </td>
            <td>
                <asp:HyperLink ID="linkPM" runat="server" CssClass="entity_menu_header" NavigateUrl="#"></asp:HyperLink>
                <button type="button" id="cmdGo2Grid" runat="server"  onclick="go2grid()" style="font-size:10px;">
                    Zpět do přehledu
                </button>
            </td>
            <td style="text-align: right;">
                <button type="button" onclick="entity_menu_reload()" style="font-size:10px;">
                    Obnovit stránku
                </button>
                <button type="button" id="cmdPageSetup" runat="server"  onclick="page_setting()" style="font-size:10px;">
                    Nastavení stránky                    
                </button>
            </td>
            <td style="float: right; width: 40px;">

                <asp:Image ID="imgPM" runat="server" EnableViewState="false" />
            </td>
        </tr>
    </table>




</asp:Panel>


<asp:PlaceHolder ID="place0" runat="server" Visible="true"></asp:PlaceHolder>
<asp:PlaceHolder ID="place1" runat="server" Visible="true"></asp:PlaceHolder>

<telerik:RadTabStrip ID="tabs1" runat="server" Skin="Default" Width="100%" AutoPostBack="false" ShowBaseLine="true" EnableViewState="false" OnClientTabSelected="OnClientTabSelected">
    <TabTemplate>
        <button type="button" onclick="lockTabs()" id="cmdLock" runat="server" style="z-index:1;width:16px;height:16px;padding:0px;" title="Ukotvit vybranou záložku"><img src="Images/lock_10.png" /></button>
        <asp:HyperLink runat="server" ID="link1" CssClass="entity_menu_tablink" Text='<%# DataBinder.Eval(Container, "Text") %>' NavigateUrl='<%# DataBinder.Eval(Container, "NavigateUrl") %>'></asp:HyperLink>
        
        
    </TabTemplate>
</telerik:RadTabStrip>


<asp:HiddenField ID="hidIsCanApprove" runat="server" />
<asp:HiddenField ID="hidSource" runat="server" />
<asp:HiddenField ID="hidParentWidth" runat="server" />
<asp:HiddenField ID="hidDataPrefix" runat="server" />
<asp:HiddenField ID="hidPlugin" runat="server" />
<asp:HiddenField ID="hidPlugin_FileName" runat="server" />
<asp:HiddenField ID="hidPlugin_Height" runat="server" />
<asp:HiddenField ID="hidLockedTab" runat="server" />

<script type="text/javascript">
    function entity_menu_reload(){
        var source=document.getElementById("<%=hidSource.ClientID%>").value;
        location.replace("<%=Me.DataPrefix%>_framework_detail.aspx?pid=<%=Me.DataPID%>&source="+source);
    }
    function go2grid(){
        var source=document.getElementById("<%=hidSource.ClientID%>").value;
        location.replace("<%=Me.DataPrefix%>_framework.aspx?pid=<%=Me.DataPID%>&source="+source);
    }
    function OnClientTabSelected(sender, eventArgs)
    {
        var tab = eventArgs.get_tab();
        var attributes = tab.get_attributes();
        var url = attributes.getAttribute( "myurl");
        
        location.replace(url);
    }

    function cbxSearch_OnClientSelectedIndexChanged(sender, eventArgs){
        var combo = sender;
        var pid = combo.get_value();
        var source=document.getElementById("<%=hidSource.ClientID%>").value;
        location.replace("<%=Me.DataPrefix%>_framework_detail.aspx?pid=" + pid+"&source="+source);
    }
    function cbxSearch_OnClientItemsRequesting(sender, eventArgs){
        var context = eventArgs.get_context();
        var combo = sender;

        if (combo.get_value() == "")
            context["filterstring"] = eventArgs.get_text();
        else
            context["filterstring"] = "";

        
        context["j03id"] = "<%=Factory.SysUser.PID%>";
        context["flag"] = "searchbox";
        <%If Me.DataPrefix = "p41" Then%>
        context["j02id_explicit"]="<%=Factory.SysUser.j02ID%>";
        <%End If%>
    }
    
    function report() {
            
        sw_decide("report_modal.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=Me.DataPID%>","Images/reporting.png",true);

    }
    function plugin() {            
        sw_decide("plugin_modal.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=Me.DataPID%>","Images/plugin.png",true);
    }
    function tags_menu() {                    
        sw_decide("tag_binding.aspx?prefix=<%=Me.DataPrefix%>&pids=<%=Me.DataPID%>", "", true);
    }

    function sw_decide(url, iconUrl, is_maximize) {
        var w=window.innerWidth;      
        
        if (w<1100){
            window.parent.sw_master(url, iconUrl, is_maximize);
            return;
        }

        <%If Me.hidSource.Value = "2" Then%>
        window.parent.sw_master(url, iconUrl, is_maximize);
        <%Else%>
        sw_local(url, iconUrl, is_maximize);
        <%End If%>
        
    }
        
        
        
    function p31_entry_menu(p34id) {
        ///z menu1
        <%If Me.DataPrefix = "p41" Then%>
        sw_decide("p31_record.aspx?pid=0&p41id=<%=Me.DataPID%>&p34id="+p34id,"Images/worksheet.png",true);
        <%End If%>
        <%If Me.DataPrefix = "p56" Then%>
        sw_decide("p31_record.aspx?pid=0&p56id=<%=Me.DataPID%>&p34id="+p34id,"Images/worksheet.png",true);
        <%End If%>
        

    }

    function record_edit() {
        var pid = <%=Me.DataPID%>;
        if (pid == "" || pid == null) {
            alert("Není vybrán záznam.");
            return
        }
        sw_decide("<%=Me.DataPrefix%>_record.aspx?pid=" + pid,"Images/project.png",true);

    }
        
        
    function record_clone() {
        var pid = <%=Me.DataPID%>;
        if (pid == "" || pid == null) {
            alert("Není vybrán záznam.");
            return
        }
        <%If Me.DataPrefix = "p41" Then%>
        sw_decide("p41_create.aspx?clone=1&pid=" + pid,"Images/project.png",true);
        <%Else%>
        sw_decide("<%=Me.DataPrefix%>_record.aspx?clone=1&pid=" + pid,"Images/copy.png",true);
        <%End If%>

    }
    function record_new() {
        <%If Me.DataPrefix = "p41" Then%>
        sw_decide("p41_create.aspx?client_family=1&pid=<%=Me.DataPID%>","Images/project.png",true);
        <%End If%>
        <%If Me.DataPrefix = "p28" Then%>
        sw_decide("p28_record.aspx?pid=0","Images/contact.png",true);
        <%End If%>
        <%If Me.DataPrefix = "j02" Then%>
        sw_decide("j02_record.aspx?pid=0","Images/person.png",true);
        <%End If%>       
        <%If Me.DataPrefix = "o23" Then%>
        sw_decide("select_doctype.aspx?masterprefix=<%=Me.DataPrefix%>&masterpid=<%=Me.DataPID%>","Images/notepad.png",true);
        <%End If%>

    }
    function record_new_child(){
        var pid = <%=Me.DataPID%>;
        sw_decide("p41_create.aspx?client_family=1&pid=<%=Me.DataPID%>&create_parent=1","Images/project.png",true);
    }
    function p28_p41_new() {
            
        sw_decide("p41_create.aspx?p28id=<%=Me.DataPID%>","Images/project.png",true);

    }
    function p31_recurrence_record(pid) {
        sw_decide("p31_record.aspx?pid=" + pid, "Images/worksheet.png");

    }
    
    function page_setting(){
        sw_decide("entity_framework_detail_setting.aspx?prefix=<%=Me.DataPrefix%>", "Images/setting.png",false);
    }
    
        
    function draft2normal() {

        if (confirm("Převést záznam z režimu DRAFT?")) {
            hardrefresh(<%=Me.DataPID%>,'draft2normal');
        }
        else {
            return (false);
        }
    }
        
        
    function menu_b07_record() {

        sw_decide("b07_create.aspx?masterprefix=<%=Me.DataPrefix%>&masterpid=<%=Me.DataPID%>", "Images/comment.png", true);

    }
    function scheduler(){            
        window.open("entity_scheduler.aspx?masterprefix=<%=Me.DataPrefix%>&masterpid=<%=Me.DataPID%>","_top")
    }
    
        
    function p48_plan(){            
        window.open("p48_framework.aspx?masterprefix=<%=me.DataPrefix%>&masterpid=<%=Me.DataPID%>","_top");
    }
    

    function workflow(){            
        sw_decide("workflow_dialog.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=me.datapid%>","Images/workflow.png",false);
    }
    function p31_move2bin(){            
        sw_decide("p31_move2bin.aspx?prefix=p41&pid=<%=Me.DataPID%>","Images/bin.png",true);
    }
    function p31_move2project(){            
        sw_decide("p31_move2project.aspx?prefix=p41&pid=<%=Me.DataPID%>","Images/cut.png",true);
    }
    function p31_recalc(){            
        sw_decide("p31_recalc.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=me.datapid%>","Images/recalc.png",true);
    }
    function menu_o23_record(pid) {
        <%If Me.DataPrefix = "o23" Then%>
        sw_decide("select_doctype.aspx","Images/notepad.png",true);
        return;
        <%End If%>
        sw_decide("o23_record.aspx?masterprefix=<%=Me.DataPrefix%>&masterpid=<%=Me.DataPID%>&pid="+pid,"Images/notepad.png",true);

    }
    function menu_o22_record(pid) {
            
        sw_decide("o22_record.aspx?masterprefix=<%=Me.DataPrefix%>&masterpid=<%=Me.DataPID%>&pid="+pid,"Images/calendar.png",true);

    }
    

    function approve(){             
        var isInIFrame = (window.location != window.parent.location);
        if (isInIFrame==true){
            window.parent.sw_master("entity_modal_approving.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=Me.DataPID%>","Images/approve.png",true);
        }
        else{
            sw_decide("entity_modal_approving.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=Me.DataPID%>","Images/approve.png",true);
        }
            
    }
    
    function timeline(){            
        sw_decide("entity_timeline.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=Me.DataPID%>","Images/timeline.png",true);
    }
    function changelog() {
        window.top.sw_everywhere("changelog.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=Me.DataPID%>", "Images/log.png")        
    }
    function menu_sendmail() {
        sw_decide("sendmail.aspx?prefix=<%=me.DataPrefix%>&pid=<%=me.DataPID%>", "Images/email.png")


    }

    <%If Me.DataPrefix = "p41" Then%>
    function menu_p40_record(p40id){            
        sw_decide("p40_record.aspx?p41id=<%=Me.DataPID%>&pid="+p40id,"Images/worksheet_recurrence.png",true);
    }
    function menu_p41_recur(){
        sw_decide("p41_recurrence.aspx?pid=<%=Me.DataPID%>","Images/recurrence.png",true);
    }

    function menu_p56_record(pid,bolReturnFalse) {
        sw_decide("p56_record.aspx?masterprefix=p41&masterpid=<%=Me.DataPID%>&pid="+pid,"Images/task.png",true);
        if (bolReturnFalse==true)
            return(false)
    }
   
    <%End If%>
    <%If Me.DataPrefix = "p28" Or Me.DataPrefix = "p41" Then%>
    function p30_binding() {
        sw_decide("p30_binding.aspx?masterprefix=<%=Me.DataPrefix%>&masterpid=<%=me.datapid%>", "Images/person.png", false);
    }
    function p30_record(pid) {            
        sw_decide("p30_binding.aspx?masterprefix=<%=Me.DataPrefix%>&masterpid=<%=Me.DataPID%>&pid="+pid,"Images/person.png",true);
    }
    function p64_record(pid) {            
        sw_decide("p64_record.aspx?p41id=<%=Me.DataPID%>&pid="+pid,"Images/binder.png",true);
    }
    function menu_<%=me.DataPrefix%>_invoice_draft() {        

        sw_decide("entity_modal_invoicing.aspx?prefix=<%=Me.DataPrefix%>&pids=<%=me.DataPID%>", "Images/invoice.png", true);

    }
    <%End If%>
    function menu_fullscreen(){
        
        window.open("<%=Me.DataPrefix%>_framework_detail.aspx?pid=<%=me.DataPID%>&tab=<%=Me.tabs1.SelectedTab.Value%>&saw=1","_blank");
    }
    function menu_barcode() {
        sw_decide("barcode.aspx?prefix=<%=Me.DataPrefix%>&pid=<%=Me.DataPID%>", "Images/barcode.png", true);
    }

    function lockTabs(tabval){
       
        if (tabval ==null)
            tabval="<%=tabs1.SelectedTab.Value%>";

        var key="<%=Me.DataPrefix%>_menu-remember-tab";
        
        $.post("Handler/handler_userparam.ashx", { x36value: "1", x36key: key, oper: "set" }, function (data) {
            if (data == ' ') {
                alert("Neznámá chyba.");
                return;
            }
            
        });
        
        key="<%=Me.DataPrefix%>_framework_detail-tab";        

        $.ajax({
            method: "POST",
            url: "Handler/handler_userparam.ashx",                         
            data: { x36value: tabval, x36key: key, oper: "set" },
            success: function (data) {
                //ok - uloženo
                location.replace("<%=Me.DataPrefix%>_framework_detail.aspx?source="+document.getElementById("<%=hidSource.ClientID%>").value);
            }          
        });

            
        
        
    }
    
</script>
