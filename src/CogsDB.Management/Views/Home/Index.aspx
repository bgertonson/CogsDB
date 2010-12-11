<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Layout.Master" Inherits="System.Web.Mvc.ViewPage<CogsDB.Management.Models.DocumentStoreListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Available Document Stores</h2>
    <% foreach (var store in Model.DocumentStores) { %>
        <div class="standard-link"><%=Html.ActionLink(store, "DocumentTypeList", new { @class="link-standard", docStore=store})%></div>
    <% } %>
</asp:Content>
