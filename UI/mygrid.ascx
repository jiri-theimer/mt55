<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="mygrid.ascx.vb" Inherits="UI.mygrid" %>
<asp:HyperLink ID="clue_query" runat="server" CssClass="reczoom" ToolTip="Detail filtru" Text="i"></asp:HyperLink>
<asp:DropDownList ID="j70ID" runat="server" AutoPostBack="false" DataTextField="NameWithMark" DataValueField="pid" Style="width: 200px;" ToolTip="Pojmenovaný přehled" onchange="mygrid_reloadurl(this)"></asp:DropDownList>
<button type="button" class="button-link" id="cmdSetting" runat="server" onclick="mygrid_setting()" title="Návrhář přehledu"><img src="Images/setting.png" /></button>


<asp:HiddenField ID="hidX29ID" runat="server" />
<asp:HiddenField ID="hidPrefix" runat="server" />
<asp:HiddenField ID="hidMasterPrefix" runat="server" />
<asp:HiddenField ID="hidJ62ID" runat="server" />
<asp:HiddenField ID="hidReloadURL" runat="server" />
<asp:HiddenField ID="hidX36Key" runat="server" />
<asp:HiddenField ID="hidMasterPrefixFlag" Value="1" runat="server" />
<asp:HiddenField ID="hidModeFlag" runat="server" Value="1" />
<script type="text/javascript">
    function mygrid_setting() {
        var j70id = document.getElementById("<%=me.j70ID.clientid%>").value;
        var prefix = document.getElementById("<%=me.hidPrefix.clientid%>").value;
        var masterprefix = document.getElementById("<%=Me.hidMasterPrefix.ClientID%>").value;
        var modeflag = document.getElementById("<%=Me.hidModeFlag.ClientID%>").value;
        
        var url = "query_builder.aspx?prefix=" + prefix + "&pid=" + j70id + "&masterprefix=" + masterprefix + "&modeflag=" + modeflag;
      
        url = url + "&masterprefixflag="+document.getElementById("<%=Me.hidMasterPrefixFlag.ClientID%>").value;
        
        if (parent == top) {
            sw_everywhere(url, "Images/griddesigner.png", true);    //top okno
        }
        else {
            window.parent.sw_everywhere(url, "Images/griddesigner.png", true);  //stránka uvnitř iframe/pane
        }
        

    }
    function mygrid_reloadurl(ctl) {
        var keyx = document.getElementById("<%=Me.hidX36Key.ClientID%>").value;
        
        if (keyx != "") {
            $.post("Handler/handler_userparam.ashx", { x36value: ctl.value, x36key: keyx, oper: "set" }, function (data) {
                if (data == ' ') {
                    alert("handler_userparam error for key: "+keyx);
                    return;
                }

            });
        }
                

        var url = document.getElementById("<%=Me.hidReloadURL.ClientID%>").value;
        if (url.indexOf("?") > 0) {
            url = url + "&";
        }
        else {
            url = url + "?";
        }

        if (ctl.value != "")
            url = url + "j70id=" + ctl.value;

        
        location.replace(url);
    }
</script>

