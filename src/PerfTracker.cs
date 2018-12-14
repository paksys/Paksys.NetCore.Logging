using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Paksys.NetCore.Logging
{
    public partial class PerfTracker
    {
        private readonly Stopwatch _sw;
        private readonly LogDetail _logDetail;
        private readonly string _logFolderPath;

        public PerfTracker(string logFolderPath, LogDetail logDetailDetail)
        {
            _logFolderPath = logFolderPath;

            _sw = Stopwatch.StartNew();
            _logDetail = logDetailDetail;

            var beginTime = DateTime.Now;
            if (_logDetail.AdditionalInfo == null)
            {
                _logDetail.AdditionalInfo = new Dictionary<string, object>()
                {
                    {"Started", beginTime.ToString(CultureInfo.InvariantCulture)}
                };
            }
            else
            {
                _logDetail.AdditionalInfo.Add("Started", beginTime.ToString(CultureInfo.InvariantCulture));
            }
        }

        public PerfTracker(string name, string userId, string userName, string location, string product, string layer)
        {
            _sw = Stopwatch.StartNew();
            _logDetail = new LogDetail()
            {
                Message = name,
                UserId = userId,
                UserName = userName,
                Product = product,
                Layer = layer,
                Location = location,
                Hostname = Environment.MachineName
            };

            var beginTime = DateTime.Now;
            _logDetail.AdditionalInfo = new Dictionary<string, object>()
            {
                { "Started", beginTime.ToString(CultureInfo.InvariantCulture) }
            };
        }

        public PerfTracker(string name, string userId, string userName, string location, string product, string layer, Dictionary<string, object> perfParams)
            : this(name, userId, userName, location, product, layer)
        {
            foreach (var item in perfParams)
            {
                _logDetail.AdditionalInfo.Add("input-" + item.Key, item.Value);
            }
        }

        public void Stop()
        {
            _sw.Stop();
            _logDetail.ElapsedMilliseconds = _sw.ElapsedMilliseconds;
            var logger = new Logger(_logFolderPath);
            logger.Perf(_logDetail);
        }
    }
}