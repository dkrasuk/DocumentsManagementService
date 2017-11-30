using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using BusinessLayer.Configuration;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;

namespace BusinessLayer
{
    public static class Bootstraper
    {
        public static void Register(IUnityContainer container)
        {
            DataAccessLayer.Bootstraper.Register(container);
            Logger.Bootstraper.Register(container);
            AutomapperConfig.RegisterMappings();

            container.RegisterType<IDocumentManagementService, DocumentManagementService>();
            container.RegisterType<IDictionaryService, DictionaryService>();
            container.RegisterType<IAccountService, AccountService>();
        }        
    }
}
