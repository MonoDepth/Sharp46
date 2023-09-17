using Sharp46.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp46.Rest
{
    public interface IRestClient: IDisposable
    {
        public Task<RequestResponse> GetAsync(string endpoint, params object[] queryParams);        
        public Task<RequestResponse> PostAsync(string endpoint, IFormRequest body);
        public Task<RequestResponse> DeleteAsync(string endpoint);
        public Task<RequestResponse> PutAsync(string endpoint, IFormRequest body);
        public Task<RequestResponse> PatchAsync(string endpoint, IFormRequest body);
    }
}
