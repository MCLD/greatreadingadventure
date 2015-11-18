using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SRPApp.Classes {
    public class StringResources {

        public static void LoadProgramResourceFile(string pid) {
            var file = HttpContext.Current.Server.MapPath("~/resources/program." + pid.ToString() + ".en-US.txt");
            var lines = File.ReadAllLines(HttpContext.Current.Server.MapPath("~/resources/program.default.en-US.txt"));
            try { lines = File.ReadAllLines(file); } catch { }

            var strings = new Hashtable();
            foreach(var line in lines) {
                var l = line.Trim();
                if(!string.IsNullOrEmpty(l) && !l.StartsWith(";")) {
                    try {
                        var key = l.Substring(0, l.IndexOf("="));
                        var value = l.Substring(l.IndexOf("=") + 1);
                        strings.Add(key, value);
                    } catch(Exception) {
                    }
                }
            }

            Hashtable allStrings = null;
            if(HttpContext.Current.Application["ProgramStrings"] == null) {
                allStrings = new Hashtable();
            } else {
                allStrings = (Hashtable)HttpContext.Current.Application["ProgramStrings"];
            }

            if(allStrings.Contains(pid)) {
                allStrings.Remove(pid);
            }
            allStrings.Add(pid, strings);
            HttpContext.Current.Application.Lock();
            HttpContext.Current.Application["ProgramStrings"] = allStrings;
            HttpContext.Current.Application.UnLock();
        }

        public static string getString(string pid, string key) {
            Hashtable allStrings = null;
            if(HttpContext.Current.Application["ProgramStrings"] == null) {
                LoadProgramResourceFile(pid);
            }
            allStrings = (Hashtable)HttpContext.Current.Application["ProgramStrings"];
            var strings = (Hashtable)allStrings[pid];
            if(strings == null) {
                LoadProgramResourceFile(pid);
                allStrings = (Hashtable)HttpContext.Current.Application["ProgramStrings"];
                strings = (Hashtable)allStrings[pid];
            }
            if(strings.ContainsKey(key)) {
                return strings[key].ToString();
            }
            return key;
        }

        public static string getString(string key) {
            string pid = "default";
            if(HttpContext.Current.Session["ProgramID"] != null) {
                pid = HttpContext.Current.Session["ProgramID"].ToString();
            }

            // Comment the next line to use default instead of real program strings
            // pid = "default";

            return getString(pid, key);
        }
    }
}