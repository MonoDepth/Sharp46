using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp46.SMS
{
    public class SmsHistoryResponse
    {
        public List<Sms> Data { get; set; } = new();
        public string? Next { get; set; }
    }
}
