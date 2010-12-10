# NServiceBus Contrib

To build the NServiceBus Contrib you need to have Ruby installed. You can get the latest Ruby Installer from [http://rubyinstaller.org](http://rubyinstaller.org/)

You'll also need the Albacore gem, version 0.1.5 (current version 0.2.2 is not yet supported), to run the rakefiles
<pre>
	<code>
		gem install albacore
	</code>
</pre>

## Template rakefile for projects

The NServiceBus Contrib uses a single root rakefile wich calls every rakefile it can find in the subdirectories and executes the 'build' task. For the moment, put the _rakefiletemplate in the same folder as your .sln-file and rename it "rakefile".

<pre>
	<code>
	gem 'albacore', '<= 0.1.5'
	require 'albacore'
	require 'FileUtils'

	COMPILE_TARGET = "debug" unless defined?(COMPILE_TARGET)
	PLATFORM = "Any CPU" unless defined?(PLATFORM)
	
	build_dir = "#{File.dirname(__FILE__)}/build"

	# Change these two to match your solution and project
	solution_file = "SolutionFile.sln"
	project_directory = "ProjectRootDirectory"

	task :default => ['build']
	 
	desc "Prepares the working directory for a new build"
	task :clean do
		unless defined?(GLOBAL_BUILD_DIR) then
			FileUtils.rm_rf build_dir
			Dir.mkdir build_dir
		end
	end 

	desc "Compile the project"
	msbuild :compile do |msb|
		msb.properties :configuration => COMPILE_TARGET, :platform => PLATFORM
		msb.targets :Clean, :Build
		msb.solution = File.dirname(__FILE__) + "/#{solution_file}" 
		msb.command = File.join(ENV['windir'], 'Microsoft.NET', 'Framework', 'v4.0.30319', 'MSBuild.exe') 
	end

	task :build => [:clean, :compile] do  
		
		if defined?(GLOBAL_BUILD_DIR) then
			copyOutputFiles File.dirname(__FILE__)+ "/#{project_directory}/bin/#{COMPILE_TARGET}", "*.{dll,exe,config,pdb}", "#{GLOBAL_BUILD_DIR}/#{project_directory}"
		else
			copyOutputFiles File.dirname(__FILE__)+ "/#{project_directory}/bin/#{COMPILE_TARGET}", "*.{dll,exe,config,pdb}", build_dir
		end
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