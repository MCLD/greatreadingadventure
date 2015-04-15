using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Microsoft.ApplicationBlocks.Data;
using STG.SRP.DAL;

namespace STG.SRP
{
    public class STGOnlyUtilities
    {
         
        public static string PasswordStrengthRE() { return "(?=^.{7,}$)(?=.*\\d)(?=.*[a-zA-Z]+)(?![.\\n]).*$"; }
        public static string PasswordStrengthError() { return "New Password must be at least seven characters in length and contain one alpha and one numeric character.<br/>"; }
         
        public static void LogoffPatron(int PID)
        {
           //????  
        }


        

    }
}