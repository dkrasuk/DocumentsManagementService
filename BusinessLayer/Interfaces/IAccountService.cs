using BusinessLayer.Models.Request;
using BusinessLayer.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IAccountService
    {
        bool Authenticate(string login);
        bool Authorize(string login, string permissions);
        PermissionsResponse GetUserPermissions(string login, PermissionsRequest request);
    }
}
