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

    function messages() {
        sw_menu_decide("j03_messages.aspx", "Images/globe.png", false);
    }
    function defpage(url_def_page) {
        $.post("Handler/handler_default_page.ashx", { url: url_def_page }, function (data) {
            if (data == '1') {
                $.alert("Aktuální stránka byla nastavena jako moje výchozí.");
                
            }


        });
    }

    


    function mysearch() {
        sw_menu_decide("clue_search.aspx", "Images/search.png")

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
            location.href="entity_scheduler.aspx";

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
