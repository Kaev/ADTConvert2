using ADTConvert2.Extensions;
using ADTConvert2.Files.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace ADTConvert2.Files.ADT.Base
{
    /// <summary>
    /// MWMO Chunk - Contains a list of all referenced WMO models in this ADT.
    /// </summary>
    class MWMO : IIFFChunk, IBinarySerializable
    {
        public const string Signature = "MWMO";

        /// <summary>
        /// Gets or sets a list of full paths to the M2 models referenced in this ADT.
        /// </summary>
        public List<string> Filenames { get; set; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MWMO"/> class.
        /// </summary>
        public MWMO()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MWMO"/> class.
        /// </summary>
        /// <param name="inData">ExtendedData.</param>
        public MWMO(byte[] inData)
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
                Filenames.Add(br.ReadNullTerminatedString());
            }
        }

        /// <inheritdoc/>
        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.WriteNullTerminatedString(Filenames.ToArray());

                return ms.ToArray();
            }
        }
    }
}
