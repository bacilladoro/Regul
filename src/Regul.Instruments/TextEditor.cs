using Avalonia;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Utils;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Utilities;
using Avalonia.Controls.Metadata;
using Avalonia.Controls;

namespace Regul.Instruments
{
    [PseudoClasses(":empty")]
    public class TextEditor : TemplatedControl
    {
        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<TextEditor, string>(nameof(Text));


        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private string _text;

        private bool _doubleClick;
        //private UndoRedoHelper<UndoRedoState> _undoRedoHelper;

        static TextEditor()
        {

        }

        struct UndoRedoState : IEquatable<UndoRedoState>
        {
            public string Text { get; }
            public int CaretPosition { get; }

            public UndoRedoState(string text, int caretPosition)
            {
                Text = text;
                CaretPosition = caretPosition;
            }

            public bool Equals(UndoRedoState other) => ReferenceEquals(Text, other.Text) || Equals(Text, other.Text);
        }

        //UndoRedoState UndoRedoHelper<UndoRedoState>.IUndoRedoHost.UndoRedoState
        //{

        //}
    }
}
