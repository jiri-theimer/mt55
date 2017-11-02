<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="pokus2.aspx.vb" Inherits="UI.pokus2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>pokus</title>
     <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="x-ua-compatible" content="IE=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <link href="~/Styles/Site_v10.css" rel="stylesheet" type="text/css" />

    <link href="Scripts/contextmenu/jquery.contextMenu.css" rel="stylesheet" type="text/css" />

    <style type="text/css">
        A.pp1 {
    display: block;
    background-image: url('../Images/p1.png');
    background-repeat: no-repeat;    
    width:16px;
    height:20px;
    cursor: pointer;
       
  
}

    A.pp1:hover {
         background-image: url('../Images/p2.png');
         background-repeat: no-repeat;
         background-color:lightskyblue;
    } 

    </style>
    <script src="Scripts/jquery-1.11.1.min.js" type="text/javascript"></script>

    <script src="Scripts/contextmenu/jquery.contextMenu.js" type="text/javascript"></script>

    <script src="Scripts/contextmenu/jquery.ui.position.min.js" type="text/javascript"></script>




    <script type="text/javascript">
        

        $(function () {
            $.contextMenu({
                selector: '.pp1',
                trigger: 'left',
                build: function ($trigger, e) {
                    // this callback is executed every time the menu is to be shown
                    // its results are destroyed every time the menu is hidden
                    // e is the original contextmenu event, containing e.pageX and e.pageY (amongst other data)
                    
                    //FillMenu($trigger.attr("prefix"), $trigger.attr("pid"), "blank");

                    return {
                        callback: function (key, options) {
                            var m = "clicked: " + key;
                            alert(m + " element id" + $trigger.attr("id"));
                        },
                        items: $.contextMenu.fromMenu($('#html5menu'))
                    };
                }
            });
        });


        

        function FillMenu(curPREFIX,curPID,curPAGE) {
            $.ajax({
                method: "POST",
                url: "Handler/handler_popupmenu.ashx",
                beforeSend: function () {
                    alert("načítání");
                    $('#loading1').show();
                },
                async: false,
                timeout:3000,
                data: { prefix: curPREFIX, pid: curPID, page: curPAGE },
                success: function (data) {
                    alert("načítání");
                    $('#html5menu').html('');
                    
                    $("#html5menu").append(data);

                },
                complete: function () {
                    // do the job here
                    alert("načítání");
                    $('#loading1').hide();
                }
            });

            ;

        }


        function contMenu(url) {

        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="height:50px;">
            <img src="Images/loading.gif" id="loading1" style="display:none;" />

            
        </div>

        <a class="pp1 btn btn-neutral" id="row1" prefix="p56", pid="9"></a>

        <br>
        <a class="pp1" id="row2" prefix="p41" pid="8"></a>

        <br>
        <a class="pp1 btn btn-neutral" id="row3" prefix="j02" pid="10"></a>

        



        <menu id="html5menu" style="display: none" class="showcase">           
        </menu>
       
    </form>
</body>
</html>
