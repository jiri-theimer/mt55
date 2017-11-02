<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="datepicker.ascx.vb" Inherits="UI.datepicker" %>
<script type="text/javascript">

    

    $(document).ready(function () {

        

        $("#<%=txtDate.ClientID%>").datepicker({
            format: '<%=Me.DateFormat%>',            
            todayBtn: "linked",
            clearBtn: true,
            language: "cs",
            todayHighlight: true,
            autoclose: true                              
        });

        <%if Me.IsUseTimepicker then%>
        
        $("#<%=txtTime.ClientID%>").click(function () {
            $("#<%=Me.cmdTime.ClientID%>").click();
        });

        $("#<%=txtTime.ClientID%>").focus(function () {
            this.select();
        });
        
        <%end if%>
    });


    $("#<%=txtTime.ClientID%>").click(function () {
        $("#<%=Me.cmdTime.ClientID%>").click();
    });
    

    function si_<%=me.clientid%>(time) {        
        $("#<%=txtTime.ClientID%>").val(time);
        $("#<%=txtTime.ClientID%>").focus();
    }



</script>
<div class="btn-group">
    <asp:TextBox ID="txtDate" runat="server" CssClass="btn btn-default" Width="110px" Style="cursor: text;background-color:transparent;"></asp:TextBox>
    <asp:TextBox ID="txtTime" runat="server" CssClass="btn btn-default" Width="70px" Style="cursor: text;background-color:transparent;">            
    </asp:TextBox>
    <button type="button" id="cmdTime" runat="server" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
        <span class="caret"></span>

    </button>
    <ul class="dropdown-menu" id="ul1" runat="server">
        <asp:DataList ID="dlTime" runat="server" RepeatColumns="3" RepeatDirection="Horizontal">
            <ItemTemplate>
                
                    <asp:HyperLink ID="ti" runat="server" style="margin:4px;"></asp:HyperLink>
                    
                
            </ItemTemplate>                
        </asp:DataList>
        

    </ul>

</div>
<asp:HiddenField ID="hidIsUseTime" runat="server" Value="0" />
<asp:HiddenField ID="hidFormat" runat="server" Value="dd.mm.yyyy" />
<asp:HiddenField ID="hidNumberOfMonths" runat="server" Value="1" />
<asp:HiddenField ID="hidStartHour" runat="server" Value="6" />
<asp:HiddenField ID="hidEndHour" runat="server" Value="23" />
<asp:HiddenField ID="hidIsTimePer30Minutes" runat="server" Value="0" />
