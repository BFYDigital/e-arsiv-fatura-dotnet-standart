using System.Threading.Tasks;

namespace BFY.Fatura.Services
{
    public interface IHttpServices<T>
    {
        T DispatchCommand(string command, string pageName, object data, bool encodeUrl);
        T DispatchCommand(string command, string pageName, object data);
        T DispatchCommand(string command, string pageName);
        Task<T> Login();
        Task<bool> Logout(string token);
    }
}