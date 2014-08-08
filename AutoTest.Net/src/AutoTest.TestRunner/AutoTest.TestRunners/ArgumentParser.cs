using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace AutoTest.TestRunners
{
    public class Arguments
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public bool StartSuspended { get; set; }
        public bool Silent { get; set; }
        public bool Logging { get; set; }
        public string Channel { get; set; }
        public bool CompatabilityMode { get; set; }
    }

    public class ArgumentParser
    {
        private string[] _arguments;
        private Arguments _parsedArgument;

        public ArgumentParser(string[] arguments)
        {
            _arguments = arguments;
        }

        public Arguments Parse()
        {
            _parsedArgument = new Arguments();
            foreach (var argument in _arguments)
                parse(argument);
            if (_parsedArgument.Channel == null)
                _parsedArgument.Channel = getDefaultChannel();
            return _parsedArgument;
        }

        private void parse(string argument)
        {
            if (iAm(argument, "--input="))
                _parsedArgument.InputFile = getValue(argument, "--input=");
            if (iAm(argument, "--output="))
                _parsedArgument.OutputFile = getValue(argument, "--output=");
            if (iAm(argument, "--startsuspended"))
                _parsedArgument.StartSuspended = true;
            if (iAm(argument, "--silent"))
                _parsedArgument.Silent = true;
            if (iAm(argument, "--logging"))
                _parsedArgument.Logging = true;
            if (iAm(argument, "--channel"))
                _parsedArgument.Channel = getValue(argument, "--channel=");
            if (iAm(argument, "--compatibility-mode"))
                _parsedArgument.CompatabilityMode = true;
        }

        private bool iAm(string argument, string parameterName)
        {
            return argument.StartsWith(parameterName);
        }

        private string getValue(string parameter, string parameterName)
        {
            if (parameterName.Length >= parameter.Length)
                return "";
            return parameter.Substring(parameterName.Length, parameter.Length - parameterName.Length).Replace("\"", "");
        }

        private string getDefaultChannel()
        {
            return "AutoTest.TestRunner." + Process.GetCurrentProcess().Id.ToString();
        }
    }
}
