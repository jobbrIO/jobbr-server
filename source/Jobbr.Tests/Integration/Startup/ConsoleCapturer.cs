using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jobbr.Tests.Integration.Startup
{
    public class ConsoleCapturer : TextWriter
    {
        private readonly TextWriter _consoleWriter;
        private readonly StringBuilder _capture;

        public ConsoleCapturer()
        {
            _consoleWriter = Console.Out;
            _capture = new StringBuilder();

            Console.SetOut(this);
        }

        public IEnumerable<string> GetLines()
        {
            return _capture.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public IEnumerable<string> GetLines(params string[] needles)
        {
            return GetLines().Where(l => needles.Any(l.Contains));
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            _consoleWriter.Write(value);
            _capture.Append(value);
        }

        protected override void Dispose(bool disposing)
        {
            Console.SetOut(_consoleWriter);
            base.Dispose(disposing);
        }
    }
}