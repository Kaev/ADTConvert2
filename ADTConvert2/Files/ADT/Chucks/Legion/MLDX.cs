using ADTConvert2.Files.ADT.Entrys.Legion;
using ADTConvert2.Files.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace ADTConvert2.Files.ADT.Chucks.Legion
{
    /// <summary>
    /// MLDX Chunk - Contains model bounding information.
    /// </summary>
    public class MLDX : IIFFChunk, IBinarySerializable
    {
        /// <summary>
        /// Holds the binary chunk signature.
        /// </summary>
        public const string Signature = "MLDX";

        /// <summary>
        /// Gets or sets model extents.
        /// </summary>
        public List<MLDXEntry> Entries { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MLDX"/> class.
        /// </summary>
        public MLDX()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MLDX"/> class.
        /// </summary>
        /// <param name="inData">ExtendedData.</param>
        public MLDX(byte[] inData)
        {
            LoadBinaryData(inData);
        }

        /// <inheritdoc/>
        public void LoadBinaryData(byte[] inData)
        {
            using (var ms = new MemoryStream(inData))
            using (var br = new BinaryReader(ms))
            {
                var entryCount = br.BaseStream.Length / MLDXEntry.GetSize();

                for (var i = 0; i < entryCount; ++i)
                {
                    Entries.Add(new MLDXEntry(br.ReadBytes(MLDXEntry.GetSize())));
                }
            }
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
        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                foreach (MLDXEntry entry in Entries)
                {
                    ms.Write(entry.Serialize());
                }

                return ms.ToArray();
            }
        }
    }
}