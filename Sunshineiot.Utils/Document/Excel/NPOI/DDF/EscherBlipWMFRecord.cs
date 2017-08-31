
/* ====================================================================
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for Additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
==================================================================== */


namespace Sunshineiot.Utils.NPOI.DDF
{
    using System;
    using System.Text;
    using System.Collections;
    using Sunshineiot.Utils.NPOI.Util;
    using System.IO;
    using Sunshineiot.Utils.NPOI.HSSF.Record;
    using Sunshineiot.Utils.Ionic.Zlib;


    /// <summary>
    /// The blip record is used to hold details about large binary objects that occur in escher such
    /// as JPEG, GIF, PICT and WMF files.  The contents of the stream is usually compressed.  Inflate
    /// can be used to decompress the data.
    /// @author Glen Stampoultzis
    /// </summary>
    public class EscherBlipWMFRecord : EscherBlipRecord
    {
        //    public static short  RECORD_ID_START    = (short) 0xF018;
        //    public static short  RECORD_ID_END      = (short) 0xF117;
        public new const String RECORD_DESCRIPTION = "msofbtBlip";
        private const int HEADER_SIZE = 8;

        private byte[] field_1_secondaryUID;
        private int field_2_cacheOfSize;
        private int field_3_boundaryTop;
        private int field_4_boundaryLeft;
        private int field_5_boundaryWidth;
        private int field_6_boundaryHeight;
        private int field_7_width;
        private int field_8_height;
        private int field_9_cacheOfSavedSize;
        private byte field_10_compressionFlag;
        private byte field_11_filter;
        private byte[] field_12_data;


        /// <summary>
        /// This method deserializes the record from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the escher record information</param>
        /// <param name="offset">The starting offset into</param>
        /// <param name="recordFactory">May be null since this is not a container record.</param>
        /// <returns>
        /// The number of bytes Read from the byte array.
        /// </returns>
        public override int FillFields(byte[] data, int offset,
                                  EscherRecordFactory recordFactory
                                  )
        {
            int bytesAfterHeader = ReadHeader(data, offset);
            int pos = offset + HEADER_SIZE;

            int size = 0;
            field_1_secondaryUID = new byte[16];
            Array.Copy(data, pos + size, field_1_secondaryUID, 0, 16); size += 16;
            field_2_cacheOfSize = LittleEndian.GetInt(data, pos + size); size += 4;
            field_3_boundaryTop = LittleEndian.GetInt(data, pos + size); size += 4;
            field_4_boundaryLeft = LittleEndian.GetInt(data, pos + size); size += 4;
            field_5_boundaryWidth = LittleEndian.GetInt(data, pos + size); size += 4;
            field_6_boundaryHeight = LittleEndian.GetInt(data, pos + size); size += 4;
            field_7_width = LittleEndian.GetInt(data, pos + size); size += 4;
            field_8_height = LittleEndian.GetInt(data, pos + size); size += 4;
            field_9_cacheOfSavedSize = LittleEndian.GetInt(data, pos + size); size += 4;
            field_10_compressionFlag = data[pos + size]; size++;
            field_11_filter = data[pos + size]; size++;

            int bytesRemaining = bytesAfterHeader - size;
            field_12_data = new byte[bytesRemaining];
            Array.Copy(data, pos + size, field_12_data, 0, bytesRemaining);
            size += bytesRemaining;

            return HEADER_SIZE + size;
        }


        /// <summary>
        /// This method Serializes this escher record into a byte array.
        /// @param offset   
        /// </summary>
        /// <param name="offset">The offset into data to start writing the record data to.</param>
        /// <param name="data">the data array to Serialize to</param>
        /// <param name="listener">a listener for begin and end serialization events.</param>
        /// <returns>the number of bytes written.</returns>
        public override int Serialize(int offset, byte[] data, EscherSerializationListener listener)
        {
            listener.BeforeRecordSerialize(offset, RecordId, this);

            LittleEndian.PutShort(data, offset, Options);
            LittleEndian.PutShort(data, offset + 2, RecordId);
            int remainingBytes = field_12_data.Length + 36;
            LittleEndian.PutInt(data, offset + 4, remainingBytes);

            int pos = offset + HEADER_SIZE;
            Array.Copy(field_1_secondaryUID, 0, data, pos, 16); pos += 16;
            LittleEndian.PutInt(data, pos, field_2_cacheOfSize); pos += 4;
            LittleEndian.PutInt(data, pos, field_3_boundaryTop); pos += 4;
            LittleEndian.PutInt(data, pos, field_4_boundaryLeft); pos += 4;
            LittleEndian.PutInt(data, pos, field_5_boundaryWidth); pos += 4;
            LittleEndian.PutInt(data, pos, field_6_boundaryHeight); pos += 4;
            LittleEndian.PutInt(data, pos, field_7_width); pos += 4;
            LittleEndian.PutInt(data, pos, field_8_height); pos += 4;
            LittleEndian.PutInt(data, pos, field_9_cacheOfSavedSize); pos += 4;
            data[pos++] = field_10_compressionFlag;
            data[pos++] = field_11_filter;
            Array.Copy(field_12_data, 0, data, pos, field_12_data.Length); pos += field_12_data.Length;

            listener.AfterRecordSerialize(pos, RecordId, pos - offset, this);
            return pos - offset;
        }

        /// <summary>
        /// Returns the number of bytes that are required to Serialize this record.
        /// </summary>
        /// <value>Number of bytes</value>
        public override int RecordSize
        {
            get { return 58 + field_12_data.Length; }
        }

        /// <summary>
        /// The short name for this record
        /// </summary>
        /// <value></value>
        public override String RecordName
        {
            get { return "Blip"; }
        }

        /// <summary>
        /// Gets or sets the secondary UID.
        /// </summary>
        /// <value>The secondary UID.</value>
        public byte[] SecondaryUID
        {
            get { return field_1_secondaryUID; }
            set { this.field_1_secondaryUID = value; }
        }


        /// <summary>
        /// Gets or sets the size of the cache of.
        /// </summary>
        /// <value>The size of the cache of.</value>
        public int CacheOfSize
        {
            get { return field_2_cacheOfSize; }
            set { this.field_2_cacheOfSize = value; }
        }

        /// <summary>
        /// Gets or sets the top boundary of the metafile drawing commands
        /// </summary>
        /// <value>The boundary top.</value>
        public int BoundaryTop
        {
            get { return field_3_boundaryTop; }
            set { this.field_3_boundaryTop = value; }
        }

        /// <summary>
        /// Gets or sets the left boundary of the metafile drawing commands
        /// </summary>
        /// <value>The boundary left.</value>
        public int BoundaryLeft
        {
            get { return field_4_boundaryLeft; }
            set { this.field_4_boundaryLeft = value; }
        }


        /// <summary>
        /// Gets or sets the boundary width of the metafile drawing commands
        /// </summary>
        /// <value>The width of the boundary.</value>
        public int BoundaryWidth
        {
            get { return field_5_boundaryWidth; }
            set { this.field_5_boundaryWidth = value; }
        }


        /// <summary>
        /// Gets or sets the boundary height of the metafile drawing commands
        /// </summary>
        /// <value>The height of the boundary.</value>
        public int BoundaryHeight
        {
            get { return field_6_boundaryHeight; }
            set { this.field_6_boundaryHeight = value; }
        }

        /// <summary>
        /// Gets or sets the width of the metafile in EMU's (English Metric Units).
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get { return field_7_width; }
            set { this.field_7_width = value; }
        }

        /// <summary>
        /// Gets or sets the height of the metafile in EMU's (English Metric Units).
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get { return field_8_height; }
            set { this.field_8_height = value; }
        }

        /// <summary>
        /// Gets or sets the cache of the saved size
        /// </summary>
        /// <value>the cache of the saved size.</value>
        public int CacheOfSavedSize
        {
            get{return field_9_cacheOfSavedSize;}
            set { this.field_9_cacheOfSavedSize = value; }
        }

        /// <summary>
        /// Is the contents of the blip compressed?
        /// </summary>
        /// <value>The compression flag.</value>
        public byte CompressionFlag
        {
            get { return field_10_compressionFlag; }
            set { this.field_10_compressionFlag = value; }
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        public byte Filter
        {
            get { return field_11_filter; }
            set { this.field_11_filter = value; }
        }

        /// <summary>
        /// Gets or sets The BLIP data
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data
        {
            get { return field_12_data; }
            set { this.field_12_data = value; }
        }


        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override String ToString()
        {
            String nl = Environment.NewLine;

            String extraData;
            using (MemoryStream b = new MemoryStream())
            {
                try
                {
                    HexDump.Dump(this.field_12_data, 0, b, 0);
                    extraData = b.ToString();
                }
                catch (Exception e)
                {
                    extraData = e.ToString();
                }
                return GetType().Name + ":" + nl +
                        "  RecordId: 0x" + HexDump.ToHex(RecordId) + nl +
                        "  Options: 0x" + HexDump.ToHex(Options) + nl +
                        "  Secondary UID: " + HexDump.ToHex(field_1_secondaryUID) + nl +
                        "  CacheOfSize: " + field_2_cacheOfSize + nl +
                        "  BoundaryTop: " + field_3_boundaryTop + nl +
                        "  BoundaryLeft: " + field_4_boundaryLeft + nl +
                        "  BoundaryWidth: " + field_5_boundaryWidth + nl +
                        "  BoundaryHeight: " + field_6_boundaryHeight + nl +
                        "  X: " + field_7_width + nl +
                        "  Y: " + field_8_height + nl +
                        "  CacheOfSavedSize: " + field_9_cacheOfSavedSize + nl +
                        "  CompressionFlag: " + field_10_compressionFlag + nl +
                        "  Filter: " + field_11_filter + nl;
                //"  Data:" + nl + extraData;
            }
        }

        /// <summary>
        /// Compress the contents of the provided array
        /// </summary>
        /// <param name="data">An uncompressed byte array</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] data)
        {
            using (MemoryStream out1 = new MemoryStream())
            {
                ZlibStream deflaterOutputStream = new ZlibStream(out1, CompressionMode.Compress);
                try
                {
                    //for (int i = 0; i < data.Length; i++)
                    //deflaterOutputStream.WriteByte(data[i]);
                    deflaterOutputStream.Write(data, 0, data.Length);   //Tony Qu changed the code
                    return out1.ToArray();
                }
                catch (IOException e)
                {
                    throw new RecordFormatException(e.ToString());
                }
                finally
                {
                    out1.Close();
                    if (deflaterOutputStream != null)
                    {
                        deflaterOutputStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Decompresses the specified data.
        /// </summary>
        /// <param name="data">The compressed byte array.</param>
        /// <param name="pos">The starting position into the byte array.</param>
        /// <param name="Length">The number of compressed bytes to decompress.</param>
        /// <returns>An uncompressed byte array</returns>
        public static byte[] Decompress(byte[] data, int pos, int Length)
        {
            byte[] compressedData = new byte[Length];
            Array.Copy(data, pos + 50, compressedData, 0, Length);
            using (MemoryStream ms = new MemoryStream(compressedData))
            {
                using (ZlibStream inflaterInputStream = new ZlibStream(ms, CompressionMode.Decompress))
                {
                    using (MemoryStream out1 = new MemoryStream())
                    {
                        int c;
                        try
                        {
                            while ((c = inflaterInputStream.ReadByte()) != -1)
                                out1.WriteByte((byte)c);
                            return out1.ToArray();
                        }
                        catch (IOException e)
                        {
                            throw new RecordFormatException(e.ToString());
                        }
                    }
                }
            }
        }

    }
}