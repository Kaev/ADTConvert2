using System.IO;
using ADTConvert2.Files.ADT.Entrys;
using ADTConvert2.Files.Interfaces;

namespace ADTConvert2.Files.ADT.Chunks
{
    public class MH2O : IIFFChunk, IBinarySerializable
    {
        public const string Signature = "MH2O";

        /// <summary>
        /// Gets or sets <see cref="MH2OHeader"/>s.
        /// </summary>
        public MH2OHeader[] MH2OHeaders { get; set; } = new MH2OHeader[256];

        public MH2O()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MH2O"/> class.
        /// </summary>
        /// <param name="inData">The binary data.</param>
        public MH2O(byte[] inData)
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
            uint size = MH2OHeader.GetSize() * 256;
            for (int i = 0; i < 256; i++)
            {
                if (MH2OHeaders[i].LayerCount > 0)
                    size += MH2OAttribute.GetSize();

                for (int j = 0; j < MH2OHeaders[i].LayerCount; j++)
                {
                    size += MH2OInstance.GetSize();

                    if (MH2OHeaders[i].Instances[j].OffsetExistsBitmap > 0)
                        size += (uint)MH2OHeaders[i].Instances[j].RenderBitmapBytes.Length;

                    if (MH2OHeaders[i].Instances[j].OffsetVertexData > 0)
                        size += MH2OInstanceVertexData.GetSize();
                }
            }
            return size;
        }

        /// <inheritdoc/>
        public void LoadBinaryData(byte[] inData)
        {
            using (var ms = new MemoryStream(inData))
            using (var br = new BinaryReader(ms))
            {
                for (int i = 0; i < 256; i++)
                    MH2OHeaders[i] = new MH2OHeader(this);
            }
        }

        /// <inheritdoc/>
        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                // Write header data
                for (int i = 0; i < 256; i++)
                    MH2OHeaders[i].Write(bw);

                // Write instance data
                for (int i = 0; i < 256; i++)
                {
                    // We already wrote 0 for the offsets so we don't need to write anything here if LayerCount == 0
                    if (MH2OHeaders[i].LayerCount > 0)
                    {
                        MH2OHeaders[i].OffsetInstances = (uint)bw.BaseStream.Position;
                        var positionBeforeInstances = bw.BaseStream.Position;
                        bw.BaseStream.Position = MH2OHeaders[i].OffsetInstancesPosition;
                        bw.Write(MH2OHeaders[i].OffsetInstances);
                        bw.BaseStream.Position = positionBeforeInstances;
                        for (int j = 0; j < MH2OHeaders[i].LayerCount; j++)
                            MH2OHeaders[i].Instances[j].Write(bw);
                    }
                }

                // Write referenced data
                for (int i = 0; i < 256; i++)
                {
                    // We already wrote 0 for the offsets so we don't need to write anything here if LayerCount == 0
                    if (MH2OHeaders[i].LayerCount > 0)
                    {
                        // Write header attributes data
                        // We can omit this chunk if it only contains 0. In this case we already have 0 as an offset.
                        if (!MH2OHeaders[i].Attributes.HasOnlyZeroes)
                        {
                            MH2OHeaders[i].OffsetAttributes = (uint)bw.BaseStream.Position;
                            MH2OHeaders[i].Attributes.Write(bw);
                            var positionAfterCurrentAttributes = bw.BaseStream.Position;
                            bw.BaseStream.Position = MH2OHeaders[i].OffsetAttributesPosition;
                            bw.Write(MH2OHeaders[i].OffsetAttributes);
                            bw.BaseStream.Position = positionAfterCurrentAttributes;
                        }

                        for (int j = 0; j < MH2OHeaders[i].LayerCount; j++)
                        {
                            // Write RenderBitmapBytes if the length of the array is correct
                            if (MH2OHeaders[i].Instances[j].RenderBitmapBytes.Length == (MH2OHeaders[i].Instances[j].Width * MH2OHeaders[i].Instances[j].Height + 7) / 8)
                            {
                                MH2OHeaders[i].Instances[j].OffsetExistsBitmap = (uint)bw.BaseStream.Position;
                                bw.Write(MH2OHeaders[i].Instances[j].RenderBitmapBytes);
                                var positionAfterRenderBitmapBytes = bw.BaseStream.Position;
                                bw.BaseStream.Position = MH2OHeaders[i].Instances[j].OffsetExistsBitmapPosition;
                                bw.Write(MH2OHeaders[i].Instances[j].OffsetExistsBitmap);
                                bw.BaseStream.Position = positionAfterRenderBitmapBytes;
                            }

                            // Write instance vertex data - TODO: When can we omit this?
                            MH2OHeaders[i].Instances[j].OffsetVertexDataPosition = (uint)bw.BaseStream.Position;
                            MH2OHeaders[i].Instances[j].VertexData.Write(bw, MH2OHeaders[i].Instances[j]);
                            var positionAfterVertexData = bw.BaseStream.Position;
                            bw.BaseStream.Position = MH2OHeaders[i].Instances[j].OffsetVertexDataPosition;
                            bw.Write(MH2OHeaders[i].Instances[j].OffsetVertexDataPosition);
                            bw.BaseStream.Position = positionAfterVertexData;
                        }
                    }
                }
                return ms.ToArray();
            }
        }
    }
}
