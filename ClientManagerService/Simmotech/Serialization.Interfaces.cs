#define NETFW
// Supervisor: HEB
// Part of ASOL Framework, proprietary software, (c) Asseco Solutions, a. s.
// Redistribution and use in source and binary forms, with or without modification, 
// is not permitted without valid contract with Asseco Solutions, a. s.

//copied from ASOL.Framework.Shared on 2016-04-14

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 3021

namespace ASOL.Framework.Shared.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    #region interface for General Factory - ISerializationFormatter
    public interface ISerializationFormatter
    {
        /// <summary>
        /// Returns reader for creating object from serialized data
        /// </summary>
        /// <returns></returns>
        ISerializationReader CreateSerializationReader(byte[] data);

        /// <summary>
        /// Returns writer for writing serialized stream
        /// </summary>
        /// <returns></returns>
        ISerializationWriter CreateSerializationWriter();
    }
    #endregion

    #region ISerializationWriter and ISerializationReader interfaces
    /// <summary>
    /// For internal purposes. Part of client-server communication. Do not use for application development.
    /// </summary>
    public interface ISerializationWriter : IDisposable
    {
        #region SimmoTech
        // Summary:
        //     Gets or Sets a boolean flag to indicate whether to optimize for size (default)
        //     by storing data as packed bits or sections where possible.  Setting this
        //     value to false will turn off this optimization and store data directly which
        //     increases the speed.  Note: This only affects optimization of data passed
        //     to the WriteObject method and direct calls to the WriteOptimized methods
        //     will always pack data into the smallest space where possible.
        bool OptimizeForSize { get; set; }

        // Summary:
        //     Writes the contents of the string and object token tables into the stream.
        //      Also write the starting offset into the first 4 bytes of the stream.  Notes:
        //     Called automatically by ToArray().  Can be used to ensure that the complete
        //     graph is written before using an alternate technique of extracting a Byte[]
        //     such as using compression on the underlying stream.
        //
        // Returns:
        //     The length of the string and object tables.
        int UpdateHeader(); // VS 0001532 Replaced AppendTokenTables() with UpdateHeader() since there is no longer any appending.
        //
        // Summary:
        //     Returns a byte[] containing all of the serialized data.  The current implementation
        //     has the data in 3 sections: 1) A 4 byte Int32 giving the offset to the 3rd
        //     section.  2) The main serialized data.  3) The serialized string tokenization
        //     lists and object tokenization lists.  Only call this method once all of the
        //     data has been serialized.  This method appends all of the tokenized data
        //     (string and object) to the end of the stream and ensures that the first four
        //     bytes reflect the offset of the tokenized data so that it can be deserialized
        //     first.  This is the reason for requiring a rewindable stream.  Future implementations
        //     may also allow the serialized data to be accessed via 2 byte[] arrays. This
        //     would remove the requirement for a rewindable stream opening the possibility
        //     of streaming the serialized data directly over the network allowing simultaneous
        //     of partially simultaneous deserialization.
        //
        // Returns:
        //     A byte[] containing all serialized data.
        byte[] ToArray();

        #region Write
        //
        // Summary:
        //     Writes an ArrayList into the stream using the fewest number of bytes possible.
        //      Stored Size: 1 byte upwards depending on data content Notes: A null Arraylist
        //     takes 1 byte.  An empty ArrayList takes 2 bytes.  The contents are stored
        //     using WriteOptimized(ArrayList) which should be used if the ArrayList is
        //     guaranteed never to be null.
        //
        // Parameters:
        //   value:
        //     The ArrayList to store.
        void Write(System.Collections.ArrayList value);

        //
        // Summary:
        //     Writes a BitArray value into the stream using the fewest number of bytes
        //     possible.  Stored Size: 1 byte upwards depending on data content Notes: A
        //     null BitArray takes 1 byte.  An empty BitArray takes 2 bytes.
        //
        // Parameters:
        //   value:
        //     The BitArray value to store.
        void Write(System.Collections.BitArray value);
        //zruseno:
        ////
        //// Summary:
        ////     Writes a BitVector32 into the stream.  Stored Size: 4 bytes.
        ////
        //// Parameters:
        ////   value:
        ////     The BitVector32 to store.
        //void Write(System.Collections.Specialized.BitVector32 value);
        //
        // Summary:
        //     Writes a Boolean[] into the stream.  Notes: A null or empty array will take
        //     1 byte.  Calls WriteOptimized(Boolean[]).
        //
        // Parameters:
        //   values:
        //     The Boolean[] to store.
        void Write(bool[] values);
        //
        // Summary:
        //     Writes a Byte[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The Byte[] to store.
        void Write(byte[] values);
        //
        // Summary:
        //     Writes a DateTime value into the stream.  Stored Size: 8 bytes
        //
        // Parameters:
        //   value:
        //     The DateTime value to store.
        void Write(DateTime value);
        //
        // Summary:
        //     Writes a DateTime[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The DateTime[] to store.
        void Write(DateTime[] values);
        //
        // Summary:
        //     Writes a DateTime value into the stream.  Stored Size: 8 bytes
        //
        // Parameters:
        //   value:
        //     The DateTime value to store.
        void Write(DateTimeOffset value);
        //
        // Summary:
        //     Writes a DateTime[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The DateTime[] to store.
        void Write(DateTimeOffset[] values);
        //
        // Summary:
        //     Writes a Decimal[] into the stream.  Notes: A null or empty array will take
        //     1 byte.  Calls WriteOptimized(Decimal[]).
        //
        // Parameters:
        //   values:
        //     The Decimal[] to store.
        void Write(decimal[] values);
        //
        // Summary:
        //     Writes a non-null generic Dictionary into the stream.
        //
        // Parameters:
        //   value:
        //     The generic dictionary.
        //
        // Type parameters:
        //   K:
        //     The key Type.
        //
        //   V:
        //     The value Type.
        //
        // Remarks:
        //     The key and value types themselves are not stored - they must be supplied
        //     at deserialization time.
        //      An array of keys is stored followed by an array of values.
        void Write<K, V>(Dictionary<K, V> value);
        //
        // Summary:
        //     Writes a Double[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The Double[] to store.
        void Write(double[] values);
        //
        // Summary:
        //     Writes a Single[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The Single[] to store.
        void Write(float[] values);
        //
        // Summary:
        //     Writes a Guid into the stream.  Stored Size: 16 bytes.
        //
        // Parameters:
        //   value:
        void Write(Guid value);
        //
        // Summary:
        //     Writes a Guid[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The Guid[] to store.
        void Write(Guid[] values);
        //
        // Summary:
        //     Writes a Char[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The Char[] to store.
        void Write(char[] values);
        //
        // Summary:
        //     Writes an Int32[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The Int32[] to store.
        void Write(int[] values);
        //
        // Summary:
        //     Writes a non-null generic List into the stream.
        //
        // Parameters:
        //   value:
        //     The generic List.
        //
        // Type parameters:
        //   T:
        //     The list Type.
        //
        // Remarks:
        //     The list type itself is not stored - it must be supplied at deserialization
        //     time.
        //      The list contents are stored as an array.
        void Write<T>(List<T> value);
        //
        // Summary:
        //     Writes an Int64[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The Int64[] to store.
        void Write(long[] values);
        //
        // Summary:
        //     Writes an object[] into the stream.  Stored Size: 2 bytes upwards depending
        //     on data content Notes: A null object[] takes 1 byte.  An empty object[] takes
        //     2 bytes.  The contents of the array will be stored optimized.
        //
        // Parameters:
        //   values:
        //     The object[] to store.
        void Write(object[] values);
        //
        // Summary:
        //     Writes an SByte[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The SByte[] to store.
        void Write(sbyte[] values);
        //
        // Summary:
        //     Writes an Int16[]or a null into the stream.  Notes: A null or empty array
        //     will take 1 byte.  Calls WriteOptimized(decimal[]).
        //
        // Parameters:
        //   values:
        //     The Int16[] to store.
        void Write(short[] values);
        //
        // Summary:
        //     Calls WriteOptimized(string).  This override to hide base BinaryWriter.Write(string).
        //
        // Parameters:
        //   value:
        //     The string to store.
        void Write(string value);
        //
        // Summary:
        //     Writes a TimeSpan value into the stream.  Stored Size: 8 bytes
        //
        // Parameters:
        //   value:
        //     The TimeSpan value to store.
        void Write(TimeSpan value);
        //
        // Summary:
        //     Writes a TimeSpan[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The TimeSpan[] to store.
        void Write(TimeSpan[] values);
        //
        // Summary:
        //     Writes a UInt32[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The UInt32[] to store.
        void Write(uint[] values);
        //
        // Summary:
        //     Writes a UInt64[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The UInt64[] to store.
        void Write(ulong[] values);
        //
        // Summary:
        //     Writes a UInt16[] into the stream.  Notes: A null or empty array will take
        //     1 byte.
        //
        // Parameters:
        //   values:
        //     The UInt16[] to store.
        void Write(ushort[] values);
        //
        // Summary:
        //     Stores a Type object into the stream.  Stored Size: Depends on the length
        //     of the Type's name and whether the fullyQualified parameter is set.  A null
        //     Type takes 1 byte.
        //
        // Parameters:
        //   value:
        //     The Type to store.
        //
        //   fullyQualified:
        //     true to store the AssemblyQualifiedName or false to store the FullName. 
        void Write(Type value, bool fullyQualified);
        #endregion

        //
        // Summary:
        //     Writes a byte[] directly into the stream.  The size of the array is not stored
        //     so only use this method when the number of bytes will be known at deserialization
        //     time.  A null value will throw an exception
        //
        // Parameters:
        //   value:
        //     The byte[] to store. Must not be null.
        void WriteBytesDirect(byte[] value);
        //
        // Summary:
        //     Writes a Nullable type into the stream.  Synonym for WriteObject().
        //
        // Parameters:
        //   value:
        //     The Nullable value to store.
        void WriteNullable(ValueType value);
        //
        // Summary:
        //     Stores an object into the stream using the fewest number of bytes possible.
        //      Stored Size: 1 byte upwards depending on type and/or content.  1 byte: null,
        //     DBNull.Value, Boolean 1 to 2 bytes: Int16, UInt16, Byte, SByte, Char, 1 to
        //     4 bytes: Int32, UInt32, Single, BitVector32 1 to 8 bytes: DateTime, TimeSpan,
        //     Double, Int64, UInt64 1 or 16 bytes: Guid 1 plus content: string, object[],
        //     byte[], char[], BitArray, Type, ArrayList Any other object be stored using
        //     a .Net Binary formatter but this should only be allowed as a last resort:
        //     Since this is effectively a different serialization session, there is a possibility
        //     of the same shared object being serialized twice or, if the object has a
        //     reference directly or indirectly back to the parent object, there is a risk
        //     of looping which will throw an exception.  The type of object is checked
        //     with the most common types being checked first.  Each 'section' can be reordered
        //     to provide optimum speed but the check for null should always be first and
        //     the default serialization always last.  Once the type is identified, a SerializedType
        //     byte is stored in the stream followed by the data for the object (certain
        //     types/values may not require storage of data as the SerializedType may imply
        //     the value).  For certain objects, if the value is within a certain range
        //     then optimized storage may be used. If the value doesn't meet the required
        //     optimization criteria then the value is stored directly.  The checks for
        //     optimization may be disabled by setting the OptimizeForSize property to false
        //     in which case the value is stored directly. This could result in a slightly
        //     larger stream but there will be a speed increate to compensate.
        //
        // Parameters:
        //   value:
        //     The object to store.
        void WriteObject(object value);
        #region WriteOptimized
        //
        // Summary:
        //     Writes an non-null ArrayList into the stream using the fewest number of bytes
        //     possible.  Stored Size: 1 byte upwards depending on data content Notes: An
        //     empty ArrayList takes 1 byte.
        //
        // Parameters:
        //   value:
        //     The ArrayList to store. Must not be null.
        void WriteOptimized(System.Collections.ArrayList value);
        //
        // Summary:
        //     Writes a BitArray into the stream using the fewest number of bytes possible.
        //      Stored Size: 1 byte upwards depending on data content Notes: An empty BitArray
        //     takes 1 byte.
        //
        // Parameters:
        //   value:
        //     The BitArray value to store. Must not be null.
        void WriteOptimized(System.Collections.BitArray value);
        //zruseno:
        ////
        //// Summary:
        ////     Writes a BitVector32 into the stream using the fewest number of bytes possible.
        ////      Stored Size: 1 to 4 bytes. (.Net is 4 bytes) 1 to 7 bits takes 1 byte 8
        ////     to 14 bits takes 2 bytes 15 to 21 bits takes 3 bytes 22 to 28 bits takes
        ////     4 bytes -------------------------------------------------------------------
        ////     29 to 32 bits takes 5 bytes - use Write(BitVector32) method instead Try to
        ////     order the BitVector32 masks so that the highest bits are least-likely to
        ////     be set.
        ////
        //// Parameters:
        ////   value:
        ////     The BitVector32 to store. Must not use more than 28 bits.
        //void WriteOptimized(System.Collections.Specialized.BitVector32 value);
        //
        // Summary:
        //     Writes an optimized Boolean[] into the stream using the fewest possible bytes.
        //      Notes: A null or empty array will take 1 byte.  Stored as a BitArray.  
        //
        // Parameters:
        //   values:
        //     The Boolean[] to store.
        void WriteOptimized(bool[] values);
        //
        // Summary:
        //     Writes a DateTime value into the stream using the fewest number of bytes
        //     possible.  Stored Size: 3 bytes to 7 bytes (.Net is 8 bytes) Notes: A DateTime
        //     containing only a date takes 3 bytes (except a .NET 2.0 Date with a specified
        //     DateTimeKind which will take a minimum of 5 bytes - no further optimization
        //     for this situation felt necessary since it is unlikely that a DateTimeKind
        //     would be specified without hh:mm also) Date plus hh:mm takes 5 bytes.  Date
        //     plus hh:mm:ss takes 6 bytes.  Date plus hh:mm:ss.fff takes 7 bytes.
        //
        // Parameters:
        //   value:
        //     The DateTime value to store. Must not contain sub-millisecond data.
        void WriteOptimized(DateTime value);
        //
        // Summary:
        //     Writes a DateTime[] into the stream using the fewest possible bytes.  Notes:
        //     A null or empty array will take 1 byte.
        //
        // Parameters:
        //   values:
        //     The DateTime[] to store.
        void WriteOptimized(DateTime[] values);
        //
        // Summary:
        //     Writes a Decimal value into the stream using the fewest number of bytes possible.
        //      Stored Size: 1 byte to 14 bytes (.Net is 16 bytes) Restrictions: None
        //
        // Parameters:
        //   value:
        //     The Decimal value to store
        void WriteOptimized(decimal value);
        //
        // Summary:
        //     Writes a Decimal[] into the stream using the fewest possible bytes.  Notes:
        //     A null or empty array will take 1 byte.
        //
        // Parameters:
        //   values:
        //     The Decimal[] to store.
        void WriteOptimized(decimal[] values);
        //
        // Summary:
        //     Write an Int32 value using the fewest number of bytes possible.
        //      Only call this method if the value is known to be between 0 and 268,435,455
        //     otherwise use Write(Int32 value)
        //
        // Parameters:
        //   value:
        //     The Int32 to store. Must be between 0 and 268,435,455 inclusive.
        //
        // Remarks:
        //     0x00000000 - 0x0000007f (0 to 127) takes 1 byte 0x00000080 - 0x000003FF (128
        //     to 16,383) takes 2 bytes 0x00000400 - 0x001FFFFF (16,384 to 2,097,151) takes
        //     3 bytes 0x00200000 - 0x0FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes
        //     ---------------------------------------------------------------- 0x10000000
        //     - 0x07FFFFFF (268,435,456 and above) takes 5 bytes All negative numbers take
        //     5 bytes Only call this method if the value is known to be between 0 and 268,435,455
        //     otherwise use Write(Int32 value)
        void WriteOptimized(int value);
        //
        // Summary:
        //     Writes an Int32[] into the stream using the fewest possible bytes.  Notes:
        //     A null or empty array will take 1 byte.
        //
        // Parameters:
        //   values:
        //     The Int32[] to store.
        void WriteOptimized(int[] values);
        //
        // Summary:
        //     Write an Int64 value using the fewest number of bytes possible.  0x0000000000000000
        //     - 0x000000000000007f (0 to 127) takes 1 byte 0x0000000000000080 - 0x00000000000003FF
        //     (128 to 16,383) takes 2 bytes 0x0000000000000400 - 0x00000000001FFFFF (16,384
        //     to 2,097,151) takes 3 bytes 0x0000000000200000 - 0x000000000FFFFFFF (2,097,152
        //     to 268,435,455) takes 4 bytes 0x0000000010000000 - 0x00000007FFFFFFFF (268,435,456
        //     to 34,359,738,367) takes 5 bytes 0x0000000800000000 - 0x000003FFFFFFFFFF
        //     (34,359,738,368 to 4,398,046,511,103) takes 6 bytes 0x0000040000000000 -
        //     0x0001FFFFFFFFFFFF (4,398,046,511,104 to 562,949,953,421,311) takes 7 bytes
        //     0x0002000000000000 - 0x00FFFFFFFFFFFFFF (562,949,953,421,312 to 72,057,594,037,927,935)
        //     takes 8 bytes ------------------------------------------------------------------
        //     0x0100000000000000 - 0x7FFFFFFFFFFFFFFF (72,057,594,037,927,936 to 9,223,372,036,854,775,807)
        //     takes 9 bytes 0x7FFFFFFFFFFFFFFF - 0xFFFFFFFFFFFFFFFF (9,223,372,036,854,775,807
        //     and above) takes 10 bytes All negative numbers take 10 bytes Only call this
        //     method if the value is known to be between 0 and 72,057,594,037,927,935 otherwise
        //     use Write(Int64 value)
        //
        // Parameters:
        //   value:
        //     The Int64 to store. Must be between 0 and 72,057,594,037,927,935 inclusive.
        void WriteOptimized(long value);
        //
        // Summary:
        //     Writes an Int64[] into the stream using the fewest possible bytes.  Notes:
        //     A null or empty array will take 1 byte.
        //
        // Parameters:
        //   values:
        //     The Int64[] to store.
        void WriteOptimized(long[] values);
        //
        // Summary:
        //     Writes a not-null object[] into the stream using the fewest number of bytes
        //     possible.  Stored Size: 2 bytes upwards depending on data content Notes:
        //     An empty object[] takes 1 byte.  The contents of the array will be stored
        //     optimized.
        //
        // Parameters:
        //   values:
        //     The object[] to store. Must not be null.
        void WriteOptimized(object[] values);
        //
        // Summary:
        //     Writes a string value into the stream using the fewest number of bytes possible.
        //      Stored Size: 1 byte upwards depending on string length Notes: Encodes null,
        //     Empty, 'Y', 'N', ' ' values as a single byte Any other single char string
        //     is stored as two bytes All other strings are stored in a string token list:
        //     The TypeCode representing the current string token list is written first
        //     (1 byte), followed by the string token itself (1-4 bytes) When the current
        //     string list has reached 128 values then a new string list is generated and
        //     that is used for generating future string tokens. This continues until the
        //     maximum number (128) of string lists is in use, after which the string lists
        //     are used in a round-robin fashion.  By doing this, more lists are created
        //     with fewer items which allows a smaller token size to be used for more strings.
        //      The first 16,384 strings will use a 1 byte token.  The next 2,097,152 strings
        //     will use a 2 byte token. (This should suffice for most uses!) The next 268,435,456
        //     strings will use a 3 byte token. (My, that is a lot!!) The next 34,359,738,368
        //     strings will use a 4 byte token. (only shown for completeness!!!)
        //
        // Parameters:
        //   value:
        //     The string to store.
        void WriteOptimized(string value);
        //
        // Summary:
        //     Writes a TimeSpan value into the stream using the fewest number of bytes
        //     possible.  Stored Size: 2 bytes to 8 bytes (.Net is 8 bytes) Notes: hh:mm
        //     (time) are always stored together and take 2 bytes.  If seconds are present
        //     then 3 bytes unless (time) is not present in which case 2 bytes since the
        //     seconds are stored in the minutes position.  If milliseconds are present
        //     then 4 bytes.  In addition, if days are present they will add 1 to 4 bytes
        //     to the above.
        //
        // Parameters:
        //   value:
        //     The TimeSpan value to store. Must not contain sub-millisecond data.
        void WriteOptimized(TimeSpan value);
        //
        // Summary:
        //     Writes a TimeSpan[] into the stream using the fewest possible bytes.  Notes:
        //     A null or empty array will take 1 byte.
        //
        // Parameters:
        //   values:
        //     The TimeSpan[] to store.
        void WriteOptimized(TimeSpan[] values);
        //
        // Summary:
        //     Stores a non-null Type object into the stream.  Stored Size: Depends on the
        //     length of the Type's name.  If the type is a System type (mscorlib) then
        //     it is stored without assembly name information, otherwise the Type's AssemblyQualifiedName
        //     is used.
        //
        // Parameters:
        //   value:
        //     The Type to store. Must not be null.
        void WriteOptimized(Type value);
        //
        // Summary:
        //     Write a UInt32 value using the fewest number of bytes possible.  0x00000000
        //     - 0x0000007f (0 to 127) takes 1 byte 0x00000080 - 0x000003FF (128 to 16,383)
        //     takes 2 bytes 0x00000400 - 0x001FFFFF (16,384 to 2,097,151) takes 3 bytes
        //     0x00200000 - 0x0FFFFFFF (2,097,152 to 268,435,455) takes 4 bytes ----------------------------------------------------------------
        //     0x10000000 - 0xFFFFFFFF (268,435,456 and above) takes 5 bytes Only call this
        //     method if the value is known to be between 0 and 268,435,455 otherwise use
        //     Write(UInt32 value)
        //
        // Parameters:
        //   value:
        //     The UInt32 to store. Must be between 0 and 268,435,455 inclusive.
        void WriteOptimized(uint value);
        //
        // Summary:
        //     Writes a UInt32[] into the stream using the fewest possible bytes.  Notes:
        //     A null or empty array will take 1 byte.
        //
        // Parameters:
        //   values:
        //     The UInt32[] to store.
        void WriteOptimized(uint[] values);
        //
        // Summary:
        //     Write a UInt64 value using the fewest number of bytes possible.  0x0000000000000000
        //     - 0x000000000000007f (0 to 127) takes 1 byte 0x0000000000000080 - 0x00000000000003FF
        //     (128 to 16,383) takes 2 bytes 0x0000000000000400 - 0x00000000001FFFFF (16,384
        //     to 2,097,151) takes 3 bytes 0x0000000000200000 - 0x000000000FFFFFFF (2,097,152
        //     to 268,435,455) takes 4 bytes 0x0000000010000000 - 0x00000007FFFFFFFF (268,435,456
        //     to 34,359,738,367) takes 5 bytes 0x0000000800000000 - 0x000003FFFFFFFFFF
        //     (34,359,738,368 to 4,398,046,511,103) takes 6 bytes 0x0000040000000000 -
        //     0x0001FFFFFFFFFFFF (4,398,046,511,104 to 562,949,953,421,311) takes 7 bytes
        //     0x0002000000000000 - 0x00FFFFFFFFFFFFFF (562,949,953,421,312 to 72,057,594,037,927,935)
        //     takes 8 bytes ------------------------------------------------------------------
        //     0x0100000000000000 - 0x7FFFFFFFFFFFFFFF (72,057,594,037,927,936 to 9,223,372,036,854,775,807)
        //     takes 9 bytes 0x7FFFFFFFFFFFFFFF - 0xFFFFFFFFFFFFFFFF (9,223,372,036,854,775,807
        //     and above) takes 10 bytes Only call this method if the value is known to
        //     be between 0 and 72,057,594,037,927,935 otherwise use Write(UInt64 value)
        //
        // Parameters:
        //   value:
        //     The UInt64 to store. Must be between 0 and 72,057,594,037,927,935 inclusive.
        void WriteOptimized(ulong value);
        //
        // Summary:
        //     Writes a UInt64[] into the stream using the fewest possible bytes.  Notes:
        //     A null or empty array will take 1 byte.
        //
        // Parameters:
        //   values:
        //     The UInt64[] to store.
        void WriteOptimized(ulong[] values);
        //
        // Summary:
        //     Writes a pair of object[] arrays into the stream using the fewest number
        //     of bytes possible.  The arrays must not be null and must have the same length
        //     The first array's values are written optimized The second array's values
        //     are compared against the first and, where identical, will be stored using
        //     a single byte.  Useful for storing entity data where there is a before-change
        //     and after-change set of value pairs and, typically, only a few of the values
        //     will have changed.
        //
        // Parameters:
        //   values1:
        //     The first object[] value which must not be null and must have the same length
        //     as values2
        //
        //   values2:
        //     The second object[] value which must not be null and must have the same length
        //     as values1
        void WriteOptimized(object[] values1, object[] values2);
        #endregion
        //
        // Summary:
        //     Writes a non-null string directly to the stream without tokenization.
        //
        // Parameters:
        //   value:
        //     The string to store. Must not be null.
        void WriteStringDirect(string value);
        //
        // Summary:
        //     Writes a token (an Int32 taking 1 to 4 bytes) into the stream that represents
        //     the object instance.  The same token will always be used for the same object
        //     instance.  The object will be serialized once and recreated at deserialization
        //     time.  Calls to SerializationReader.ReadTokenizedObject() will retrieve the
        //     same object instance.
        //
        // Parameters:
        //   value:
        //     The object to tokenize. Must not be null and must not be a string.
        void WriteTokenizedObject(object value);
        //
        // Summary:
        //     Writes a token (an Int32 taking 1 to 4 bytes) into the stream that represents
        //     the object instance.  The same token will always be used for the same object
        //     instance.  When recreateFromType is set to true, the object's Type will be
        //     stored and the object recreated using Activator.GetInstance with a parameterless
        //     contructor. This is useful for stateless, factory-type classes.  When recreateFromType
        //     is set to false, the object will be serialized once and recreated at deserialization
        //     time.  Calls to SerializationReader.ReadTokenizedObject() will retrieve the
        //     same object instance.
        //
        // Parameters:
        //   value:
        //     The object to tokenize. Must not be null and must not be a string.
        //
        //   recreateFromType:
        //     true if the object can be recreated using a parameterless constructor; false
        //     if the object should be serialized as-is
        void WriteTokenizedObject(object value, bool recreateFromType);
        //
        // Summary:
        //     Writes a null or a typed array into the stream.
        //
        // Parameters:
        //   values:
        //     The array to store.
        void WriteTypedArray(Array values);

        //
        // Summary:
        //     Writes a null or a typed array into the stream.
        //
        // Parameters:
        //   values:
        //     The array to store.
        void Write<T>(T[] value);

        #region Additional members // VS 0001532

        /// <summary>
        /// Allows any object implementing ISelfSerializable to serialize itself into this SerializationWriter.
        /// </summary>
        /// <param name="value"></param>
        void WriteObject(ISelfSerializable value); // VS 0001532

        /// <summary>
        /// Writes Enum value into the stream.
        /// </summary>
        /// <param name="value"></param>
        void Write(Enum value); // VS 0001532
       
        /// <summary>
        /// Stores a Type object with the fullyQualified name into the stream. A null Type takes 1 byte.
        /// </summary>
        /// <param name="value">The Type to store.</param>
        void Write(Type value); // VS 0001532

        #endregion
#endregion
#region BinaryWriter
#if NETFW
        // Summary:
        //     Closes the current System.IO.BinaryWriter and the underlying stream.
        void Close();
#endif
        //
        // Summary:
        //     Clears all buffers for the current writer and causes any buffered data to
        //     be written to the underlying device.
        void Flush();
        //
        // Summary:
        //     Writes a one-byte Boolean value to the current stream, with 0 representing
        //     false and 1 representing true.
        //
        // Parameters:
        //   value:
        //     The Boolean value to write (0 or 1).
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(bool value);
        //
        // Summary:
        //     Writes an unsigned byte to the current stream and advances the stream position
        //     by one byte.
        //
        // Parameters:
        //   value:
        //     The unsigned byte to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(byte value);
        //
        // Summary:
        //     Writes a decimal value to the current stream and advances the stream position
        //     by sixteen bytes.
        //
        // Parameters:
        //   value:
        //     The decimal value to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(decimal value);
        //
        // Summary:
        //     Writes an eight-byte floating-point value to the current stream and advances
        //     the stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte floating-point value to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(double value);
        //
        // Summary:
        //     Writes a four-byte floating-point value to the current stream and advances
        //     the stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte floating-point value to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(float value);
        //
        // Summary:
        //     Writes a Unicode character to the current stream and advances the current
        //     position of the stream in accordance with the Encoding used and the specific
        //     characters being written to the stream.
        //
        // Parameters:
        //   ch:
        //     The non-surrogate, Unicode character to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentException:
        //     ch is a single surrogate character.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(char ch);
        //
        // Summary:
        //     Writes a four-byte signed integer to the current stream and advances the
        //     stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte signed integer to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(int value);
        //
        // Summary:
        //     Writes an eight-byte signed integer to the current stream and advances the
        //     stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte signed integer to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(long value);
        //
        // Summary:
        //     Writes a signed byte to the current stream and advances the stream position
        //     by one byte.
        //
        // Parameters:
        //   value:
        //     The signed byte to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        [CLSCompliant(false)]
        void Write(sbyte value);
        //
        // Summary:
        //     Writes a two-byte signed integer to the current stream and advances the stream
        //     position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte signed integer to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        void Write(short value);
        //
        // Summary:
        //     Writes a four-byte unsigned integer to the current stream and advances the
        //     stream position by four bytes.
        //
        // Parameters:
        //   value:
        //     The four-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        [CLSCompliant(false)]
        void Write(uint value);
        //
        // Summary:
        //     Writes an eight-byte unsigned integer to the current stream and advances
        //     the stream position by eight bytes.
        //
        // Parameters:
        //   value:
        //     The eight-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        [CLSCompliant(false)]
        void Write(ulong value);
        //
        // Summary:
        //     Writes a two-byte unsigned integer to the current stream and advances the
        //     stream position by two bytes.
        //
        // Parameters:
        //   value:
        //     The two-byte unsigned integer to write.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        [CLSCompliant(false)]
        void Write(ushort value);
        //
        // Summary:
        //     Writes a region of a byte array to the current stream.
        //
        // Parameters:
        //   count:
        //     The number of bytes to write.
        //
        //   buffer:
        //     A byte array containing the data to write.
        //
        //   index:
        //     The starting point in buffer at which to begin writing.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   System.ArgumentException:
        //     The buffer length minus index is less than count.
        void Write(byte[] buffer, int index, int count);
        //
        // Summary:
        //     Writes a section of a character array to the current stream, and advances
        //     the current position of the stream in accordance with the Encoding used and
        //     perhaps the specific characters being written to the stream.
        //
        // Parameters:
        //   chars:
        //     A character array containing the data to write.
        //
        //   count:
        //     The number of characters to write.
        //
        //   index:
        //     The starting point in buffer from which to begin writing.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   System.ArgumentException:
        //     The buffer length minus index is less than count.
        //
        //   System.ArgumentNullException:
        //     chars is null.
        void Write(char[] chars, int index, int count);

#endregion
    }

    /// <summary>
    /// For internal purposes. Part of client-server communication. Do not use for application development.
    /// </summary>
    public interface ISerializationReader : IDisposable
    {
#region Simmotech
#region Arrays, lists, dictionary, datetime
        //
        // Summary:
        //     Returns an ArrayList or null from the stream.
        //
        // Returns:
        //     An ArrayList instance.
        System.Collections.ArrayList ReadArrayList();
        //
        // Summary:
        //     Returns a BitArray or null from the stream.
        //
        // Returns:
        //     A BitArray instance.
        System.Collections.BitArray ReadBitArray();
        //zruseno:
        ////
        //// Summary:
        ////     Returns a BitVector32 value from the stream.
        ////
        //// Returns:
        ////     A BitVector32 value.
        //System.Collections.Specialized.BitVector32 ReadBitVector32();
        //
        // Summary:
        //     Returns a Boolean[] from the stream.
        //
        // Returns:
        //     A Boolean[] instance; or null.
        bool[] ReadBooleanArray();
        //
        // Summary:
        //     Returns a Byte[] from the stream.
        //
        // Returns:
        //     A Byte instance; or null.
        byte[] ReadByteArray();
        //
        // Summary:
        //     Reads the specified number of bytes directly from the stream.
        //
        // Parameters:
        //   count:
        //     The number of bytes to read
        //
        // Returns:
        //     A byte[] containing the read bytes
        byte[] ReadBytesDirect(int count);
        //
        // Summary:
        //     Returns a DateTime value from the stream.
        //
        // Returns:
        //     A DateTime value.
        DateTime ReadDateTime();
        //
        // Summary:
        //     Returns a DateTimeOffset value from the stream.
        //
        // Returns:
        //     A DateTimeOffset value.
        DateTimeOffset ReadDateTimeOffset(); 
        //
        // Summary:
        //     Returns a DateTime[] from the stream.
        //
        // Returns:
        //     A DateTime[] instance; or null.
        DateTime[] ReadDateTimeArray();
        //
        // Summary:
        //     Returns a DateTimeOffset[] from the stream.
        //
        // Returns:
        //     A DateTimeOffset[] instance; or null.
        DateTimeOffset[] ReadDateTimeOffsetArray();
        //
        // Summary:
        //     Returns a Decimal[] from the stream.
        //
        // Returns:
        //     A Decimal[] instance; or null.
        decimal[] ReadDecimalArray();
        //
        // Summary:
        //     Returns a new, simple generic dictionary populated with keys and values from
        //     the stream.
        //
        // Type parameters:
        //   K:
        //     The key Type.
        //
        //   V:
        //     The value Type.
        //
        // Returns:
        //     A new, simple, populated generic Dictionary.
        Dictionary<K, V> ReadDictionary<K, V>();
        //
        // Summary:
        //     Populates a pre-existing generic dictionary with keys and values from the
        //     stream.  This allows a generic dictionary to be created without using the
        //     default constructor.
        //
        // Type parameters:
        //   K:
        //     The key Type.
        //
        //   V:
        //     The value Type.
        void ReadDictionary<K, V>(Dictionary<K, V> dictionary);
        //
        // Summary:
        //     Returns a Double[] from the stream.
        //
        // Returns:
        //     A Double[] instance; or null.
        double[] ReadDoubleArray();
        //
        // Summary:
        //     Returns a Guid value from the stream.
        //
        // Returns:
        //     A DateTime value.
        Guid ReadGuid();
        //
        // Summary:
        //     Returns a Guid[] from the stream.
        //
        // Returns:
        //     A Guid[] instance; or null.
        Guid[] ReadGuidArray();
        //
        // Summary:
        //     Returns a Char[] from the stream.
        //
        // Returns:
        //     A Char[] value; or null.
        char[] ReadCharArray();
        //
        // Summary:
        //     Returns an Int16[] from the stream.
        //
        // Returns:
        //     An Int16[] instance; or null.
        short[] ReadInt16Array();
        //
        // Summary:
        //     Returns an Int32[] from the stream.
        //
        // Returns:
        //     An Int32[] instance; or null.
        int[] ReadInt32Array();
        //
        // Summary:
        //     Returns an Int64[] from the stream.
        //
        // Returns:
        //     An Int64[] instance; or null.
        long[] ReadInt64Array();
        //
        // Summary:
        //     Returns a generic List populated with values from the stream.
        //
        // Type parameters:
        //   T:
        //     The list Type.
        //
        // Returns:
        //     A new generic List.
        List<T> ReadList<T>();
#endregion

#region nullable types
        //
        // Summary:
        //     Returns a Nullable struct from the stream.  The value returned must be cast
        //     to the correct Nullable type.  Synonym for ReadObject();
        //
        // Returns:
        //     A struct value or null
        ValueType ReadNullable();
        //
        // Summary:
        //     Returns a Nullable Boolean from the stream.
        //
        // Returns:
        //     A Nullable Boolean.
        bool? ReadNullableBoolean();
        //
        // Summary:
        //     Returns a Nullable Byte from the stream.
        //
        // Returns:
        //     A Nullable Byte.
        byte? ReadNullableByte();
        //
        // Summary:
        //     Returns a Nullable DateTime from the stream.
        //
        // Returns:
        //     A Nullable DateTime.
        DateTime? ReadNullableDateTime();
        //
        // Summary:
        //     Returns a Nullable Decimal from the stream.
        //
        // Returns:
        //     A Nullable Decimal.
        decimal? ReadNullableDecimal();
        //
        // Summary:
        //     Returns a Nullable Double from the stream.
        //
        // Returns:
        //     A Nullable Double.
        double? ReadNullableDouble();
        //
        // Summary:
        //     Returns a Nullable Guid from the stream.
        //
        // Returns:
        //     A Nullable Guid.
        Guid? ReadNullableGuid();
        //
        // Summary:
        //     Returns a Nullable Char from the stream.
        //
        // Returns:
        //     A Nullable Char.
        char? ReadNullableChar();
        //
        // Summary:
        //     Returns a Nullable Int16 from the stream.
        //
        // Returns:
        //     A Nullable Int16.
        short? ReadNullableInt16();
        //
        // Summary:
        //     Returns a Nullable Int32 from the stream.
        //
        // Returns:
        //     A Nullable Int32.
        int? ReadNullableInt32();
        //
        // Summary:
        //     Returns a Nullable Int64 from the stream.
        //
        // Returns:
        //     A Nullable Int64.
        long? ReadNullableInt64();
        //
        // Summary:
        //     Returns a Nullable SByte from the stream.
        //
        // Returns:
        //     A Nullable SByte.
        [CLSCompliant(false)]
        sbyte? ReadNullableSByte();
        //
        // Summary:
        //     Returns a Nullable Single from the stream.
        //
        // Returns:
        //     A Nullable Single.
        float? ReadNullableSingle();
        //
        // Summary:
        //     Returns a Nullable TimeSpan from the stream.
        //
        // Returns:
        //     A Nullable TimeSpan.
        TimeSpan? ReadNullableTimeSpan();
        //
        // Summary:
        //     Returns a Nullable UInt16 from the stream.
        //
        // Returns:
        //     A Nullable UInt16.
        [CLSCompliant(false)]
        ushort? ReadNullableUInt16();
        //
        // Summary:
        //     Returns a Nullable UInt32 from the stream.
        //
        // Returns:
        //     A Nullable UInt32.
        [CLSCompliant(false)]
        uint? ReadNullableUInt32();
        //
        // Summary:
        //     Returns a Nullable UInt64 from the stream.
        //
        // Returns:
        //     A Nullable UInt64.
        [CLSCompliant(false)]
        ulong? ReadNullableUInt64();
#endregion
        //
        // Summary:
        //     Returns an object based on the SerializedType read next from the stream.
        //
        // Returns:
        //     An object instance.
        object ReadObject();
        //
        // Summary:
        //     Returns an object[] or null from the stream.
        //
        // Returns:
        //     A DateTime value.
        object[] ReadObjectArray();
        //
        // Summary:
        //     Returns an object[] or null from the stream.  The returned array will be
        //     typed according to the specified element type and the resulting array can
        //     be cast to the expected type.  e.g.  string[] myStrings = (string[]) reader.ReadObjectArray(typeof(string));
        //     An exception will be thrown if any of the deserialized values cannot be cast
        //     to the specified elementType.
        //
        // Parameters:
        //   elementType:
        //     The Type of the expected array elements. null will return a plain object[].
        //
        // Returns:
        //     An object[] instance.
        object[] ReadObjectArray(Type elementType);

#region ReadOptimized ... methods
        //
        // Summary:
        //     Returns an ArrayList from the stream that was stored optimized.
        //
        // Returns:
        //     An ArrayList instance.
        System.Collections.ArrayList ReadOptimizedArrayList();
        //
        // Summary:
        //     Returns a BitArray from the stream that was stored optimized.
        //
        // Returns:
        //     A BitArray instance.
        System.Collections.BitArray ReadOptimizedBitArray();
        //zruseno:
        ////
        //// Summary:
        ////     Returns a BitVector32 value from the stream that was stored optimized.  
        ////
        //// Returns:
        ////     A BitVector32 value.
        //System.Collections.Specialized.BitVector32 ReadOptimizedBitVector32();
        //
        // Summary:
        //     Returns a Boolean[] from the stream.
        //
        // Returns:
        //     A Boolean[] instance; or null.
        bool[] ReadOptimizedBooleanArray();
        //
        // Summary:
        //     Returns a DateTime value from the stream that was stored optimized.
        //
        // Returns:
        //     A DateTime value.
        DateTime ReadOptimizedDateTime();
        //
        // Summary:
        //     Returns a DateTime[] from the stream.
        //
        // Returns:
        //     A DateTime[] instance; or null.
        DateTime[] ReadOptimizedDateTimeArray();
        //
        // Summary:
        //     Returns a Decimal value from the stream that was stored optimized.
        //
        // Returns:
        //     A Decimal value.
        decimal ReadOptimizedDecimal();
        //
        // Summary:
        //     Returns a Decimal[] from the stream.
        //
        // Returns:
        //     A Decimal[] instance; or null.
        decimal[] ReadOptimizedDecimalArray();
        //
        // Summary:
        //     Returns an Int32 value from the stream that was stored optimized.
        //
        // Returns:
        //     An Int32 value.
        int ReadOptimizedInt32();
        //
        // Summary:
        //     Returns a Int32[] from the stream.
        //
        // Returns:
        //     An Int32[] instance; or null.
        int[] ReadOptimizedInt32Array();
        //
        // Summary:
        //     Returns an Int64 value from the stream that was stored optimized.
        //
        // Returns:
        //     An Int64 value.
        long ReadOptimizedInt64();
        //
        // Summary:
        //     Returns a Int64[] from the stream.
        //
        // Returns:
        //     A Int64[] instance; or null.
        long[] ReadOptimizedInt64Array();
        //
        // Summary:
        //     Returns an object[] from the stream that was stored optimized.
        //
        // Returns:
        //     An object[] instance.
        object[] ReadOptimizedObjectArray();
        //
        // Summary:
        //     Returns an object[] from the stream that was stored optimized.  The returned
        //     array will be typed according to the specified element type and the resulting
        //     array can be cast to the expected type.  e.g.  string[] myStrings = (string[])
        //     reader.ReadOptimizedObjectArray(typeof(string)); An exception will be thrown
        //     if any of the deserialized values cannot be cast to the specified elementType.
        //
        // Parameters:
        //   elementType:
        //     The Type of the expected array elements. null will return a plain object[].
        //
        // Returns:
        //     An object[] instance.
        object[] ReadOptimizedObjectArray(Type elementType);
        //
        // Summary:
        //     Returns a pair of object[] arrays from the stream that were stored optimized.
        //
        // Returns:
        //     A pair of object[] arrays.
        void ReadOptimizedObjectArrayPair(out object[] values1, out object[] values2);
        //
        // Summary:
        //     Returns a string value from the stream that was stored optimized.
        //
        // Returns:
        //     A string value.
        string ReadOptimizedString();
        //
        // Summary:
        //     Returns a string[] from the stream that was stored optimized.
        //
        // Returns:
        //     An string[] instance.
        string[] ReadOptimizedStringArray();
        //
        // Summary:
        //     Returns a TimeSpan value from the stream that was stored optimized.
        //
        // Returns:
        //     A TimeSpan value.
        TimeSpan ReadOptimizedTimeSpan();
        //
        // Summary:
        //     Returns a TimeSpan[] from the stream.
        //
        // Returns:
        //     A TimeSpan[] instance; or null.
        TimeSpan[] ReadOptimizedTimeSpanArray();
        //
        // Summary:
        //     Returns a Type from the stream.  Throws an exception if the Type cannot be
        //     found.
        //
        // Returns:
        //     A Type instance.
        Type ReadOptimizedType();
        //
        // Summary:
        //     Returns a Type from the stream.  Throws an exception if the Type cannot be
        //     found and throwOnError is true.
        //
        // Returns:
        //     A Type instance.
        Type ReadOptimizedType(bool throwOnError);
        //
        // Summary:
        //     Returns a UInt32 value from the stream that was stored optimized.
        //
        // Returns:
        //     A UInt32 value.
        [CLSCompliant(false)]
        uint ReadOptimizedUInt32();
        //
        // Summary:
        //     Returns a UInt32[] from the stream.
        //
        // Returns:
        //     A UInt32[] instance; or null.
        [CLSCompliant(false)]
        uint[] ReadOptimizedUInt32Array();
        //
        // Summary:
        //     Returns a UInt64 value from the stream that was stored optimized.
        //
        // Returns:
        //     A UInt64 value.
        [CLSCompliant(false)]
        ulong ReadOptimizedUInt64();
        //
        // Summary:
        //     Returns a UInt64[] from the stream.
        //
        // Returns:
        //     A UInt64[] instance; or null.
        ulong[] ReadOptimizedUInt64Array();
#endregion
        //
        // Summary:
        //     Returns an SByte[] from the stream.
        //
        // Returns:
        //     An SByte[] instance; or null.
        sbyte[] ReadSByteArray();
        //
        // Summary:
        //     Returns a Single[] from the stream.
        //
        // Returns:
        //     A Single[] instance; or null.
        float[] ReadSingleArray();
        //
        // Summary:
        //     Called ReadOptimizedString().  This override to hide base BinaryReader.ReadString().
        //
        // Returns:
        //     A string value.
        string ReadString();
        //
        // Summary:
        //     Returns a string[] or null from the stream.
        //
        // Returns:
        //     An string[] instance.
        string[] ReadStringArray();
        //
        // Summary:
        //     Returns a string value from the stream.
        //
        // Returns:
        //     A string value.
        string ReadStringDirect();
        //
        // Summary:
        //     Returns a TimeSpan value from the stream.
        //
        // Returns:
        //     A TimeSpan value.
        TimeSpan ReadTimeSpan();
        //
        // Summary:
        //     Returns a TimeSpan[] from the stream.
        //
        // Returns:
        //     A TimeSpan[] instance; or null.
        TimeSpan[] ReadTimeSpanArray();
        //
        // Summary:
        //     Returns the object associated with the object token read next from the stream.
        //
        // Returns:
        //     An object.
        object ReadTokenizedObject();
        //
        // Summary:
        //     Returns a Type or null from the stream.  Throws an exception if the Type
        //     cannot be found.
        //
        // Returns:
        //     A Type instance.
        Type ReadType();
        //
        // Summary:
        //     Returns a Type or null from the stream.  Throws an exception if the Type
        //     cannot be found and throwOnError is true.
        //
        // Returns:
        //     A Type instance.
        Type ReadType(bool throwOnError);
        //
        // Summary:
        //     Returns a typed array from the stream.
        //
        // Returns:
        //     A typed array.
        Array ReadTypedArray();
        //
        // Summary:
        //     Returns a typed array from the stream.
        //
        // Returns:
        //     A typed array.
        T[] ReadArray<T>();
        //
        // Summary:
        //     Returns a UInt16[] from the stream.
        //
        // Returns:
        //     A UInt16[] instance; or null.
        [CLSCompliant(false)]
        ushort[] ReadUInt16Array();
        //
        // Summary:
        //     Returns a UInt[] from the stream.
        //
        // Returns:
        //     A UInt[] instance; or null.
        [CLSCompliant(false)]
        uint[] ReadUInt32Array();
        //
        // Summary:
        //     Returns a UInt64[] from the stream.
        //
        // Returns:
        //     A UInt64[] instance; or null.
        [CLSCompliant(false)]
        ulong[] ReadUInt64Array();
#region Additional members // VS 0001532

        /// <summary>
        /// Returns a Enum value from the stream.
        /// </summary>
        /// <returns>An Enum value.</returns>
        TEnum ReadEnum<TEnum>() where TEnum : struct;

#endregion

#endregion
#region BinaryReader
#if NETFW
        // Summary:
        //     Closes the current reader and the underlying stream.
        void Close();
#endif
        //
        // Summary:
        //     Returns the next available character and does not advance the byte or character
        //     position.
        //
        // Returns:
        //     The next available character, or -1 if no more characters are available or
        //     the stream does not support seeking.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     An I/O error occurs.
        int PeekChar();
        //
        // Summary:
        //     Reads characters from the underlying stream and advances the current position
        //     of the stream in accordance with the Encoding used and the specific character
        //     being read from the stream.
        //
        // Returns:
        //     The next character from the input stream, or -1 if no characters are currently
        //     available.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        int Read();
        //
        // Summary:
        //     Reads count bytes from the stream with index as the starting point in the
        //     byte array.
        //
        // Parameters:
        //   count:
        //     The number of characters to read.
        //
        //   buffer:
        //     The buffer to read data into.
        //
        //   index:
        //     The starting point in the buffer at which to begin reading into the buffer.
        //
        // Returns:
        //     The number of characters read into buffer. This might be less than the number
        //     of bytes requested if that many bytes are not available, or it might be zero
        //     if the end of the stream is reached.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   System.ArgumentException:
        //     The buffer length minus index is less than count.
        int Read(byte[] buffer, int index, int count);
        //
        // Summary:
        //     Reads count characters from the stream with index as the starting point in
        //     the character array.
        //
        // Parameters:
        //   count:
        //     The number of characters to read.
        //
        //   buffer:
        //     The buffer to read data into.
        //
        //   index:
        //     The starting point in the buffer at which to begin reading into the buffer.
        //
        // Returns:
        //     The total number of characters read into the buffer. This might be less than
        //     the number of characters requested if that many characters are not currently
        //     available, or it might be zero if the end of the stream is reached.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     buffer is null.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.ArgumentOutOfRangeException:
        //     index or count is negative.
        //
        //   System.ArgumentException:
        //     The buffer length minus index is less than count.
        int Read(char[] buffer, int index, int count);
        //
        // Summary:
        //     Reads a Boolean value from the current stream and advances the current position
        //     of the stream by one byte.
        //
        // Returns:
        //     true if the byte is nonzero; otherwise, false.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        bool ReadBoolean();
        //
        // Summary:
        //     Reads the next byte from the current stream and advances the current position
        //     of the stream by one byte.
        //
        // Returns:
        //     The next byte read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        byte ReadByte();
        //
        // Summary:
        //     Reads count bytes from the current stream into a byte array and advances
        //     the current position by count bytes.
        //
        // Parameters:
        //   count:
        //     The number of bytes to read.
        //
        // Returns:
        //     A byte array containing data read from the underlying stream. This might
        //     be less than the number of bytes requested if the end of the stream is reached.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.ArgumentOutOfRangeException:
        //     count is negative.
        byte[] ReadBytes(int count);
        //
        // Summary:
        //     Reads a decimal value from the current stream and advances the current position
        //     of the stream by sixteen bytes.
        //
        // Returns:
        //     A decimal value read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        decimal ReadDecimal();
        //
        // Summary:
        //     Reads an 8-byte floating point value from the current stream and advances
        //     the current position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte floating point value read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        double ReadDouble();
        //
        // Summary:
        //     Reads the next character from the current stream and advances the current
        //     position of the stream in accordance with the Encoding used and the specific
        //     character being read from the stream.
        //
        // Returns:
        //     A character read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ArgumentException:
        //     A surrogate character was read.
        char ReadChar();
        //
        // Summary:
        //     Reads count characters from the current stream, returns the data in a character
        //     array, and advances the current position in accordance with the Encoding
        //     used and the specific character being read from the stream.
        //
        // Parameters:
        //   count:
        //     The number of characters to read.
        //
        // Returns:
        //     A character array containing data read from the underlying stream. This might
        //     be less than the number of characters requested if the end of the stream
        //     is reached.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        //
        //   System.ArgumentOutOfRangeException:
        //     count is negative.
        char[] ReadChars(int count);
        //
        // Summary:
        //     Reads a 2-byte signed integer from the current stream and advances the current
        //     position of the stream by two bytes.
        //
        // Returns:
        //     A 2-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        short ReadInt16();
        //
        // Summary:
        //     Reads a 4-byte signed integer from the current stream and advances the current
        //     position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        int ReadInt32();
        //
        // Summary:
        //     Reads an 8-byte signed integer from the current stream and advances the current
        //     position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte signed integer read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        long ReadInt64();
        //
        // Summary:
        //     Reads a signed byte from this stream and advances the current position of
        //     the stream by one byte.
        //
        // Returns:
        //     A signed byte read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        [CLSCompliant(false)]
        sbyte ReadSByte();
        //
        // Summary:
        //     Reads a 4-byte floating point value from the current stream and advances
        //     the current position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte floating point value read from the current stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        float ReadSingle();
        //
        // Summary:
        //     Reads a 2-byte unsigned integer from the current stream using little endian
        //     encoding and advances the position of the stream by two bytes.
        //
        // Returns:
        //     A 2-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        [CLSCompliant(false)]
        ushort ReadUInt16();
        //
        // Summary:
        //     Reads a 4-byte unsigned integer from the current stream and advances the
        //     position of the stream by four bytes.
        //
        // Returns:
        //     A 4-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        [CLSCompliant(false)]
        uint ReadUInt32();
        //
        // Summary:
        //     Reads an 8-byte unsigned integer from the current stream and advances the
        //     position of the stream by eight bytes.
        //
        // Returns:
        //     An 8-byte unsigned integer read from this stream.
        //
        // Exceptions:
        //   System.ObjectDisposedException:
        //     The stream is closed.
        //
        //   System.IO.IOException:
        //     An I/O error occurs.
        //
        //   System.IO.EndOfStreamException:
        //     The end of the stream is reached.
        [CLSCompliant(false)]
        ulong ReadUInt64();
#endregion
    }
#endregion

    /// <summary>
    /// Interface which allows a class to save/retrieve its data to/from an existing <see cref="ISerializationWriter"/>/<see cref="ISerializationReader"/>.
    /// </summary>
    public interface ISelfSerializable // VS 0001532
    {
            /// <summary>
            /// Lets the implementing class store its data directly into a <see cref="ISerializationWriter"/>.
            /// </summary>
            /// <param name="writer">The SerializationWriter to use</param>
            /// <param name="context">Optional context to use as a hint as to what to store (BitVector32 is useful)</param>
            void Serialize(ISerializationWriter writer, object context);

            /// <summary>
            /// Lets the implementing class retrieve its data directly from a <see cref="ISerializationReader"/>.
            /// </summary>
            /// <param name="reader">The SerializationReader to use</param>
            /// <param name="context">Optional context to use as a hint as to what to retrieve (BitVector32 is useful) </param>
            void Deserialize(ISerializationReader reader, object context);
    }

}

#pragma warning restore 3021
