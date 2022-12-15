using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitFile
{
    internal abstract partial class Arguments
    {
        protected string[] args;
        public Arguments(string[] args) { this.args = args; }

        private static readonly string[] HelpArgs = new string[] { "-?", "-h", "-help" };
        protected static readonly string[] FileArgs = new string[]
        {
            "-f", "-file"
        };
        protected static readonly string[] SizeArgs = new string[]
        {
            "-s", "-size"
        };
        protected static readonly string[] TargetArgs = new string[]
        {
            "-t", "-target"
        };
        protected static readonly string[] SequentialArgs = new string[]
        {
            "-sequential"
        };

        private static readonly string CurrentStateTitle = "SGShardMigrate Current State";
        private static readonly string UpdatedStateTitle = "SGShardMigrate Updated State";

        internal static async Task Process(string[] args)
        {
            try
            {
                if (args.ContainsAny(Arguments.HelpArgs))
                {
                    ShowHelp();
                    return;
                }
                Arguments arguments = Create(args);
                await arguments.Run();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        protected virtual async Task Run()
        {
            ValidateArgs();
            SetParameters();
            await Execute();
        }

        protected virtual async Task Execute()
        {
        }

        protected virtual void SetParameters()
        {
            SetFilePath();
        }

        private void SetFilePath()
        {
            int index = GetNextArgsIndex(FileArgs);
            if (!File.Exists(args[index]))
                throw new Exception("<parameter> not a valid file path name");
            this.FilePath= args[index];
        }

        private static Arguments Create(string[] args)
        {
            if (args.ContainsAny(Arguments.TargetArgs))
                return new Combiner(args);
            if (args.ContainsAny(Arguments.SizeArgs) && args.ContainsAny(Arguments.SequentialArgs))
                return new SizeSplitterSequential(args);
            if (args.ContainsAny(Arguments.SizeArgs))
                return new SizeSplitterParallel(args);
            throw new Exception("No valid <arguments>");
        }

        protected virtual void ValidateArgs()
        {
            if (args.ContainsAny(Arguments.TargetArgs) && args.ContainsAny(Arguments.SizeArgs))
                throw new Exception("<arguments> combination not allowed");
        }

        protected string FilePath { get; set; }
        protected long FileLength
        {
            get
            {
                FileInfo fileInfo = new FileInfo(FilePath);
                return fileInfo.Length;
            }
        }

        protected int GetNextArgsIndex(string[] argTypes)
        {
            int index = args.Select(s => s.ToLowerInvariant()).IndexOfAny(argTypes) + 1;
            if (index >= args.Length)
                throw new Exception("Index out of range detected");
            return index;
        }

        private static readonly string HelpFileName = "SplitFile.hlp";
        private static void ShowHelp()
        {
            if (!File.Exists(HelpFileName))
                throw new Exception($"Help file {HelpFileName} not available");
            Console.WriteLine(File.ReadAllText(HelpFileName));
        }

        private static void ShowError(Exception ex)
        {
            ShowError($"Error: {ex.Message}\n{InnerException(ex)}");

            try
            {
                ShowHelp();
            }
            catch (Exception) { }
        }

        private static void ShowError(string message)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{message}");

            Console.ForegroundColor = color;
        }

        private static string InnerException(Exception ex)
            => ex.InnerException != null ? $"{ex.InnerException.Message}\n" : "";
    }
}
