using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TemplateMaschine;

namespace ProjectGeneratorTests
{
	[TestFixture]
	public class GeneratorTests
	{
		private readonly string templateDir = "..\\..\\Tests";
		private readonly string testDir = "tests";

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			if (!Directory.Exists(testDir))
				Directory.CreateDirectory(testDir);
		}

		[Test]
		public void TemplateCS()
		{
			string templateFile = Path.Combine(templateDir, "Addin.cs.template");
			string argsFile = Path.Combine(templateDir, "Addin.cs.args");
			string etalonFile = Path.Combine(templateDir, "Addin.cs.etalon");
			string outFile = Path.Combine(testDir, "Addin.cs");
			Template template = new Template(templateFile);
			Dictionary<string, object> args = Template.ReadArgs(argsFile);
			template.Generate(args, outFile);
			FileAssert.AreEqual(etalonFile, outFile);
		}

		[Test]
		public void TemplateCatalog()
		{
			string templateCatalog = Path.Combine(templateDir, "catalog");
			string projectDir = Path.Combine(testDir, "MyProject");
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", "MyProject");
			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
		}

		[Test]
		public void TemplateCatalogAndFile()
		{
			string templateCatalog = Path.Combine(templateDir, "CatalogAndFile");
			string projectDir = Path.Combine(testDir, "MyCatalogAndFile");
			string projectFile = Path.Combine(testDir, "MyCatalogAndFile.sln");
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);
			if (File.Exists(projectFile))
				File.Delete(projectFile);

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", "MyCatalogAndFile");
			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
			Assert.IsTrue(File.Exists(projectFile));
		}

		[Test]
		public void RecursiveCatalog()
		{
			string templateCatalog = Path.Combine(templateDir, "RecursiveCatalog");
			string projectDir = Path.Combine(testDir, "MyRecursiveCatalog");
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);
			string projectSubDir = Path.Combine(projectDir, "Second.Common");
			string projectSubFile = Path.Combine(projectSubDir, "Second.Common.csproj");

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", "MyRecursiveCatalog");
			args.Add("SecondName", "Second.Common");
			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
			Assert.IsTrue(Directory.Exists(projectSubDir));
			Assert.IsTrue(File.Exists(projectSubFile));
		}

		[Test]
		public void RecursiveCatalogMain()
		{
			string templateCatalog = Path.Combine(templateDir, "RecursiveCatalog");
			string projectDir = Path.Combine(testDir, "MyRecursiveCatalogMain");
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);
			string projectSubDir = Path.Combine(projectDir, "Second.Common");
			string projectSubFile = Path.Combine(projectSubDir, "Second.Common.csproj");

			string[] args = {"ProjectGenerator.exe", 
				"-template:"+templateCatalog, 
				"-name:project=MyRecursiveCatalogMain",
				"-n:SecondName=Second.Common", 
				"-out:" + testDir};

			ProjectGenerator.ProjectGenerator.Main(args);

			Assert.IsTrue(Directory.Exists(projectDir));
			Assert.IsTrue(Directory.Exists(projectSubDir));
			Assert.IsTrue(File.Exists(projectSubFile));
		}

		[Test]
		public void ProjectWithArgs()
		{
			string templateCatalog = Path.Combine(templateDir, "ProjectWithArgs");
			string projectName = "MyProjectWithArgs";
			string projectDir = Path.Combine(testDir, projectName);
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);
			string projectSubDir = Path.Combine(projectDir, projectName + ".Common");
			string projectSubFile = Path.Combine(projectSubDir, projectName + ".Common.csproj");

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", projectName);
			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
			Assert.IsTrue(Directory.Exists(projectSubDir));
			Assert.IsTrue(File.Exists(projectSubFile));

		}

		[Test]
		public void ProjectWithArgsCache()
		{
			string templateCatalog = Path.Combine(templateDir, "ProjectWithArgs");
			string projectName = "MyProjectWithArgsCache";
			string projectDir = Path.Combine(testDir, projectName);
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);
			string projectSubDir = Path.Combine(projectDir, projectName + ".Common");
			string projectSubFile = Path.Combine(projectSubDir, projectName + ".Common.csproj");

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);
			generator.CacheArgs = true;
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", projectName);
			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
			Assert.IsTrue(Directory.Exists(projectSubDir));
			Assert.IsTrue(File.Exists(projectSubFile));

			string cacheFile = Path.Combine(projectDir, ".args.cache");
			Assert.IsTrue(File.Exists(cacheFile));
			Dictionary<string, object> cachArgs = Template.ReadArgs(cacheFile);
			Assert.AreEqual("MyProjectWithArgsCache.Common", cachArgs["SecondName"]);

		}

		[Test]
		public void ProjectWithArgsUseCache()
		{
			string templateCatalog = Path.Combine(templateDir, "ProjectWithArgs");
			string projectName = "MyProjectWithArgsUseCache";
			string projectDir = Path.Combine(testDir, projectName);
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);
			generator.CacheArgs = true;
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", projectName);
			generator.Generate(args, testDir);
			
			string cacheFile = Path.Combine(projectDir, ".args.cache");
			Dictionary<string, object> cacheArgs = new Dictionary<string, object>();
			cacheArgs["SecondName"] = projectName + "2.Common";
			Template.WriteArgs(cacheArgs, cacheFile);
			string projectSubDir = Path.Combine(projectDir, projectName + "2.Common");
			string projectSubFile = Path.Combine(projectSubDir, projectName + "2.Common.csproj");

			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
			Assert.IsTrue(Directory.Exists(projectSubDir));
			Assert.IsTrue(File.Exists(projectSubFile));
		}

		[Test]
		public void ProjectFileTemplates()
		{
			string templateCatalog = Path.Combine(templateDir, "ProjectFileTemplates");
			string projectName = "MyProjectFileTemplates";
			string projectDir = Path.Combine(testDir, projectName);
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);

			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", projectName);
			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
			string etalonSln = Path.Combine(templateCatalog, ".etalon.sln");
			string newSln = Path.Combine(projectDir, projectName + ".sln");
			FileAssert.AreEqual(etalonSln, newSln);
		}

		[Test]
		public void XmlUtf8()
		{
			string templateCatalog = Path.Combine(templateDir, "XmlUtf8");
			string projectName = "MyXmlUtf8";
			string projectDir = Path.Combine(testDir, projectName);
			if (Directory.Exists(projectDir))
				Directory.Delete(projectDir, true);

			ProjectGenerator.TemplateDir generator = new ProjectGenerator.TemplateDir(templateCatalog);
			Dictionary<string, object> args = new Dictionary<string, object>();
			args.Add("project", projectName);
			generator.Generate(args, testDir);

			Assert.IsTrue(Directory.Exists(projectDir));
			string etalonXml = Path.Combine(templateCatalog, "$(project)\\ProfileCatalog.xml");
			string newXml = Path.Combine(projectDir, "ProfileCatalog.xml");
			FileAssert.AreEqual(etalonXml, newXml);
		}

	
	}
}

