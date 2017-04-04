using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        //private string _mailTo = "per.hornell@loxysoft.se";
        //private string _mailFrom = "info@loxysoft.se";

        // Use the values from our appsetting file instead
        private string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        public void Send(string subject, string message)
        {
            // Just a demo for sending mail service
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo} with LocalMailService");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }    
}
