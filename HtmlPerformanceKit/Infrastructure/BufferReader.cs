﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HtmlPerformanceKit.Infrastructure
{
    internal class BufferReader : IDisposable
    {
        private readonly TextReader textReader;
        private readonly LinkedList<int> peekBuffer = new LinkedList<int>();

        internal BufferReader(TextReader textReader)
        {
            this.textReader = textReader;
        }

        internal int LineNumber { get; private set; }

        internal int LinePosition { get; private set; }

        public void Dispose()
        {
            textReader?.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal string Peek(int length)
        {
            var outputLength = Math.Min(peekBuffer.Count, length);

            while (peekBuffer.Count < length)
            {
                var currentInputCharacter = textReader.Read();

                peekBuffer.AddLast(currentInputCharacter);
                outputLength++;

                if (currentInputCharacter == -1)
                {
                    break;
                }
            }

            var intBuffer = peekBuffer.ToArray();
            var charBuffer = new char[outputLength];
            for (var index = 0; index < outputLength; index++)
            {
                if (intBuffer[index] == -1)
                {
                    break;
                }

                charBuffer[index] = (char)intBuffer[index];
            }

            return new string(charBuffer, 0, outputLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Consume(int length)
        {
            while (length > 0)
            {
                Consume();
                length--;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Consume()
        {
            if (peekBuffer.Count != 0)
            {
                var peekResult = peekBuffer.First.Value;
                peekBuffer.RemoveFirst();
                return peekResult;
            }

            if (LineNumber == 0)
            {
                LineNumber = 1;
                LinePosition = 1;
            }

            var result = textReader.Read();
            if (result == '\n')
            {
                LineNumber++;
                LinePosition = 1;
            }
            else
            {
                LinePosition++;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int ConsumeDigits()
        {
            long number = 0;
            var digitConsumed = false;

            while (true)
            {
                var currentInputChar = Peek();
                switch (currentInputChar)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        var thisNumber = Consume() - '0';

                        if (number < int.MaxValue)
                        {
                            digitConsumed = true;
                            number *= 10;
                            number += thisNumber;
                        }
                      
                        break;

                    default:
                        if (digitConsumed == false)
                        {
                            return int.MinValue;
                        }

                        if (number > int.MaxValue)
                        {
                            return int.MaxValue;
                        }

                        return (int)number;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int ConsumeHexDigits()
        {
            long number = 0;
            var digitConsumed = false;

            while (true)
            {
                var currentInputChar = Peek();
                var thisNumber = -1;
                switch (currentInputChar)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        thisNumber = Consume() - '0';
                        goto default;

                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                        thisNumber = Consume() - 'A' + 10;
                        goto default;

                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        thisNumber = Consume() - 'a' + 10;
                        goto default;

                    default:
                        if (thisNumber != -1 && number < int.MaxValue)
                        {
                            digitConsumed = true;
                            number *= 16;
                            number += thisNumber;
                            continue;
                        }

                        if (digitConsumed == false)
                        {
                            return int.MinValue;
                        }

                        if (number > int.MaxValue)
                        {
                            return int.MaxValue;
                        }

                        return (int)number;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reconsume(int data)
        {
            peekBuffer.AddFirst(data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int Peek()
        {
            if (peekBuffer.Count > 0)
            {
                return peekBuffer.First.Value;
            }

            var currentInputCharacter = textReader.Read();
            peekBuffer.AddLast(currentInputCharacter);

            return currentInputCharacter;
        }
    }
}
