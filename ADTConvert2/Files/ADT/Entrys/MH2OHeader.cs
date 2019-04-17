using System.IO;

namespace ADTConvert2.Files.ADT.Entrys
{
    public class MH2OHeader
    {
        /// <summary>
        /// Gets or sets the offset of the <see cref="MH2OInstance"/>s.
        /// </summary>
        public uint OffsetInstances { get; set; }
        /// <summary>
        /// Gets or sets the amount of <see cref="MH2OInstance"/>.
        /// </summary>
        public uint LayerCount { get; set; }
        /// <summary>
        /// Gets or sets the offset of the <see cref="MH2OAttribute"/>s.
        /// </summary>
        public uint OffsetAttributes { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="MH2OInstance"/>s of the current <see cref="MH2OHeader"/>.
        /// </summary>
        public MH2OInstance[] Instances { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="MH2OAttribute"/> of the current <see cref="MH2OHeader"/>.
        /// </summary>
        public MH2OAttribute Attributes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MH2OHeader"/> class.
        /// </summary>
        /// <param name="data"></param>
        public MH2OHeader(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var br = new BinaryReader(ms))
                {
                    OffsetInstances = br.ReadUInt32();
                    LayerCount = br.ReadUInt32();
                    OffsetAttributes = br.ReadUInt32();
                    Instances = new MH2OInstance[LayerCount];

                    if (LayerCount > 0)
                    {
                        long positionAfterHeader = br.BaseStream.Position;

                        br.BaseStream.Position = OffsetInstances;
                        for (int i = 0; i < LayerCount; i++)
                            Instances[i] = new MH2OInstance(br);

                        if (OffsetAttributes > 0)
                        {
                            br.BaseStream.Position = OffsetAttributes;
                            Attributes = new MH2OAttribute(br);
                        }

                        br.BaseStream.Position = positionAfterHeader;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the size of an entry.
        /// </summary>
        /// <returns>The size.</returns>
        public static uint GetSize()
        {
            return sizeof(uint) * 3;
        }

        /// <inheritdoc/>
        public byte[] Serialize()
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                // We will write the Offset later in MH2O.Write
                bw.Write(0);
                bw.Write(LayerCount);
                // We will write the Offset later in MH2O.Write
                bw.Write(0);

                return ms.ToArray();
            }
        }
    }
}
