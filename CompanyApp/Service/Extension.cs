using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyApp.Service
{
    public static class Extensions
    {
        // метод расширения для string => во входной строке будет вырезаться слово Controller
        public static string CutController(this string str)
        {
            return str.Replace("Controller", "");
        }
    }
}
