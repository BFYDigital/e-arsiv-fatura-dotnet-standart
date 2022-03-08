using System.Threading.Tasks;

namespace BFY.Fatura.Commands
{
    public interface ICommandDispatcher<T>
    {
        string CommandName { get; }
        string PageName { get; }
        object Data { get; set; }

        Task<T> Dispatch();
    }
}
