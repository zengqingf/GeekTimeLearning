// automatically generated by the FlatBuffers compiler, do not modify

package TMSDKClient.FlatBuffers;

import java.nio.*;
import java.lang.*;
import java.util.*;
import com.google.flatbuffers.*;

@SuppressWarnings("unused")
public final class Dict extends Table {
  public static void ValidateVersion() { Constants.FLATBUFFERS_1_12_0(); }
  public static Dict getRootAsDict(ByteBuffer _bb) { return getRootAsDict(_bb, new Dict()); }
  public static Dict getRootAsDict(ByteBuffer _bb, Dict obj) { _bb.order(ByteOrder.LITTLE_ENDIAN); return (obj.__assign(_bb.getInt(_bb.position()) + _bb.position(), _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __reset(_i, _bb); }
  public Dict __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public TMSDKClient.FlatBuffers.DictEntry entries(int j) { return entries(new TMSDKClient.FlatBuffers.DictEntry(), j); }
  public TMSDKClient.FlatBuffers.DictEntry entries(TMSDKClient.FlatBuffers.DictEntry obj, int j) { int o = __offset(4); return o != 0 ? obj.__assign(__indirect(__vector(o) + j * 4), bb) : null; }
  public int entriesLength() { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; }
  public TMSDKClient.FlatBuffers.DictEntry entriesByKey(String key) { int o = __offset(4); return o != 0 ? TMSDKClient.FlatBuffers.DictEntry.__lookup_by_key(null, __vector(o), key, bb) : null; }
  public TMSDKClient.FlatBuffers.DictEntry entriesByKey(TMSDKClient.FlatBuffers.DictEntry obj, String key) { int o = __offset(4); return o != 0 ? TMSDKClient.FlatBuffers.DictEntry.__lookup_by_key(obj, __vector(o), key, bb) : null; }
  public TMSDKClient.FlatBuffers.DictEntry.Vector entriesVector() { return entriesVector(new TMSDKClient.FlatBuffers.DictEntry.Vector()); }
  public TMSDKClient.FlatBuffers.DictEntry.Vector entriesVector(TMSDKClient.FlatBuffers.DictEntry.Vector obj) { int o = __offset(4); return o != 0 ? obj.__assign(__vector(o), 4, bb) : null; }

  public static int createDict(FlatBufferBuilder builder,
      int entriesOffset) {
    builder.startTable(1);
    Dict.addEntries(builder, entriesOffset);
    return Dict.endDict(builder);
  }

  public static void startDict(FlatBufferBuilder builder) { builder.startTable(1); }
  public static void addEntries(FlatBufferBuilder builder, int entriesOffset) { builder.addOffset(0, entriesOffset, 0); }
  public static int createEntriesVector(FlatBufferBuilder builder, int[] data) { builder.startVector(4, data.length, 4); for (int i = data.length - 1; i >= 0; i--) builder.addOffset(data[i]); return builder.endVector(); }
  public static void startEntriesVector(FlatBufferBuilder builder, int numElems) { builder.startVector(4, numElems, 4); }
  public static int endDict(FlatBufferBuilder builder) {
    int o = builder.endTable();
    return o;
  }

  public static final class Vector extends BaseVector {
    public Vector __assign(int _vector, int _element_size, ByteBuffer _bb) { __reset(_vector, _element_size, _bb); return this; }

    public Dict get(int j) { return get(new Dict(), j); }
    public Dict get(Dict obj, int j) {  return obj.__assign(__indirect(__element(j), bb), bb); }
  }
}

