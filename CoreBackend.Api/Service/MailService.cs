using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace CoreBackend.Api.Service
{
    public interface IMailService
    {
        void Send(string subject, string msg);
    }

    public class MailService : IMailService
    {
        //private string _mailto = "mailto@qq.com";
        //private string _mailfrom = "mailfrom@qq.com";
        private readonly string _mailto = Startup._configuration["mailSettings:mailToAddress"];
        private readonly string _mailfrom = Startup._configuration["mailSettings:mailFromAddress"];

        public void Send(string subject,string msg)
        {
            Debug.WriteLine($"从{_mailfrom}到{_mailto}通过{nameof(MailService)}发送邮件!");
        }
    }

    public class NewMailService : IMailService
    {
        private readonly ILogger<NewMailService> _logger;
        public NewMailService(ILogger<NewMailService> logger)
        {
            _logger = logger;
        }
        private string _mailto = "newmailto@qq.com";
        private string _mailfrom = "newmailfrom@qq.com";

        public void Send(string subject, string msg)
        {
            Debug.WriteLine($"11111new从{_mailfrom}到{_mailto}通过{nameof(MailService)}发送邮件!");
            _logger.LogInformation($"by logger new从{_mailfrom}到{_mailto}通过{nameof(MailService)}发送邮件!");
        }
    }
}
