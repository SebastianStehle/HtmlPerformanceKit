﻿using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.31 Script data double escaped dash dash state
        ///
        /// Consume the next input character:
        /// 
        /// "-" (U+002D)
        /// Emit a U+002D HYPHEN-MINUS character token.
        /// 
        /// "&lt;" (U+003C)
        /// Switch to the script data double escaped less-than sign state. Emit a U+003C LESS-THAN SIGN character token.
        /// 
        /// "&gt;" (U+003E)
        /// Switch to the script data state. Emit a U+003E GREATER-THAN SIGN character token.
        /// 
        /// U+0000 NULL
        /// Parse error. Switch to the script data double escaped state. Emit a U+FFFD REPLACEMENT CHARACTER character token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Switch to the script data double escaped state. Emit the current input character as a character token.
        /// </summary>
        private Action BuildScriptDataDoubleEscapedDashDashState() => () =>
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '-':
                    buffers.CurrentDataBuffer.Add('-');
                    return;

                case '<':
                    State = ScriptDataDoubleEscapedLessThanSignState;
                    buffers.CurrentDataBuffer.Add('<');
                    return;

                case '>':
                    State = ScriptDataState;
                    buffers.CurrentDataBuffer.Add('>');
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedNullCharacterInStream);
                    State = ScriptDataDoubleEscapedState;
                    buffers.CurrentDataBuffer.Add(HtmlChar.ReplacementCharacter);
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedEndOfFile);
                    State = DataState;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    State = ScriptDataDoubleEscapedState;
                    buffers.CurrentDataBuffer.Add((char)currentInputCharacter);
                    return;
            }
        };
    }
}
