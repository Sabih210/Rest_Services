using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ChallengeSpekit.Models
{

    public class FolderRequest
    {
        public string _id { get; set; }
        public string _action { get; set; }
        public string _folderName { get; set; }

    }

    public class FolderResponse
    {

        public string ResponseCode { get; set; }
        public List<Folder> Folders { get; set; }

    }

    public class Folder
    {

        public string FolderID { get; set; }
        public string FolderName { get; set; }

    }
}