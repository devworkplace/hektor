using CrudGeneratorTest.Models.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace CrudGeneratorTest.Models.DAL
{
    public static class Util
    {
        public static string CONNECTION_STRING = WebConfigurationManager.ConnectionStrings["TesteGerador"].ConnectionString;
        //public static string CONNECTION_STRING = WebConfigurationManager.ConnectionStrings["SETI"].ConnectionString;
    }
}