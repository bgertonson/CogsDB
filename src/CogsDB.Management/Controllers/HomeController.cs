using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CogsDB.Management.Core;
using CogsDB.Management.Core.Contracts;
using CogsDB.Management.Models;

namespace CogsDB.Management.Controllers
{
    public class HomeController : Controller
    {
        private IDocumentStoreLister _documentStoreLister;
        private IDocumentStoreAccess _documentStoreAccess;

        public HomeController(IDocumentStoreLister documentStoreLister, IDocumentStoreAccess documentStoreAccess)
        {
            _documentStoreLister = documentStoreLister;
            _documentStoreAccess = documentStoreAccess;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            var model = new DocumentStoreListViewModel();
            model.DocumentStores = _documentStoreLister.AvailableDocumentStores();

            return View(model);
        }

        public ActionResult DocumentTypeList(string docStore)
        {
            var model = new DocumentTypeListViewModel();
            model.DocStore = docStore;
            model.DocumentTypes = _documentStoreAccess.ListDocumentTypes(docStore).ToList();
            return View("DocTypeList", model);
        }

        public ActionResult DocumentList(string docStore, string docType)
        {
            var model = new DocumentListViewModel();
            model.Documents = _documentStoreAccess.ListDocuments(docStore, docType).ToList();
            model.DocStore = docStore;
            model.DocType = docType;
            return View("DocList", model);
        }

        public ActionResult ViewDocument(string docStore, string docType, string docId)
        {
            var model = new DocumentViewModel();
            var document = _documentStoreAccess.GetDocument(docStore, docId);
            model.Content = document.Content;
            return View("DocView", model);
        }

    }
}
