<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/ModalForm.Master" CodeBehind="p91_delete.aspx.vb" Inherits="UI.p91_delete" %>

<%@ MasterType VirtualPath="~/ModalForm.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="uc" TagName="datagrid" Src="~/datagrid.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function () {
            $(".tooltiptext").hide();
            $(".show_hide1").show();

            $('.show_hide1').click(function () {
                $(".tooltiptext").slideToggle();
            });



        });

        function individual(p31id, oper) {
            alert(p31id)
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="OverMainContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">


    <fieldset>
        <legend>Co se stane se zdrojovými úkony po odstranění faktury?</legend>
        <asp:RadioButtonList ID="opg1" runat="server" AutoPostBack="true" RepeatDirection="Vertical">
        <asp:ListItem Text="Přesunout do schválených" Value="1" Selected="true"></asp:ListItem>
        <asp:ListItem Text="Přesunout do rozpracovaných" Value="2"></asp:ListItem>
        <asp:ListItem Text="Přesunout do archivu" Value="3"></asp:ListItem>
        <asp:ListItem Text="Rozhodnu individuálně u každého úkonu" Value="0"></asp:ListItem>
    </asp:RadioButtonList>
    </fieldset>
    


    <asp:Panel ID="panIndividual" runat="server" style="margin-top:20px;">

        <fieldset>
            <legend>Individuální rozhodnutí o vybraných (zaškrtlých úkonech)</legend>
            <asp:Button ID="cmdBatch1" runat="server" Text="Zaškrtlé budou schválené" CssClass="cmd" />
            <asp:Button ID="cmdBatch2" runat="server" Text="Zaškrtlé budou rozpracované" CssClass="cmd" />
            <asp:Button ID="cmdBatch3" runat="server" Text="Zaškrtlé budou v archivu" CssClass="cmd" />
            <asp:Button ID="cmdBatch4" runat="server" Text="Zaškrtlé budou nenávratně odstraněny" CssClass="cmd" />
        </fieldset>

    </asp:Panel>
  
    <uc:datagrid ID="grid1" runat="server" ClientDataKeyNames="pid"></uc:datagrid>

    <a class="show_hide1" id="linkHelp" href="#">
        <img src="Images/help.png" />
        Jak funguje odstranění faktury...
    </a>
    <div class="tooltiptext" style="display: none;">
        <div class="content-box2">
            <div class="title">
                Logika odstranění faktury:
            </div>
            <div class="content" style="padding: 0px;">
                <ul>
                    <li>Odstraněním faktury nedochází automaticky k odstranění vyfakturovaných worksheet úkonů.</li>
                    <li>Vyfakturované úkony budou uvolněny z faktury. Stanou se z nich schválené, rozpracované nebo archivované úkony (záleží na vašem rozhodnutí v tomto dialogu).</li>
                    <li>Uvolněné úkony můžete později znovu vyfakturovat.</li>
                </ul>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hidGUID" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
