using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class localizationPersistenceSubtitleMap : ISerializable
	{
		[Ordinal(0)]  [RED("entries")] public CArray<localizationPersistenceSubtitleMapEntry> Entries { get; set; }

		public localizationPersistenceSubtitleMap(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
