using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace dbnet2.Models.Visitors
{
    public class AccessLogReader
    {
        public string LogPath { get; set; }
        private readonly String logEntryPattern = "^([\\d.]+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(.+?)\" (\\d{3}) (\\d+) \"([^\"]+)\" \"([^\"]+)\"";

        public AccessLogReader(string logPath)
        {
            LogPath = logPath;
        }

        public IList<Visitor> GetLastVisitors(int amount)
        {
            var visitors = new List<Visitor>();

            var lines = System.IO.File.ReadAllLines(LogPath).Reverse();
            var api = new IpInfoApi();
            int taken = 0;

            foreach (var line in lines)
            {
                LogLine logLine = null;

                if (taken >= amount)
                    break;

                if(!IsValidLine(line, out logLine))
                    continue;

                var details = api.GetIpaddressDetails(logLine.IpAddress);
                var visitor = new Visitor
                {
                    City = details.City,
                    Country = details.Country,
                    Flag = details.Country,
                    LandingPage = logLine.Request,
                    Date = logLine.DateTime
                };
                visitors.Add(visitor);

                taken++;
            }

            return visitors;
        }

        protected bool IsValidLine(string inStr, out LogLine logOut)
        {          
            Match theMatch = Regex.Match(inStr, logEntryPattern);
            logOut = default(LogLine);

            if (theMatch.Success)
            {
                LogLine logLine = new LogLine
                {
                    IpAddress = theMatch.Groups[1].Value,
                    DateTime = theMatch.Groups[4].Value,
                    Request = theMatch.Groups[5].Value,
                    Response = theMatch.Groups[6].Value,
                    BytesSent = theMatch.Groups[7].Value,
                    Referrer = theMatch.Groups[8].Value,
                    UserAgent = theMatch.Groups[9].Value
                };

                // status code valid
                int statusCode;
                if (!int.TryParse(logLine.Response, out statusCode) || (statusCode % 200 != 0 && statusCode % 300 != 0))
                {
                    return false;
                }

                // not an ignored file/filetype
                string[] ignore = new string[] { ".css", ".jpg", ".jpeg", ".png", ".gif", ".ico", ".js", ".swf", "/feed/?fromlocal" };
                foreach (var fileType in ignore)
                {
                    if (logLine.Request.Contains(fileType))
                    {
                        return false;
                    }
                }

                logOut = logLine;
                return true;
            }

            return false;
        }
    }

    public class LogLine
    {
        public string IpAddress { get; set; }
        public string DateTime { get; set; }
        
        protected string _request;
        public string Request
        {
            get
            {
                var request = _request.Replace("GET", String.Empty);
                request = Regex.Replace(request, "HTTP/[0-9]\\.[0-9]", String.Empty);

                return request;
            }
            set
            {
                _request = value;
            }
        }
        public string Response { get; set; }
        public string BytesSent { get; set; }
        public string Referrer { get; set; }
        public string UserAgent { get; set; }
    }
}