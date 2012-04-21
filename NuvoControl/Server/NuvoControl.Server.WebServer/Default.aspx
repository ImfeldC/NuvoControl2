<%@ Page Title="Home Page" Language="C#" Trace="true" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="NuvoControl.Server.WebServer.DefaultPage" %>

<script runat="server">
private void page_load(object sender, EventArgs e) {

    Trace.Write("page_load", "page_load called.");
    Trace.Write("page_load", "page_load finished.");
}
</script>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to ASP.NET!
    </h2>
    <p>
        To learn more about ASP.NET visit <a href="http://www.asp.net" title="ASP.NET Website">www.asp.net</a></p>
<p>
        <asp:Label ID="lableHostNum" runat="server" Text="Label"></asp:Label>
        &nbsp;
        <asp:Label ID="labelHostAdress" runat="server" Text="Label"></asp:Label>
        <asp:HyperLink ID="linkHostAdress" runat="server">HyperLink</asp:HyperLink>
    </p>
    <p>
        You can also find <a href="http://go.microsoft.com/fwlink/?LinkID=152368&amp;clcid=0x409"
            title="MSDN ASP.NET Docs">documentation on ASP.NET at MSDN</a>.
    </p>
</asp:Content>
