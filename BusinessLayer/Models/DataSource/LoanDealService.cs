using DataAccessLayer.Interfaces;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models.DataSource
{
    public class LoanDealService
    {
        private readonly ILoanDealDataService  _service;
        private DataAccessLayer.LoanDealSoapWebService.getLoanDealsItem _loanDeal = null;

        public LoanDealService()
        {
            _service = ServiceLocator.Current.GetInstance<ILoanDealDataService>();
        }

        public string DealNo { get; set; }

        public string ContragentId
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).Wait();
                }

                return _loanDeal?.contragentId;
            }
        }
        
        public DateTime? ContractStartDate
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).Wait();
                }

                return _loanDeal?.valueDate;
            }
        }

        public string ProductType
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).Wait();
                }

                return _loanDeal?.productCode;
            }
        }

        public string ProductSubType
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).Wait();
                }

                return _loanDeal?.productCode;
            }
        }

        public string ContractCurrency
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).Wait();
                }

                return _loanDeal?.currencyId;
            }
        }

        public string ContractID
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    _loanDeal = Task.Run(() =>
                    {
                        return _service.getLoanDealsByDealNo(DealNo)[0];
                    }).Result;
                }

                return _loanDeal?.dealId;
            }
        }

        public DateTime? ApplicationDate
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).GetAwaiter().GetResult();
                }
                
                return _loanDeal?.agreementDate;
            }
        }

        public string ContractBranchOffice
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).GetAwaiter().GetResult();
                }


                return _loanDeal?.toboCode;
            }
        }

        public string CreatorB2ID
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).GetAwaiter().GetResult();
                }

                return _loanDeal?.userName;
            }
        }

        public DateTime? ContractDate
        {
            get
            {
                if (string.IsNullOrEmpty(DealNo))
                    throw new NullReferenceException("dealNo isn't specified");

                if (_loanDeal == null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        _loanDeal = _service.getLoanDealsByDealNo(DealNo)[0];
                    }).GetAwaiter().GetResult();
                }

                return _loanDeal?.agreementDate;
            }
        }
    }
}
