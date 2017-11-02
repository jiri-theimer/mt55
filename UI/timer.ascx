<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="timer.ascx.vb" Inherits="UI.timer" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>

<script type="text/javascript">
    <%For Each ri As RepeaterItem In rp1.Items%>
    <%If CType(ri.FindControl("isrunning"), TextBox).Text = "true" Then%>
    var saldo_<%=ri.FindControl("p85id").ClientID%>=<%=CType(ri.FindControl("CurrentDuration"), TextBox).Text%>;
    var run_<%=ri.FindControl("p85id").ClientID%> = new (function () {
        var $stopwatch, // Stopwatch element on the page
            incrementTime = 100, // Timer speed in milliseconds
            currentTime = 0, // Current time in hundredths of a second
            updateTimer = function () {
                var strD = "<%=CType(ri.FindControl("DateLastDuration"), TextBox).Text%>";
                
                var d = ConvertStringToDate(strD);
                secs = GetDur(d);
                currentTime = saldo_<%=ri.FindControl("p85id").ClientID%>+parseInt(secs * 100);

                $stopwatch.html(formatTime(currentTime));
                
                document.getElementById("<%=ri.FindControl("CurrentDuration").ClientID%>").value = currentTime;
                
            },
            init = function () {
                $stopwatch = $('#<%=ri.FindControl("timer").ClientID%>');
                
                run_<%=ri.FindControl("p85id").ClientID%>.Timer = $.timer(updateTimer, incrementTime, true);
            };
        this.resetStopwatch = function () {
            currentTime = 0;
            this.Timer.stop().once();
        };
        $(init);
    });
    <%End If%>
    <%Next%>


    // Common functions
    function pad(number, length) {
        var str = '' + number;
        while (str.length < length) { str = '0' + str; }
        return str;
    }
    
    function formatTime(time) {
        var hrs=parseInt(time / 6000/60)
        var min = parseInt(time / 6000) - (hrs*60);
        var sec = parseInt(time / 100) - (min * 60)-(hrs*60*60);
        var hundredths = pad(time - (sec * 100) - (min * 6000)-(hrs*60*6000), 2);

        return pad(hrs, 2) +":"+ pad(min, 2) + ":" + pad(sec, 2) + ":" + hundredths;
        
    }

   

    
    function ConvertStringToDate(strD) {
        
        var a = strD.split("-");
        
        var year = new Number(a[2]);        
        var month = new Number(a[1]);
        var day = new Number(a[0]);
        var hrs = new Number(a[3]);
        var min = new Number(a[4]);
        var sec = new Number(a[5]);
        

        var d = new Date(year, month - 1, day, hrs,min, sec, 0);
        return d;
    }

    function GetDur(d1) {
        var d2 = new Date()

        var dif = d2.getTime() - d1.getTime();
        var Seconds_from_T1_to_T2 = dif / 1000;
        var Seconds_Between_Dates = Math.abs(Seconds_from_T1_to_T2);

        return Seconds_Between_Dates;
       
    }

    function ChangeTimerMode() {
        
        var val=document.getElementById("<%=me.cbxTimerMode.ClientID%>").value;

        $.post("Handler/handler_userparam.ashx", { x36value: val, x36key: "timer-mode", oper: "set" }, function (data) {               
            if (data == ' ') {
                return;
            }


        });
            
            
    }

    function SaveP31Text(pid,txt) {
        $.post("Handler/handler_tempbox.ashx", {p85id: pid, value: txt.value, field: "p85Message", oper: "save" }, function (data) {               
            if (data == ' ') {
                alert("Chyba v uložení podrobného popisu.")
                return;
            }


        });
         
    }

    function ProjectChanged(sender, eventArgs){
        var item = eventArgs.get_item();
        var p41id=item.get_value();
        var pid=sender.get_attributes().getAttribute("pid");
        
        $.post("Handler/handler_tempbox.ashx", {p85id: pid, value: p41id, field: "p85OtherKey1", oper: "save" }, function (data) {             
            if (data == ' ') {
                alert("Chyba v uložení vazby na projekt.")
                return;
            }
            

        });
         
    }

    function p31_save(p85id) {
        <%If Me.IsIFrame = False Then%>
        ///volá se z p31_subgrid
        sw_master("p31_record.aspx?tabqueryflag=time&p85id="+p85id,"Images/worksheet_32.png",false);
        return(false);
        <%end if%>
        <%If Me.IsIFrame = True Then%>
        ///volá se z p31_framework.aspx
        window.parent.sw_local("p31_record.aspx?tabqueryflag=time&p85id="+p85id,"Images/worksheet_32.png",false);
        return(false);
        <%end if%>
    }


    
</script>

<div>
    <div style="float: left; width: 100px;">
        <asp:Button ID="cmdAddRow" runat="server" CssClass="cmd" Text="Přidat" meta:resourcekey="cmdAddRow" />
    </div>
    <div style="float: left; width: 100px;">
        <asp:Button ID="cmdClear" runat="server" CssClass="cmd" Text="Vyčistit" meta:resourcekey="cmdClear" />
    </div>
    <div style="float: right;" id="setting">
        <asp:DropDownList ID="cbxTimerMode" runat="server" onchange="ChangeTimerMode()">
            <asp:ListItem Text="Povolit spuštění více časovačů souběžně" Value="1"></asp:ListItem>
            <asp:ListItem Text="V jednom okamžiku pouze jeden spuštěný časovač" Value="2"></asp:ListItem>

        </asp:DropDownList>
    </div>
</div>
<div style="clear: both;"></div>
<asp:Panel ID="panContainer" runat="server" style="margin-top:10px;">

    <asp:Repeater ID="rp1" runat="server">
        <ItemTemplate>
            <asp:panel ID="panSave" runat="server" style="float:left;width:45px;">
                <asp:Label ID="RowIndex" runat="server" style="padding-right:2px;"></asp:Label>
                <asp:ImageButton ID="cmdFinalSave" runat="server" ImageUrl="Images/save.png" CssClass="button-link" ToolTip="Uložit úkon a vyjmout ho ze stopek" meta:resourcekey="cmdFinalSave" />
               
                
            </asp:panel>
            
            <div style="float: left;">
                <uc:project ID="p41ID" runat="server" Width="400px" AutoPostBack="false" Flag="p31_entry" />
                
               
                <div style="padding-top: 4px;">

                    <asp:Label ID="timer" runat="server" Style="font-weight: bold;" Text="00:00:00" Width="70px"></asp:Label>

                    <asp:ImageButton ID="cmdStart" runat="server" ImageUrl="Images/timer_start_24.png" ToolTip="Start" CommandName="start" />
                    <asp:ImageButton ID="cmdPause" runat="server" ImageUrl="Images/timer_pause_24.png" ToolTip="Pozastavit" CommandName="pause" meta:resourcekey="cmdPause" />
                    <span style="margin-left: 20px;"></span>
                    <asp:ImageButton ID="cmdReset" runat="server" ImageUrl="Images/timer_reset_24.png" ToolTip="Zastavit a vynulovat" CommandName="reset" meta:resourcekey="cmdReset" />

                    <asp:ImageButton ID="del" runat="server" ImageUrl="Images/delete_row.png" ToolTip="Odstranit řádek" CssClass="button-link" CommandName="delete" style="float:right;" meta:resourcekey="del" />
                </div>
            </div>


            <div style="float: left;">
                <asp:TextBox ID="p31Text" runat="server" TextMode="MultiLine" Style="width: 450px; height: 60px;" ToolTip="Podrobný popis úkonu"></asp:TextBox>

                <asp:HiddenField ID="p85id" runat="server" />
                <asp:TextBox ID="isrunning" runat="server" value="false" Style="display: none;"></asp:TextBox>
                <asp:TextBox ID="CurrentDuration" runat="server" value="0" Style="display: none;"></asp:TextBox>
                <asp:TextBox ID="DateLastDuration" runat="server" Width="400px" Style="display: none;"></asp:TextBox>
            </div>
           
            <div style="clear: both;"></div>
        </ItemTemplate>
    </asp:Repeater>


</asp:Panel>
<asp:HiddenField ID="hidIsPanelView" runat="server" Value="0" />
<asp:HiddenField ID="hidIsIframe" runat="server" Value="0" />