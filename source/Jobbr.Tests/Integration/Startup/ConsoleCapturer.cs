using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jobbr.Tests.Integration.Startup
{
    public class ConsoleCapturer : TextWriter
    {
        private readonly TextWriter consoleWriter;
        private readonly StringBuilder capture;

        public IEnumerable<string> GetLines()
        {
            return this.capture.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public IEnumerable<string> GetLines(params string[] needles)
        {
            var allLines = this.GetLines();

            foreach (var line in allLines)
            {
                if (needles.All(n => line.Contains(n)))
                {
                    yield return line;
                }
            }
        }

        public ConsoleCapturer()
        {
            this.consoleWriter = Console.Out;
            this.capture = new StringBuilder();

            Console.SetOut(this);
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            this.consoleWriter.Write(value);
            this.capture.Append(value);
        }

        protected override void Dispose(bool disposing)
        {
            Console.SetOut(this.consoleWriter);
            base.Dispose(disposing);
        }
    }
}