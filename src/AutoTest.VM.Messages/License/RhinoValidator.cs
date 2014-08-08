using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;

namespace AutoTest.VM.Messages.License
{
    public class RhinoValidator : IValidateLicense
    {
        private Environment.SpecialFolder _location;
		private DateTime _openUntil = new DateTime(2012, 3, 1);

        public RhinoValidator(Environment.SpecialFolder location)
        {
            _location = location;
        }

        public bool IsInitialized
        {
            get
            {
                try
                {
                    var path = getOfflineLicense();
                    var name = getname();
                    var email = getEmail();
                    return File.Exists(path) && name.Trim().Length > 0 && email.Trim().Length > 0;
                }
                catch
                {
                    killLicenseFiles();
                    return false;
                }
            }
        }

        public bool IsValid
        {
            get
            {
                try
                {
                    if (validateLicense(getPath()))
                    {
                        if (isPurchasedLicense(getPath()))
                            return true;
                        else {
                            validateOfflineLicense(getOfflineLicense()); // TODO: add && isValidTrialLicense after 1.1.2012
                            return true;
                        }
                    }
                    return validateOfflineLicense(getOfflineLicense());
                }
                catch (Exception ex)
                {
                    return true;
                }
            }
        }

        private bool isPurchasedLicense(string path)
        {
            try
            {
                if (!File.Exists(path))
                    pullLicense(getname(), getEmail());
                var validator = getValidator(path);
                validator.AssertValidLicense();
                return validator.LicenseType == Rhino.Licensing.LicenseType.Standard ||
                       validator.LicenseType == Rhino.Licensing.LicenseType.Personal;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool validateLicense(string path)
        {
            try
            {
                if (!File.Exists(path))
                    pullLicense(getname(), getEmail());
                var validator = getValidator(path);
                validator.AssertValidLicense();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string getname()
        {
            var lines = File.ReadAllLines(getOfflineLicense());
            return lines[0];
        }

        private string getEmail()
        {
            var lines = File.ReadAllLines(getOfflineLicense());
            return lines[1];
        }

        public string Register(string name, string email)
        {
            if (name == "Lay it on me bro")
            {
                return "Honestly if you are so bad off that you have to hack this product to get it why didn't you just get in touch? " +
                       "We would probably have sponsored you with a license until you're back on track. " +
                       "Have some guts and be honest, don't just go off and steal everything you want";
            }
			try
			{
            	return createLicense(name, email);
			}
			catch
			{
				return null;
			}
        }

        private string createLicense(string name, string email)
        {
            createOfflineLicense(name, email);
            if (pullLicense(name, email))
                return null;
            return null;
        }

        private void createOfflineLicense(string name, string email)
        {
            var path = getOfflineLicense();
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(name);
                writer.WriteLine(email);
            }
        }

        private bool pullLicense(string name, string email)
        {
            if (name == null || email == null)
                return false;
            try
            {
                var url = new WebClient().DownloadString("http://www.continuoustests.com/licenseproviders.xml")
                    .Replace("\n", "")
                    .Replace("\r", "");
                url += string.Format("/trial.php?name={0}&email={1}&version={2}.0",
                    name,
                    email,
                    Assembly.GetExecutingAssembly().GetName().Version.Major);
                var license = new WebClient().DownloadString(url);
                File.WriteAllText(getPath(), license);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool validateOfflineLicense(string path)
        {
            if (!File.Exists(path))
                return false;
            var time = new DateTime[]
                            {
                                new FileInfo(path).CreationTime,
                                new FileInfo(Assembly.GetExecutingAssembly().Location).CreationTime,
                                new DirectoryInfo(Path.Combine(Environment.GetFolderPath(_location), "MightyMoose")).CreationTime
                            }.Min(x => x);
            // Always free :)
            return true;
            return  DateTime.Now.Subtract(time).TotalDays < 31;
        }

        private string getPublickey()
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AutoTest.VM.Messages.License.public.key")))
            {
                return reader.ReadToEnd();
            }
        }

        private string getPath()
        {
            var id = System.Environment.MachineName + getname();
            return Path.Combine(Path.Combine(Environment.GetFolderPath(_location), "VPT"), md5(id).ToLower() + ".vpt");
        }

        private string getOfflineLicense()
        {
            var vptFolder = Path.Combine(Environment.GetFolderPath(_location), "VPT");
            var id = System.Environment.MachineName + vptFolder;
            return Path.Combine(vptFolder, md5(id).ToLower() + ".vpt");
        }

        private void killLicenseFiles()
        {
            try
            {
                var vptFolder = Path.Combine(Environment.GetFolderPath(_location), "VPT");
                if (Directory.Exists(vptFolder)) {
                    foreach (var file in Directory.GetFiles(vptFolder))
                        File.Delete(file);
                }
            }
            catch
            {
            }
        }

        private string md5(string name)
        {
            var textBytes = System.Text.Encoding.Default.GetBytes(name);
            System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
            cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash = cryptHandler.ComputeHash(textBytes);
            string ret = "";
            foreach (byte a in hash)
            {
                if (a < 16)
                    ret += "0" + a.ToString("x");
                else
                    ret += a.ToString("x");
            }
            return ret;
        }

        private Rhino.Licensing.LicenseValidator getValidator(string path)
        {
            var validator = new Rhino.Licensing.LicenseValidator(getPublickey(), path);
            validator.DisableFutureChecks();
            return validator;
        }
    }
}
