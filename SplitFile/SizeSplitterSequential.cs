using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitFile
{
    internal abstract partial class Arguments
    {
        private class SizeSplitterSequential : Arguments
        {
            public SizeSplitterSequential(string[] args) : base(args) { }

            protected long Size { get; set; }

            private string DestinationFilePath
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
                SetSize();

                base.SetParameters();
            }

            private void SetSize()
            {
                int index = GetNextArgsIndex(SizeArgs);
                if (!long.TryParse(args[index], out long Size))
                    throw new Exception("Size is not a valid integer");
                if (Size < 100)
                    throw new Exception($"Size is smaller than 100");
                this.Size = Size;
            }

            protected override void ValidateArgs()
            {
                if (args.ContainsMultiple(SizeArgs))
                    throw new Exception("Multiple -size arguments found");

                base.ValidateArgs();
            }

            protected override async Task Execute()
            {
                string destination = DestinationFilePath;
                byte[] buffer = new byte[4000];
                int partNbr = 1;
                int totalRead = 0;
                int read;
                int length;
                using (var reader = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    while (totalRead < reader.Length)
                    {
                        var writerPath = $"{destination}.{partNbr}";
                        using (var writer = new FileStream(writerPath, FileMode.Create, FileAccess.Write))
                        {
                            int partRead = 0;
                            do
                            {
                                length = buffer.Length < (int)(Size - partRead) ? buffer.Length : (int)(Size - partRead);
                                read = await reader.ReadAsync(buffer, 0, length);
                                totalRead += read;
                                partRead += read;

                                await writer.WriteAsync(buffer, 0, read);
                            } while (read == length && partRead < Size);
                        }

                        partNbr += 1;
                    }
                }
            }
        }
    }
}
