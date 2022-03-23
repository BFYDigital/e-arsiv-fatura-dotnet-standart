using BFY.Fatura.Configuration;

namespace BFY.Fatura.Commands
{
    class GetSignSMSCodeCommand<T> : CommandDispatcherBase<T>
    {
        public GetSignSMSCodeCommand(IFaturaServiceConfiguration configuration) : base(configuration)
        {
            CommandName = "EARSIV_PORTAL_TELEFONNO_SORGULA";
            PageName = "RG_SMSONAY";
        }
    }
}
