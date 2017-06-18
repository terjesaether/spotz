using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Extensionmethods
{
    public static class ExtensionMethods
    {
        public static string CapitalizeFirstLetter(this string input)
        {
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}