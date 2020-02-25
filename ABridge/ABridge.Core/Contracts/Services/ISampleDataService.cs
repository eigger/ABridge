using System.Collections.Generic;
using System.Threading.Tasks;

using ABridge.Core.Models;

namespace ABridge.Core.Contracts.Services
{
    public interface ISampleDataService
    {
        Task<IEnumerable<SampleOrder>> GetMasterDetailDataAsync();
    }
}
