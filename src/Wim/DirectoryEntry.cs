﻿//
// Copyright (c) 2008-2009, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.IO;
using System.Text;

namespace DiscUtils.Wim
{
    internal class DirectoryEntry
    {
        public long Length;
        public FileAttributes Attributes;
        public uint SecurityId;
        public long SubdirOffset;
        public long CreationTime;
        public long LastAccessTime;
        public long LastWriteTime;
        public byte[] Hash;
        public uint ReparseTag;
        public uint HardLink;
        public ushort StreamCount;
        public string ShortName;
        public string FileName;

        public static DirectoryEntry ReadFrom(DataReader reader)
        {
            long startPos = reader.Position;

            long length = reader.ReadInt64();
            if (length == 0)
            {
                return null;
            }

            DirectoryEntry result = new DirectoryEntry();
            result.Length = length;
            result.Attributes = (FileAttributes)reader.ReadUInt32();
            result.SecurityId = reader.ReadUInt32();
            result.SubdirOffset = reader.ReadInt64();
            reader.Skip(16);
            result.CreationTime = reader.ReadInt64();
            result.LastAccessTime = reader.ReadInt64();
            result.LastWriteTime = reader.ReadInt64();
            result.Hash = reader.ReadBytes(20);
            reader.Skip(4);
            result.ReparseTag = reader.ReadUInt32();
            result.HardLink = reader.ReadUInt32();
            result.StreamCount = reader.ReadUInt16();
            int shortNameLength = reader.ReadUInt16();
            int fileNameLength = reader.ReadUInt16();

            if (fileNameLength > 0)
            {
                result.FileName = Encoding.Unicode.GetString(reader.ReadBytes(fileNameLength + 2)).TrimEnd('\0');
            }
            else
            {
                result.FileName = "";
            }

            if (shortNameLength > 0)
            {
                result.ShortName = Encoding.Unicode.GetString(reader.ReadBytes(shortNameLength + 2)).TrimEnd('\0');
            }
            else
            {
                result.ShortName = "";
            }

            if (startPos + length > reader.Position)
            {
                int toRead = (int)(startPos + length - reader.Position);
                reader.Skip(toRead);
            }

            if (result.StreamCount > 0)
            {
                throw new NotImplementedException("Streams");
            }

            return result;
        }

        public string SearchName
        {
            get
            {
                if (FileName.IndexOf('.') == -1)
                {
                    return FileName + ".";
                }
                else
                {
                    return FileName;
                }
            }
        }
    }
}
