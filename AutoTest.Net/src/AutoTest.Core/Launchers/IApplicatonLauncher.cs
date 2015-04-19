using System;
namespace AutoTest.Core.Launchers
{
	public interface IApplicatonLauncher
	{
		void Initialize(string path);
        void FocusEditor();
		void LaunchEditor(string file, int lineNumber, int column);
	}
}

