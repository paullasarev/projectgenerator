using System;
using System.Collections.Generic;
using Commons.GetOptions;

//[assembly: AssemblyTitle("ProjectGenerator")]
//[assembly: AssemblyCopyright("(c)2008 KINT")]
//[assembly: AssemblyDescription("Template project generator")]
//[assembly: AssemblyVersion("2.0.0.0")]

[assembly: Commons.About("Generate projects by template")]
[assembly: Commons.Author("Paul Lasarev")]
[assembly: Commons.UsageComplement("")]
[assembly: Commons.AdditionalInfo("")]
[assembly: Commons.ReportBugsTo("support@kint.ru")]


namespace ProjectGenerator
{
	public class ProjectGeneratoreOptions : Options
	{
		[Option("Template path (required)", "template", "t")]
		public string TemplatePath;

		[Option("Output path (optional, default='.')", "out", "o")]
		public string OutputPath = ".";

		[Option(-1, "Define name=value pair (optional)", "name", "n")]
		public string[] Names;

		[Option("Overwrite output files", "overwrite", "w")]
		public bool Overwrite = false;

		[Option("Verbose output", "verbose", "v")]
		public bool Verbose = false;

		[Option("Cache .args.template files", "cache", "c")]
		public bool CacheArgs = false;

		public ProjectGeneratoreOptions(string[] args) : base(args) { }

		public bool Validate()
		{
			if (TemplatePath == null)
				return false;
			return true;
		}
	}

	public class ProjectGenerator
	{
		[STAThread]
		public static int Main(string[] args)
		{
			ProjectGeneratoreOptions options = new ProjectGeneratoreOptions(args);
			if (!options.Validate())
			{
				options.DoHelp();
				return 1;
			}
			TemplateDir template = new TemplateDir(options.TemplatePath);
			template.Verbose = options.Verbose;
			template.CacheArgs = options.CacheArgs;
			Dictionary<string, object> templateArgs = new Dictionary<string, object>();
			foreach (string pair in options.Names)
			{
				int index = pair.IndexOf('=');
				string name = (index < 0 ? pair : pair.Substring(0, index));
				string value = (index < 0 ? "" : pair.Substring(index + 1));
				templateArgs.Add(name, value);
			}
			template.Generate(templateArgs, options.OutputPath, options.Overwrite);
			return 0;
		}
	}
}
