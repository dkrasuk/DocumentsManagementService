using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.App_Start
{
    public static class Bootstraper
    {
        public static void Register(IUnityContainer container)
        {
            BusinessLayer.Bootstraper.Register(container);
        }
    }
}