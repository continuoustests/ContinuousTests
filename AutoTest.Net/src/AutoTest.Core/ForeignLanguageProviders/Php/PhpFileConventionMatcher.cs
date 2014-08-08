using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace AutoTest.Core.ForeignLanguageProviders.Php
{
	public class PhpFileConventionMatcher
	{
		private string _token;
		private string[] _patterns;
		private string[] _testLocations;

		public PhpFileConventionMatcher(string token, string[] patterns, string[] testLocations)
		{
			if (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX) {
				_token = token.Replace("/", "\\");
				_patterns = patterns.Select(x => x.Replace("/", "\\")).ToArray();
				_testLocations = testLocations.Select(x => x.Replace("/", "\\")).ToArray();
				return;
			}
			_token = token;
			_patterns = patterns;
			_testLocations = testLocations;
		}

		public string[] Match(string file)
		{
			var locations = new List<string>();
			foreach (var pattern in _patterns) {
				if (file.Length < pattern.Length)
					continue;
				var start = file.IndexOf(pattern, _token.Length);
				if (start == -1)
					continue;
				start += pattern.Length;
				
				foreach (var location in _testLocations) {
					if (!location.EndsWith("*")) {
						locations.Add(Path.Combine(file.Substring(0, start), location));
						continue;
					}
					locations.Add(
						Path.GetDirectoryName(
							Path.Combine(
								Path.Combine(
									file.Substring(0, start),
									location.Trim(new[] {'*'})),
								file.Substring(start, file.Length - start))));
				}
			}
			return locations.ToArray();
		}
	}
}