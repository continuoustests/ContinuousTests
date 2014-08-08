using System;
namespace AutoTest.Core.Launchers
{
	public interface IApplicatonLauncher
	{
		void Initialize(string path);
		void LaunchEditor(string file, int lineNumber, int column);
	}
}

