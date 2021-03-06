using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class Point : CVariable
	{
		[Ordinal(0)]  [RED("x")] public CInt32 X { get; set; }
		[Ordinal(1)]  [RED("y")] public CInt32 Y { get; set; }

		public Point(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
