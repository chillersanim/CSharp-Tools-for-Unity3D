// Solution:         Unity Tools
// Project:          UnityTools
// Filename:         StreamReplacement.cs
// 
// Created:          27.01.2020  22:50
// Last modified:    05.02.2020  19:39
// 
// --------------------------------------------------------------------------------------
// 
// MIT License
// 
// Copyright (c) 2019 chillersanim
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

using System;
using System.Collections.Generic;

namespace Unity_Tools.Text
{
    /// <summary>
    /// Class that handles replacement of string occurrences that match a mask in a string stream. 
    /// </summary>
    public class StreamReplacement
    {
        // Buffer for unverified characters
        private readonly List<char> buffer;

        private string mask;

        // Current index on the mask for the next character added
        private int maskPosition;
        private Action<char> output;

        private string replacement;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamReplacement"/> class.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="mask">The mask.</param>
        /// <param name="replacement">The replacement string for mask encounters.</param>
        /// <exception cref="System.ArgumentNullException">Output cannot be null.</exception>
        /// <exception cref="System.ArgumentException">The mask must contain at least one character.</exception>
        public StreamReplacement(Action<char> output, string mask, string replacement)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.mask = string.IsNullOrEmpty(mask) ? throw new ArgumentException("The mask must contain at least one character.") : mask;
            this.replacement = replacement ?? string.Empty;
            this.buffer = new List<char>(mask.Length);
        }

        /// <summary>
        /// Gets or sets the mask to test for.
        /// </summary>
        /// <exception cref="System.ArgumentException">The mask must contain at least one character.</exception>
        public string Mask
        {
            get => this.mask;
            set
            {
                if (string.IsNullOrEmpty(mask))
                {
                    throw new ArgumentException("The mask must contain at least one character.");
                }

                this.mask = value;

                // Clean buffer and throw out characters that can't be part of the mask anymore.
                WriteVerifiedCharactersFromBuffer();
            }
        }

        /// <summary>
        /// All verified characters will be passed to the output
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The output cannot be null.</exception>
        public Action<char> Output
        {
            get => output;
            set => output = value ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Gets or sets the replacement string to output when the mask has been encountered.
        /// </summary>
        public string Replacement
        {
            get => replacement;
            set => replacement = value ?? string.Empty;
        }

        /// <summary>
        /// Flushes all buffered characters, even if they could be part of the mask.<br/>
        /// Starts from scratch after flushing.
        /// </summary>
        public void Flush()
        {
            foreach (var c in buffer)
            {
                output(c);
            }

            buffer.Clear();
            maskPosition = 0;
        }

        /// <summary>
        /// Clears the buffer without writing any buffered data to the output stream.
        /// </summary>
        public void Reset()
        {
            buffer.Clear();
            maskPosition = 0;
        }

        /// <summary>
        /// Writes a single character to the stream.
        /// </summary>
        public void Write(char c)
        {
            WriteCharacter(c);
        }

        /// <summary>
        /// Writes the specified text to the stream.
        /// </summary>
        public void Write(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            foreach (var c in text)
            {
                WriteCharacter(c);
            }
        }

        /// <summary>
        /// Writes the specified characters to the stream.
        /// </summary>
        /// <param name="characters">The characters.</param>
        public void Write(params char[] characters)
        {
            if (characters == null)
            {
                return;
            }

            foreach (var c in characters)
            {
                WriteCharacter(c);
            }
        }

        /// <summary>
        /// Stores a character in the buffer that could be part of the mask.
        /// </summary>
        private void AddUnverifiedCharacter(char c)
        {
            buffer.Add(c);
            maskPosition++;

            if (maskPosition == mask.Length)
            {
                buffer.Clear();
                maskPosition = 0;

                foreach (var repChar in replacement)
                {
                    output(repChar);
                }
            }
        }

        /// <summary>
        /// Determines whether the buffer content can be part of the mask, starting at the given index.
        /// </summary>
        private bool IsBufferPartOfMask(int offset)
        {
            for (var i = 0; i < buffer.Count - offset; i++)
            {
                var c = buffer[offset + i];
                if (c != mask[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Writes the character either to the buffer or output stream and handles masking.
        /// </summary>
        private void WriteCharacter(char c)
        {
            if (mask[maskPosition] == c)
            {
                AddUnverifiedCharacter(c);
            }
            else
            {
                buffer.Add(c);
                WriteVerifiedCharactersFromBuffer();
            }
        }

        /// <summary>
        /// Writes the beginning part of buffer to the output
        /// </summary>
        private void WritePartOfBuffer(int count)
        {
            for (var i = 0; i < count; i++)
            {
                output(buffer[i]);
            }
        }

        /// <summary>
        /// Writes characters that cannot be part of the mask to the output.
        /// </summary>
        private void WriteVerifiedCharactersFromBuffer()
        {
            // Find possible new start position in buffer
            var newStart = 0;

            for (; newStart < buffer.Count; newStart++)
            {
                if (IsBufferPartOfMask(newStart))
                {
                    WritePartOfBuffer(newStart);
                    buffer.RemoveRange(0, newStart);
                    maskPosition = buffer.Count;
                    return;
                }
            }

            WritePartOfBuffer(buffer.Count);
            buffer.Clear();
            maskPosition = 0;
        }
    }
}
