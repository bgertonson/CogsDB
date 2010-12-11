<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Layout.Master" Inherits="System.Web.Mvc.ViewPage<CogsDB.Management.Models.DocumentTypeListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	DocTypeList
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Document Types</h2>
    <%foreach(var docType in Model.DocumentTypes) { %>
        <div><%=Html.ActionLink(docType, "DocumentList", new {docStore = Model.DocStore, docType = docType}, new { @class="link-standard"})%></div>
    <% }%>
</asp:Content>
