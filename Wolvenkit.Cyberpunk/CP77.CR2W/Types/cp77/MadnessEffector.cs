using System.IO;
using CP77.CR2W.Reflection;
using FastMember;
using static CP77.CR2W.Types.Enums;

namespace CP77.CR2W.Types
{
	[REDMeta]
	public class MadnessEffector : gameEffector
	{
		[Ordinal(0)]  [RED("owner")] public wCHandle<ScriptedPuppet> Owner { get; set; }
		[Ordinal(1)]  [RED("squadMembers")] public CArray<entEntityID> SquadMembers { get; set; }

		public MadnessEffector(CR2WFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
