<%@ Page Title="" Language="C#" Trace="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Configuration.aspx.cs" Inherits="NuvoControl.Server.WebServer.ConfigurationPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" runat="server" contentplaceholderid="MainContent">
    <asp:TreeView ID="treeConfiguration" runat="server" 
    onselectednodechanged="treeConfiguration_SelectedNodeChanged">
    </asp:TreeView>
    <hr />
    <asp:Button ID="buttonReadFunctions" runat="server" 
        onclick="buttonReadFunctions_Click" Text="Read Functions" />
    <asp:Label ID="labelFunctions" runat="server" Text="...."></asp:Label>
    <hr />
    <asp:Label ID="labelConfiguration" runat="server" Text="Label"></asp:Label>
</asp:Content>

