using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SRPApp.Classes {
    public class StringResources {
        private const string ResourcesPath = "~/Resources/program.{0}.en-US.txt";
        public static void LoadProgramResourceFile(string pid) {
            string[] lines = null;
            string resourcesPath = HttpContext.Current.Server.MapPath(string.Format(ResourcesPath, pid));
            bool success = true;
            if(File.Exists(resourcesPath)) {
                try {
                    lines = File.ReadAllLines(resourcesPath);
                } catch(Exception) {
                    success = false;
                }
            }

            if(!success || lines == null || lines.Length == 0) {
                resourcesPath = HttpContext.Current.Server.MapPath(string.Format(ResourcesPath, "default"));
                lines = File.ReadAllLines(resourcesPath);
            }

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