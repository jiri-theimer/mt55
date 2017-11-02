<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="searchbox.ascx.vb" Inherits="UI.searchbox" %>
<%If Me.ashx <> "" Then%>
<script type="text/javascript">
    $(function () {

        $("#search2").autocomplete({
            source: "Handler/<%=Me.ashx%>",
            minLength: 1,
            delay:0,
            select: function (event, ui) {
                if (ui.item) {
                    if (ui.item.PID != null)
                        window.open("<%=Me.aspx%>?pid=" + ui.item.PID, "_top");

                    return false;
                }
            },
            open: function (event, ui) {
                $('ul.ui-autocomplete')
                   .removeAttr('style').hide()
                   .appendTo('#search2_result').show();
            },
            close: function (event, ui) {
                $('ul.ui-autocomplete')
                .hide();
            }



        }).data("ui-autocomplete")._renderItem = function (ul, item) {
            var s = "<div";

            if (item.ToolTip != null)
                s = s + " title='" + item.ToolTip + "'>"
            else
                s = s + ">";

            if (item.Closed == "1")
                s = s + "<a style='text-decoration:line-through;'>";
            else
                s = s + "<a>";

            if (item.Italic == "1")
                item.ItemText = "<i>" + item.ItemText+"</i>"

            if (item.PID == null)
                s = "<div><span style='color:silver;'>" + item.ItemText;
            else
                s = s + __highlight(item.ItemText, item.FilterString);



            if (item.ItemComment != null)
                s = s + "<br><i style='color:gray;font-size:11px;'>" + item.ItemComment + "</i>";


            if (item.PID == null)
                s = s + "</span>";
            else
                s = s + "</a>";

            if (item.Draft == "1")
                s = s + "<img src='Images/draft.png' alt='DRAFT'/>";

            s = s + "</div>";

            
                


            return $(s).appendTo(ul);


        };
    });

    function __highlight(s, t) {
        var matcher = new RegExp("(" + $.ui.autocomplete.escapeRegex(t) + ")", "ig");
        return s.replace(matcher, "<strong>$1</strong>");
    }

    function search2Focus() {        
        document.getElementById("search2").value = "";
        document.getElementById("search2").style.background = "yellow";
    }
    function search2Blur() {
        
        document.getElementById("search2").style.background = "";
        document.getElementById("search2").value = "<%=me.TextboxLabel%>";

    }
</script>
<%End If%>