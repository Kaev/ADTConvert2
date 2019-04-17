using System.IO;

namespace ADTConvert2.Files.ADT.Entrys
{
    public class MH2OInstance
    {
        /// <summary>
        /// Gets or sets the liquid type.
        /// </summary>
        public ushort LiquidTypeId { get; set; }
        /// <summary>
        /// Gets or sets the liquid vertex format.
        /// </summary>
        public ushort LiquidVertexFormat { get; set; }
        /// <summary>
        /// Gets or sets the minimum height level.
        /// </summary>
        public float MinHeightLevel { get; set; }
        /// <summary>
        /// Gets or sets the maximum height level.
        /// </summary>
        public float MaxHeightLevel { get; set; }
        /// <summary>
        /// Gets or sets the X axis offset.
        /// </summary>
        public byte OffsetX { get; set; }
        /// <summary>
        /// Gets or sets the Y axis offset.
        /// </summary>
        public byte OffsetY { get; set; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public byte Width { get; set; }
        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public byte Height { get; set; }
        /// <summary>
        /// Gets or sets the offset of ExistsBitmap.
        /// </summary>
        public uint OffsetExistsBitmap { get; set; }
        /// <summary>
        /// Gets or sets the offset of the vertex data.
        /// </summary>
        public uint OffsetVertexData { get; set; }
        /// <summary>
        /// Gets or sets the render bitmap bytes.
        /// </summary>
        public byte[] RenderBitmapBytes { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="MH2OInstanceVertexData"/> of the current <see cref="MH2OInstance"/>.
        /// </summary>
        public MH2OInstanceVertexData VertexData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MH2OInstance"/> class.
        /// </summary>
        /// <param name="data"></param>
        public MH2OInstance(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var br = new BinaryReader(ms))
                {
                    LiquidTypeId = br.ReadUInt16();
                    LiquidVertexFormat = br.ReadUInt16();
                    MinHeightLevel = br.ReadSingle();
                    MaxHeightLevel = br.ReadSingle();
                    OffsetX = br.ReadByte();
                    OffsetY = br.ReadByte();
                    Width = br.ReadByte();
                    Height = br.ReadByte();
                    OffsetExistsBitmap = br.ReadUInt32();
                    OffsetVertexData = br.ReadUInt32();

                    long positionAfterInstance = br.BaseStream.Position;

                    if (OffsetExistsBitmap > 0)
                    {
                        br.BaseStream.Position = OffsetExistsBitmap;
                        RenderBitmapBytes = br.ReadBytes((Width * Height + 7) / 8);
                    }

                    if (OffsetVertexData > 0)
                    {
                        br.BaseStream.Position = OffsetVertexData;
                        VertexData = new MH2OInstanceVertexData(br, this);
                    }

                    br.BaseStream.Position = positionAfterInstance;
                }
            }
        }

        /// <summary>
        /// Gets the size of an entry.
        /// </summary>
        /// <returns>The size.</returns>
        public static uint GetSize()
        {
            return sizeof(ushort) * 2 + sizeof(float) * 2 + sizeof(byte) * 4 + sizeof(uint) * 2;
        }

        /// <inheritdoc/>
        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(LiquidTypeId);
                // Write 2 vertex data can be ommitted - TODO: When can we omit this?
                /*if (OffsetVertexData == 0 && LiquidTypeId != 2)
                    writer.Write(2);
                else*/
                bw.Write(LiquidVertexFormat);
                bw.Write(MinHeightLevel);
                bw.Write(MaxHeightLevel);
                bw.Write(OffsetX);
                bw.Write(OffsetY);
                bw.Write(Width);
                bw.Write(Height);
                // We will write the Offset later in MH2O.Write
                bw.Write(0);
                // We will write the Offset later in MH2O.Write
                bw.Write(0);
                return ms.ToArray();
            }
        }
    }
}
