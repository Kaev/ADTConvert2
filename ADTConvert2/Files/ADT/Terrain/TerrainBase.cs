using ADTConvert2.Attribute;
using ADTConvert2.Extensions;
using ADTConvert2.Files.ADT.Chunks;
using ADTConvert2.Files.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ADTConvert2.Files.ADT.Terrain
{
    public abstract class TerrainBase
    {
        /// <summary>
        /// Gets or sets the contains the ADT version.
        /// </summary>
        [Order(1)]
        public MVER Version { get; set; }

        /// <summary>
        /// Gets or sets the contains the ADT Header with offsets. The header has offsets to the other chunks in the
        /// ADT.
        /// </summary>
        [Order(2)]
        public MHDR Header { get; set; }

        /// <summary>
        /// Gets or sets the contains an array of offsets where MCNKs are in the file.
        /// </summary>
        //public MCNK Chunk { get; set; }

        /// <summary>
        /// Gets or sets the contains the ADT bounding box
        /// ADT.
        /// </summary>
        [Order(99)]
        public MFBO BoundingBox { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainBase"/> class.
        /// </summary>
        /// <param name="inData">The binary data.</param>
        public TerrainBase(byte[] inData)
        {
            LoadBinaryData(inData);
        }

        /// <inheritdoc/>
        public void LoadBinaryData(byte[] inData)
        {
            using (var ms = new MemoryStream(inData))
            using (var br = new BinaryReader(ms))
            {
                var terrainChunkProperties = GetType()
                    .GetProperties()
                    .OrderBy(p => ((OrderAttribute)p.GetCustomAttributes(typeof(OrderAttribute), false).Single()).Order);

                foreach (PropertyInfo chunkPropertie in terrainChunkProperties)
                {
                    var readIFFChunkMethod = br.GetType().GetExtensionMethod(Assembly.GetExecutingAssembly(), "ReadIFFChunk");
                    var methodCall = readIFFChunkMethod.MakeGenericMethod(chunkPropertie.PropertyType);
                    IIFFChunk chunk = (IIFFChunk)methodCall.Invoke(null, new[] { br });

                    chunkPropertie.SetValue(this, chunk);

                    Console.WriteLine(chunk.GetSignature());
                }
            }
        }
    }
}
