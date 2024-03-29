// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace TMSDKClient.FlatBuffers
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct DictEntry : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_1_12_0(); }
  public static DictEntry GetRootAsDictEntry(ByteBuffer _bb) { return GetRootAsDictEntry(_bb, new DictEntry()); }
  public static DictEntry GetRootAsDictEntry(ByteBuffer _bb, DictEntry obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public DictEntry __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Key { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetKeyBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetKeyBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetKeyArray() { return __p.__vector_as_array<byte>(4); }
  public string Value { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetValueBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
  public ArraySegment<byte>? GetValueBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public byte[] GetValueArray() { return __p.__vector_as_array<byte>(6); }

  public static Offset<TMSDKClient.FlatBuffers.DictEntry> CreateDictEntry(FlatBufferBuilder builder,
      StringOffset keyOffset = default(StringOffset),
      StringOffset valueOffset = default(StringOffset)) {
    builder.StartTable(2);
    DictEntry.AddValue(builder, valueOffset);
    DictEntry.AddKey(builder, keyOffset);
    return DictEntry.EndDictEntry(builder);
  }

  public static void StartDictEntry(FlatBufferBuilder builder) { builder.StartTable(2); }
  public static void AddKey(FlatBufferBuilder builder, StringOffset keyOffset) { builder.AddOffset(0, keyOffset.Value, 0); }
  public static void AddValue(FlatBufferBuilder builder, StringOffset valueOffset) { builder.AddOffset(1, valueOffset.Value, 0); }
  public static Offset<TMSDKClient.FlatBuffers.DictEntry> EndDictEntry(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    builder.Required(o, 4);  // key
    return new Offset<TMSDKClient.FlatBuffers.DictEntry>(o);
  }

  public static VectorOffset CreateSortedVectorOfDictEntry(FlatBufferBuilder builder, Offset<DictEntry>[] offsets) {
    Array.Sort(offsets, (Offset<DictEntry> o1, Offset<DictEntry> o2) => Table.CompareStrings(Table.__offset(4, o1.Value, builder.DataBuffer), Table.__offset(4, o2.Value, builder.DataBuffer), builder.DataBuffer));
    return builder.CreateVectorOfTables(offsets);
  }

  public static DictEntry? __lookup_by_key(int vectorLocation, string key, ByteBuffer bb) {
    byte[] byteKey = System.Text.Encoding.UTF8.GetBytes(key);
    int span = bb.GetInt(vectorLocation - 4);
    int start = 0;
    while (span != 0) {
      int middle = span / 2;
      int tableOffset = Table.__indirect(vectorLocation + 4 * (start + middle), bb);
      int comp = Table.CompareStrings(Table.__offset(4, bb.Length - tableOffset, bb), byteKey, bb);
      if (comp > 0) {
        span = middle;
      } else if (comp < 0) {
        middle++;
        start += middle;
        span -= middle;
      } else {
        return new DictEntry().__assign(tableOffset, bb);
      }
    }
    return null;
  }
};


}
