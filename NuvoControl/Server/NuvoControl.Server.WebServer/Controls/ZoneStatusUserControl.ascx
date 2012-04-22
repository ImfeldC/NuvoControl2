<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ZoneStatusUserControl.ascx.cs" Inherits="NuvoControl.Server.WebServer.ZoneStatusUserControl" %>
<asp:Panel ID="Panel2" runat="server">
    <asp:Button ID="btnPower" runat="server" onclick="btnPower_Click" Text="?" />
    <asp:Label ID="labelZoneName" runat="server" Text="Zone Name"></asp:Label>
    <asp:Button ID="btnVolDown" runat="server" onclick="btnVolDown_Click" 
        Text="&lt;" />
    <asp:Label ID="lblVolume" runat="server" Text="???"></asp:Label>
    <asp:Button ID="btnVolUp" runat="server" onclick="btnVolUp_Click" Text="&gt;" />
</asp:Panel>
<asp:Panel ID="Panel1" runat="server">
    <asp:Label ID="labelSource" runat="server" Text="Label"></asp:Label>
    <asp:Label ID="labelArrow" runat="server" Text="--&gt;"></asp:Label>
    <asp:DropDownList ID="listSources" runat="server" Height="25px" 
        onselectedindexchanged="listSources_SelectedIndexChanged" 
        AutoPostBack="True">
    </asp:DropDownList>
</asp:Panel>


