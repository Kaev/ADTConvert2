using System.IO;

namespace ADTConvert2.Files.ADT.Entrys
{
    public class MH2OInstanceVertexData
    {
        /// <summary>
        /// Gets or sets the height map.
        /// </summary>
        public float[,] HeightMap { get; set; } = new float[8, 8];
        /// <summary>
        /// Gets or sets the depth map.
        /// </summary>
        public byte[,] DepthMap { get; set; } = new byte[8, 8];

        /// <summary>
        /// Initializes a new instance of the <see cref="MH2OInstanceVertexData"/> class.
        /// </summary>
        /// <param name="data"></param>
        public MH2OInstanceVertexData(byte[] data, MH2OInstance instance)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var br = new BinaryReader(ms))
                {
                    if (instance.LiquidVertexFormat != 2)
                    {
                        for (byte z = instance.OffsetY; z < instance.Height + instance.OffsetY; z++)
                            for (byte x = instance.OffsetX; x < instance.Width + instance.OffsetX; x++)
                                HeightMap[z, x] = br.ReadSingle();
                    }

                    for (byte z = instance.OffsetY; z < instance.Height + instance.OffsetY; z++)
                        for (byte x = instance.OffsetX; x < instance.Width + instance.OffsetX; x++)
                            DepthMap[z, x] = br.ReadByte();
                }
            }
        }

        /// <summary>
        /// Gets the size of an entry.
        /// </summary>
        /// <returns>The size.</returns>
        public static uint GetSize()
        {
            return sizeof(float) * 64 + sizeof(byte) * 64;
        }

        /// <inheritdoc/>
        public byte[] Serialize(MH2OInstance instance)
        {
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                if (instance.LiquidVertexFormat != 2)
                {
                    for (byte z = instance.OffsetY; z < instance.Height + instance.OffsetY; z++)
                        for (byte x = instance.OffsetX; x < instance.Width + instance.OffsetX; x++)
                            bw.Write(HeightMap[z, x]);
                }

                for (byte z = instance.OffsetY; z < instance.Height + instance.OffsetY; z++)
                    for (byte x = instance.OffsetX; x < instance.Width + instance.OffsetX; x++)
                        bw.Write(DepthMap[z, x]);

                return ms.ToArray();
            }
        }
    }
}
