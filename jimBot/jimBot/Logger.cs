using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace jimBot {
    public class Logger {
        private const string LoggerLogPath = @"..\\..\\..\\logs\\";
        private static string lastError = string.Empty;
        private static string logType = "LOG";
        private static string defaultFileName = string.Empty;
        private static string defaultFilePath = string.Empty;

        public static string Log(string type = "", string str = "") {
            string filePath = string.Empty;
            string date = Helpers.GetDate("dd.MM.yyy HH:mm::ss");

            if (type.Equals("")) {
                Logger.Log("ERROR", "No logger type");
                return Logger.lastError;
            }

            Logger.logType = type;

            if (str.Equals(string.Empty)) {
                Logger.Log("ERROR", "Empty logger string");
                return Logger.lastError;
            }

            str = date + " " + type + " - " + str;

            switch (type) {
                case "ERROR":
                    Logger.lastError = str;
                    str += "\r\nBug trace\r\n";
                    StackTrace trace = new StackTrace();
                    str += trace.ToString();
                    break;
                case "INFO":
                    break;
                default:
                    Logger.Log("ERROR", "Unknown type for log");
                    return Logger.lastError;
            }

            //str += "\r\n";

            filePath = Logger.getFile();
            
            Logger.LogWriter(filePath, str);
            Logger.LogWriter(defaultFilePath, str);

            return string.Empty;
        }

        private static string getFile () {
            string date = Helpers.GetDate("dd.MM.yyy");
            Logger.defaultFileName = date + "_loggerLog";
            Logger.defaultFilePath = LoggerLogPath + defaultFileName;
            string fileName = string.Empty;

            fileName = Logger.PrepareFolder();

            return fileName;
        }

        private static string PrepareFolder(int newIndex = 0, bool needCreate = false) {
            if (needCreate) {
                using (var stream = File.Create(LoggerLogPath + defaultFileName + "_" + newIndex + ".txt")) {
                    stream.Close();
                }
            }

            List<string> files = Directory
                .GetFiles(LoggerLogPath, "*", SearchOption.AllDirectories)
                .ToList();
            int fileCount = files.Count();
            int fileIndex = 0;

            if(fileCount > 0) {
                fileIndex = fileCount - 1;
            }

            string filePath = LoggerLogPath + defaultFileName + "_" + fileIndex + ".txt";

            if (!File.Exists(filePath)) {
                using (var stream = File.Create(filePath)) {
                    stream.Close();
                }
                files.Add(defaultFileName + "_" + fileIndex + ".txt");
            }

            long fileSize = new FileInfo(filePath).Length;

            if (fileSize > (30 * 1024)) {
                PrepareFolder((fileIndex + 1), true);
            }

            return filePath;
        }

        private static void LogWriter (string filePath, string str) {
            FileInfo fileInf = new FileInfo(filePath);

            if (fileInf.Exists) {
                try {
                    using (StreamWriter sw = new StreamWriter(filePath, true, System.Text.Encoding.Default)) {
                        sw.WriteLine(str);
                    }
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
