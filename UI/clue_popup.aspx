<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_popup.aspx.vb" Inherits="UI.clue_popup" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
       
        
        /* Dropdown Content (Hidden by Default) */
        .dropdown-content {
            
            background-color:#f0f8ff;
            width:100%;
            
            
        }

            /* Links inside the dropdown */
            .dropdown-content a {
                color: black;
                padding: 6px 12px;
                
                text-decoration: none;
                display: block;
            }

                /* Change color of dropdown links on hover */
                .dropdown-content a:hover {
                    background-color:#e0e0e0;
                    
                }

       
        DIV.separator {
            
            width:100%;            
            height:1px;
            padding:0px;
            border-top: solid 1px #e0e0e0;

        }
       
    </style>

    <script type="text/javascript">
        function gg(url) {

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
        <div class="dropdown-content" style="max-height:200px;overflow:auto;">
            <a href="javascript:window.parent.p56_subgrid_workflow();">Posunout/Doplnit</a>
            <a href="javascript:window.parent.p31_entry_p56();">Vykázat k úkolu worksheet úkon</a>
            <div class="separator"></div>
            <a href="#">Upravit</a>
            <div class="separator"></div>
            <a href="javascript:window.parent.p56_clone();">Zkopírovat do nového úkolu</a>
            
            <asp:Label ID="gogo1" runat="server"></asp:Label>
        </div>


        


   
</asp:Content>
