using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTest.VM.Messages
{
    public interface IValidateLicense
    {
        bool IsInitialized { get; }
        bool IsValid { get; }
        string Register(string name, string email);
    }
}
