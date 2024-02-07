﻿using HtmlPerformanceKit.Infrastructure;
using System;

namespace HtmlPerformanceKit.StateMachine
{
    internal partial class HtmlStateMachine
    {
        /// <summary>
        /// 8.2.4.64 DOCTYPE system identifier (double-quoted) state
        ///
        /// Consume the next input character:
        /// 
        /// U+0022 QUOTATION MARK (")
        /// Switch to the after DOCTYPE system identifier state.
        /// 
        /// U+0000 NULL
        /// Parse error. Append a U+FFFD REPLACEMENT CHARACTER character to the current DOCTYPE token's system identifier.
        /// 
        /// "&gt;" (U+003E)
        /// Parse error. Set the DOCTYPE token's force-quirks flag to on. Switch to the data state. Emit that DOCTYPE token.
        /// 
        /// EOF
        /// Parse error. Switch to the data state. Set the DOCTYPE token's force-quirks flag to on. Emit that DOCTYPE token. Reconsume the EOF character.
        /// 
        /// Anything else
        /// Append the current input character to the current DOCTYPE token's system identifier.
        /// </summary>
        private Action BuildDoctypeSystemIdentifierDoubleQuotedState()
        {
            var currentInputCharacter = bufferReader.Consume();

            switch (currentInputCharacter)
            {
                case '"':
                    State = AfterDoctypeSystemIdentifierState;
                    return;

                case HtmlChar.Null:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    buffers.CurrentDoctypeToken.Attributes.Current.Value.Add(HtmlChar.ReplacementCharacter);
                    return;

                case '>':
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = DataState;
                    EmitDoctypeToken = buffers.CurrentDoctypeToken;
                    return;

                case EofMarker:
                    ParseError(ParseErrorMessage.UnexpectedCharacterInStream);
                    State = DataState;
                    EmitDoctypeToken = buffers.CurrentDoctypeToken;
                    bufferReader.Reconsume(EofMarker);
                    return;

                default:
                    buffers.CurrentDoctypeToken.Attributes.Current.Value.Add((char)currentInputCharacter);
                    return;
            }
        }
    }
}
