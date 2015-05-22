

# What is Project Generator? #
ProjectGenerator is a command-line program, which can create set of files and directories by template.

# How it work? #
ProjectGenerator recursievly process the _template_ directory and create files/dirs in _target_ directory (see CommandLine).

Every template file is processed by template engine (see below).

File- and directory- template names may contain macro-substitutions (see below).

Template directory may contain special files **".args.template"** (see below). This file contain variable declarations and processed specially.

Target directory may contain special files **".args.cache"** (see below). This file contain cache of previously-generated variables and processes after all.

# What is a file template? #
File template is an _any_ file whith some syntax constructions like this:

```
<%Argument Name="project" Type="string" %>

project_Guid <% Guid.NewGuid()%>

project_Common_Name <%=project%>.Common
project_Common_Guid <% Guid.NewGuid()%>
project_Common_Namespace <%=project%>.Common
```

File content is processed by [TemplateMachine engine](TemplateMachine.md) and then will be written to target diretory.

# What is a file- and directory- name template? #
Directory names in template tree can contain special symbols like **$(project).sln**.

During processing the engine substitute this names (here **$(project)**) whith contents of _variable_ (here **project**).

# What is a special file .args.template? #

In any template directory may exist special file named **".args.template"**. This file is NOT copied to target dir, but variables, defined  in this file, goes deep in recursion.

This file have a simple format, for example
```
# this is comment
project ProjectName
description One line description

AssemblyName TheProject
```

Empty lines are ignored.

One-line comments starts from "#"

Each line define variable name (first word) and it's content (rest of line)

This file (.args.template) is also processed by TemplateMachine engine, so it is possible declare variable in any kind of C# operators, for example:

```
<%Argument Name="project" Type="string" %>

project_Guid <% Guid.NewGuid()%>

project_Common_Name <%=project%>.Common
project_Common_Guid <% Guid.NewGuid()%>
project_Common_Namespace <%=project%>.Common
```


# What is a cache file .args.cache? #

If set command option -cache (see CommandLine), the template engine specially process files named **".args.cache"**  in target directory.

First, this file will be created if it was not exist in every target directory, where in correspond source (template) directory was a file ".args.template".

The file ".args.cache" will be filled with name/value pairs, generated in ".args.template".

In future engine starts the variables from file ".args.cache" will be read _after_ processing ".args.template" and will overwrite the variable content.

This is very useful, if, for example, we need first time generate some GUID and use it in future generations.

# How will be processed binary files? #
Binary files in template directory are simply copied to target.

Currently, "binary" means one of extention: .dll .exe .png .jpg .bmp .ico .doc .rtf .xsl

# Will be target files overriden? #
By default, if target file already exist, it will be skipped.

This behaviour may be changed by option -w (see CommandLine)