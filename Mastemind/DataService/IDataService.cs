using System.Collections.Generic;
using System.ServiceModel;
using DataService.Models;

namespace DataService
{
    [ServiceContract]
    public interface IDataService
    {
        [OperationContract]
        IEnumerable<Statistic> GetTop(int n);

        [OperationContract]
        void AddResult(Statistic stat);
    }
}
