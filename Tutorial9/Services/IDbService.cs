using Azure.Core;
using Tutorial9.Model;

namespace Tutorial9.Services;

public interface IDbService
{
    
    Task<int> DoRequestAsync(RequestDTO request);
    Task<int> DoRequestProcedureAsync(RequestDTO request);
}