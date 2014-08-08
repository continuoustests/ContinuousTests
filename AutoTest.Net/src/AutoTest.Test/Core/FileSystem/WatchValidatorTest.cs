using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NUnit.Framework;
using AutoTest.Core.FileSystem;
using AutoTest.Core.Configuration;
using Rhino.Mocks;

namespace AutoTest.Test.Core.FileSystem
{
    [TestFixture]
    public class WatchValidatorTest
    {
        private WatchValidator _validator;
		private IConfiguration _configuration;
        private ICustomIgnoreProvider _ignoreProvider;

        [SetUp]
        public void SetUp()
        {
			_configuration = MockRepository.GenerateMock<IConfiguration>();
            _ignoreProvider = MockRepository.GenerateMock<ICustomIgnoreProvider>();
            _ignoreProvider.Stub(i => i.ShouldPublish("")).IgnoreArguments().Return(true);
            var providers = new ICustomIgnoreProvider[] { _ignoreProvider };
            _validator = new WatchValidator(_configuration, providers);
        }
        
        [Test]
        public void Should_return_true_if_normal_file()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] {});
            _validator
                .ShouldPublish(Path.GetTempFileName())
                .ShouldBeTrue();
        }

        [Test]
        public void Should_invalidate_bin_debug()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}Debug{0}"))
                .ShouldBeFalse();
        }
		
		[Test]
        public void Should_invalidate_bin_debug_dir()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}Debug"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_bin_release()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}Release{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_bin_x86()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}bin{0}x86{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_obj_debug()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}Debug{0}"))
                .ShouldBeFalse();
        }
		
		[Test]
        public void Should_invalidate_obj_debug_dir()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}Debug"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_obj_release()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}Release{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_obj_x86()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("something{0}obj{0}x86{0}"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_bin_autotest_net()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("asomething{0}bin{0}AutoTest.Net{0}somefile.mm.dll"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_mm_cache_files()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("asomething{0}with{0}somefolder{0}AutoTest.NET_mm_cache.bin"))
                .ShouldBeFalse();
        }

		[Test]
        public void Should_invalidate_swp()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("asomething{0}somefile.swp"))
                .ShouldBeFalse();
        }
		
		[Test]
        public void Should_invalidate_swx()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_validator
                .ShouldPublish(getInfo("asomething{0}somefile.swx"))
                .ShouldBeFalse();
		}

        [Test]
        public void Should_invalidate_realtime_build_files()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator
                .ShouldPublish(getInfo("asomething{0}something_rltm_build_fl_kdfj.cs"))
                .ShouldBeFalse();
        }
		
		[Test]
        public void Should_invalidate_endswith_tilde()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_validator
                .ShouldPublish(getInfo("asomething{0}MyFile.cs~"))
                .ShouldBeFalse();
		}

        [Test]
        public void Should_invalidate_monos_filelistabsolute_file()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator.ShouldPublish(getInfo("something{0}obj{0}SomeProject.csproj.FileListAbsolute.txt"))
                .ShouldBeFalse();
        }

        [Test]
        public void Should_invalidate_monos_fileswrittenabsolute_file()
        {
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator.ShouldPublish(getInfo("something{0}obj{0}SomeProject.csproj.FilesWrittenAbsolute.txt"))
                .ShouldBeFalse();
        }
		
		[Test]
		public void Should_ignore_directory_with_pattern_name()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "meh" });
			_validator.ShouldPublish("/Somedirectory/meh/AndAnotherOne").ShouldBeFalse();
		}

        [Test]
        public void Should_match_exact_pattern()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { @"C:/Users/ack/src/Castle.Windsor/src/Castle.Windsor/AssemblyInfo.cs" });
            _validator.ShouldPublish(@"C:\Users\ack\src\Castle.Windsor\src\Castle.Windsor\AssemblyInfo.cs").ShouldBeFalse();
        }

		[Test]
		public void Should_match_pattern_to_path_end()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "xmloutput.xml" });
			_validator.ShouldPublish("/Somedirectory/hoi/AndAnotherOne/xmloutput.xml").ShouldBeFalse();
		}
		
		[Test]
		public void Should_match_sub_directories_through_glob()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
			_validator.ShouldPublish("/Somedirectory/hoi/myFolder/somexmlfile.xml").ShouldBeFalse();
		}
		
		[Test]
		public void Should_glob_case_sensitive_on_unix()
		{
            if (!OS.IsUnix)
                return;
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
			_validator.ShouldPublish("/Somedirectory/hoi/myfolder/somexmlfile.xml").ShouldBeTrue();
		}

        [Test]
        public void Should_glob_case_insensitive_on_non_unix_platforms()
        {
            if (OS.IsUnix)
                return;
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
            _validator.ShouldPublish("/Somedirectory/hoi/myfolder/somexmlfile.xml").ShouldBeFalse();
        }
		
		[Test]
		public void Should_return_list_of_ignore_items()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*", "whatever.txt", "*.bat" });
			_validator.GetIgnorePatterns().ShouldEqual("myFolder/*|whatever.txt|*.bat");
		}
		
		[Test]
		public void Should_return_list_of_ignore_items_included_custom_output_directory()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*", "whatever.txt", "*.bat" });
			_configuration.Stub(c => c.CustomOutputPath).Return("bin/MyCustomOutDir/");
			_validator.GetIgnorePatterns().ShouldEqual("myFolder/*|whatever.txt|*.bat|bin/MyCustomOutDir/");
		}

        [Test]
        public void Should_return_list_of_ignore_items_included_custom_output_directory_using_back_slash()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*", "whatever.txt", "*.bat" });
            _configuration.Stub(c => c.CustomOutputPath).Return("bin\\MyCustomOutDir\\");
            _validator.GetIgnorePatterns().ShouldEqual("myFolder/*|whatever.txt|*.bat|bin/MyCustomOutDir/");
        }

        [Test]
        public void Should_return_list_of_ignore_items_included_custom_output_directory_in_ignores()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*", "whatever.txt", "*.bat" });
            _configuration.Stub(c => c.CustomOutputPath).Return(@"bin\AutoTest.NET");
            _validator.ShouldPublish(@"C:\Users\ack\src\AutoTest.Net\src\AutoTest.Console.Test\bin\AutoTest.NET\AutoTest.Console.Test.mm.dll").ShouldBeFalse();
        }
		
		[Test]
		public void Should_respect_configuration_setting()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(true);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
			_validator.ShouldPublish("/Somedirectory/hoi/myfolder/somexmlfile.xml").ShouldBeFalse();
		}

        [Test]
        public void Should_respect_configuration_setting_and_let_through_exe_files()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(true);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
            _validator.ShouldPublish("/Somedirectory/hoi/myfolder/somexmlfile.exe").ShouldBeTrue();
        }

        public void Should_respect_configuration_setting_and_let_through_dll_files()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(true);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
            _validator.ShouldPublish("/Somedirectory/hoi/myfolder/somexmlfile.dll").ShouldBeTrue();
        }

        [Test]
        public void Should_respect_configuration_setting_but_ignore_vshot_exe()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(true);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "myFolder/*" });
            _validator.ShouldPublish("/Somedirectory/hoi/myfolder/somefile.vshost.exe").ShouldBeFalse();
        }

		[Test]
		public void Should_not_consume_vs_suo_files()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_validator.ShouldPublish("/Somedirectory/hoi/myfolder/myproject.suo").ShouldBeFalse();
		}

        [Test]
        public void Should_not_consume_vs_unmanagedregistration_cache()
        {
            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _validator.ShouldPublish("/Somedirectory/hoi/myfolder/myproject.csproj.UnmanagedRegistration.cache").ShouldBeFalse();
        }
		
		[Test]
		public void Should_always_glob_with_slash()
		{
			_configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
			_configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { "src/*/obj" });
			_validator.ShouldPublish(@"C:\Somedirectory\src\myfolder\another\obj\somexmlfile.xml").ShouldBeFalse();
		}

        [Test]
        public void Should_use_custom_providers_on_normal_run()
        {
            var ignoreProvider = MockRepository.GenerateMock<ICustomIgnoreProvider>();
            var providers = new ICustomIgnoreProvider[] { ignoreProvider };
            var validator = new WatchValidator(_configuration, providers);

            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(false);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] {});
            validator.ShouldPublish(@"C:\Somedirectory\src\myfolder\another\obj\somexmlfile.xml").ShouldBeFalse();
        }

        [Test]
        public void Should_use_custom_providers_on_binary_run()
        {
            var ignoreProvider = MockRepository.GenerateMock<ICustomIgnoreProvider>();
            var providers = new ICustomIgnoreProvider[] { ignoreProvider };
            var validator = new WatchValidator(_configuration, providers);

            _configuration.Stub(c => c.ShouldUseBinaryChangeIgnoreLists).Return(true);
            _configuration.Stub(c => c.WatchIgnoreList).Return(new string[] { });
            validator.ShouldPublish(@"C:\Somedirectory\src\myfolder\another\obj\meh.mm.dll").ShouldBeFalse();
        }

        private string getInfo(string path)
        {
            return string.Format(path, Path.DirectorySeparatorChar);
        }
    }
}
