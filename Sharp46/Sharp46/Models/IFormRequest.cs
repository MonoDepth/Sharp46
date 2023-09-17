using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp46.Models
{
    public interface IFormRequest
    {
        public FormUrlEncodedContent ToFormEncoded();
    }
}
