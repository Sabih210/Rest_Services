using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PITB_Service.Models
{

    public class TopicRequest
    {

        public string _id { get; set; }
        public string _action { get; set; }
        public string _topicName { get; set; }

        public string _documentid { get; set; }
        public string _folderid { get; set; }

    }


    public class TopicResponse
    {

        public string ResponseCode { get; set; }
        public List<Topic> Topics { get; set; }

    }

    public class Topic
    {

        public string TopicID { get; set; }
        public string TopicName { get; set; }

    }
}