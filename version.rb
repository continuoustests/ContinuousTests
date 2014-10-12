if ARGV.length != 1
	exit
end
version = ARGV[0]

def set_version(filepath, version)
	lines = File.readlines(filepath)
				.select {|line|
					!line.start_with?("[assembly: AssemblyVersion(") &&
					!line.start_with?("[assembly: AssemblyFileVersion(")
				}
	
	File.open(filepath, "w") { |file|
		lines.each{|line| file.puts line}
		file.puts "[assembly: AssemblyVersion(\"" + version + "\")]"
		file.puts "[assembly: AssemblyFileVersion(\"" + version + "\")]"
	}
end

def update_installer(filepath, version)
	lines = File.readlines(filepath)
				.map {|line|
					modified = line
					if line.start_with?("AppVersion=")
						modified = "AppVersion=" + version
					end
					if line.start_with?("AppVerName=")
						modified = "AppVerName=ContinuousTests " + version
					end
					if line.start_with?("OutputBaseFilename=")
						modified = "OutputBaseFilename=ContinuousTests-v" + version
					end
					modified
				}
	File.open(filepath, "w") { |file|
		lines.each{|line| file.puts line}
	}
end

def update_download(filepath, version)
	lines = File.readlines(filepath)
				.map {|line|
					modified = line
					if line.include?(">Windows installer package (with Visual Studio integration)</a>")
						modified = "                	<a href=\"ContinuousTests-v" + version + ".exe\">Windows installer package (with Visual Studio integration)</a><br><br>"
					end
					if line.include?(">Cross platform standalone client</a>")
						modified = "                   	<a href=\"ContinuousTests-Standalone-v" + version + ".zip\">Cross platform standalone client</a><br>"
					end
					modified
				}
	File.open(filepath, "w") { |file|
		lines.each{|line| file.puts line}
	}
end

def update_version_xml(filepath, version)
	lines = File.readlines(filepath)
				.map {|line|
					modified = line
					if line.include?("<version>")
						modified = "	<version>" + version + "</version>"
					end
					modified
				}
	File.open(filepath, "w") { |file|
		lines.each{|line| file.puts line}
	}
end

puts "Updateing Client"
set_version("src\\AutoTest.Client\\Properties\\AssemblyInfo.cs", version)

puts "Updateing Profiler"
set_version("src\\AutoTest.Profiler\\Properties\\AssemblyInfo.cs", version)

puts "Updateing VM"
set_version("src\\AutoTest.VM\\Properties\\AssemblyInfo.cs", version)

puts "Updateing VM Messages"
set_version("src\\AutoTest.VM.Messages\\Properties\\AssemblyInfo.cs", version)

puts "Updateing Minimizer"
set_version("src\\AutoTest.Minimizer\\Properties\\AssemblyInfo.cs", version)

puts "Updating Installer"
update_installer("Installer\\ATEInstaller.iss", version)

puts "Updating download.html"
update_download("www\\download.html", version)

puts "Updating version.xml"
update_version_xml("www\\version.xml", version)
