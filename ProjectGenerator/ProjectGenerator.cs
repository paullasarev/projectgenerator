using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TemplateMaschine;

namespace ProjectGenerator
{
	public class TemplateDir : IDisposable
	{
		private readonly string templateDir;
		private bool overwrite = false;
		private bool verbose = false;
		private readonly TextWriter log = Console.Out;
		private bool cacheArgs = false;

		public TemplateDir(string dir)
		{
			templateDir = dir;
		}

		public bool Verbose
		{
			get { return verbose;}
			set { verbose = value; }
		}

		public bool CacheArgs
		{
			get { return cacheArgs; }
			set { cacheArgs = value; }
		}

		public bool Overwrite
		{
			get { return overwrite; }
			set { overwrite = value; }
		}

		public void Generate(IDictionary<string, object> args, string outDir)
		{
			generate(args, outDir, "");
		}

		public void Generate(Dictionary<string, object> args, string outDir, bool overwrite)
		{
			this.Overwrite = overwrite;
			generate(args, outDir, "");
		}

		private IDictionary<string, object> readArgs(IDictionary<string, object> args,  string outDir, string templateSubDir)
		{
			string dir = Path.Combine(templateDir, templateSubDir);
			string argsFileName = Path.Combine(dir, ".args.template");
			if (!File.Exists(argsFileName))
				return args;

			Log("Process template file {0}", argsFileName);
			Template template = new Template(argsFileName);
			string outFile = Path.GetTempFileName();
			template.Generate(args, outFile);
			Dictionary<string, object> templateArgs = Template.ReadArgs(outFile);
			if (CacheArgs)
			{
				string cacheFile = Path.Combine(outDir, ".args.cache");
				if (File.Exists(cacheFile))
				{
					Log("Process cached arguments file {0}", cacheFile);
					Dictionary<string, object> cacheArgs = Template.ReadArgs(cacheFile);
					foreach (KeyValuePair<string, object> arg in cacheArgs)
					{
						templateArgs[arg.Key] = arg.Value;
					}
				}

				Template.WriteArgs(templateArgs, cacheFile);
			}

			foreach (KeyValuePair<string, object> arg in args)
			{
				//templateArgs.Add(arg.Key, arg.Value);
				templateArgs[arg.Key] = arg.Value;
			}

			return templateArgs;
		}

		private void generate(IDictionary<string, object> args, string outDir, string templateSubDir)
		{
			args = readArgs(args, outDir, templateSubDir);
			generateDirs(args, outDir, templateSubDir);
			generateFiles(args, outDir, templateSubDir);
		}

		private void generateFiles(IDictionary<string, object> args, string outDir, string templateSubDir)
		{
			string[] files = Directory.GetFiles(Path.Combine(templateDir, templateSubDir));
			foreach (string file in files)
			{
				string fileName = Path.GetFileName(file);
				if (fileName.StartsWith("."))
					continue;
				string outName = getTemplateName(fileName, args);
				string outFullName = Path.Combine(outDir, outName);
				if (!Overwrite && File.Exists(outFullName))
				{
					Log("Skip existed file {0}", outFullName);
					continue;
				}
				if (isBinaryFile(file))
				{
					Log("Copy binary file {0}", outFullName);
					File.Copy(file, outFullName);
				}
				else
				{
					Log("Process file {0}", outFullName);
					Template template = new Template(file);
					template.Generate(args, outFullName);
				}
			}
		}

		private bool isBinaryFile(string file)
		{
			string extension = Path.GetExtension(file);
			switch (extension)
			{
				case ".dll":
					return true;
				case ".exe":
					return true;
				case ".png":
					return true;
				case ".jpg":
					return true;
				case ".ico":
					return true;
				case ".bmp":
					return true;
				case ".rtf":
					return true;
				case ".doc":
					return true;
				case ".xsl":
					return true;
			}
			return false;
		}

		private void generateDirs(IDictionary<string, object> args, string outDir, string templateSubDir)
		{
			string[] dirs = Directory.GetDirectories(Path.Combine(templateDir, templateSubDir));
			foreach (string dirPath in dirs)
			{
				string dir = Path.GetFileName(dirPath);
				if (dir.StartsWith("."))
					continue;
				string outName = getTemplateName(dir, args);
				string dirName = Path.Combine(outDir, outName);
				if (Directory.Exists(dirName))
				{
					Log("Skip existed directory {0}", dirName);
				}
				else
				{
					Log("Create directory {0}", dirName);
					Directory.CreateDirectory(dirName);
				}
				generate(args, Path.Combine(outDir, outName), Path.Combine(templateSubDir, dir));
			}
		}

		private void Log(string format, string parameter)
		{
			if (Verbose)
				log.WriteLine(format, parameter);
		}

		private string getTemplateName(string file, IDictionary<string, object> args)
		{
			string outName = file;
			Regex regex = new Regex("\\$\\((?<name>[^)]+)\\)");
			MatchCollection matches = regex.Matches(file);
			foreach (Match match in matches)
			{
				string name = match.Groups["name"].Value;
				if (!args.ContainsKey(name))
					throw new ArgumentException(String.Format("Cannot get <{0}> parameter name", name));
				string value = (string) args[name];
				outName = outName.Replace(String.Format("$({0})", name), value);
			}
			return outName;
		}

		public void Dispose()
		{
		}

	}
}
