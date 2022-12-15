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
        private class Combiner : Arguments
        {
            public Combiner(string[] args) : base(args) { }

            protected string TargetPath { get; set; }

            private string SourceFilePath
            {
                get
                {
                    string fileName = Path.GetFileNameWithoutExtension(FilePath);
                    string directory = Path.GetDirectoryName(FilePath);
                    return Path.Combine(directory, fileName);
                }
            }

            protected override void SetParameters()
            {
                SetTargetPath();

                base.SetParameters();
            }

            private void SetTargetPath()
            {
                int index = GetNextArgsIndex(TargetArgs);
                if (File.Exists(args[index]))
                    throw new Exception("<parameter> not a valid file path name");
                this.TargetPath = args[index];
            }

            protected override void ValidateArgs()
            {
                if (args.ContainsMultiple(TargetArgs))
                    throw new Exception("Multiple -target arguments found");

                base.ValidateArgs();
            }

            protected override async Task Execute()
            {
                string source = SourceFilePath;
                byte[] buffer = new byte[4000];
                int partNbr = 1;
                int read;
                using (var writer = new FileStream(TargetPath, FileMode.Create, FileAccess.Write))
                {
                    var readerPath = $"{source}.{partNbr}";
                    while (File.Exists(readerPath))
                    {
                        using (var reader = new FileStream(readerPath, FileMode.Open, FileAccess.Read))
                        {
                            do
                            {
                                read = await reader.ReadAsync(buffer, 0, buffer.Length);

                                await writer.WriteAsync(buffer, 0, read);
                            } while (read == buffer.Length);
                        }

                        partNbr += 1;
                        readerPath = $"{source}.{partNbr}";
                    }
                }
            }

        }
    }
}
