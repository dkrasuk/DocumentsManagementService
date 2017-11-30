using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Services;
using log4net.Repository.Hierarchy;
using DataAccessLayer.Interfaces;
using BusinessLayer.Interfaces;
using Moq;
using Logger;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Tests
{
    [TestClass()]
    public class DocumentManagementServiceTests
    {
        Mock<IDocumentManagementDataService> _service;
        Mock<ILogger> _logger;
        Mock<IDictionaryService> _dictionary;
        Dictionary<string, object> _dataObjects;
        Dictionary<string, object> _documentTypeAttributes;
        DocumentManagementService _documentManagementService;
        List<Models.AdditionalParametersRequest> parameters;
        DocumentTypeAttributeItem attr;

        [TestInitialize]
        public void Initialize()
        {
            InitTestData();
            InitMocks();
        }

        private void InitMocks()
        {
            _dictionary = new Mock<IDictionaryService>();
            _logger = new Mock<ILogger>();
            _service = new Mock<IDocumentManagementDataService>();
            _documentManagementService = new DocumentManagementService(_service.Object, _logger.Object, _dictionary.Object);
        }

        private void InitTestData()
        {
            _dataObjects = new Dictionary<string, object>();
            parameters = new List<Models.AdditionalParametersRequest>();
            attr = new DocumentTypeAttributeItem();
            
        }

        //[TestMethod()]
        //public void getDocumentAttributeListWithDataAsyncTest()
        //{
           
        //    Assert.Fail();
        //}



        [TestMethod()]
        public void ValidateParametersAsyncReturnFalseIfParametersareNotFull()
        {
            parameters = new List<AdditionalParametersRequest>();
            attr = new DocumentTypeAttributeItem();
            attr.DataSourceObjects = new List<DataSourceObject>();
            string documentType = "documentType";
            for (int i = 0; i < 2; i++)
            {
                parameters.Add(
                   new AdditionalParametersRequest() { Name = "Name" + i, Value = "Value"+i });
            }
            List<DataSourceObject> dataSourceObjects = new List<DataSourceObject>();
            for (int i = 0; i < 4; i++)
            {
                
                List<DataSourceObjectRequiredParameters> requiredParameters = new List<DataSourceObjectRequiredParameters>();
                for (int j = 0; j < 4; j++)
                {
                    requiredParameters.Add(new DataSourceObjectRequiredParameters() { InputParameterName = i + "InputParameterName" + j, PropertyName = i + "PropertyName" + j });
                }
                dataSourceObjects.Add(new DataSourceObject() { Name = "Name" + i, RequiredParameters = requiredParameters });
                
            }
            attr.DataSourceObjects = dataSourceObjects;
            List<AttributeParameter> attributeParameters = new List<AttributeParameter>();
            attr.Attributes = attributeParameters;

            _dictionary.Setup(x => x.getDocumentTypeAttributesAsync(It.IsAny<string>())).ReturnsAsync(attr);
           var res =  _documentManagementService.ValidateParametersAsync(documentType,parameters).Result;
           Assert.IsFalse(res);


        }
        [TestMethod()]
        public void ValidateParametersAsyncReturnTrueIfParametersareFull()
        {
            parameters = new List<AdditionalParametersRequest>();
            attr = new DocumentTypeAttributeItem();
            attr.DataSourceObjects = new List<DataSourceObject>();
            string documentType = "documentType";
            for (int i = 0; i < 4; i++)
            {
                parameters.Add(
                   new AdditionalParametersRequest() { Name = "Name" + i, Value = "Value" + i });
            }
            List<DataSourceObject> dataSourceObjects = new List<DataSourceObject>();
            for (int i = 0; i < 4; i++)
            {

                List<DataSourceObjectRequiredParameters> requiredParameters = new List<DataSourceObjectRequiredParameters>();
                for (int j = 0; j < 4; j++)
                {
                    requiredParameters.Add(new DataSourceObjectRequiredParameters() { InputParameterName = "Name" + j, PropertyName = "Name" + j });
                }
                dataSourceObjects.Add(new DataSourceObject() { Name = "Name" + i, RequiredParameters = requiredParameters });

            }
            attr.DataSourceObjects = dataSourceObjects;
            List<AttributeParameter> attributeParameters = new List<AttributeParameter>();
            attr.Attributes = attributeParameters;
            _dictionary.Setup(x => x.getDocumentTypeAttributesAsync(It.IsAny<string>())).ReturnsAsync(attr);
            var res = _documentManagementService.ValidateParametersAsync(documentType, parameters).Result;
            Assert.IsTrue(res);


        }

    }
}