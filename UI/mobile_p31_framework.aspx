<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Mobile.Master" CodeBehind="mobile_p31_framework.aspx.vb" Inherits="UI.mobile_p31_framework" %>

<%@ MasterType VirtualPath="~/Mobile.Master" %>
<%@ Register TagPrefix="uc" TagName="mobile_p31_list" Src="~/mobile_p31_list.ascx" %>
<%@ Register TagPrefix="uc" TagName="project" Src="~/project.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        tr.trTextRow {
            border-bottom: solid 2px gray;
        }

        .task_in_table {
            background-color: #5bc0de;
            color: white;
            text-align: center;
            white-space: pre-wrap;
            padding: 4px;
            margin-bottom: 5px;
            font-weight: bold;
            border-radius: 4px;
            font-size: 90%;
        }

        .person_in_table {
            background-color: green;
            color: white;
            text-align: center;
            white-space: pre-wrap;
            padding: 4px;
            margin-bottom: 5px;
            font-weight: bold;
            border-radius: 4px;
            font-size: 90%;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%=p31Date.ClientID%>").datepicker({
                format: 'dd.mm.yyyy',
                autoclose: true,
                todayHighlight: true,
                language: 'cs'
            });


        });



        function hardrefresh(flag, value) {

            document.getElementById("<%=HardRefreshValue.ClientID%>").value = value;
            document.getElementById("<%=HardRefreshFlag.ClientID%>").value = flag;
            <%=Me.ClientScript.GetPostBackEventReference(Me.cmdHardRefresh, "", False)%>;


        }

        function trydel() {

            if (confirm("Opravdu odstranit záznam?")) {
                hardrefresh('delete', '1');
                return (true);
            }
            else {
                return (false);
            }
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <nav class="navbar navbar-default" style="margin-bottom: 0px !important;">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#myNavbarOnSite">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <img src="Images/worksheet.png" class="navbar-brand" />                
                <asp:Image ID="imgHeader" runat="server" ImageUrl="Images/new.png" CssClass="navbar-brand"/>
                <asp:label ID="lblRecordHeader" runat="server" CssClass="navbar-brand" Text="Zapsat úkon"></asp:label>

               
                
            </div>
            <div class="collapse navbar-collapse" id="myNavbarOnSite">
                <ul class="nav navbar-nav">
                    <li><asp:HyperLink ID="linkCurProject" runat="server" NavigateUrl="mobile_p41_framework.aspx"></asp:HyperLink></li>
                </ul>
            </div>

    </nav>



    <uc:project ID="p41id" runat="server" Width="99%" Flag="p31_entry" AutoPostBack="true" />




    <asp:panel ID="panRecord" runat="server"  CssClass="panel panel-default">        
        <div class="panel-body">

            <div>
                <asp:TextBox ID="p31Date" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <a id="record_p32id"></a>
            <div style="margin-top: 15px;">
                <asp:DropDownList ID="p34ID" runat="server" AutoPostBack="true" DataTextField="p34Name" DataValueField="pid" ToolTip="Sešit úkonu" CssClass="form-control"></asp:DropDownList>
            </div>
            <div style="margin-top: 10px;">
                <asp:DropDownList ID="p32ID" runat="server" AutoPostBack="true" DataTextField="p32Name" DataValueField="pid" ToolTip="Aktivita úkonu" CssClass="form-control"></asp:DropDownList>

            </div>

            <asp:Panel ID="panT" runat="server" Visible="false">
                <table>
                    <tr>
                        <td>Hodiny:</td>
                        <td>
                            <asp:TextBox ID="p31Hours_Orig" runat="server" CssClass="form-control"></asp:TextBox>
                        </td>
                    </tr>
                </table>

            </asp:Panel>
            <asp:Panel ID="panM" runat="server" Visible="false">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblp31Amount_WithoutVat_Orig" runat="server" Text="Částka:"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="p31Amount_WithoutVat_Orig" runat="server" CssClass="form-control"></asp:TextBox>

                        </td>
                        <td>
                            <asp:DropDownList ID="j27ID_Orig" runat="server" DataTextField="j27Code" DataValueField="pid" CssClass="form-control"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblp31VatRate_Orig" runat="server" Text="DPH:"></asp:Label></td>
                        <td>
                            <asp:DropDownList ID="p31VatRate_Orig" runat="server" DataTextField="p53Value" DataValueField="p53Value" CssClass="form-control">
                            </asp:DropDownList>

                        </td>
                        <td>
                            <asp:TextBox ID="p31Amount_Vat_Orig" runat="server" CssClass="form-control"></asp:TextBox>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="lblp31Amount_WithVat_Orig" runat="server" Text="Vč.DPH:"></asp:Label></td>
                        <td>
                            <asp:TextBox ID="p31Amount_WithVat_Orig" runat="server" CssClass="form-control"></asp:TextBox>
                        </td>
                        <td></td>
                    </tr>
                </table>
            </asp:Panel>
            <asp:Panel ID="panU" runat="server" Visible="false">
                <table>
                    <tr>
                        <td>Počet:</td>
                        <td>
                            <asp:TextBox ID="p31Value_Orig" runat="server" CssClass="form-control"></asp:TextBox>
                        </td>
                    </tr>
                </table>

            </asp:Panel>
            <div>
                <span>Popis:</span>
                <asp:TextBox ID="p31Text" runat="server" CssClass="form-control" Style="height: 90px; background-color: #ffffcc;" TextMode="MultiLine"></asp:TextBox>
            </div>
            <div style="margin-top: 5px;">
                <asp:DropDownList ID="p56ID" runat="server" AutoPostBack="false" DataTextField="NameWithTypeAndCode" DataValueField="pid" ToolTip="Úkol" CssClass="form-control"></asp:DropDownList>
            </div>
            <div style="margin-top: 5px;">
                <asp:DropDownList ID="j02ID_ContactPerson" runat="server" AutoPostBack="false" DataTextField="FullNameDescWithEmail" DataValueField="pid" ToolTip="Kontaktní osoba" CssClass="form-control"></asp:DropDownList>
            </div>
            <div>
                <asp:Label ID="TimeStamp" runat="server" Font-Italic="true" Font-Size="Small"></asp:Label>
            </div>
            <div style="margin-top: 20px;">
                <button type="button" class="btn btn-primary" onclick="hardrefresh('save','1')">Uložit změny</button>
                <button type="button" class="btn btn-primary" onclick="hardrefresh('saveandcopy','1')">Uložit & kopírovat</button>


            </div>
            <div>
                <button type="button" class="btn btn-danger" onclick="trydel()" id="cmdDelete" runat="server" style="margin-top: 20px;">Odstranit záznam</button>
                <button type="button" class="btn btn-default" onclick="hardrefresh('clear','1')" style="margin-top: 20px;">Vyčistit & Nový</button>
            </div>



        </div>
    </asp:panel>

    <a id="record_message"></a>
    <asp:Panel ID="panMessage" runat="server" CssClass="alert alert-danger" role="alert" Visible="false">
        <asp:Label ID="WarningMessage" runat="server"></asp:Label>
    </asp:Panel>

    <a id="record_list"></a>
    <uc:mobile_p31_list ID="list1" runat="server"></uc:mobile_p31_list>

    <asp:HiddenField ID="hidTab" runat="server" Value="p41" />
    <asp:HiddenField ID="HardRefreshValue" runat="server" />
    <asp:HiddenField ID="HardRefreshFlag" runat="server" />
    <asp:HiddenField ID="hidMask" runat="server" Value="1" />

    <asp:HiddenField ID="hidP56ID" runat="server" />
    <asp:HiddenField ID="hidP33ID" runat="server" />
    <asp:HiddenField ID="hidP31ID" runat="server" />
    <asp:HiddenField ID="hidDaysQueryBefore" runat="server" Value="10" />
    <asp:HiddenField ID="hidDirectCallP41ID" runat="server" />
    <asp:HiddenField ID="hidRef" runat="server" />
    <asp:HiddenField ID="hidAllowRates" runat="server" Value="1" />
    <asp:HiddenField ID="hidVirtualCountP56" runat="server" Value="0" />
    <asp:Button ID="cmdHardRefresh" runat="server" Style="display: none;" />

</asp:Content>
