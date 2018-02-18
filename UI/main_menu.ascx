<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="main_menu.ascx.vb" Inherits="UI.main_menu" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Panel runat="server" ID="panContainer" Style="height: 34px;" Visible="false">
    <telerik:RadNavigation ID="menu1" runat="server" MenuButtonPosition="Right" Skin="Windows7" EnableViewState="false">             
        <Nodes>
            <telerik:NavigationNode Text="" width="165px" Enabled="false" ID="logo">                 
            </telerik:NavigationNode>           
        </Nodes>
    </telerik:RadNavigation>

    <a href="Default.aspx" title="MARKTIME" style="position:absolute;top:5px;left:4px;"><img src="Images/logo_transparent.png" border="0" /></a>
</asp:Panel>

<div id="divFrameBox" class="content-box2" style="position:absolute;width:500px;top:35px;z-index:5000;display:none;background-color:#F1F1F1;">
    <div class="title" style="background-color:ButtonFace; !important;">        
        <span id="divFrameBoxTitle">Hovado</span>
        <button type="button" style="width:400px;margin-left:100px;" onclick="close_framebox()">Zavřít</button>
    </div>
    <div class="content">
        <iframe id="fraBox" frameborder="0" width="100%"></iframe>
    </div>
    
</div>


<asp:HiddenField ID="hidAllowSearch1" runat="server" Value="0" />
<asp:HiddenField ID="hidMasterPageName" runat="server" Value="Site" />

<script type="text/javascript">
    <%If panContainer.Visible Then%>



    function help(page) {
        window.open("help.aspx?page=" + page, "_blank");
    }

    function sw_menu_decide(url, img, is_max) {
        try {
            sw_master(url, img, is_max)
        }
        catch (err) {
            sw_local(url, img, is_max)
        }
    }

    
    function defpage(url_def_page) {
        $.post("Handler/handler_default_page.ashx", { url: url_def_page }, function (data) {
            if (data == '1') {
                $.alert("Aktuální stránka byla nastavena jako moje výchozí.");
                
            }


        });
    }

    
    function close_framebox() {
        document.getElementById("divFrameBox").style.display = "none";
    }
    function handle_framebox(url) {
        var h = new Number;
        h = $(window).height();
        h = h - 40;

        var ctl = document.getElementById("divFrameBox");
        if (ctl.style.display == "block" && document.getElementById("fraBox").src.indexOf(url) > 0) {
            ctl.style.display = "none";
            return (1);
        }
        if (ctl.style.display == "none" && document.getElementById("fraBox").src.indexOf(url) > 0) {
            ctl.style.display = "block";
            ctl.style.width = "700px";
            document.getElementById("fraBox").style.height = (h - 80) + "px";
            return (1);
        }

        

        ctl.style.width = "700px";
        ctl.style.height = h + "px";
        ctl.style.display = "block";
        
        document.getElementById("fraBox").style.height = (h - 80) + "px";
        return (2);
    }

    function mysearch() {        
        if (handle_framebox("search.aspx") == 1) {            
            return;
        }
        $("#divFrameBoxTitle").html("<img src='Images/search.png'/> HLEDÁNÍ");
        

        var ctl = document.getElementById("fraBox");        
        ctl.src = "clue_search.aspx";
        
    }

    function mynavigator() {
        if (handle_framebox("navigator.aspx") == 1)
            return;

        $("#divFrameBoxTitle").html("<img src='Images/tree.png'/> NAVIGATOR");
        var ctl = document.getElementById("fraBox");        
        ctl.src = "clue_navigator.aspx";

    }


    function setlang(value) {

        createCookie('MT50-CultureInfo', value, 30);
        location.replace(window.location.href);
    }

    function createCookie(name, value, days) {
        if (days) {
            var date = new Date();
            date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
            var expires = "; expires=" + date.toGMTString();
        }
        else var expires = "";
        document.cookie = name + "=" + value + expires + "; path=/";
    }

    function readCookie(name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }




    function hardrefresh_menu(pid, flag) {
        if (flag == "p41-create" || flag == "p41-save") {
            location.href="p41_framework.aspx?pid=" + pid;
            return;
        }
        if (flag == "p56-save" || flag == "p56-create") {
            location.href="p56_framework.aspx?pid=" + pid;
            return;
        }
        if (flag == "p91-create" || flag == "p91-save") {
            location.href="p91_framework.aspx?pid=" + pid;
            return;
        }

        if (flag == "p28-save" || flag == "p28-create") {
            location.href="p28_framework.aspx?pid=" + pid;

        }
        if (flag == "o23-save" || flag == "o23-create") {
            location.href="o23_framework.aspx?pid=" + pid;

        }
        if (flag == "j03_myprofile_defaultpage") {
            location.replace("default.aspx");

        }
        if (flag == "o22-save" || flag == "o22-create") {
            location.replace("default.aspx");

        }
        if (flag == "j02-save" || flag == "j02-create") {
            location.href="j02_framework.aspx?pid="+pid;

        }

    }



    <%End If%>

    function MainMenuClose() {
        <%If panContainer.Visible Then%>
        var menu = $find("<%= menu1.ClientID%>");
        var n = menu.get_expandedNode();
        if (n != null) {
            n.collapse();
        }
        
        
        <%End If%>
    }
</script>
