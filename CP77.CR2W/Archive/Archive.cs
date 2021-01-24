﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Catel.IoC;
using WolvenKit.Common.Services;
using CP77.CR2W.Extensions;
using CP77Tools.Model;
using Newtonsoft.Json;
using WolvenKit.Common;
using WolvenKit.Common.Extensions;
using CP77.CR2W.Types;
using WolvenKit.Common.Oodle;

namespace CP77.CR2W.Archive
{
    public class Archive : IGameArchive
    {
        #region constructors

        public Archive()
        {
            Header = new ArHeader();
            Table = new ArTable();
        }

        /// <summary>
        /// Creates and reads an archive from a path
        /// </summary>
        /// <param name="path"></param>
        public Archive(string path)
        {
            ArchiveAbsolutePath = path;

            ReadTables();
        }

        #endregion

        #region properties
        public EArchiveType TypeName => EArchiveType.Archive;

        public ArHeader Header { get; set; }
        public ArTable Table { get; set; }
        public string ArchiveAbsolutePath { get; set; }

        [JsonIgnore]
        public Dictionary<ulong, ArchiveItem> Files => Table?.FileInfo;

        public int FileCount => Files?.Count ?? 0;

        [JsonIgnore]
        public string Name => Path.GetFileName(ArchiveAbsolutePath);
        #endregion

        #region methods

        /// <summary>
        /// Reads the tables info to the archive.
        /// </summary>
        private void ReadTables()
        {
            //using var mmf = MemoryMappedFile.CreateFromFile(Filepath, FileMode.Open, Mmfhash, 0, MemoryMappedFileAccess.Read);

            // using (var vs = mmf.CreateViewStream(0, ArHeader.SIZE, MemoryMappedFileAccess.Read))
            // {
            //     _header = new ArHeader(new BinaryReader(vs));
            // }

            // using (var vs = mmf.CreateViewStream((long)_header.Tableoffset, (long)_header.Tablesize,
            //     MemoryMappedFileAccess.Read))
            // {
            //     _table = new ArTable(new BinaryReader(vs), this);
            // }

            using var vs = new FileStream(ArchiveAbsolutePath, FileMode.Open, FileAccess.Read);
            Header = new ArHeader(new BinaryReader(vs));
            vs.Seek((long) Header.Tableoffset, SeekOrigin.Begin);
            Table = new ArTable(new BinaryReader(vs), this);
            vs.Close();
        }

        /// <summary>
        /// Serializes this archive to a redengine .archive file
        /// </summary>
        public void Serialize()
        {




        }


        public bool CanUncook(ulong hash)
        {
            if (!Files.ContainsKey(hash))
                return false;
            var archiveItem = Files[hash]; 
            string name = archiveItem.FileName;
            var hasBuffers = (archiveItem.LastOffsetTableIdx - archiveItem.FirstOffsetTableIdx) > 1;

            var values = Enum.GetNames(typeof(ECookedFileFormat));
            var b = values.Any(e => e == Path.GetExtension(name)[1..]) || hasBuffers ;
            return b;
        }

        /// <summary>
        /// Gets the bytes of one file by index from the archive.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="decompressBuffers"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public (byte[], List<byte[]>) GetFileData(ulong hash, bool decompressBuffers)
        {
            var logger = ServiceLocator.Default.ResolveType<ILoggerService>();
            if (!Files.ContainsKey(hash)) return (null, null);

            var entry = Files[hash];
            var startindex = (int)entry.FirstOffsetTableIdx;
            var nextindex = (int)entry.LastOffsetTableIdx;
            
            // decompress main file
            var file = ExtractFile(this.Table.Offsets[startindex], true);
            
            // don't decompress buffers
            var buffers = new List<byte[]>();
            for (int j = startindex + 1; j < nextindex; j++)
            {
                var offsetentry = this.Table.Offsets[j];
                var buffer = ExtractFile(offsetentry, decompressBuffers);
                buffers.Add(buffer);
            }

            return (file, buffers);

            // local
            byte[] ExtractFile(OffsetEntry offsetentry, bool decompress)
            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);
                
                using var stream = new FileStream(ArchiveAbsolutePath, FileMode.Open, FileAccess.Read);
                using var binaryReader = new BinaryReader(stream);
                binaryReader.BaseStream.Seek((long) offsetentry.Offset, SeekOrigin.Begin);


                var zSize = offsetentry.ZSize;
                var size = offsetentry.Size;

                if (!decompress)
                {
                    var buffer = binaryReader.ReadBytes((int)zSize);
                    bw.Write(buffer);
                }
                else
                {
                    binaryReader.DecompressBuffer(bw, zSize, size);
                }

                return ms.ToArray();
            }
        }



        #endregion

        
    }


}


