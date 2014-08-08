using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AutoTest.Messages;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpUnitParseErrorParser
    {
        public static TestResult Parse(string error) {
            try {
                var parsedTtest = parse(error);
                if (parsedTtest  != null)
                    return parsedTtest  ;
            } catch {
            }
            var test =
                new TestResult(
                    TestRunner.PhpParseError,
                    TestRunStatus.Failed,
                    trimMessage(error));
            test.Message = trimMessage(error);
            return test;
        }

        private static TestResult parse(string error) {
            var msgStartToken = "PHP Parse error:";
            var stackTraceToken = "PHP Stack trace:";
            if (error.IndexOf(msgStartToken) == -1)
                return null;
            
            var message = trimMessage(error.Substring(msgStartToken.Length, error.Length - msgStartToken.Length));
            var stackLines = new List<StackLine>();
            var stackTraceStart = message.IndexOf(stackTraceToken);
            if (stackTraceStart != -1) {
                stackLines.AddRange(getStackTrace(message.Substring(stackTraceStart, message.Length - stackTraceStart)));
                message = trimMessage(message.Substring(0, stackTraceStart));
            }
            var msgStackLine = getMsgStackLine(message);
            if (msgStackLine != null)
                stackLines.Insert(0, msgStackLine);
            var test =
                new TestResult(
                    TestRunner.PhpParseError,
                    TestRunStatus.Failed,
                    message);
            test.Message = message;
            test.StackTrace = stackLines.ToArray();
            return test;
        }

        private static StackLine getMsgStackLine(string msg) {
            var inToken = " in ";
            var start = msg.LastIndexOf(inToken);
            if (start == -1)
                return null;
            var lineToken = " on line ";
            var end = msg.LastIndexOf(lineToken);
            if (end == -1)
                end = msg.Length;
            var file = msg.Substring(start + inToken.Length, end - start - inToken.Length);
            int line;
            if (end != -1) {
                if (!int.TryParse(msg.Substring(end + lineToken.Length, msg.Length - end - lineToken.Length), out line))
                    line = 0;
            } else {
                line = 0;
            }
            return 
                new StackLine() {
                    Method = "",
                    File = file,
                    LineNumber = line
                };
        }

        private static IEnumerable<StackLine> getStackTrace(string trace) {
            var stackLines = new List<StackLine>();
            var lines = trace.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                var chunks = line.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                if (chunks.Length != 4)
                    continue;
                var method = chunks[2];
                var file = chunks[3];
                int lineNumber;
                var numIndex = file.LastIndexOf(":");
                if (numIndex != -1) {
                    if (!int.TryParse(file.Substring(numIndex + 1, file.Length - numIndex - 1), out lineNumber))
                        lineNumber = 0;
                    file = file.Substring(0, numIndex);
                } else {
                    lineNumber = 0;
                }
                stackLines.Add(
                    new StackLine() {
                        Method = method,
                        File = file,
                        LineNumber = lineNumber
                    });
            }
            return stackLines;
        }

        private static string trimMessage(string msg) {
            return msg.Trim(new[] {'\r','\n',' ','\t'});
        }
    }

}