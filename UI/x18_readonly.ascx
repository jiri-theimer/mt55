<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="x18_readonly.ascx.vb" Inherits="UI.x18_readonly" %>
<asp:Repeater ID="rp1" runat="server">
    <ItemTemplate>
        <div class="div6" style="clear: both;">
            <asp:Label ID="lblHeader" runat="server"></asp:Label>
            <button type="button" runat="server" id="cmdNew"><img src="Images/new.png" />Přidat</button>
            <asp:Repeater ID="rpItems" runat="server">
                <ItemTemplate>
                    <div class="badge_label" style="background-color: <%#Eval("BackColor")%>" title="<%#Eval("BindName") %>">
                        <a class="reczoom" title="Detail dokumentu" rel="clue_o23_record.aspx?pid=<%#Eval("o23ID")%>">i</a>
                        <%If hidNoLinksAndButtons.Value <> "1" Then%>
                        <a style="color: <%#Eval("ForeColor")%>" href="javascript:r25(<%#Eval("o23ID")%>,<%#Eval("x18ID")%>)"><%# Eval("NameWithCode") %></a>
                        <%Else%>
                        <span class="val" style="color: <%#Eval("ForeColor")%>;"><%# Eval("NameWithCode") %></span>
                        <%End If%>
                    </div>
                </ItemTemplate>
            </asp:Repeater>


            <asp:Repeater ID="rpLabels" runat="server">
                <ItemTemplate>
                    <div class="badge_label" style="background-color: <%#Eval("BackColor")%>" title="<%#Eval("BindName") %>">
                        <a class="reczoom" title="Detail dokumentu" rel="clue_o23_record.aspx?pid=<%#Eval("o23ID")%>">i</a>
                        <span class="val" style="color: <%#Eval("ForeColor")%>;"><%# Eval("NameWithCode") %></span>
                        
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </ItemTemplate>
</asp:Repeater>
<asp:HiddenField ID="hidX29ID" runat="server" />
<asp:HiddenField ID="hidMasterPrefix" runat="server" />
<asp:HiddenField ID="hidRecordPID" runat="server" />
<asp:HiddenField ID="hidNoLinksAndButtons" runat="server" />
<script type="text/javascript">
    function r25(o23id, x18id) {
        sw_everywhere("o23_record.aspx?pid=" + o23id + "&x18id=" + x18id, "", true);
    }
    function r25new(x18id) {
        var masterprefix = document.getElementById("<%=hidMasterPrefix.clientid%>").value;
        var masterpid = document.getElementById("<%=hidRecordPID.clientid%>").value;

        sw_everywhere("o23_record.aspx?pid=0&masterprefix=" + masterprefix + "&masterpid="+masterpid+"&x18id=" + x18id, "", true);
    }
</script>
