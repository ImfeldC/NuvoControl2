<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ZoneStatusUserControl.ascx.cs" Inherits="NuvoControl.Server.WebServer.ZoneStatusUserControl" %>
<asp:Button ID="btnPower" runat="server" onclick="btnPower_Click" Text="?" />
<asp:Label ID="labelZoneName" runat="server" Text="Zone Name"></asp:Label>
<asp:Panel ID="Panel1" runat="server">
    <asp:Button ID="btnVolDown" runat="server" onclick="btnVolDown_Click" 
        Text="&lt; -" />
    <asp:Label ID="lblVolume" runat="server" Text="???"></asp:Label>
    <asp:Button ID="btnVolUp" runat="server" onclick="btnVolUp_Click" 
        Text="+ &gt;" />
    <asp:DropDownList ID="listSources" runat="server" Height="25px" 
        onselectedindexchanged="listSources_SelectedIndexChanged" 
        AutoPostBack="True">
    </asp:DropDownList>
</asp:Panel>


