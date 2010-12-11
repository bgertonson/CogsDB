<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Layout.Master" Inherits="System.Web.Mvc.ViewPage<CogsDB.Management.Models.DocumentListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	DocList
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Documents</h2>
    <% foreach (var doc in Model.Documents) { %>
        <div class="document-summary">
            <span class="document-id"><%=Html.ActionLink(doc.Id, "ViewDocument", new {docStore = Model.DocStore, docType = Model.DocType, docId = doc.Id},
                                             new {@class = "link-standard"})%>
            </span>
            <span>
                <%= doc.CreateDate.ToShortDateString() %>
            </span>
        </div>
    <% } %>
</asp:Content>
