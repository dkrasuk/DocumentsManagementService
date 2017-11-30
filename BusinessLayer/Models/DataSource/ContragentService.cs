using DataAccessLayer.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models.DataSource
{
    public class ContragentService
    {
        private readonly IContragentDataService  _service;
        private DataAccessLayer.ContragentSoapWebService.contragent[] _contragent = null;

        public ContragentService()
        {
            _service = ServiceLocator.Current.GetInstance<IContragentDataService>();
        }

        public string ContragentId { get; set; }
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(ContragentId))
                    throw new NullReferenceException("ContragentId isn't specified");

                if (_contragent == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _contragent = _service.getContarentId(ContragentId);
                    }).GetAwaiter().GetResult();                    
                }

                return _contragent?[0].nameUkr;
            }
        }

        public string ShortName
        {
            get
            {
                if (string.IsNullOrEmpty(ContragentId))
                    throw new NullReferenceException("ContragentId isn't specified");

                if (_contragent == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _contragent = _service.getContarentId(ContragentId);
                    }).GetAwaiter().GetResult();
                }

                return _contragent?[0].shortNameUkr;
            }
        }

        public string ContragentID
        {
            get
            {
                if (string.IsNullOrEmpty(ContragentId))
                    throw new NullReferenceException("ContragentId isn't specified");

                if (_contragent == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _contragent = _service.getContarentId(ContragentId);
                    }).GetAwaiter().GetResult();
                }

                return _contragent?[0].contragentId;
            }
        }

        public DateTime? ContractorSetDate
        {
            get
            {
                if (string.IsNullOrEmpty(ContragentId))
                    throw new NullReferenceException("ContragentId isn't specified");

                if (_contragent == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _contragent = _service.getContarentId(ContragentId);
                    }).GetAwaiter().GetResult();
                }

                return _contragent?[0].bankRegDate;
            }
        }

        public string ContragentTaxCode
        {
            get
            {
                if (string.IsNullOrEmpty(ContragentId))
                    throw new NullReferenceException("ContragentId isn't specified");

                if (_contragent == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _contragent = _service.getContarentId(ContragentId);
                    }).GetAwaiter().GetResult();
                }

                return _contragent?[0].taxId;
            }
        }

        public string Tobo
        {
            get
            {
                if (string.IsNullOrEmpty(ContragentId))
                    throw new NullReferenceException("ContragentId isn't specified");

                if (_contragent == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _contragent = _service.getContarentId(ContragentId);
                    }).GetAwaiter().GetResult();
                }

                return _contragent?[0].accountOfficer;
            }
        }
    }
}
