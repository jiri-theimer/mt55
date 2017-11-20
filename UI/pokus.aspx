<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="pokus.aspx.vb" Inherits="UI.pokus" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">



    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%=me.txt1.ClientID%>").keyup(function () {
                TrySearch(this.value);
            });

        });

        function TrySearch(searchExpr) {
            

            $.post("Handler/handler_search_person.ashx", { term: searchExpr, fo: "j02LastName" }, function (data) {
                if (data == ' ') {
                    return;
                }

                $('#list1').empty()

                for (var i in data) {
                    
                    var output = '<li>' + data[i].ItemText + '</li>';
                    $('#list1').append(output);
                }

                
                
                document.getElementById("hovado").value = data[1].ItemText;
                

            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <telerik:RadComboBox ID="cbx1" runat="server" ShowToggleImage="true"></telerik:RadComboBox>
    <span>Příjmení:</span>
    <asp:TextBox ID="txt1" runat="server"></asp:TextBox>

    <input type="text" id="hovado" />

    <div id="list1">

    </div>
</asp:Content>



