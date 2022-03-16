using BFY.Fatura.Configuration;

namespace BFY.Fatura.Commands
{
    class VerifySignSMSCodeCommand<T> : CommandDispatcherBase<T>
    {
        public VerifySignSMSCodeCommand(IFaturaServiceConfiguration configuration) : base(configuration)
        {
            CommandName = "0lhozfib5410mp";
            PageName = "RG_SMSONAY";
        }
    }
}
