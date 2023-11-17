using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Domain.CodeAnalysis.Contract.DataClasses
{
    public struct LexerToken<TKind> where TKind : struct
    {
        public TKind Kind { get; }
        public string Text { get; }

        public int Position { get; }
        public int Line { get; }
        public int Column { get; }

        public LexerToken(TKind kind, int position, int line, int column, string? text = null)
        {
            Text = text ?? string.Empty;
            Kind = kind;
            Position = position;
            Line = line;
            Column = column;
        }
    }
}
