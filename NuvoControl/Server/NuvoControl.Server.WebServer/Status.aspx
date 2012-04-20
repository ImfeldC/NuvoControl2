<%@ Page Title="" Language="C#" Trace="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="NuvoControl.Server.WebServer.Status" %>
<%@ Register TagPrefix="uc" TagName="ZoneStatus" 
    Src="~/Controls/ZoneStatusUserControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <uc:ZoneStatus id="ucZone1" runat="server"/>
    <uc:ZoneStatus id="ucZone2" runat="server"/>
    <uc:ZoneStatus id="ucZone3" runat="server"/>
    <asp:DropDownList ID="listZones" runat="server" AutoPostBack="True"> </asp:DropDownList>
    <uc:ZoneStatus id="ucZoneX" runat="server"/>
    <asp:Button ID="btnRefresh" runat="server" onclick="btnRefresh_Click" Text="Refresh" />
</asp:Content>
