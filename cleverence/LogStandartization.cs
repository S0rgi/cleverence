using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cleverence
{
    public class LogEntry
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string LogLevel { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
    }
    public static class LogStandartization
    {
        private static string logsPath = "C:\\Users\\ilyux\\source\\repos\\cleverence\\cleverence\\logs.txt";
        private static string errorsPath = "C:\\Users\\ilyux\\source\\repos\\cleverence\\cleverence\\errors.txt";
        public static (string, bool) Standartization(string logEntry)
        {
            //Первый формат
            var match = Regex.Match(logEntry, @"(\d{2}\.\d{2}\.\d{4}) (\d{2}:\d{2}:\d{2}\.\d{3}) (\w+) (.*)");

            if ( match.Success )
            {
                var date = match.Groups[1].Value.Replace('.', '-'); ;
                var time = match.Groups[2].Value;
                var logLevel = match.Groups[3].Value;
                if ( logLevel == "INFORMATION" )
                    logLevel = "INFO";
                else if ( logLevel == "WARNING" )
                    logLevel = "WARN";
                var methodAndMessage = match.Groups[4].Value;

                string method = "DEFAULT";
                string message = methodAndMessage;

                var words = methodAndMessage.Split(' ');
                if ( words.Length > 0 )
                {
                    method = words[0] == "" ? "DEFAULT" : words[0];
                    message = string.Join(" ", words, 1, words.Length - 1);
                }

                var parsedEntry = new LogEntry
                {
                    Date = date,
                    Time = time,
                    LogLevel = logLevel,
                    Method = method,
                    Message = message
                };
                return ($"{parsedEntry.Date}\t{parsedEntry.Time}\t{parsedEntry.LogLevel}\t{parsedEntry.Method}\t{parsedEntry.Message}", true);
            }
            else
            {
                //Второй формат
                match = Regex.Match(logEntry, @"(\d{4}-\d{2}-\d{2}) (\d{2}:\d{2}:\d{2}\.\d{4})\| (\w+)\|\d+\|(.*)\| (.*)");

                if ( match.Success )
                {
                    var date = match.Groups[1].Value;
                    var time = match.Groups[2].Value;
                    var logLevel = match.Groups[3].Value;
                    if ( logLevel == "INFORMATION" )
                        logLevel = "INFO";
                    else if ( logLevel == "WARNING" )
                        logLevel = "WARN";
                    var method = match.Groups[4].Value == "" ? "DEFAULT" : match.Groups[4].Value;
                    var message = match.Groups[5].Value;

                    var parsedEntry = new LogEntry
                    {
                        Date = date,
                        Time = time,
                        LogLevel = logLevel,
                        Method = method,
                        Message = message
                    };

                    return ($"{parsedEntry.Date}\t{parsedEntry.Time}\t{parsedEntry.LogLevel}\t{parsedEntry.Method}\t{parsedEntry.Message}", true);
                }
                //если запись не подошла ни на один формат
                else
                    return (logEntry, false);
            }
        }


        private static void Output(string logsContent, bool status)
        {
            if ( status )
            {
                File.AppendAllText(logsPath, logsContent + Environment.NewLine);
                Console.WriteLine($"added new log : \" {logsContent} \"");
            }
            else
            {
                File.AppendAllText(errorsPath, logsContent + Environment.NewLine);
                Console.WriteLine($"added new error: \" {logsContent} \" ");
            }
        }
        private static void Input()
        {
            string filePath = "C:\\Users\\ilyux\\source\\repos\\cleverence\\cleverence\\InputLogs.txt";
            using ( var reader = new StreamReader(filePath) )
            {
                string line;
                while ( ( line = reader.ReadLine() ) != null )
                {
                    var result = Standartization(line);
                    Output(result.Item1, result.Item2);
                }
            }
        }
        private static void ClearLogs()
        {
            File.WriteAllText(logsPath, string.Empty);
            File.WriteAllText(errorsPath, string.Empty);
            Console.WriteLine("logs cleared");
        }
        static void Main()
        {
            ClearLogs();
            Input();
        }
    }

}

