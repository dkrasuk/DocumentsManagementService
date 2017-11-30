using AutoMapper;

namespace BusinessLayer.Configuration
{
    public static class AutomapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<DataAccessLayer.Models.BaseDictionaryItem, Models.DictionaryItem>();
                cfg.CreateMap<DataAccessLayer.Models.DocumentTypeAttributeItem, Models.DocumentTypeAttributeItem>();
                cfg.CreateMap<DataAccessLayer.Models.DataSourceObjectRequiredParameters, Models.DataSourceObjectRequiredParameters>();
                cfg.CreateMap<DataAccessLayer.Models.DataSourceObject, Models.DataSourceObject>();
                cfg.CreateMap<DataAccessLayer.Models.AttributeParameter, Models.AttributeParameter>();
                cfg.CreateMap<DataAccessLayer.Models.SearchAttribute, Models.SearchAttribute>();
                cfg.CreateMap<DataAccessLayer.Models.DisplayParameter, Models.DisplayParameter>();
                cfg.CreateMap<DataAccessLayer.DocumentManagement2SoapService.AttributeTypeWithAnyAttribute, DataAccessLayer.DocumentManagement2SoapService.AttributeTypeAttribute>();
                cfg.CreateMap<Models.AdditionalParametersRequest, Models.AdditionalParameters>();
                cfg.CreateMap<Models.Request.PermissionsRequest, Models.Response.PermissionsResponse>();
            });
        }
    }
}
