using System;
using System.Collections.Generic;

namespace Paksys.NetCore.Logging
{
    public partial class LogDetail
    {
        public LogDetail()
        {
            Timestamp = DateTime.Now;
            AdditionalInfo = new Dictionary<string, object>();
        }

        public DateTime Timestamp { get; }
        public string Message { get; set; }

        //WHERE
        public string Product { get; set; }
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }

        //WHO
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        //EVERYTHING ELSE
        public string CorrelationId { get; set; }   //exception shielding from server to client
        public long? ElapsedMilliseconds { get; set; }  //only for performance entries
        public Dictionary<string, object> AdditionalInfo { get; set; }  //catch-all for anything else
        public Exception Exception { get; set; }    //the exception for error logging

    }
}