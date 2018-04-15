using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWindSoft.Data
{
    public class BaseBll<T> : SingleInstance<T> where T:new()
    {
    }
}
