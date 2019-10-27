// Supervisor: HEB
// Part of Helios Green, proprietary software, (c) LCS International, a. s.
// Redistribution and use in source and binary forms, with or without modification, 
// is not permitted without valid contract with LCS International, a. s.

// HEB created in cooperation with SimmoTech - codeproject.com (http://www.codeproject.com/Articles/15700/Optimizing-Serialization-in-NET).

//copied from ASOL.Framework.Shared on 2016-04-14

using System;

namespace SimmoTech.Utils.Serialization
{
	/// <summary>
	/// Interface to allow helper classes to be used to serialize objects
	/// that are not directly supported by SerializationWriter/SerializationReader
	/// </summary>
	public interface IFastSerializationTypeSurrogate
	{
		/// <summary>
		/// Allows a surrogate to be queried as to whether a particular type is supported
		/// </summary>
		/// <param name="type">The type being queried</param>
		/// <returns>true if the type is supported; otherwise false</returns>
		bool SupportsType(Type type);

		/// <summary>
		/// FastSerializes the object into the SerializationWriter.
		/// </summary>
		/// <param name="writer">The SerializationWriter into which the object is to be serialized.</param>
		/// <param name="value">The object to serialize.</param>
		void Serialize(ASOL.Framework.Shared.Serialization.ISerializationWriter writer, object value);

		/// <summary>
		/// Deserializes an object of the supplied type from the SerializationReader.
		/// </summary>
		/// <param name="reader">The SerializationReader containing the serialized object.</param>
		/// <param name="type">The type of object required to be deserialized.</param>
		/// <returns></returns>
        object Deserialize(ASOL.Framework.Shared.Serialization.ISerializationReader reader, Type type);
	}
}