using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using DataService.Models;

namespace AndroidDemo.ServiceReferences
{
    public class DataServiceClientWrapper
    {
        private static DataServiceClient _client;
        private static readonly EndpointAddress _endPoint = new EndpointAddress("http://projektwcf.azurewebsites.net/DataService.svc/DataService");
        public Exception Error { get; set; }

        public DataServiceClientWrapper()
        {
            if (_client == null || _client.State == CommunicationState.Faulted)
                _client = new DataServiceClient(BasicHttpBinding(), _endPoint);
            _client.GetTopCompleted += DataServiceClient_GetTopCompleted;
            _client.AddResultCompleted += DataServiceClient_AddResultCompleted;
        }

        private static BasicHttpBinding BasicHttpBinding()
        {
            var binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };
            var timeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            return binding;
        }

        private bool _getTopCompleted = true;
        private Statistic[] _topNStatistics;
        private void DataServiceClient_GetTopCompleted(object sender, GetTopCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                HandleErrors(e);

                _topNStatistics = null;
                _getTopCompleted = true;
                return;
            }

            _topNStatistics = e.Result.ToArray();
            _getTopCompleted = true;
        }

        public IEnumerable<Statistic> GetTop(int n = 100)
        {
            _topNStatistics = null;
            _getTopCompleted = false;
            
            _client.GetTopAsync(n);
            while (!_getTopCompleted)
                Task.Delay(100).Wait();
            return _topNStatistics;
        }

        private bool _addResultCompleted = true;
        private void DataServiceClient_AddResultCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                HandleErrors(e);
                _addResultCompleted = true;
                return;
            }

            _addResultCompleted = true;
        }

        public void AddResult(Statistic stat)
        {
            _addResultCompleted = false;
            _client.AddResultAsync(stat);
            while (!_addResultCompleted)
                Task.Delay(100).Wait();
        }

        private void HandleErrors(AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                Error = new Exception("Anulowano");
            else if (e.Error is EndpointNotFoundException)
                Error = new EndpointNotFoundException("Serwis nie odpowiada");
            else
                Error = e.Error;
        }
    }
}
