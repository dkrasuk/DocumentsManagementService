using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Services;

namespace DataAccessLayer
{
    public static class Bootstraper
    {
        public static void Register(IUnityContainer container)
        {
            var locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            DictionariesClient.Bootsrap.Register(container);

            container.RegisterType<IDocumentManagementDataService, DocumentManagementDataService>();
            container.RegisterType<IDictionaryDataService, DictionaryDataService>();
            container.RegisterType<ILoanDealDataService, LoanDealDataService>();
            container.RegisterType<IContragentDataService, ContragentDataService>();

            HR.Client.Bootstraper.Register(container);

        }
    }
}
