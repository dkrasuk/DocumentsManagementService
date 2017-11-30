using DataAccessLayer.Interfaces;
using DataAccessLayer.Services;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Protocols;

namespace DataAccessLayer.Services
{
    
    public class ContragentDataService : IContragentDataService
    {
        ContragentSoapWebService.Contragent _service;

        public ContragentDataService()
        {
            _service = new ContragentSoapWebService.Contragent();

            var userName = ConfigurationManager.AppSettings["LoanDealDataServiceClientLogin"];
            var password = ConfigurationManager.AppSettings["LoanDealDataServiceClientPassword"];

            var token = new UsernameToken(userName, password, PasswordOption.SendPlainText);

            
            _service.RequestSoapContext.Security.Tokens.Add(token);
            _service.RequestSoapContext.Security.Timestamp.TtlInSeconds = 6000;
            _service.RequestSoapContext.Security.MustUnderstand = true;
            _service.SetClientCredential(token);
            
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
        }

        public ContragentSoapWebService.contragent[] getContarentId(string contragentId)
        {
            var res = _service.getContragentById(contragentId);
            
            return res;
        }

    }
}
