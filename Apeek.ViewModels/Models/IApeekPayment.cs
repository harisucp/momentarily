using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Apeek.Core.Services
{
    internal interface IApeekPayment
    {
        int GoodId { get; set; }
        string Type { get;set;}
    }
}
