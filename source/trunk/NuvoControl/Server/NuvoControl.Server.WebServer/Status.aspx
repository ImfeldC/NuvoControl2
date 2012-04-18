<%@ Page Title="" Language="C#" Trace="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="NuvoControl.Server.WebServer.Status" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
    <asp:Button ID="Button1" runat="server" onclick="Button_Click" Text="??" />
    <div>
        <asp:Label ID="labelZoneState" runat="server" Text="Label"></asp:Label>
    </div>
    <div>
    </div>
</div>
</asp:Content>
