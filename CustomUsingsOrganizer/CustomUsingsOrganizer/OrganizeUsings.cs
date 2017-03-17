//------------------------------------------------------------------------------
// <copyright file="OrganizeUsings.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;


namespace CustomUsingsOrganizer
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class OrganizeUsings
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("4a1069a2-6151-45e9-a036-e09a1f87e22c");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizeUsings"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private OrganizeUsings(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OrganizeUsings Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new OrganizeUsings(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));

            IVsTextManager txtMgr = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
            int mustHaveFocus = 1;
            IVsTextView currentTextView;
            txtMgr.GetActiveView(mustHaveFocus, null, out currentTextView);
            IVsUserData userData = currentTextView as IVsUserData;
            object holder;
            Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out holder);
            IWpfTextViewHost viewHost = (IWpfTextViewHost)holder;

            string allText = viewHost.TextView.TextSnapshot.GetText();

            CompilationUnitSyntax sourceCodeRoot = (CompilationUnitSyntax)CSharpSyntaxTree.ParseText(allText).GetRoot();
            CustomUsingsCollector collector = new CustomUsingsCollector("System");
            collector.Visit(sourceCodeRoot);

            var groupedMatchedUsings = collector.MatchedUsings.
                GroupBy(matchedUsing => matchedUsing.Parent).
                OrderByDescending(group => group.First().SpanStart);

            foreach (var matchedUsingGroup in groupedMatchedUsings)
            {
                foreach (var matchedUsing in matchedUsingGroup)
                {
                    VsShellUtilities.ShowMessageBox
                    (
                        ServiceProvider,
                        matchedUsing.ToString() + "\n" + matchedUsingGroup.Key.ToFullString(),
                        "AAA",
                        OLEMSGICON.OLEMSGICON_INFO,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                    );
                }
            }

            SnapshotPoint start = default(SnapshotPoint);
            SnapshotPoint end = default(SnapshotPoint);

            List<string> lines = new List<string>();

            foreach (var line in viewHost.TextView.TextSnapshot.Lines)
            {
                string lineContent = line.GetText();

                if(lineContent.StartsWith("#define"))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(lineContent))
                {
                    continue;
                }

                if (lineContent.StartsWith("using"))
                {
                    if(start != default(SnapshotPoint))
                    {
                        start = line.Start;
                    }

                    lines.Add(lineContent);
                }
                else
                {
                    end = line.Start;
                    break;
                }
            }

            Dictionary<int, List<string>> blocks = new Dictionary<int, List<string>>();

            string filePath = 
                Path.GetDirectoryName(dte.ActiveDocument.ProjectItem.ContainingProject.FullName) +
                Path.DirectorySeparatorChar +
                "UsingsOrder.txt";

            Dictionary<string, int> usingPriorities;

            if(File.Exists(filePath))
            {
                FileStream fileStream = File.OpenRead(filePath);

                StreamReader reader = new StreamReader(fileStream);

                int priority = 0;

                usingPriorities = new Dictionary<string, int>();

                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();

                    if (!string.IsNullOrEmpty(line))
                    {
                        usingPriorities[line] = priority++;
                    }
                }

                reader.Close();
            }
            else
            {
                usingPriorities = new Dictionary<string, int>()
                {
                    { "System" , 0 },
                    { "UnityEngine", 1 },
                    { "UniRx", 2 },
                    { "DG", 3 },
                    { "UnityDI", 4 },
                    { "DTI", 5 }
                };

                FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);

                StreamWriter writer = new StreamWriter(fileStream);

                foreach(string line in usingPriorities.Keys)
                {
                    writer.WriteLine(line);
                }

                writer.Close();
            }

            Action<int, string> addToBlock = (int priority, string line) =>
            {
                if (!blocks.ContainsKey(priority))
                {
                    blocks[priority] = new List<string>();
                }

                blocks[priority].Add(line);
            };

            foreach(string line in lines)
            {
                bool unclassified = true;

                foreach (string priorityKey in usingPriorities.Keys.Reverse())
                {
                    if(line.TrimPrefix("using").TrimStart().StartsWith(priorityKey))
                    {
                        addToBlock(usingPriorities[priorityKey], line);

                        unclassified = false;
                        break;
                    }
                }

                if(unclassified)
                {
                    addToBlock(usingPriorities.Count, line);
                }
            }

            string newLine = Environment.NewLine;
            string twoNewLines = newLine + newLine;
            string threeNewLines = twoNewLines + newLine;

            string sortedUsingsBlock = blocks.Keys.OrderBy(key => key).
                Select(key => blocks[key].OrderBy(text => text.TrimPrefix("using").TrimEnd(';').Trim()).Aggregate((all, next) => all + newLine + next)).
                Aggregate((all, next) => all + twoNewLines + next) + threeNewLines;

            viewHost.TextView.TextBuffer.Replace(new Span(start.Position, end.Position), sortedUsingsBlock);
        }
    }

    public sealed class CustomUsingsCollector : CSharpSyntaxWalker
    {
        private string _inspectedNamespace;

        public List<UsingDirectiveSyntax> MatchedUsings { get; private set; }


        public CustomUsingsCollector(string inspectedNamespace) : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            _inspectedNamespace = inspectedNamespace;
            MatchedUsings = new List<UsingDirectiveSyntax>();
        }


        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            string nodeContent = node.Name.ToString();

            if (nodeContent == _inspectedNamespace || nodeContent.StartsWith(_inspectedNamespace + "."))
            {
                MatchedUsings.Add(node);
            }
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            foreach (var descendant in node.WithBranchTaken(true).DescendantNodes())
            {
                CustomUsingsCollector collector = new CustomUsingsCollector(_inspectedNamespace);
                collector.Visit(descendant);

                MatchedUsings.AddRange(collector.MatchedUsings);
            }
        }
    }
}
