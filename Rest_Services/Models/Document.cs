using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TPE.BO;

namespace PITB_Service.Models
{

    public class DocumentRequest
    {

        public string _id { get; set; }
        public string _action { get; set; }
        public string _documentName { get; set; }
        public string _folderid { get; set; }

    }


    public class DocumentResponse
    {

        public string ResponseCode { get; set; }
        public List<Document> Documents { get; set; }

    }

    public class Document
    {

        public string DocumentID { get; set; }
        public string DocumentName { get; set; }

    }
}