﻿
namespace KSoft.Phoenix.Phx
{
	public sealed class BProtoTechEffectTarget
		: IO.ITagElementStringNameStreamable
	{
		#region Xml constants
		public static readonly XML.BListXmlParams kBListXmlParams = new XML.BListXmlParams("Target")
		{
			RootName = null,
			Flags = 0
		};
		#endregion

		BProtoTechEffectTargetType mType = BProtoTechEffectTargetType.None;
		public BProtoTechEffectTargetType Type
		{
			get { return mType; }
			set { mType = value; }
		}

		int mValueID = TypeExtensions.kNone;
		public int ValueID
		{
			get { return mValueID; }
			set { mValueID = value; }
		}

		public DatabaseObjectKind ObjectKind { get {
			switch (mType)
			{
			case BProtoTechEffectTargetType.ProtoUnit:
				return DatabaseObjectKind.Unit;
			case BProtoTechEffectTargetType.ProtoSquad:
				return DatabaseObjectKind.Squad;
			case BProtoTechEffectTargetType.Tech:
				return DatabaseObjectKind.Tech;

			default:
				return DatabaseObjectKind.None;
			}
		} }

		#region ITagElementStreamable<string> Members
		void StreamValueID<TDoc, TCursor>(IO.TagElementStream<TDoc, TCursor, string> s, XML.BXmlSerializerInterface xs)
			where TDoc : class
			where TCursor : class
		{
			DatabaseObjectKind kind = ObjectKind;

			if (kind != DatabaseObjectKind.None)
				xs.StreamDBID(s, XML.XmlUtil.kNoXmlName, ref mValueID, kind, false, XML.XmlUtil.kSourceCursor);
		}
		public void Serialize<TDoc, TCursor>(IO.TagElementStream<TDoc, TCursor, string> s)
			where TDoc : class
			where TCursor : class
		{
			var xs = s.GetSerializerInterface();

			s.StreamAttributeEnum("type", ref mType);
			StreamValueID(s, xs);
		}
		#endregion
	};
}