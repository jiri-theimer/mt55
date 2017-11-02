<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Clue.Master" CodeBehind="clue_p42_record.aspx.vb" Inherits="UI.clue_p42_record" %>

<%@ MasterType VirtualPath="~/Clue.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-height: 270px; overflow-x: auto; overflow-y: auto; width: 100%;">
        <div>
           <img src="Images/setting_32.png" />
           <asp:Label ID="ph1" runat="server" CssClass="clue_header_span"></asp:Label>
       </div>
       <div class="content-box2">
        <div class="title">
            V projektu jsou zapnuty níže uvedené nástroje:
        </div>
        <asp:panel ID="panModules" runat="server"  CssClass="content" Enabled="false">  
            <div style="float:left;padding:10px;">
                <asp:CheckBox ID="p42IsModule_p31" runat="server" Text="WORKSHEET" />
            </div>
            <div style="float:left;padding:10px;">
                <asp:CheckBox ID="p42IsModule_p56" runat="server" Text="ÚKOLY"/>
            </div>
            <div style="float:left;padding:10px;">
                <asp:CheckBox ID="p42IsModule_o22" runat="server" Text="Kalendářové UDÁLOSTI"  />
            </div>
            <div style="float:left;padding:10px;">
                <asp:CheckBox ID="p42IsModule_o23" runat="server" Text="DOKUMENTY" />
            </div>
            <div style="float:left;padding:10px;">
                <asp:CheckBox ID="p42IsModule_p45" runat="server" Text="Projektové ROZPOČTY"  />
            </div>
            <div style="float:left;padding:10px;">
                <asp:CheckBox ID="p42IsModule_p48" runat="server" Text="Operativní PLÁNOVÁNÍ" />
            </div>
            
        </asp:panel>
    </div>
    <asp:Panel ID="panP34IDs" runat="server" CssClass="content-box2">
        <div class="title">
            Povolené sešity pro vykazování
        </div>
        <div class="content">
             <asp:Repeater ID="rpP34" runat="server">
                    <ItemTemplate>
                        <li>
                            <%# Eval("p34Name")%>
                        </li>


                    </ItemTemplate>
                </asp:Repeater>
        </div>
    </asp:Panel>
        
    </div>
</asp:Content>
