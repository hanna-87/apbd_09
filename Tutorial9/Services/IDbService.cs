using Azure.Core;
using Tutorial9.Model;

namespace Tutorial9.Services;

public interface IDbService
{
    Task DoSomethingAsync();
    Task ProcedureAsync();
    Task<int> DoRequestAsync(RequestDTO request);
    Task<int> DoRequestProcedureAsync(RequestDTO request);
}