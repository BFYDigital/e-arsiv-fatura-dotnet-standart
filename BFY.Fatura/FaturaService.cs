using BFY.Fatura.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BFY.Fatura.Models;
using BFY.Fatura.Services;
using BFY.Fatura.Commands;
using System.Text;
using System.Globalization;

namespace BFY.Fatura
{
    public class FaturaService
    {
        private const string DATE_FORMAT = "dd/MM/yyyy";
        protected IFaturaServiceConfiguration _configuration;

        public FaturaService() : this(FaturaServiceConfigurationFactory.Create()) { }

        public FaturaService(IFaturaServiceConfiguration configuration)
        {
            _configuration = configuration;
            // GetToken();
        }

        public void GetToken()
        {
            var httpServices = new HttpServices<LoginModel>(_configuration);
            LoginModel loginModel = httpServices.Login().GetAwaiter().GetResult();

            _configuration.Token = loginModel.Token;
        }
        public bool Logout(string token)
        {
            var httpServices = new HttpServices<bool>(_configuration);
            var result = httpServices.Logout(token).GetAwaiter().GetResult();

            return result;
        }

        public DraftInvoiceResponseModel CreateDraftInvoice(InvoiceDetailsModel invoiceDetails)
        {
            var data = new DraftInvoiceModel()
            {
                aliciAdi = invoiceDetails.name,
                aliciSoyadi = invoiceDetails.surname,
                aliciUnvan = invoiceDetails.title,
                faturaTarihi = invoiceDetails.date,
                saat = invoiceDetails.time,
                vknTckn = invoiceDetails.taxIDOrTRID,
                vergiDairesi = invoiceDetails.taxOffice,
                matrah = invoiceDetails.grandTotal.ToString("F2").Replace(",", "."),
                malhizmetToplamTutari = invoiceDetails.grandTotal.ToString("F2").Replace(",", "."),
                hesaplanankdv = invoiceDetails.totalVAT.ToString("F2").Replace(",", "."),
                vergilerToplami = invoiceDetails.totalVAT.ToString("F2").Replace(",", "."),
                vergilerDahilToplamTutar = invoiceDetails.grandTotalInclVAT.ToString("F2").Replace(",", "."),
                odenecekTutar = invoiceDetails.paymentTotal.ToString("F2").Replace(",", "."),
                bulvarcaddesokak = invoiceDetails.fullAddress
            };

            for (int i = 0; i < invoiceDetails.items.Count; i++)
            {
                data.malHizmetTable.Add(new MalHizmetTableModel(invoiceDetails.items[i]));
            }

            var command = new CreateDraftInvoiceCommand<DraftInvoiceResponseModel>(_configuration) { Data = data };

            DraftInvoiceResponseModel response = command.Dispatch();
            response.date = data.faturaTarihi;
            response.uuid = data.faturaUuid;

            return response;
        }

        public FoundDraftInvoiceResponseModel GetAllInvoicesByDateRange(DateTime start, DateTime end)
        {
            return GetAllInvoicesByDateRange(start.ToString(DATE_FORMAT).Replace(".", "/"), end.ToString(DATE_FORMAT).Replace(".", "/"));
        }

        public FoundDraftInvoiceResponseModel GetAllInvoicesByDateRange(string start, string end)
        {
            // InvoicesByDateRangeCommand
            var command = new InvoicesByDateRangeCommand<FoundDraftInvoiceResponseModel>(_configuration)
            {
                Data = new { baslangic = start, bitis = end, hangiTip = "5000/30000" }
            };

            FoundDraftInvoiceResponseModel response = command.Dispatch();
            return response;
        }

        public FoundDraftInvoiceModel FindDraftInvoice(DateTime date, string uuid)
        {
            return FindDraftInvoice(date.ToString(DATE_FORMAT), uuid);
        }

        public FoundDraftInvoiceModel FindDraftInvoice(string date, string uuid)
        {
            var invoices = GetAllInvoicesByDateRange(date, date);
            for (int i = 0; i < invoices.data.Count; i++)
            {
                if (invoices.data[i].ettn == uuid)
                {
                    return invoices.data[i];
                }
            }
            return null;
        }

        public SignDraftInvoiceModel SignDraftInvoice(FoundDraftInvoiceModel invoice)
        {
            var command = new SignDraftInvoiceCommand<SignDraftInvoiceModel>(_configuration)
            {
                Data = new { imzalanacaklar = invoice }
            };

            SignDraftInvoiceModel signedInvoice = command.Dispatch();
            return signedInvoice;
        }

        public GIBResponseModel<string> GetInvoiceHTML(string uuid)
        {
            return GetInvoiceHTML(uuid, true);
        }

        public GIBResponseModel<string> GetInvoiceHTML(string uuid, bool signed)
        {
            var command = new GetInvoiceHTMLCommand<GIBResponseModel<string>>(_configuration)
            {
                Data = new { ettn = uuid, onayDurumu = signed ? "Onaylandı" : "Onaylanmadı" }
            };

            GIBResponseModel<string> html = command.Dispatch();
            return html;
        }

        public string GetDownloadURL(string uuid, bool signed)
        {
            string signStatus = System.Net.WebUtility.UrlEncode(signed ? "Onaylandı" : "Onaylanmadı");
            return $"{_configuration.BaseUrl}/earsiv-services/download?token={_configuration.Token}&ettn={uuid}&belgeTip=FATURA&onayDurumu={signStatus}&cmd=downloadResource";
        }

        public CreatedInvoiceModel CreateInvoice(InvoiceDetailsModel invoiceDetails)
        {
            return CreateInvoice(invoiceDetails, true);
        }

        public CreatedInvoiceModel CreateInvoice(InvoiceDetailsModel invoiceDetails, bool signInvoice)
        {
            DraftInvoiceResponseModel draftInvoice = CreateDraftInvoice(invoiceDetails);
            FoundDraftInvoiceModel invoice = FindDraftInvoice(draftInvoice.date, draftInvoice.uuid);

            if (signInvoice) { SignDraftInvoice(invoice); }

            return new CreatedInvoiceModel()
            {
                uuid = draftInvoice.uuid,
                signed = signInvoice
            };
        }

        public string CreateInvoiceAndGetDownloadURL(InvoiceDetailsModel invoiceDetails, bool signInvoice)
        {
            CreatedInvoiceModel invoice = CreateInvoice(invoiceDetails, signInvoice);
            return GetDownloadURL(invoice.uuid, invoice.signed);
        }

        public GIBResponseModel<string> CreateInvoiceAndGetHTML(InvoiceDetailsModel invoiceDetails, bool signInvoice)
        {
            CreatedInvoiceModel invoice = CreateInvoice(invoiceDetails, signInvoice);
            return GetInvoiceHTML(invoice.uuid, invoice.signed);
        }

        public GIBResponseModel<string> CancelDraftInvoice(FoundDraftInvoiceModel invoice, string reason)
        {
            // todo: determine the proper response type
            var command = new CancelDraftInvoiceCommand<GIBResponseModel<string>>(_configuration)
            {
                Data = new { silinecekler = invoice, aciklama = reason }
            };

            return command.Dispatch();
        }

        public GIBResponseModel<List<RecipientModel>> GetRecipientDataByTaxIDOrTRID(long taxId)
        {
            var data = new { vknTcknn = taxId };
            var command = new GetRecipientDataByIDCommand<GIBResponseModel<List<RecipientModel>>>(_configuration) { Data = data };

            return command.Dispatch();
        }

        public GIBResponseModel<SMSCodeResponseModel> SendSignSMSCode()
        {
            var dataTel = new { };
            var command = new GetSignSMSCodeCommand<GIBResponseModel<SMSPhoneResponseModel>>(_configuration) { Data = dataTel };
            GIBResponseModel<SMSPhoneResponseModel> responseTel = command.Dispatch();

            if(!string.IsNullOrEmpty(responseTel.data.telefon))
            {
                var data = new { CEPTEL = responseTel.data.telefon, KCEPTEL = false, TIP = string.Empty };
                var command2 = new SendSignSMSCodeCommand<GIBResponseModel<SMSCodeResponseModel>>(_configuration) { Data = data };

                GIBResponseModel<SMSCodeResponseModel> response = command2.Dispatch();
                return response;
            } else
            {
                GIBResponseModel<SMSCodeResponseModel> response = new GIBResponseModel<SMSCodeResponseModel>()
                {
                    message = "Telefon numarası sistemden alınamadı.",
                    status = false
                };
                return response;
            }
        }

        public async Task<GIBResponseModel<SMSCodeResponseModel>> VerifySignSMSCode(Models.Sms.SMSCodeInputModel input)
        {
            var command = new VerifySignSMSCodeCommand<GIBResponseModel<SMSCodeResponseModel>>(_configuration) { Data = input };

            GIBResponseModel<SMSCodeResponseModel> response = command.Dispatch();
            return response;
        }

        public UserModel GetUserData()
        {
            var command = new GetUserDataCommand<UserModelDTO>(_configuration) { Data = new { } };
            UserModelDTO response = command.Dispatch();

            return new UserModel(response);
        }

        public UserModel UpdateUserData(UserModel user)
        {
            var command = new UpdateUserDataCommand<UserModelDTO>(_configuration) { Data = user };
            UserModelDTO response = command.Dispatch();

            return new UserModel(response);
        }
    }
}
