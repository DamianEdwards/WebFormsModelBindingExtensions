<%@ Page Title="Products" Async="true" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="AspNet.WebForms.ModelBinding.Extensions.Samples.Products" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2>Products</h2>

    <asp:ListView runat="server" ID="productsList"
        ItemType="AspNet.WebForms.ModelBinding.Extensions.Samples.Model.Product" DataKeyNames="ID"
        SelectMethod="GetProductsAsync"
        DeleteMethod="DeleteProductAsync">
        <LayoutTemplate>
            <table>
                <thead>
                    <tr>
                        <th><asp:LinkButton runat="server" CommandName="Sort" CommandArgument="ID">ID</asp:LinkButton></th>
                        <th><asp:LinkButton runat="server" CommandName="Sort" CommandArgument="Name">Name</asp:LinkButton></th>
                        <th>Category</th>
                        <th>&nbsp;</th>
                    </tr>
                </thead>
                <tfoot>
                    <tr>
                        <td colspan="4">
                            <asp:DataPager runat="server" PageSize="5" QueryStringField="page">
                                <Fields><asp:NumericPagerField /></Fields>
                            </asp:DataPager>
                        </td>
                    </tr>
                </tfoot>
                <tbody>
                    <tr runat="server" id="itemPlaceholder"></tr>
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr>
                <td><%#: Item.ID %></td><td><%#: Item.Name %></td><td><%#: Item.Category.Name %></td>
                <td><asp:LinkButton runat="server" CommandName="Delete" CommandArgument="<%# Item.ID %>">delete</asp:LinkButton></td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
</asp:Content>