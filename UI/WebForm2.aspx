<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="WebForm2.aspx.vb" Inherits="UI.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <script src="Scripts/jquery-1.11.1.min.js" type="text/javascript"></script>
      <script src="Scripts/jquery.timer.js" type="text/javascript"></script>

    <script type="text/javascript">
        var Example1a = new (function () {
            var $stopwatch, // Stopwatch element on the page
                incrementTime = 70, // Timer speed in milliseconds
                currentTime = 0, // Current time in hundredths of a second
                updateTimer = function () {
                    $stopwatch.html(formatTime(currentTime));
                    currentTime += incrementTime / 10;
                },
                init = function () {
                    $stopwatch = $('#stopwatch_a');
                    Example1a.Timer = $.timer(updateTimer, incrementTime, false);
                    
                };
            this.resetStopwatch = function () {
                currentTime = 0;
                this.Timer.stop().once();
            };
            $(init);
        });


        // Common functions
        function pad(number, length) {
            var str = '' + number;
            while (str.length < length) { str = '0' + str; }
            return str;
        }
        function formatTime(time) {
            var min = parseInt(time / 6000),
                sec = parseInt(time / 100) - (min * 60),
                hundredths = pad(time - (sec * 100) - (min * 6000), 2);
            return (min > 0 ? pad(min, 2) : "00") + ":" + pad(sec, 2) + ":" + hundredths;
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
   

         <hr/><h3 style='margin-top:20px;'>Example 1a - Stopwatch (counts ups)</h3>
    <span id="stopwatch_a">00:00:00</span>
    <p>
        <input type='button' value='Play/Pause' onclick='Example1a.Timer.toggle();' />
        <input type='button' value='Stop/Reset' onclick='Example1a.resetStopwatch();' />
    </p>
    <br/>
    <br/>

    </form>
</body>
</html>
