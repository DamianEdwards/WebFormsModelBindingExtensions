<%@ Page Title="Home Page" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AspNet.WebForms.ModelBinding.Extensions.Samples._Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Modify this template to jump-start your ASP.NET application.</h2>
            </hgroup>
            <p>
                To learn more about ASP.NET, visit <a href="http://asp.net" title="ASP.NET Website">http://asp.net</a>.
                The page features <mark>videos, tutorials, and samples</mark> to help you get the most from ASP.NET.
                If you have any questions about ASP.NET visit
                <a href="http://forums.asp.net/18.aspx" title="ASP.NET Forum">our forums</a>.
            </p>
        </div>
    </section>
</asp:Content>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <asp:ListView runat="server" ID="productsList" ItemType="AspNet.WebForms.ModelBinding.Extensions.Samples.Model.Category"
        SelectMethod="GetCategoriesAsync">
        <LayoutTemplate>
            <table>
                <thead>
                    <tr>
                        <th><asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ID">ID</asp:LinkButton></th>
                        <th><asp:LinkButton runat="server" CommandName="Sort" CommandArgument="Name">Name</asp:LinkButton></th>
                        <th>Product Count</th></tr>
                </thead>
                <tfoot>
                    <tr>
                        <td colspan="3">
                            <asp:DataPager runat="server" PageSize="5" QueryStringField="page">
                                <Fields>
                                    <asp:NumericPagerField />
                                </Fields>
                            </asp:DataPager>
                        </td></tr>
                </tfoot>
                <tbody>
                    <tr runat="server" id="itemPlaceholder"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td><%#: Item.ID %></td><td><%#: Item.Name %></td><td><%#: Item.Products.Count %></td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>
