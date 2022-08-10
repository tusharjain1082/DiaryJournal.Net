﻿// Copyright (c) 2013-2018 Cemalettin Dervis, MIT License.
// https://github.com/cemdervis/SharpConfig

using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SharpConfig
{
  internal static class ConfigurationWriter
  {
    // We need this, as we never want to close the stream the user has given us.
    // But we also want to call the specified writer's Dispose() method.
    // We wouldn't need this if we were targeting .NET 4+, because BinaryWriter
    // gives us the option to leave the stream open after Dispose(), but not
    // on .NET lower than 4.0.
    // To circumvent this, we just define our own writer that does not close
    // the underlying stream in Dispose().
    private class NonClosingBinaryWriter : BinaryWriter
    {
      public NonClosingBinaryWriter(Stream stream)
          : base(stream)
      { }

      protected override void Dispose(bool disposing)
      { }
    }

    internal static void WriteToStreamTextual(Configuration cfg, Stream stream, Encoding encoding)
    {
      Debug.Assert(cfg != null);

      if (stream == null)
        throw new ArgumentNullException("stream");

      if (encoding == null)
        encoding = Encoding.UTF8;

      var str = cfg.StringRepresentation;

      // Encode & write the string.
      var byteBuffer = new byte[encoding.GetByteCount(str)];
      int byteCount = encoding.GetBytes(str, 0, str.Length, byteBuffer, 0);

      stream.Write(byteBuffer, 0, byteCount);
      stream.Flush();
    }

    internal static void WriteToStreamBinary(Configuration cfg, Stream stream, BinaryWriter writer)
    {
      Debug.Assert(cfg != null);

      if (stream == null)
        throw new ArgumentNullException("stream");

      if (writer == null)
        writer = new NonClosingBinaryWriter(stream);

      writer.Write(cfg.SectionCount);

      foreach (var section in cfg)
      {
        writer.Write(section.Name);
        writer.Write(section.SettingCount);

        WriteCommentsBinary(writer, section);

        // Write the section's settings.
        foreach (var setting in section)
        {
          writer.Write(setting.Name);
          writer.Write(setting.RawValue);

          WriteCommentsBinary(writer, setting);
        }
      }

      writer.Close();
    }

    private static void WriteCommentsBinary(BinaryWriter writer, ConfigurationElement element)
    {
      writer.Write(element.Comment != null);
      if (element.Comment != null)
      {
        // SharpConfig <3.0 wrote the comment char here.
        // We'll just write a single char for backwards-compatibility.
        writer.Write(' ');
        writer.Write(element.Comment);
      }

      writer.Write(element.PreComment != null);
      if (element.PreComment != null)
      {
        // Same as with inline comments above.
        writer.Write(' ');
        writer.Write(element.PreComment);
      }
    }

  }
}