﻿using ADTConvert2.Files.ADT.Entry;
using ADTConvert2.Files.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace ADTConvert2.Files.ADT
{
    class MODF : IIFFChunk, IBinarySerializable
    {
        public const string Signature = "MODF";

        /// <summary>
        /// Gets or sets <see cref="MODFEntry"/>s.
        /// </summary>
        public List<MODFEntry> MODFEntrys { get; set; }

        public MODF()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MODF"/> class.
        /// </summary>
        /// <param name="inData">The binary data.</param>
        public MODF(byte[] inData)
        {
            LoadBinaryData(inData);
        }

        /// <inheritdoc/>
        public string GetSignature()
        {
            return Signature;
        }

        /// <inheritdoc/>
        public uint GetSize()
        {
            return (uint)Serialize().Length;
        }

        /// <inheritdoc/>
        public void LoadBinaryData(byte[] inData)
        {
            using (var ms = new MemoryStream(inData))
            using (var br = new BinaryReader(ms))
            {
                var objCount = br.BaseStream.Length / MODFEntry.GetSize();

                for (var i = 0; i < objCount; ++i)
                {
                    MODFEntrys.Add(new MODFEntry(br.ReadBytes(MODFEntry.GetSize())));
                }
            }
        }

        /// <inheritdoc/>
        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                foreach (MODFEntry obj in MODFEntrys)
                {
                    ms.Write(obj.Serialize());
                }

                return ms.ToArray();
            }
        }
    }
}
