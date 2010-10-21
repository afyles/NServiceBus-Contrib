# NServiceBus Contrib

<em>This readme is a Work in Progress</em>

To build the NServiceBus Contrib you need to have Ruby installed. You can get the latest Ruby Installer from [http://rubyinstaller.org](http://rubyinstaller.org/)

You'll also need the Albacore gem to run the rakefiles

<code>gem install albacore</code>

## Template rakefile for projects

The NServiceBus Contrib uses a single root rakefile wich calls every rakefile it can find in the subdirectories and executes the 'build' task. For the moment, put the _rakefiletemplate in the same folder as your .sln-file and rename it rakefile.

<pre>
	<code>
	require 'albacore'
	require 'FileUtils'

	COMPILE_TARGET = "debug" unless defined?(COMPILE_TARGET)
	BUILD_DIR = "build" unless defined?(BUILD_DIR)

	# Change these two to match your solution and project
	SOLUTION_FILE = "SolutionFile.sln"
	PROJECT_DIRECTORY = "ProjectRootDirectory"

	task :default => ['build']
	 
	desc "Prepares the working directory for a new build"
	task :clean do
		FileUtils.rm_rf BUILD_DIR
		Dir.mkdir BUILD_DIR
	end 

	desc "Compile the project"
	msbuild :compile do |msb|
		msb.properties :configuration => COMPILE_TARGET
		msb.targets :Clean, :Build
		msb.solution = File.dirname(__FILE__) + "/#{SOLUTION_FILE}" 
		msb.path_to_command = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', 'v4.0.30319', 'MSBuild.exe') 
	end

	task :build => [:clean, :compile] do  
		copyOutputFiles File.dirname(__FILE__)+ "/#{PROJECT_DIRECTORY}/bin/#{COMPILE_TARGET}", "*.{dll,exe,config,pdb}", File.dirname(__FILE__) + "/#{BUILD_DIR}"
	end 

	def copyOutputFiles(fromDir, filePattern, outDir)
	  mkdir outDir unless File.exists? outDir
	  Dir.glob(File.join(fromDir, filePattern)){|file| 		
		copy(file, outDir) if File.file?(file)
	  } 
	end
	</code>
</pre>
If you have several projects in your solution, just add a copyOutputFiles for those projects too.