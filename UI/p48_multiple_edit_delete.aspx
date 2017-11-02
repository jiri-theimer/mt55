<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p48_multiple_edit_delete.aspx.vb" Inherits="UI.p48_multiple_edit_delete" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datacombo" Src="~/datacombo.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="Scripts/jqueryui/jquery-ui.min.css" />
    <style type="text/css">
        .ui-autocomplete {
            max-height: 300px;
            overflow-y: auto;
            width: 80px;
            /* prevent horizontal scrollbar */
            overflow-x: hidden;
        }
    </style>

    <script src="Scripts/jqueryui/jquery-ui.min.js" type="text/javascript"></script>


    <script type="text/javascript">
        $(document).ready(function () {
            $(document).ready(function () {
                var hours_interval = [
                '0,5', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10'
                ];



                $(".hodiny").autocomplete({
                    source: hours_interval,
                    minLength: 0,
                    scroll: true,
                    change: function (event, ui) {
                        handle_hours();

                    }
                }).focus(function () {
                    $(this).autocomplete("search", "")
                    $(this).select();
                });



                var time_interval = [
                '07:00', '08:00', '09:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00', '21:00', '22:00'
                ];



                $(".presnycas").autocomplete({
                    source: time_interval,
                    minLength: 0,
                    scroll: true,
                    change: function (event, ui) {

                        recalcduration(this);

                    }
                }).focus(function () {
                    $(this).autocomplete("search", "")
                    $(this).select();
                });



            });


            function recalcduration(ctl) {

                var t1 = document.getElementById(ctl.attributes["cas_od"].value).value;
                var t2 = document.getElementById(ctl.attributes["cas_do"].value).value;

                $.post("Handler/handler_time.ashx", { t1: t1, t2: t2, oper: "duration" }, function (data) {
                    if (data == ' ') {
                        return;
                    }

                    var s = data.split("|");


                    if (s.length <= 1) {
                        return
                    }

                    t1 = s[0];
                    t2 = s[1];

                    var message = s[3];
                    document.getElementById(ctl.attributes["hodiny"].value).value = s[2];


                });
            }

        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <table cellpadding="6" cellspacing="2">
        <tr>            
            
            <th>Datum/Osoba</th>
            
            <th>Projekt/Sešit/Aktivita</th>
            <th>Hodiny</th>
            <th>Od</th>
            <th>Do</th>
            
            <th>Text</th>
            <th></th>
        </tr>
        <asp:Repeater ID="rp1" runat="server">
            <ItemTemplate>
                <tr class="trHover" style="vertical-align:bottom;">                    
                   
                    <td>
                        <asp:Label ID="p48Date" runat="server" ForeColor="blue"></asp:Label>
                        <div>
                            <asp:Label ID="Person" runat="server"></asp:Label>
                        </div>
                        <asp:HiddenField ID="j02id" runat="server" />
                        <asp:HiddenField ID="p41id" runat="server" />
                        <asp:HiddenField ID="date" runat="server" />
                        <asp:HiddenField ID="p85id" runat="server" />
                    </td>
                    
                    <td>
                        <asp:Label ID="Project" runat="server" ForeColor="DarkOrange"></asp:Label>
                        <div>
                            <asp:Label ID="p34Name" runat="server"></asp:Label>
                            <asp:DropDownList ID="p32ID" runat="server" DataTextField="p32Name" DataValueField="pid" style="width:200px;"></asp:DropDownList>
                        </div>
                    </td>

                    <td>
                        <asp:TextBox ID="p48Hours" runat="server" CssClass="hodiny" style="width:40px;text-align:center;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="p48TimeFrom" runat="server" CssClass="presnycas" style="width:40px;text-align:center;"></asp:TextBox>
                    </td>
                    <td>
                        <asp:TextBox ID="p48TimeUntil" runat="server" CssClass="presnycas" style="width:40px;text-align:center;"></asp:TextBox>
                    </td>
                    
                    <td>
                        <asp:TextBox ID="p48Text" runat="server" Style="width: 250px;"></asp:TextBox>
                    </td>
                    <td style="padding-left:20px;">
                        <asp:ImageButton ID="cmdDelete" runat="server" ImageUrl="Images/delete.png" CommandName="delete" ToolTip="Odstranit položku" CssClass="button-link" />
                        
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
