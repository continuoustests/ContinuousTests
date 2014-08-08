using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AutoTest.Messages;

namespace AutoTest.Core.BuildRunners
{
    public class MSBuildOutputParser
    {
        private BuildRunResults _results;
        private string[] _lines;
		private string _line;
		private string _currentProjectPath = null;
		private bool _foundOutputMarker = false;

        public MSBuildOutputParser(BuildRunResults results, string[] lines)
        {
            _results = results;
            _lines = lines;
        }

        public void Parse()
        {
            foreach (var line in _lines) {
				_line = line;
				if (!foundOutputMarker())
					continue;
				if (!detectAndValidateBuildTarget() && !isSolutionError())
					break;
				if (_currentProjectPath == null && !isSolutionError())
				    continue;
				parseLine();
			}
        }
		
		private bool foundOutputMarker()
		{
			if (_line.StartsWith("Build FAILED."))
				_foundOutputMarker = true;
			if (_line.StartsWith("Build succeeded."))
				_foundOutputMarker = true;
			return _foundOutputMarker;
		}
		
		private bool detectAndValidateBuildTarget()
		{
			if (!(_line.Contains(" (default target") || _line.Contains(" (rebuild target") || _line.Contains(" (Rebuild target")))
				return true;
			
			if (failedBuildAlreadyDetected())
				return false;
			
            var path = "";
            if (_line.Contains(" (default target"))
		        path = _line.Substring(0, _line.IndexOf(" (default target")).Replace("\"", "");
            else if (_line.Contains(" (rebuild target"))
                path = _line.Substring(0, _line.IndexOf(" (rebuild target")).Replace("\"", "");
            else if (_line.Contains(" (Rebuild target"))
                path = _line.Substring(0, _line.IndexOf(" (Rebuild target")).Replace("\"", "");
            if (Path.GetExtension(path).ToLower().Equals(".sln"))
                return true;

            _currentProjectPath = Path.GetDirectoryName(path);
			return true;
		}
		
		private bool failedBuildAlreadyDetected()
		{
			return _results.ErrorCount > 0;
		}
		
		private void parseLine()
		{
			if (_line.Contains(": error"))
            {
                BuildMessage message = parseMessage(": error");
                if (!_results.Errors.Contains(message))
                    _results.AddError(message);
            }
            else if (_line.Contains(": warning"))
            {
                BuildMessage message = parseMessage(": warning");
                if (!_results.Warnings.Contains(message))
                    _results.AddWarning(message);
            }
            else if (_line.StartsWith("\tfatal error "))
            {
                BuildMessage message = parseMessage("fatal error ");
                if (!_results.Errors.Contains(message))
                    _results.AddError(message);
            }
            else if (isSolutionError())
            {
                BuildMessage message = parseMessage(": Solution file error");
                if (!_results.Errors.Contains(message))
                    _results.AddError(message);
            }
		}

        private bool isSolutionError() {
            return _line.Contains(": Solution file error");
        }

        private BuildMessage parseMessage(string type)
        {
            var message = new BuildMessage();
            try
            {
                if (_line.IndexOf('(') != -1)
                {
                    var pathChunk = _line.Substring(0, _line.IndexOf('(')).Trim();
                    if (Path.IsPathRooted(pathChunk))
                        message.File = pathChunk;
                    else
                        message.File = Path.Combine(_currentProjectPath, pathChunk);
                    var position = _line.Substring(_line.IndexOf('(') + 1, _line.IndexOf(')') - (_line.IndexOf('(') + 1)).Split(',');
                    message.LineNumber = int.Parse(position[0]);
                    if (position.Length > 1)
                        message.LinePosition = int.Parse(position[1]);
                }
                message.ErrorMessage = _line.Substring(_line.IndexOf(type) + type.Length,
                                                       _line.Length - (_line.IndexOf(type) + type.Length)).Trim();
            }
            catch (Exception)
            {
                message.File = "Unknown";
                message.LineNumber = 0;
                message.LinePosition = 0;
                message.ErrorMessage = _line;
            }
            return message;
        }
    }
}
