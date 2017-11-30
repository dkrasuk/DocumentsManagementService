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
    
    public class LoanDealDataService : ILoanDealDataService
    {
        LoanDealSoapWebService.LoanDeal _service;

        public LoanDealDataService()
        {
            _service = new LoanDealSoapWebService.LoanDeal();

            var userName = ConfigurationManager.AppSettings["LoanDealDataServiceClientLogin"];
            var password = ConfigurationManager.AppSettings["LoanDealDataServiceClientPassword"];

            var token = new UsernameToken(userName, password, PasswordOption.SendPlainText);

            
            _service.RequestSoapContext.Security.Tokens.Add(token);
            _service.RequestSoapContext.Security.Timestamp.TtlInSeconds = 6000;
            _service.RequestSoapContext.Security.MustUnderstand = true;
            _service.SetClientCredential(token);
            
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
        }

        public LoanDealSoapWebService.getLoanDealsItem[] getLoanDealsByDealNo(string dealNo)
        {
            var res = _service.getLoanDealsByDealNo(dealNo);
            return res;
        }
    }
}
