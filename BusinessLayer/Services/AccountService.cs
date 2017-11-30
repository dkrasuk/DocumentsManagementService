using BusinessLayer.Interfaces;
using DataAccessLayer.Interfaces;
using HR.Client.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models.Request;
using BusinessLayer.Models.Response;
using AutoMapper;

namespace BusinessLayer.Services
{
    public class AccountService : IAccountService
    {
        IUserService _userService;
        IPermissionService _permissionService;

        public AccountService(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        public bool Authenticate(string login)
        {
            var _login = login.Replace("alfa\\", "");

            return _userService.IsUserLoginExist(login);
        }

        public bool Authorize(string login, string permissions)
        {
            if (string.IsNullOrEmpty(permissions) || string.IsNullOrEmpty(login))
                throw new UnauthorizedAccessException("User not authorized");

            var userPermissions = permissions.Split(',').ToList();
            
            if (userPermissions.Count() > 0)
            {
                var res = userPermissions.Any(x=> _permissionService.UserHasAccess(login, x));

                return res;
            }

            return false;
        }

        public PermissionsResponse GetUserPermissions(string login, PermissionsRequest request)
        {
            if (request == null || string.IsNullOrEmpty(login))
                throw new ArgumentNullException("GetUserPermissions. Input parameters is missing"); 
            
            Type tmp = request.GetType();

            foreach (var prop in tmp.GetProperties())
            {
                System.Boolean res = _permissionService.UserHasAccess(login, prop.Name);

                var property = tmp.GetProperty(prop.Name);

                if (property != null)
                {
                    property.SetValue(request, res, null);
                }               
            }

            var result = Mapper.Map<PermissionsResponse>(request);

            return result;
        }
    }
}
