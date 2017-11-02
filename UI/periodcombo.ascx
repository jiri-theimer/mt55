<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="periodcombo.ascx.vb" Inherits="UI.periodcombo" %>
<asp:DropDownList ID="per1" runat="server" AutoPostBack="true" ToolTip="Filtrování období" DataValueField="ComboValue" DataTextField="NameWithDates" style="width:160px;"></asp:DropDownList>
<asp:HyperLink ID="clue_period" runat="server" CssClass="button-reczoom" ImageUrl="Images/datepicker.png" rel="clue_periodcombo.aspx" ToolTip="Datumový filtr (období)"></asp:HyperLink>

<asp:HiddenField ID="hidCustomQueries" runat="server" />
<asp:HiddenField ID="hidExplicitValue" runat="server" />
<asp:HiddenField ID="hidLogin" runat="server" />

<asp:Button ID="cmdPeriodComboRefresh" runat="server" Style="display: none;" />
<script type="text/javascript">
    $(document).ready(function () {
       
        

        $("a.button-reczoom").each(function () {

            // Extract your variables here:
            var $this = $(this);
            var myurl = $this.attr('rel');

            var mytitle = $this.attr('title');
            if (mytitle == null)
                mytitle = 'Modal dialog';

            var dialogheight = $this.attr('dialogheight');
            if (dialogheight == "" || dialogheight == null) {
                dialogheight = 300;
            }
            

            $this.qtip({
                content: {
                    text: '<iframe src="' + myurl + '"' + ' width="100%" height="270"  frameborder="0"><p>Your browser does not support iframes.</p></iframe>',
                    title: {
                        text: mytitle
                    },

                },
                position: {
                    my: 'top center',  // Position my top left...
                    at: 'bottom center', // at the bottom right of...
                    viewport: $(window)
                },
                show: {
                    event: 'click', // Show it on click...
                    solo: true, // ...and hide all other tooltips...
                    modal: true // ...and make it modal
                },
                hide: false,
                style: {
                    classes: 'qtip-tipped',
                    width: 700,
                    height: dialogheight

                }
            });
        });


    });

    function hardrefresh_periodcombo(explicitVal) {        
        document.getElementById("<%=Me.hidExplicitValue.ClientID%>").value = explicitVal;
        var clickButton = document.getElementById("<%= cmdPeriodComboRefresh.ClientID%>");
        clickButton.click();
        
        
    }
</script>