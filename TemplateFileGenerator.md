
## Требования ##

Генератор файлов по шаблону должен
  * Создавать файлы и каталоги с именами, заданными по шаблону
Например:
```
для имени шаблона

$(ИмяПроекта).sln

и задании параметра ИмяПроекта="УчетДоговоров"
должен быть сгенерирован файл с именем

УчетДоговоров.sln

```
  * Делать макро-процессирование тела файлов, т.е. заменять шаблоны в теле файла на заданные в параметрах строки
Например
```
public class $(ИмяКласса)
{
    $(ИмяКласса)()
    {
    }
}
```

Синтаксис определения шаблона может быть, в общем-то, любой. Единственное, что желательно использовать один и тот же синтаксис как для работы с именами файлов, так и с макроподстановками.

## Ссылки ##
  * https://codegeneration.net/ Большая база по всевозможным генераторам кода]
  * http://www.stefansarstedt.com/dievorlagenmaschine_doc.html TemplateMachine - An open source template engine for C#]

## TemplateMachine ##

[TemplateMachine](http://www.stefansarstedt.com/dievorlagenmaschine_doc.html) вроде бы подходит.

Преимущества:
  * очень малый объём кода по сравнению с другими генераторами
  * за счёт динамической генерации позволяет использовать макро любой сложности (часть макро-кода можно просто писать на C#)

Features
  * templates use ASP.NET-like syntax (see below for an example)
  * you write your templates in C#, which can also reference other assemblies and import namespaces
  * templates can be called with arguments
  * templates can be loaded from a file or embedded as a resource in an assembly, which is useful if you want to write 'packaged' generators
  * templates can include other templates
  * script blocks with custom functions and variables can be added to a template
  * the template processor is command line based, that's why it's easy to include template processing in build-scripts like NAnt
  * it includes error handling, which means that line number and error information are  provided in case of syntax errors in your templates (does not yet work well in all cases though, but I think it's nevertheless very useful)
  * intermediate generated template code can be written to a file for debugging purposes
  * it's free and open source (released under GNU Lesser General Public License)
  * code is written in C#
  * it's fast


Особенности:
  * использует синтаксис, похожий на шаблоны ASP.NET

TemplateMachine использует динамическую генерацию: сначала по тексту шаблона генерируется текст программы на C#, затем он динамически компилируется (используется API CodeDOM .NET 2.0), затем полученный код выполняется. За счёт такого построения достигается как высокая скорость генерации, так и практически неограниченная гибкость при очень скромном размере самого генератора.

Пример шаблона:
```
<%@ Assembly Name="System.Xml" %>
<%@ Import NameSpace="System.Xml" %>
<%@ Import NameSpace="System.Collections" %>
<%@ Argument Name="className" Type="string" %>
<%@ Argument Name="attributes" Type="ArrayList" %>
public class <%=className%>
{
    <% foreach(string attr in attributes) { %>
    public string <%=attr%>;
    <% } %>
}
```

При работе с именами файлов символы **&lt;** и **&gt;** недопустимы, поэтому можно использовать, например **(%** и **%)**, а перед макроподстановкой заменять их на **&lt;%** и **%&gt;**

### Описание основных видов шаблонов ###

|`<%@ Assembly Name="System.Xml" %>` |	references and assembly that can be accessed in the current template |
|:-----------------------------------|:---------------------------------------------------------------------|
|`<%@ Import NameSpace="System.Threading" %>` |	imports a namespace for use in the current template                  |
|`<%@ Argument Name="name" Type="string" %>` |	declares a template parameter called name of type string; parameters must be passed to the generate(...) method and provided there in order of definition in the template.|
|`<%@ Include File="another.template" %>` 	|includes a template in the current template; you can therefore better organize your template files|
|`<script runat="template"></script>`|	defines helper variables and functions that can be called in your template|
|`<% for(int i=0; i<100; i++) { %>`  |	introduces a code block in the template which will be executed (don't forget to close braces, e.g. by somewhere later writing "<% } %>")|
|`<%=name%>`                         |	evaluates expression "name" and writes its result to the output      |
|`<% Response.Write("Hello!"); %>`   |	directly writes to the output stream, this is sometimes useful       |
## Diff/Merge/Patch ##
При использовании генерации возможны ситуации, когда придется ''соединять'' (merge) изменения, сделанные пользователем и сгенерированные автоматические.

Есть целый ряд библиотек для этого:

  * http://razor.occams.info/code/diff/Diff/Merge/Patch Library for C#/.NET. By Joshua Tauberer]
  * как обычно Google поможет: http://www.google.ru/search?q=C%23+diff+merge&ie=utf-8&oe=utf-8