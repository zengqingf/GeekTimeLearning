// automatically generated, do not modify

namespace FBSkillData
{

using FlatBuffers;

public enum WeaponClassesOrWhatever : byte
{
 NONE = 0,
 boolValue = 1,
 floatValue = 2,
 intValue = 3,
 QuaternionValue = 4,
 uintValue = 5,
 Vector3Value = 6,
};

public sealed class Vector2 : Struct {
  public Vector2 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }

  public static Offset<Vector2> CreateVector2(FlatBufferBuilder builder, float X, float Y) {
    builder.Prep(4, 8);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vector2>(builder.Offset);
  }
};

public sealed class Vector3 : Struct {
  public Vector3 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }

  public static Offset<Vector3> CreateVector3(FlatBufferBuilder builder, float X, float Y, float Z) {
    builder.Prep(4, 12);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vector3>(builder.Offset);
  }
};

public sealed class Quaternion : Struct {
  public Quaternion __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }
  public float W { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<Quaternion> CreateQuaternion(FlatBufferBuilder builder, float X, float Y, float Z, float W) {
    builder.Prep(4, 16);
    builder.PutFloat(W);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Quaternion>(builder.Offset);
  }
};

public sealed class ShapeBox : Table {
  public static ShapeBox GetRootAsShapeBox(ByteBuffer _bb) { return GetRootAsShapeBox(_bb, new ShapeBox()); }
  public static ShapeBox GetRootAsShapeBox(ByteBuffer _bb, ShapeBox obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ShapeBox __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Vector2 Size { get { return GetSize(new Vector2()); } }
  public Vector2 GetSize(Vector2 obj) { int o = __offset(4); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector2 Center { get { return GetCenter(new Vector2()); } }
  public Vector2 GetCenter(Vector2 obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartShapeBox(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddSize(FlatBufferBuilder builder, Offset<Vector2> sizeOffset) { builder.AddStruct(0, sizeOffset.Value, 0); }
  public static void AddCenter(FlatBufferBuilder builder, Offset<Vector2> centerOffset) { builder.AddStruct(1, centerOffset.Value, 0); }
  public static Offset<ShapeBox> EndShapeBox(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ShapeBox>(o);
  }
};

public sealed class HurtDecisionBox : Table {
  public static HurtDecisionBox GetRootAsHurtDecisionBox(ByteBuffer _bb) { return GetRootAsHurtDecisionBox(_bb, new HurtDecisionBox()); }
  public static HurtDecisionBox GetRootAsHurtDecisionBox(ByteBuffer _bb, HurtDecisionBox obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HurtDecisionBox __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ShapeBox GetBoxs(int j) { return GetBoxs(new ShapeBox(), j); }
  public ShapeBox GetBoxs(ShapeBox obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BoxsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public bool HasHit { get { int o = __offset(6); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool BlockToggle { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float ZDim { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int Damage { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HurtID { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<HurtDecisionBox> CreateHurtDecisionBox(FlatBufferBuilder builder,
      VectorOffset boxs = default(VectorOffset),
      bool hasHit = false,
      bool blockToggle = false,
      float zDim = 0,
      int damage = 0,
      int hurtID = 0) {
    builder.StartObject(6);
    HurtDecisionBox.AddHurtID(builder, hurtID);
    HurtDecisionBox.AddDamage(builder, damage);
    HurtDecisionBox.AddZDim(builder, zDim);
    HurtDecisionBox.AddBoxs(builder, boxs);
    HurtDecisionBox.AddBlockToggle(builder, blockToggle);
    HurtDecisionBox.AddHasHit(builder, hasHit);
    return HurtDecisionBox.EndHurtDecisionBox(builder);
  }

  public static void StartHurtDecisionBox(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddBoxs(FlatBufferBuilder builder, VectorOffset boxsOffset) { builder.AddOffset(0, boxsOffset.Value, 0); }
  public static VectorOffset CreateBoxsVector(FlatBufferBuilder builder, Offset<ShapeBox>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBoxsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHasHit(FlatBufferBuilder builder, bool hasHit) { builder.AddBool(1, hasHit, false); }
  public static void AddBlockToggle(FlatBufferBuilder builder, bool blockToggle) { builder.AddBool(2, blockToggle, false); }
  public static void AddZDim(FlatBufferBuilder builder, float zDim) { builder.AddFloat(3, zDim, 0); }
  public static void AddDamage(FlatBufferBuilder builder, int damage) { builder.AddInt(4, damage, 0); }
  public static void AddHurtID(FlatBufferBuilder builder, int hurtID) { builder.AddInt(5, hurtID, 0); }
  public static Offset<HurtDecisionBox> EndHurtDecisionBox(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HurtDecisionBox>(o);
  }
};

public sealed class DefenceDecisionBox : Table {
  public static DefenceDecisionBox GetRootAsDefenceDecisionBox(ByteBuffer _bb) { return GetRootAsDefenceDecisionBox(_bb, new DefenceDecisionBox()); }
  public static DefenceDecisionBox GetRootAsDefenceDecisionBox(ByteBuffer _bb, DefenceDecisionBox obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DefenceDecisionBox __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public ShapeBox GetBoxs(int j) { return GetBoxs(new ShapeBox(), j); }
  public ShapeBox GetBoxs(ShapeBox obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int BoxsLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }
  public bool HasHit { get { int o = __offset(6); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool BlockToggle { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int Type { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<DefenceDecisionBox> CreateDefenceDecisionBox(FlatBufferBuilder builder,
      VectorOffset boxs = default(VectorOffset),
      bool hasHit = false,
      bool blockToggle = false,
      int type = 0) {
    builder.StartObject(4);
    DefenceDecisionBox.AddType(builder, type);
    DefenceDecisionBox.AddBoxs(builder, boxs);
    DefenceDecisionBox.AddBlockToggle(builder, blockToggle);
    DefenceDecisionBox.AddHasHit(builder, hasHit);
    return DefenceDecisionBox.EndDefenceDecisionBox(builder);
  }

  public static void StartDefenceDecisionBox(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddBoxs(FlatBufferBuilder builder, VectorOffset boxsOffset) { builder.AddOffset(0, boxsOffset.Value, 0); }
  public static VectorOffset CreateBoxsVector(FlatBufferBuilder builder, Offset<ShapeBox>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartBoxsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddHasHit(FlatBufferBuilder builder, bool hasHit) { builder.AddBool(1, hasHit, false); }
  public static void AddBlockToggle(FlatBufferBuilder builder, bool blockToggle) { builder.AddBool(2, blockToggle, false); }
  public static void AddType(FlatBufferBuilder builder, int type) { builder.AddInt(3, type, 0); }
  public static Offset<DefenceDecisionBox> EndDefenceDecisionBox(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DefenceDecisionBox>(o);
  }
};

public sealed class TransformParam : Struct {
  public TransformParam __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Vector3 LocalPosition { get { return GetLocalPosition(new Vector3()); } }
  public Vector3 GetLocalPosition(Vector3 obj) { return obj.__init(bb_pos + 0, bb); }
  public Quaternion LocalRotation { get { return GetLocalRotation(new Quaternion()); } }
  public Quaternion GetLocalRotation(Quaternion obj) { return obj.__init(bb_pos + 12, bb); }
  public Vector3 LocalScale { get { return GetLocalScale(new Vector3()); } }
  public Vector3 GetLocalScale(Vector3 obj) { return obj.__init(bb_pos + 28, bb); }

  public static Offset<TransformParam> CreateTransformParam(FlatBufferBuilder builder, float localPosition_X, float localPosition_Y, float localPosition_Z, float localRotation_X, float localRotation_Y, float localRotation_Z, float localRotation_W, float localScale_X, float localScale_Y, float localScale_Z) {
    builder.Prep(4, 40);
    builder.Prep(4, 12);
    builder.PutFloat(localScale_Z);
    builder.PutFloat(localScale_Y);
    builder.PutFloat(localScale_X);
    builder.Prep(4, 16);
    builder.PutFloat(localRotation_W);
    builder.PutFloat(localRotation_Z);
    builder.PutFloat(localRotation_Y);
    builder.PutFloat(localRotation_X);
    builder.Prep(4, 12);
    builder.PutFloat(localPosition_Z);
    builder.PutFloat(localPosition_Y);
    builder.PutFloat(localPosition_X);
    return new Offset<TransformParam>(builder.Offset);
  }
};

public sealed class EffectsFrames : Table {
  public static EffectsFrames GetRootAsEffectsFrames(ByteBuffer _bb) { return GetRootAsEffectsFrames(_bb, new EffectsFrames()); }
  public static EffectsFrames GetRootAsEffectsFrames(ByteBuffer _bb, EffectsFrames obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EffectsFrames __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int EffectResID { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int StartFrames { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EndFrames { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string Attachname { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public sbyte Playtype { get { int o = __offset(14); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public sbyte Timetype { get { int o = __offset(16); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public float Time { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string EffectAsset { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public sbyte AttachPoint { get { int o = __offset(22); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public Vector3 LocalPosition { get { return GetLocalPosition(new Vector3()); } }
  public Vector3 GetLocalPosition(Vector3 obj) { int o = __offset(24); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Quaternion LocalRotation { get { return GetLocalRotation(new Quaternion()); } }
  public Quaternion GetLocalRotation(Quaternion obj) { int o = __offset(26); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 LocalScale { get { return GetLocalScale(new Vector3()); } }
  public Vector3 GetLocalScale(Vector3 obj) { int o = __offset(28); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public int Effecttype { get { int o = __offset(30); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool Loop { get { int o = __offset(32); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool LoopLoop { get { int o = __offset(34); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }

  public static void StartEffectsFrames(FlatBufferBuilder builder) { builder.StartObject(16); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddEffectResID(FlatBufferBuilder builder, int effectResID) { builder.AddInt(1, effectResID, 0); }
  public static void AddStartFrames(FlatBufferBuilder builder, int startFrames) { builder.AddInt(2, startFrames, 0); }
  public static void AddEndFrames(FlatBufferBuilder builder, int endFrames) { builder.AddInt(3, endFrames, 0); }
  public static void AddAttachname(FlatBufferBuilder builder, StringOffset attachnameOffset) { builder.AddOffset(4, attachnameOffset.Value, 0); }
  public static void AddPlaytype(FlatBufferBuilder builder, sbyte playtype) { builder.AddSbyte(5, playtype, 0); }
  public static void AddTimetype(FlatBufferBuilder builder, sbyte timetype) { builder.AddSbyte(6, timetype, 0); }
  public static void AddTime(FlatBufferBuilder builder, float time) { builder.AddFloat(7, time, 0); }
  public static void AddEffectAsset(FlatBufferBuilder builder, StringOffset effectAssetOffset) { builder.AddOffset(8, effectAssetOffset.Value, 0); }
  public static void AddAttachPoint(FlatBufferBuilder builder, sbyte attachPoint) { builder.AddSbyte(9, attachPoint, 0); }
  public static void AddLocalPosition(FlatBufferBuilder builder, Offset<Vector3> localPositionOffset) { builder.AddStruct(10, localPositionOffset.Value, 0); }
  public static void AddLocalRotation(FlatBufferBuilder builder, Offset<Quaternion> localRotationOffset) { builder.AddStruct(11, localRotationOffset.Value, 0); }
  public static void AddLocalScale(FlatBufferBuilder builder, Offset<Vector3> localScaleOffset) { builder.AddStruct(12, localScaleOffset.Value, 0); }
  public static void AddEffecttype(FlatBufferBuilder builder, int effecttype) { builder.AddInt(13, effecttype, 0); }
  public static void AddLoop(FlatBufferBuilder builder, bool loop) { builder.AddBool(14, loop, false); }
  public static void AddLoopLoop(FlatBufferBuilder builder, bool loopLoop) { builder.AddBool(15, loopLoop, false); }
  public static Offset<EffectsFrames> EndEffectsFrames(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EffectsFrames>(o);
  }
};

public sealed class ShockInfo : Struct {
  public ShockInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float ShockTime { get { return bb.GetFloat(bb_pos + 0); } }
  public float ShockSpeed { get { return bb.GetFloat(bb_pos + 4); } }
  public float ShockRangeX { get { return bb.GetFloat(bb_pos + 8); } }
  public float ShockRangeY { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<ShockInfo> CreateShockInfo(FlatBufferBuilder builder, float ShockTime, float ShockSpeed, float ShockRangeX, float ShockRangeY) {
    builder.Prep(4, 16);
    builder.PutFloat(ShockRangeY);
    builder.PutFloat(ShockRangeX);
    builder.PutFloat(ShockSpeed);
    builder.PutFloat(ShockTime);
    return new Offset<ShockInfo>(builder.Offset);
  }
};

public sealed class RandomLaunchInfo : Struct {
  public RandomLaunchInfo __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Num { get { return bb.GetInt(bb_pos + 0); } }
  public bool IsNumRand { get { return 0!=bb.Get(bb_pos + 4); } }
  public Vector2 NumRandRange { get { return GetNumRandRange(new Vector2()); } }
  public Vector2 GetNumRandRange(Vector2 obj) { return obj.__init(bb_pos + 8, bb); }
  public float Interval { get { return bb.GetFloat(bb_pos + 16); } }
  public float RangeRadius { get { return bb.GetFloat(bb_pos + 20); } }

  public static Offset<RandomLaunchInfo> CreateRandomLaunchInfo(FlatBufferBuilder builder, int Num, bool IsNumRand, float numRandRange_X, float numRandRange_Y, float Interval, float RangeRadius) {
    builder.Prep(4, 24);
    builder.PutFloat(RangeRadius);
    builder.PutFloat(Interval);
    builder.Prep(4, 8);
    builder.PutFloat(numRandRange_Y);
    builder.PutFloat(numRandRange_X);
    builder.Pad(3);
    builder.PutBool(IsNumRand);
    builder.PutInt(Num);
    return new Offset<RandomLaunchInfo>(builder.Offset);
  }
};

public sealed class EntityFrames : Table {
  public static EntityFrames GetRootAsEntityFrames(ByteBuffer _bb) { return GetRootAsEntityFrames(_bb, new EntityFrames()); }
  public static EntityFrames GetRootAsEntityFrames(ByteBuffer _bb, EntityFrames obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EntityFrames __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int ResID { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte Type { get { int o = __offset(8); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public int StartFrames { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string EntityAsset { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public Vector2 Gravity { get { return GetGravity(new Vector2()); } }
  public Vector2 GetGravity(Vector2 obj) { int o = __offset(14); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float Speed { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Angle { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool IsAngleWithEffect { get { int o = __offset(20); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public Vector2 Emitposition { get { return GetEmitposition(new Vector2()); } }
  public Vector2 GetEmitposition(Vector2 obj) { int o = __offset(22); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float EmitPositionZ { get { int o = __offset(24); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public sbyte AxisType { get { int o = __offset(26); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public float ShockTime { get { int o = __offset(28); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ShockSpeed { get { int o = __offset(30); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ShockRangeX { get { int o = __offset(32); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ShockRangeY { get { int o = __offset(34); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool IsRotation { get { int o = __offset(36); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float RotateSpeed { get { int o = __offset(38); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float MoveSpeed { get { int o = __offset(40); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public ShockInfo SceneShock { get { return GetSceneShock(new ShockInfo()); } }
  public ShockInfo GetSceneShock(ShockInfo obj) { int o = __offset(42); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public int HitFallUP { get { int o = __offset(44); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ForceY { get { int o = __offset(46); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int HurtID { get { int o = __offset(48); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float LifeTime { get { int o = __offset(50); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool HitThrough { get { int o = __offset(52); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int HitCount { get { int o = __offset(54); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Distance { get { int o = __offset(56); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool HitGroundClick { get { int o = __offset(58); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float DelayDead { get { int o = __offset(60); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public sbyte OffsetType { get { int o = __offset(62); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public sbyte TargetChooseType { get { int o = __offset(64); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public Vector2 Range { get { return GetRange(new Vector2()); } }
  public Vector2 GetRange(Vector2 obj) { int o = __offset(66); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public float ParaSpeed { get { int o = __offset(68); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ParaGravity { get { int o = __offset(70); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool UseRandomLaunch { get { int o = __offset(72); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public RandomLaunchInfo RandomLaunchInfo { get { return GetRandomLaunchInfo(new RandomLaunchInfo()); } }
  public RandomLaunchInfo GetRandomLaunchInfo(RandomLaunchInfo obj) { int o = __offset(74); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartEntityFrames(FlatBufferBuilder builder) { builder.StartObject(36); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddResID(FlatBufferBuilder builder, int resID) { builder.AddInt(1, resID, 0); }
  public static void AddType(FlatBufferBuilder builder, sbyte type) { builder.AddSbyte(2, type, 0); }
  public static void AddStartFrames(FlatBufferBuilder builder, int startFrames) { builder.AddInt(3, startFrames, 0); }
  public static void AddEntityAsset(FlatBufferBuilder builder, StringOffset entityAssetOffset) { builder.AddOffset(4, entityAssetOffset.Value, 0); }
  public static void AddGravity(FlatBufferBuilder builder, Offset<Vector2> gravityOffset) { builder.AddStruct(5, gravityOffset.Value, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, float speed) { builder.AddFloat(6, speed, 0); }
  public static void AddAngle(FlatBufferBuilder builder, float angle) { builder.AddFloat(7, angle, 0); }
  public static void AddIsAngleWithEffect(FlatBufferBuilder builder, bool isAngleWithEffect) { builder.AddBool(8, isAngleWithEffect, false); }
  public static void AddEmitposition(FlatBufferBuilder builder, Offset<Vector2> emitpositionOffset) { builder.AddStruct(9, emitpositionOffset.Value, 0); }
  public static void AddEmitPositionZ(FlatBufferBuilder builder, float emitPositionZ) { builder.AddFloat(10, emitPositionZ, 0); }
  public static void AddAxisType(FlatBufferBuilder builder, sbyte axisType) { builder.AddSbyte(11, axisType, 0); }
  public static void AddShockTime(FlatBufferBuilder builder, float shockTime) { builder.AddFloat(12, shockTime, 0); }
  public static void AddShockSpeed(FlatBufferBuilder builder, float shockSpeed) { builder.AddFloat(13, shockSpeed, 0); }
  public static void AddShockRangeX(FlatBufferBuilder builder, float shockRangeX) { builder.AddFloat(14, shockRangeX, 0); }
  public static void AddShockRangeY(FlatBufferBuilder builder, float shockRangeY) { builder.AddFloat(15, shockRangeY, 0); }
  public static void AddIsRotation(FlatBufferBuilder builder, bool isRotation) { builder.AddBool(16, isRotation, false); }
  public static void AddRotateSpeed(FlatBufferBuilder builder, float rotateSpeed) { builder.AddFloat(17, rotateSpeed, 0); }
  public static void AddMoveSpeed(FlatBufferBuilder builder, float moveSpeed) { builder.AddFloat(18, moveSpeed, 0); }
  public static void AddSceneShock(FlatBufferBuilder builder, Offset<ShockInfo> sceneShockOffset) { builder.AddStruct(19, sceneShockOffset.Value, 0); }
  public static void AddHitFallUP(FlatBufferBuilder builder, int hitFallUP) { builder.AddInt(20, hitFallUP, 0); }
  public static void AddForceY(FlatBufferBuilder builder, float forceY) { builder.AddFloat(21, forceY, 0); }
  public static void AddHurtID(FlatBufferBuilder builder, int hurtID) { builder.AddInt(22, hurtID, 0); }
  public static void AddLifeTime(FlatBufferBuilder builder, float lifeTime) { builder.AddFloat(23, lifeTime, 0); }
  public static void AddHitThrough(FlatBufferBuilder builder, bool hitThrough) { builder.AddBool(24, hitThrough, false); }
  public static void AddHitCount(FlatBufferBuilder builder, int hitCount) { builder.AddInt(25, hitCount, 0); }
  public static void AddDistance(FlatBufferBuilder builder, float distance) { builder.AddFloat(26, distance, 0); }
  public static void AddHitGroundClick(FlatBufferBuilder builder, bool hitGroundClick) { builder.AddBool(27, hitGroundClick, false); }
  public static void AddDelayDead(FlatBufferBuilder builder, float delayDead) { builder.AddFloat(28, delayDead, 0); }
  public static void AddOffsetType(FlatBufferBuilder builder, sbyte offsetType) { builder.AddSbyte(29, offsetType, 0); }
  public static void AddTargetChooseType(FlatBufferBuilder builder, sbyte targetChooseType) { builder.AddSbyte(30, targetChooseType, 0); }
  public static void AddRange(FlatBufferBuilder builder, Offset<Vector2> rangeOffset) { builder.AddStruct(31, rangeOffset.Value, 0); }
  public static void AddParaSpeed(FlatBufferBuilder builder, float paraSpeed) { builder.AddFloat(32, paraSpeed, 0); }
  public static void AddParaGravity(FlatBufferBuilder builder, float paraGravity) { builder.AddFloat(33, paraGravity, 0); }
  public static void AddUseRandomLaunch(FlatBufferBuilder builder, bool useRandomLaunch) { builder.AddBool(34, useRandomLaunch, false); }
  public static void AddRandomLaunchInfo(FlatBufferBuilder builder, Offset<RandomLaunchInfo> randomLaunchInfoOffset) { builder.AddStruct(35, randomLaunchInfoOffset.Value, 0); }
  public static Offset<EntityFrames> EndEntityFrames(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EntityFrames>(o);
  }
};

public sealed class AnimationFrames : Table {
  public static AnimationFrames GetRootAsAnimationFrames(ByteBuffer _bb) { return GetRootAsAnimationFrames(_bb, new AnimationFrames()); }
  public static AnimationFrames GetRootAsAnimationFrames(ByteBuffer _bb, AnimationFrames obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public AnimationFrames __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float Start { get { int o = __offset(4); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string Anim { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public float Blend { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public sbyte Mode { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public float Speed { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<AnimationFrames> CreateAnimationFrames(FlatBufferBuilder builder,
      float start = 0,
      StringOffset anim = default(StringOffset),
      float blend = 0,
      sbyte mode = 0,
      float speed = 0) {
    builder.StartObject(5);
    AnimationFrames.AddSpeed(builder, speed);
    AnimationFrames.AddBlend(builder, blend);
    AnimationFrames.AddAnim(builder, anim);
    AnimationFrames.AddStart(builder, start);
    AnimationFrames.AddMode(builder, mode);
    return AnimationFrames.EndAnimationFrames(builder);
  }

  public static void StartAnimationFrames(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddStart(FlatBufferBuilder builder, float start) { builder.AddFloat(0, start, 0); }
  public static void AddAnim(FlatBufferBuilder builder, StringOffset animOffset) { builder.AddOffset(1, animOffset.Value, 0); }
  public static void AddBlend(FlatBufferBuilder builder, float blend) { builder.AddFloat(2, blend, 0); }
  public static void AddMode(FlatBufferBuilder builder, sbyte mode) { builder.AddSbyte(3, mode, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, float speed) { builder.AddFloat(4, speed, 0); }
  public static Offset<AnimationFrames> EndAnimationFrames(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<AnimationFrames>(o);
  }
};

public sealed class EntityAttachFrames : Table {
  public static EntityAttachFrames GetRootAsEntityAttachFrames(ByteBuffer _bb) { return GetRootAsEntityAttachFrames(_bb, new EntityAttachFrames()); }
  public static EntityAttachFrames GetRootAsEntityAttachFrames(ByteBuffer _bb, EntityAttachFrames obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public EntityAttachFrames __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int ResID { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Start { get { int o = __offset(8); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float End { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string AttachName { get { int o = __offset(12); return o != 0 ? __string(o + bb_pos) : null; } }
  public string EntityAsset { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public TransformParam Trans { get { return GetTrans(new TransformParam()); } }
  public TransformParam GetTrans(TransformParam obj) { int o = __offset(16); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public AnimationFrames GetAnimations(int j) { return GetAnimations(new AnimationFrames(), j); }
  public AnimationFrames GetAnimations(AnimationFrames obj, int j) { int o = __offset(18); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AnimationsLength { get { int o = __offset(18); return o != 0 ? __vector_len(o) : 0; } }

  public static void StartEntityAttachFrames(FlatBufferBuilder builder) { builder.StartObject(8); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddResID(FlatBufferBuilder builder, int resID) { builder.AddInt(1, resID, 0); }
  public static void AddStart(FlatBufferBuilder builder, float start) { builder.AddFloat(2, start, 0); }
  public static void AddEnd(FlatBufferBuilder builder, float end) { builder.AddFloat(3, end, 0); }
  public static void AddAttachName(FlatBufferBuilder builder, StringOffset attachNameOffset) { builder.AddOffset(4, attachNameOffset.Value, 0); }
  public static void AddEntityAsset(FlatBufferBuilder builder, StringOffset entityAssetOffset) { builder.AddOffset(5, entityAssetOffset.Value, 0); }
  public static void AddTrans(FlatBufferBuilder builder, Offset<TransformParam> transOffset) { builder.AddStruct(6, transOffset.Value, 0); }
  public static void AddAnimations(FlatBufferBuilder builder, VectorOffset animationsOffset) { builder.AddOffset(7, animationsOffset.Value, 0); }
  public static VectorOffset CreateAnimationsVector(FlatBufferBuilder builder, Offset<AnimationFrames>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAnimationsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<EntityAttachFrames> EndEntityAttachFrames(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<EntityAttachFrames>(o);
  }
};

public sealed class ChargeConfig : Table {
  public static ChargeConfig GetRootAsChargeConfig(ByteBuffer _bb) { return GetRootAsChargeConfig(_bb, new ChargeConfig()); }
  public static ChargeConfig GetRootAsChargeConfig(ByteBuffer _bb, ChargeConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public ChargeConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int RepeatPhase { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int ChangePhase { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SwitchPhaseID { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float ChargeDuration { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float ChargeMinDuration { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string Effect { get { int o = __offset(14); return o != 0 ? __string(o + bb_pos) : null; } }
  public string Locator { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }

  public static Offset<ChargeConfig> CreateChargeConfig(FlatBufferBuilder builder,
      int repeatPhase = 0,
      int changePhase = 0,
      int switchPhaseID = 0,
      float chargeDuration = 0,
      float chargeMinDuration = 0,
      StringOffset effect = default(StringOffset),
      StringOffset locator = default(StringOffset)) {
    builder.StartObject(7);
    ChargeConfig.AddLocator(builder, locator);
    ChargeConfig.AddEffect(builder, effect);
    ChargeConfig.AddChargeMinDuration(builder, chargeMinDuration);
    ChargeConfig.AddChargeDuration(builder, chargeDuration);
    ChargeConfig.AddSwitchPhaseID(builder, switchPhaseID);
    ChargeConfig.AddChangePhase(builder, changePhase);
    ChargeConfig.AddRepeatPhase(builder, repeatPhase);
    return ChargeConfig.EndChargeConfig(builder);
  }

  public static void StartChargeConfig(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddRepeatPhase(FlatBufferBuilder builder, int repeatPhase) { builder.AddInt(0, repeatPhase, 0); }
  public static void AddChangePhase(FlatBufferBuilder builder, int changePhase) { builder.AddInt(1, changePhase, 0); }
  public static void AddSwitchPhaseID(FlatBufferBuilder builder, int switchPhaseID) { builder.AddInt(2, switchPhaseID, 0); }
  public static void AddChargeDuration(FlatBufferBuilder builder, float chargeDuration) { builder.AddFloat(3, chargeDuration, 0); }
  public static void AddChargeMinDuration(FlatBufferBuilder builder, float chargeMinDuration) { builder.AddFloat(4, chargeMinDuration, 0); }
  public static void AddEffect(FlatBufferBuilder builder, StringOffset effectOffset) { builder.AddOffset(5, effectOffset.Value, 0); }
  public static void AddLocator(FlatBufferBuilder builder, StringOffset locatorOffset) { builder.AddOffset(6, locatorOffset.Value, 0); }
  public static Offset<ChargeConfig> EndChargeConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<ChargeConfig>(o);
  }
};

public sealed class OperationConfig : Table {
  public static OperationConfig GetRootAsOperationConfig(ByteBuffer _bb) { return GetRootAsOperationConfig(_bb, new OperationConfig()); }
  public static OperationConfig GetRootAsOperationConfig(ByteBuffer _bb, OperationConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public OperationConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int ChangePhase { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GetChangeSkillIDs(int j) { int o = __offset(6); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int ChangeSkillIDsLength { get { int o = __offset(6); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<OperationConfig> CreateOperationConfig(FlatBufferBuilder builder,
      int changePhase = 0,
      VectorOffset changeSkillIDs = default(VectorOffset)) {
    builder.StartObject(2);
    OperationConfig.AddChangeSkillIDs(builder, changeSkillIDs);
    OperationConfig.AddChangePhase(builder, changePhase);
    return OperationConfig.EndOperationConfig(builder);
  }

  public static void StartOperationConfig(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddChangePhase(FlatBufferBuilder builder, int changePhase) { builder.AddInt(0, changePhase, 0); }
  public static void AddChangeSkillIDs(FlatBufferBuilder builder, VectorOffset changeSkillIDsOffset) { builder.AddOffset(1, changeSkillIDsOffset.Value, 0); }
  public static VectorOffset CreateChangeSkillIDsVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartChangeSkillIDsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<OperationConfig> EndOperationConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<OperationConfig>(o);
  }
};

public sealed class SkillJoystickConfig : Table {
  public static SkillJoystickConfig GetRootAsSkillJoystickConfig(ByteBuffer _bb) { return GetRootAsSkillJoystickConfig(_bb, new SkillJoystickConfig()); }
  public static SkillJoystickConfig GetRootAsSkillJoystickConfig(ByteBuffer _bb, SkillJoystickConfig obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SkillJoystickConfig __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public sbyte Mode { get { int o = __offset(4); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public string EffectName { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public Vector3 EffectMoveSpeed { get { return GetEffectMoveSpeed(new Vector3()); } }
  public Vector3 GetEffectMoveSpeed(Vector3 obj) { int o = __offset(8); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vector3 EffectMoveRange { get { return GetEffectMoveRange(new Vector3()); } }
  public Vector3 GetEffectMoveRange(Vector3 obj) { int o = __offset(10); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartSkillJoystickConfig(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddMode(FlatBufferBuilder builder, sbyte mode) { builder.AddSbyte(0, mode, 0); }
  public static void AddEffectName(FlatBufferBuilder builder, StringOffset effectNameOffset) { builder.AddOffset(1, effectNameOffset.Value, 0); }
  public static void AddEffectMoveSpeed(FlatBufferBuilder builder, Offset<Vector3> effectMoveSpeedOffset) { builder.AddStruct(2, effectMoveSpeedOffset.Value, 0); }
  public static void AddEffectMoveRange(FlatBufferBuilder builder, Offset<Vector3> effectMoveRangeOffset) { builder.AddStruct(3, effectMoveRangeOffset.Value, 0); }
  public static Offset<SkillJoystickConfig> EndSkillJoystickConfig(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SkillJoystickConfig>(o);
  }
};

public sealed class SkillEvent : Table {
  public static SkillEvent GetRootAsSkillEvent(ByteBuffer _bb) { return GetRootAsSkillEvent(_bb, new SkillEvent()); }
  public static SkillEvent GetRootAsSkillEvent(ByteBuffer _bb, SkillEvent obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public SkillEvent __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public sbyte EventType { get { int o = __offset(4); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public sbyte EventAction { get { int o = __offset(6); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public string Paramter { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }

  public static Offset<SkillEvent> CreateSkillEvent(FlatBufferBuilder builder,
      sbyte eventType = 0,
      sbyte eventAction = 0,
      StringOffset paramter = default(StringOffset)) {
    builder.StartObject(3);
    SkillEvent.AddParamter(builder, paramter);
    SkillEvent.AddEventAction(builder, eventAction);
    SkillEvent.AddEventType(builder, eventType);
    return SkillEvent.EndSkillEvent(builder);
  }

  public static void StartSkillEvent(FlatBufferBuilder builder) { builder.StartObject(3); }
  public static void AddEventType(FlatBufferBuilder builder, sbyte eventType) { builder.AddSbyte(0, eventType, 0); }
  public static void AddEventAction(FlatBufferBuilder builder, sbyte eventAction) { builder.AddSbyte(1, eventAction, 0); }
  public static void AddParamter(FlatBufferBuilder builder, StringOffset paramterOffset) { builder.AddOffset(2, paramterOffset.Value, 0); }
  public static Offset<SkillEvent> EndSkillEvent(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<SkillEvent>(o);
  }
};

public sealed class boolValue : Table {
  public static boolValue GetRootAsboolValue(ByteBuffer _bb) { return GetRootAsboolValue(_bb, new boolValue()); }
  public static boolValue GetRootAsboolValue(ByteBuffer _bb, boolValue obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public boolValue __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public bool Value { get { int o = __offset(4); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }

  public static Offset<boolValue> CreateboolValue(FlatBufferBuilder builder,
      bool value = false) {
    builder.StartObject(1);
    boolValue.AddValue(builder, value);
    return boolValue.EndboolValue(builder);
  }

  public static void StartboolValue(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddValue(FlatBufferBuilder builder, bool value) { builder.AddBool(0, value, false); }
  public static Offset<boolValue> EndboolValue(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<boolValue>(o);
  }
};

public sealed class floatValue : Table {
  public static floatValue GetRootAsfloatValue(ByteBuffer _bb) { return GetRootAsfloatValue(_bb, new floatValue()); }
  public static floatValue GetRootAsfloatValue(ByteBuffer _bb, floatValue obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public floatValue __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float Value { get { int o = __offset(4); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<floatValue> CreatefloatValue(FlatBufferBuilder builder,
      float value = 0) {
    builder.StartObject(1);
    floatValue.AddValue(builder, value);
    return floatValue.EndfloatValue(builder);
  }

  public static void StartfloatValue(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddValue(FlatBufferBuilder builder, float value) { builder.AddFloat(0, value, 0); }
  public static Offset<floatValue> EndfloatValue(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<floatValue>(o);
  }
};

public sealed class intValue : Table {
  public static intValue GetRootAsintValue(ByteBuffer _bb) { return GetRootAsintValue(_bb, new intValue()); }
  public static intValue GetRootAsintValue(ByteBuffer _bb, intValue obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public intValue __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int Value { get { int o = __offset(4); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<intValue> CreateintValue(FlatBufferBuilder builder,
      int value = 0) {
    builder.StartObject(1);
    intValue.AddValue(builder, value);
    return intValue.EndintValue(builder);
  }

  public static void StartintValue(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddValue(FlatBufferBuilder builder, int value) { builder.AddInt(0, value, 0); }
  public static Offset<intValue> EndintValue(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<intValue>(o);
  }
};

public sealed class QuaternionValue : Table {
  public static QuaternionValue GetRootAsQuaternionValue(ByteBuffer _bb) { return GetRootAsQuaternionValue(_bb, new QuaternionValue()); }
  public static QuaternionValue GetRootAsQuaternionValue(ByteBuffer _bb, QuaternionValue obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public QuaternionValue __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Quaternion Value { get { return GetValue(new Quaternion()); } }
  public Quaternion GetValue(Quaternion obj) { int o = __offset(4); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartQuaternionValue(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddValue(FlatBufferBuilder builder, Offset<Quaternion> valueOffset) { builder.AddStruct(0, valueOffset.Value, 0); }
  public static Offset<QuaternionValue> EndQuaternionValue(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<QuaternionValue>(o);
  }
};

public sealed class uintValue : Table {
  public static uintValue GetRootAsuintValue(ByteBuffer _bb) { return GetRootAsuintValue(_bb, new uintValue()); }
  public static uintValue GetRootAsuintValue(ByteBuffer _bb, uintValue obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public uintValue __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public uint Value { get { int o = __offset(4); return o != 0 ? bb.GetUint(o + bb_pos) : (uint)0; } }

  public static Offset<uintValue> CreateuintValue(FlatBufferBuilder builder,
      uint value = 0) {
    builder.StartObject(1);
    uintValue.AddValue(builder, value);
    return uintValue.EnduintValue(builder);
  }

  public static void StartuintValue(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddValue(FlatBufferBuilder builder, uint value) { builder.AddUint(0, value, 0); }
  public static Offset<uintValue> EnduintValue(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<uintValue>(o);
  }
};

public sealed class Vector3Value : Table {
  public static Vector3Value GetRootAsVector3Value(ByteBuffer _bb) { return GetRootAsVector3Value(_bb, new Vector3Value()); }
  public static Vector3Value GetRootAsVector3Value(ByteBuffer _bb, Vector3Value obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Vector3Value __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public Vector3 Value { get { return GetValue(new Vector3()); } }
  public Vector3 GetValue(Vector3 obj) { int o = __offset(4); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartVector3Value(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddValue(FlatBufferBuilder builder, Offset<Vector3> valueOffset) { builder.AddStruct(0, valueOffset.Value, 0); }
  public static Offset<Vector3Value> EndVector3Value(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Vector3Value>(o);
  }
};

public sealed class DSkillPropertyModify : Table {
  public static DSkillPropertyModify GetRootAsDSkillPropertyModify(ByteBuffer _bb) { return GetRootAsDSkillPropertyModify(_bb, new DSkillPropertyModify()); }
  public static DSkillPropertyModify GetRootAsDSkillPropertyModify(ByteBuffer _bb, DSkillPropertyModify obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillPropertyModify __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte Modifyfliter { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public float Value { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float MovedValue { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public WeaponClassesOrWhatever SvalueType { get { int o = __offset(16); return o != 0 ? (WeaponClassesOrWhatever)bb.Get(o + bb_pos) : (WeaponClassesOrWhatever)0; } }
  public TTable GetSvalue<TTable>(TTable obj) where TTable : Table { int o = __offset(18); return o != 0 ? __union(obj, o) : null; }
  public bool JumpToTargetPos { get { int o = __offset(20); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }

  public static Offset<DSkillPropertyModify> CreateDSkillPropertyModify(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      sbyte modifyfliter = 0,
      float value = 0,
      float movedValue = 0,
      WeaponClassesOrWhatever svalue_type = (WeaponClassesOrWhatever)0,
      int svalue = 0,
      bool jumpToTargetPos = false) {
    builder.StartObject(9);
    DSkillPropertyModify.AddSvalue(builder, svalue);
    DSkillPropertyModify.AddMovedValue(builder, movedValue);
    DSkillPropertyModify.AddValue(builder, value);
    DSkillPropertyModify.AddLength(builder, length);
    DSkillPropertyModify.AddStartframe(builder, startframe);
    DSkillPropertyModify.AddName(builder, name);
    DSkillPropertyModify.AddJumpToTargetPos(builder, jumpToTargetPos);
    DSkillPropertyModify.AddSvalueType(builder, svalue_type);
    DSkillPropertyModify.AddModifyfliter(builder, modifyfliter);
    return DSkillPropertyModify.EndDSkillPropertyModify(builder);
  }

  public static void StartDSkillPropertyModify(FlatBufferBuilder builder) { builder.StartObject(9); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddModifyfliter(FlatBufferBuilder builder, sbyte modifyfliter) { builder.AddSbyte(3, modifyfliter, 0); }
  public static void AddValue(FlatBufferBuilder builder, float value) { builder.AddFloat(4, value, 0); }
  public static void AddMovedValue(FlatBufferBuilder builder, float movedValue) { builder.AddFloat(5, movedValue, 0); }
  public static void AddSvalueType(FlatBufferBuilder builder, WeaponClassesOrWhatever svalueType) { builder.AddByte(6, (byte)(svalueType), 0); }
  public static void AddSvalue(FlatBufferBuilder builder, int svalueOffset) { builder.AddOffset(7, svalueOffset, 0); }
  public static void AddJumpToTargetPos(FlatBufferBuilder builder, bool jumpToTargetPos) { builder.AddBool(8, jumpToTargetPos, false); }
  public static Offset<DSkillPropertyModify> EndDSkillPropertyModify(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillPropertyModify>(o);
  }
};

public sealed class DSkillFrameTag : Table {
  public static DSkillFrameTag GetRootAsDSkillFrameTag(ByteBuffer _bb) { return GetRootAsDSkillFrameTag(_bb, new DSkillFrameTag()); }
  public static DSkillFrameTag GetRootAsDSkillFrameTag(ByteBuffer _bb, DSkillFrameTag obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillFrameTag __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte Tag { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }

  public static Offset<DSkillFrameTag> CreateDSkillFrameTag(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      sbyte tag = 0) {
    builder.StartObject(4);
    DSkillFrameTag.AddLength(builder, length);
    DSkillFrameTag.AddStartframe(builder, startframe);
    DSkillFrameTag.AddName(builder, name);
    DSkillFrameTag.AddTag(builder, tag);
    return DSkillFrameTag.EndDSkillFrameTag(builder);
  }

  public static void StartDSkillFrameTag(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddTag(FlatBufferBuilder builder, sbyte tag) { builder.AddSbyte(3, tag, 0); }
  public static Offset<DSkillFrameTag> EndDSkillFrameTag(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillFrameTag>(o);
  }
};

public sealed class DSkillSfx : Table {
  public static DSkillSfx GetRootAsDSkillSfx(ByteBuffer _bb) { return GetRootAsDSkillSfx(_bb, new DSkillSfx()); }
  public static DSkillSfx GetRootAsDSkillSfx(ByteBuffer _bb, DSkillSfx obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillSfx __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public string SoundClipAsset { get { int o = __offset(10); return o != 0 ? __string(o + bb_pos) : null; } }
  public bool Loop { get { int o = __offset(12); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public int SoundID { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }

  public static Offset<DSkillSfx> CreateDSkillSfx(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      StringOffset soundClipAsset = default(StringOffset),
      bool loop = false,
      int soundID = 0) {
    builder.StartObject(6);
    DSkillSfx.AddSoundID(builder, soundID);
    DSkillSfx.AddSoundClipAsset(builder, soundClipAsset);
    DSkillSfx.AddLength(builder, length);
    DSkillSfx.AddStartframe(builder, startframe);
    DSkillSfx.AddName(builder, name);
    DSkillSfx.AddLoop(builder, loop);
    return DSkillSfx.EndDSkillSfx(builder);
  }

  public static void StartDSkillSfx(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddSoundClipAsset(FlatBufferBuilder builder, StringOffset soundClipAssetOffset) { builder.AddOffset(3, soundClipAssetOffset.Value, 0); }
  public static void AddLoop(FlatBufferBuilder builder, bool loop) { builder.AddBool(4, loop, false); }
  public static void AddSoundID(FlatBufferBuilder builder, int soundID) { builder.AddInt(5, soundID, 0); }
  public static Offset<DSkillSfx> EndDSkillSfx(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillSfx>(o);
  }
};

public sealed class DSkillFrameEffect : Table {
  public static DSkillFrameEffect GetRootAsDSkillFrameEffect(ByteBuffer _bb) { return GetRootAsDSkillFrameEffect(_bb, new DSkillFrameEffect()); }
  public static DSkillFrameEffect GetRootAsDSkillFrameEffect(ByteBuffer _bb, DSkillFrameEffect obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillFrameEffect __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int EffectID { get { int o = __offset(10); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float BuffTime { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool PhaseDelete { get { int o = __offset(14); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool FinishDelete { get { int o = __offset(16); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)true; } }
  public bool UseBuffAni { get { int o = __offset(18); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)true; } }
  public bool UsePause { get { int o = __offset(20); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float PauseTime { get { int o = __offset(22); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<DSkillFrameEffect> CreateDSkillFrameEffect(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      int effectID = 0,
      float buffTime = 0,
      bool phaseDelete = false,
      bool finishDelete = true,
      bool useBuffAni = true,
      bool usePause = false,
      float pauseTime = 0) {
    builder.StartObject(10);
    DSkillFrameEffect.AddPauseTime(builder, pauseTime);
    DSkillFrameEffect.AddBuffTime(builder, buffTime);
    DSkillFrameEffect.AddEffectID(builder, effectID);
    DSkillFrameEffect.AddLength(builder, length);
    DSkillFrameEffect.AddStartframe(builder, startframe);
    DSkillFrameEffect.AddName(builder, name);
    DSkillFrameEffect.AddUsePause(builder, usePause);
    DSkillFrameEffect.AddUseBuffAni(builder, useBuffAni);
    DSkillFrameEffect.AddFinishDelete(builder, finishDelete);
    DSkillFrameEffect.AddPhaseDelete(builder, phaseDelete);
    return DSkillFrameEffect.EndDSkillFrameEffect(builder);
  }

  public static void StartDSkillFrameEffect(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddEffectID(FlatBufferBuilder builder, int effectID) { builder.AddInt(3, effectID, 0); }
  public static void AddBuffTime(FlatBufferBuilder builder, float buffTime) { builder.AddFloat(4, buffTime, 0); }
  public static void AddPhaseDelete(FlatBufferBuilder builder, bool phaseDelete) { builder.AddBool(5, phaseDelete, false); }
  public static void AddFinishDelete(FlatBufferBuilder builder, bool finishDelete) { builder.AddBool(6, finishDelete, true); }
  public static void AddUseBuffAni(FlatBufferBuilder builder, bool useBuffAni) { builder.AddBool(7, useBuffAni, true); }
  public static void AddUsePause(FlatBufferBuilder builder, bool usePause) { builder.AddBool(8, usePause, false); }
  public static void AddPauseTime(FlatBufferBuilder builder, float pauseTime) { builder.AddFloat(9, pauseTime, 0); }
  public static Offset<DSkillFrameEffect> EndDSkillFrameEffect(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillFrameEffect>(o);
  }
};

public sealed class DSkillCameraMove : Table {
  public static DSkillCameraMove GetRootAsDSkillCameraMove(ByteBuffer _bb) { return GetRootAsDSkillCameraMove(_bb, new DSkillCameraMove()); }
  public static DSkillCameraMove GetRootAsDSkillCameraMove(ByteBuffer _bb, DSkillCameraMove obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillCameraMove __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Offset { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Duraction { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<DSkillCameraMove> CreateDSkillCameraMove(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      float offset = 0,
      float duraction = 0) {
    builder.StartObject(5);
    DSkillCameraMove.AddDuraction(builder, duraction);
    DSkillCameraMove.AddOffset(builder, offset);
    DSkillCameraMove.AddLength(builder, length);
    DSkillCameraMove.AddStartframe(builder, startframe);
    DSkillCameraMove.AddName(builder, name);
    return DSkillCameraMove.EndDSkillCameraMove(builder);
  }

  public static void StartDSkillCameraMove(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddOffset(FlatBufferBuilder builder, float offset) { builder.AddFloat(3, offset, 0); }
  public static void AddDuraction(FlatBufferBuilder builder, float duraction) { builder.AddFloat(4, duraction, 0); }
  public static Offset<DSkillCameraMove> EndDSkillCameraMove(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillCameraMove>(o);
  }
};

public sealed class DSkillWalkControl : Table {
  public static DSkillWalkControl GetRootAsDSkillWalkControl(ByteBuffer _bb) { return GetRootAsDSkillWalkControl(_bb, new DSkillWalkControl()); }
  public static DSkillWalkControl GetRootAsDSkillWalkControl(ByteBuffer _bb, DSkillWalkControl obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillWalkControl __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte WalkMode { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public float WalkSpeedPercent { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<DSkillWalkControl> CreateDSkillWalkControl(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      sbyte walkMode = 0,
      float walkSpeedPercent = 0) {
    builder.StartObject(5);
    DSkillWalkControl.AddWalkSpeedPercent(builder, walkSpeedPercent);
    DSkillWalkControl.AddLength(builder, length);
    DSkillWalkControl.AddStartframe(builder, startframe);
    DSkillWalkControl.AddName(builder, name);
    DSkillWalkControl.AddWalkMode(builder, walkMode);
    return DSkillWalkControl.EndDSkillWalkControl(builder);
  }

  public static void StartDSkillWalkControl(FlatBufferBuilder builder) { builder.StartObject(5); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddWalkMode(FlatBufferBuilder builder, sbyte walkMode) { builder.AddSbyte(3, walkMode, 0); }
  public static void AddWalkSpeedPercent(FlatBufferBuilder builder, float walkSpeedPercent) { builder.AddFloat(4, walkSpeedPercent, 0); }
  public static Offset<DSkillWalkControl> EndDSkillWalkControl(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillWalkControl>(o);
  }
};

public sealed class DSkillFrameGrap : Table {
  public static DSkillFrameGrap GetRootAsDSkillFrameGrap(ByteBuffer _bb) { return GetRootAsDSkillFrameGrap(_bb, new DSkillFrameGrap()); }
  public static DSkillFrameGrap GetRootAsDSkillFrameGrap(ByteBuffer _bb, DSkillFrameGrap obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillFrameGrap __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte Op { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }

  public static Offset<DSkillFrameGrap> CreateDSkillFrameGrap(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      sbyte op = 0) {
    builder.StartObject(4);
    DSkillFrameGrap.AddLength(builder, length);
    DSkillFrameGrap.AddStartframe(builder, startframe);
    DSkillFrameGrap.AddName(builder, name);
    DSkillFrameGrap.AddOp(builder, op);
    return DSkillFrameGrap.EndDSkillFrameGrap(builder);
  }

  public static void StartDSkillFrameGrap(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddOp(FlatBufferBuilder builder, sbyte op) { builder.AddSbyte(3, op, 0); }
  public static Offset<DSkillFrameGrap> EndDSkillFrameGrap(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillFrameGrap>(o);
  }
};

public sealed class DSkillFrameStateOp : Table {
  public static DSkillFrameStateOp GetRootAsDSkillFrameStateOp(ByteBuffer _bb) { return GetRootAsDSkillFrameStateOp(_bb, new DSkillFrameStateOp()); }
  public static DSkillFrameStateOp GetRootAsDSkillFrameStateOp(ByteBuffer _bb, DSkillFrameStateOp obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillFrameStateOp __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte Op { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public sbyte State { get { int o = __offset(12); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public int Idata1 { get { int o = __offset(14); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Idata2 { get { int o = __offset(16); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Fdata1 { get { int o = __offset(18); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Fdata2 { get { int o = __offset(20); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public sbyte Statetag { get { int o = __offset(22); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }

  public static Offset<DSkillFrameStateOp> CreateDSkillFrameStateOp(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      sbyte op = 0,
      sbyte state = 0,
      int idata1 = 0,
      int idata2 = 0,
      float fdata1 = 0,
      float fdata2 = 0,
      sbyte statetag = 0) {
    builder.StartObject(10);
    DSkillFrameStateOp.AddFdata2(builder, fdata2);
    DSkillFrameStateOp.AddFdata1(builder, fdata1);
    DSkillFrameStateOp.AddIdata2(builder, idata2);
    DSkillFrameStateOp.AddIdata1(builder, idata1);
    DSkillFrameStateOp.AddLength(builder, length);
    DSkillFrameStateOp.AddStartframe(builder, startframe);
    DSkillFrameStateOp.AddName(builder, name);
    DSkillFrameStateOp.AddStatetag(builder, statetag);
    DSkillFrameStateOp.AddState(builder, state);
    DSkillFrameStateOp.AddOp(builder, op);
    return DSkillFrameStateOp.EndDSkillFrameStateOp(builder);
  }

  public static void StartDSkillFrameStateOp(FlatBufferBuilder builder) { builder.StartObject(10); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddOp(FlatBufferBuilder builder, sbyte op) { builder.AddSbyte(3, op, 0); }
  public static void AddState(FlatBufferBuilder builder, sbyte state) { builder.AddSbyte(4, state, 0); }
  public static void AddIdata1(FlatBufferBuilder builder, int idata1) { builder.AddInt(5, idata1, 0); }
  public static void AddIdata2(FlatBufferBuilder builder, int idata2) { builder.AddInt(6, idata2, 0); }
  public static void AddFdata1(FlatBufferBuilder builder, float fdata1) { builder.AddFloat(7, fdata1, 0); }
  public static void AddFdata2(FlatBufferBuilder builder, float fdata2) { builder.AddFloat(8, fdata2, 0); }
  public static void AddStatetag(FlatBufferBuilder builder, sbyte statetag) { builder.AddSbyte(9, statetag, 0); }
  public static Offset<DSkillFrameStateOp> EndDSkillFrameStateOp(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillFrameStateOp>(o);
  }
};

public sealed class DSkillFrameEventSceneShock : Table {
  public static DSkillFrameEventSceneShock GetRootAsDSkillFrameEventSceneShock(ByteBuffer _bb) { return GetRootAsDSkillFrameEventSceneShock(_bb, new DSkillFrameEventSceneShock()); }
  public static DSkillFrameEventSceneShock GetRootAsDSkillFrameEventSceneShock(ByteBuffer _bb, DSkillFrameEventSceneShock obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DSkillFrameEventSceneShock __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Time { get { int o = __offset(10); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Speed { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Xrange { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Yrange { get { int o = __offset(16); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }

  public static Offset<DSkillFrameEventSceneShock> CreateDSkillFrameEventSceneShock(FlatBufferBuilder builder,
      StringOffset name = default(StringOffset),
      int startframe = 0,
      int length = 0,
      float time = 0,
      float speed = 0,
      float xrange = 0,
      float yrange = 0) {
    builder.StartObject(7);
    DSkillFrameEventSceneShock.AddYrange(builder, yrange);
    DSkillFrameEventSceneShock.AddXrange(builder, xrange);
    DSkillFrameEventSceneShock.AddSpeed(builder, speed);
    DSkillFrameEventSceneShock.AddTime(builder, time);
    DSkillFrameEventSceneShock.AddLength(builder, length);
    DSkillFrameEventSceneShock.AddStartframe(builder, startframe);
    DSkillFrameEventSceneShock.AddName(builder, name);
    return DSkillFrameEventSceneShock.EndDSkillFrameEventSceneShock(builder);
  }

  public static void StartDSkillFrameEventSceneShock(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddTime(FlatBufferBuilder builder, float time) { builder.AddFloat(3, time, 0); }
  public static void AddSpeed(FlatBufferBuilder builder, float speed) { builder.AddFloat(4, speed, 0); }
  public static void AddXrange(FlatBufferBuilder builder, float xrange) { builder.AddFloat(5, xrange, 0); }
  public static void AddYrange(FlatBufferBuilder builder, float yrange) { builder.AddFloat(6, yrange, 0); }
  public static Offset<DSkillFrameEventSceneShock> EndDSkillFrameEventSceneShock(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DSkillFrameEventSceneShock>(o);
  }
};

public sealed class DActionData : Table {
  public static DActionData GetRootAsDActionData(ByteBuffer _bb) { return GetRootAsDActionData(_bb, new DActionData()); }
  public static DActionData GetRootAsDActionData(ByteBuffer _bb, DActionData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public DActionData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Startframe { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int Length { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public sbyte ActionType { get { int o = __offset(10); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public float Duration { get { int o = __offset(12); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float DeltaScale { get { int o = __offset(14); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public Vector3 DeltaPos { get { return GetDeltaPos(new Vector3()); } }
  public Vector3 GetDeltaPos(Vector3 obj) { int o = __offset(16); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartDActionData(FlatBufferBuilder builder) { builder.StartObject(7); }
  public static void AddName(FlatBufferBuilder builder, StringOffset nameOffset) { builder.AddOffset(0, nameOffset.Value, 0); }
  public static void AddStartframe(FlatBufferBuilder builder, int startframe) { builder.AddInt(1, startframe, 0); }
  public static void AddLength(FlatBufferBuilder builder, int length) { builder.AddInt(2, length, 0); }
  public static void AddActionType(FlatBufferBuilder builder, sbyte actionType) { builder.AddSbyte(3, actionType, 0); }
  public static void AddDuration(FlatBufferBuilder builder, float duration) { builder.AddFloat(4, duration, 0); }
  public static void AddDeltaScale(FlatBufferBuilder builder, float deltaScale) { builder.AddFloat(5, deltaScale, 0); }
  public static void AddDeltaPos(FlatBufferBuilder builder, Offset<Vector3> deltaPosOffset) { builder.AddStruct(6, deltaPosOffset.Value, 0); }
  public static Offset<DActionData> EndDActionData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<DActionData>(o);
  }
};

public sealed class FBSkillData : Table {
  public static FBSkillData GetRootAsFBSkillData(ByteBuffer _bb) { return GetRootAsFBSkillData(_bb, new FBSkillData()); }
  public static FBSkillData GetRootAsFBSkillData(ByteBuffer _bb, FBSkillData obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FBSkillData __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string _name { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public int SkillID { get { int o = __offset(6); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int SkillPriority { get { int o = __offset(8); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GetSkillPhases(int j) { int o = __offset(10); return o != 0 ? bb.GetInt(__vector(o) + j * 4) : (int)0; }
  public int SkillPhasesLength { get { int o = __offset(10); return o != 0 ? __vector_len(o) : 0; } }
  public int IsLoop { get { int o = __offset(12); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public bool NotLoopLastFrame { get { int o = __offset(14); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public string HitEffect { get { int o = __offset(16); return o != 0 ? __string(o + bb_pos) : null; } }
  public string GoHitEffectAsset { get { int o = __offset(18); return o != 0 ? __string(o + bb_pos) : null; } }
  public string GoSFXAsset { get { int o = __offset(20); return o != 0 ? __string(o + bb_pos) : null; } }
  public int HitSFXID { get { int o = __offset(22); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int HurtType { get { int o = __offset(24); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float HurtTime { get { int o = __offset(26); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int HurtPause { get { int o = __offset(28); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float HurtPauseTime { get { int o = __offset(30); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Forcex { get { int o = __offset(32); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float Forcey { get { int o = __offset(34); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public string Dscription { get { int o = __offset(36); return o != 0 ? __string(o + bb_pos) : null; } }
  public string CaracterAsset { get { int o = __offset(38); return o != 0 ? __string(o + bb_pos) : null; } }
  public int Fps { get { int o = __offset(40); return o != 0 ? bb.GetInt(o + bb_pos) : (int)60; } }
  public string AnimationName { get { int o = __offset(42); return o != 0 ? __string(o + bb_pos) : null; } }
  public string MoveName { get { int o = __offset(44); return o != 0 ? __string(o + bb_pos) : null; } }
  public sbyte WapMode { get { int o = __offset(46); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public float IterpolationSpeed { get { int o = __offset(48); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float AnimationSpeed { get { int o = __offset(50); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)1; } }
  public int TotalFrames { get { int o = __offset(52); return o != 0 ? bb.GetInt(o + bb_pos) : (int)15; } }
  public int StartUpFrames { get { int o = __offset(54); return o != 0 ? bb.GetInt(o + bb_pos) : (int)5; } }
  public int ActiveFrames { get { int o = __offset(56); return o != 0 ? bb.GetInt(o + bb_pos) : (int)5; } }
  public int RcoveryFrames { get { int o = __offset(58); return o != 0 ? bb.GetInt(o + bb_pos) : (int)5; } }
  public bool UeSpellBar { get { int o = __offset(60); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float SellBarTime { get { int o = __offset(62); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int CmboStartFrame { get { int o = __offset(64); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int CmboSkillID { get { int o = __offset(66); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float Skilltime { get { int o = __offset(68); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool CameraRestore { get { int o = __offset(70); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public float CameraRestoreTime { get { int o = __offset(72); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float GrabPosx { get { int o = __offset(74); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float GrabPosy { get { int o = __offset(76); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float GrabEndForceX { get { int o = __offset(78); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float GrabEndForceY { get { int o = __offset(80); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public float GrabTime { get { int o = __offset(82); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public int GrabEndEffectType { get { int o = __offset(84); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GrabAction { get { int o = __offset(86); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public int GrabNum { get { int o = __offset(88); return o != 0 ? bb.GetInt(o + bb_pos) : (int)0; } }
  public float GrabMoveSpeed { get { int o = __offset(90); return o != 0 ? bb.GetFloat(o + bb_pos) : (float)0; } }
  public bool GrabSupportQuickPressDismis { get { int o = __offset(92); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public bool IsCharge { get { int o = __offset(94); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public ChargeConfig ChargeConfig { get { return GetChargeConfig(new ChargeConfig()); } }
  public ChargeConfig GetChargeConfig(ChargeConfig obj) { int o = __offset(96); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public bool IsSpeicalOperate { get { int o = __offset(98); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public OperationConfig OperationConfig { get { return GetOperationConfig(new OperationConfig()); } }
  public OperationConfig GetOperationConfig(OperationConfig obj) { int o = __offset(100); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public SkillJoystickConfig SkillJoystickConfig { get { return GetSkillJoystickConfig(new SkillJoystickConfig()); } }
  public SkillJoystickConfig GetSkillJoystickConfig(SkillJoystickConfig obj) { int o = __offset(102); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }
  public SkillEvent GetSkillEvents(int j) { return GetSkillEvents(new SkillEvent(), j); }
  public SkillEvent GetSkillEvents(SkillEvent obj, int j) { int o = __offset(104); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SkillEventsLength { get { int o = __offset(104); return o != 0 ? __vector_len(o) : 0; } }
  public sbyte TriggerType { get { int o = __offset(106); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public HurtDecisionBox GetHurtBlocks(int j) { return GetHurtBlocks(new HurtDecisionBox(), j); }
  public HurtDecisionBox GetHurtBlocks(HurtDecisionBox obj, int j) { int o = __offset(108); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int HurtBlocksLength { get { int o = __offset(108); return o != 0 ? __vector_len(o) : 0; } }
  public DefenceDecisionBox GetDefenceBlocks(int j) { return GetDefenceBlocks(new DefenceDecisionBox(), j); }
  public DefenceDecisionBox GetDefenceBlocks(DefenceDecisionBox obj, int j) { int o = __offset(110); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int DefenceBlocksLength { get { int o = __offset(110); return o != 0 ? __vector_len(o) : 0; } }
  public EntityAttachFrames GetAttachFrames(int j) { return GetAttachFrames(new EntityAttachFrames(), j); }
  public EntityAttachFrames GetAttachFrames(EntityAttachFrames obj, int j) { int o = __offset(112); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int AttachFramesLength { get { int o = __offset(112); return o != 0 ? __vector_len(o) : 0; } }
  public EffectsFrames GetEffectFrames(int j) { return GetEffectFrames(new EffectsFrames(), j); }
  public EffectsFrames GetEffectFrames(EffectsFrames obj, int j) { int o = __offset(114); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EffectFramesLength { get { int o = __offset(114); return o != 0 ? __vector_len(o) : 0; } }
  public EntityFrames GetEntityFrames(int j) { return GetEntityFrames(new EntityFrames(), j); }
  public EntityFrames GetEntityFrames(EntityFrames obj, int j) { int o = __offset(116); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int EntityFramesLength { get { int o = __offset(116); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillFrameTag GetFrameTags(int j) { return GetFrameTags(new DSkillFrameTag(), j); }
  public DSkillFrameTag GetFrameTags(DSkillFrameTag obj, int j) { int o = __offset(118); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FrameTagsLength { get { int o = __offset(118); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillFrameGrap GetFrameGrap(int j) { return GetFrameGrap(new DSkillFrameGrap(), j); }
  public DSkillFrameGrap GetFrameGrap(DSkillFrameGrap obj, int j) { int o = __offset(120); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FrameGrapLength { get { int o = __offset(120); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillFrameStateOp GetStateop(int j) { return GetStateop(new DSkillFrameStateOp(), j); }
  public DSkillFrameStateOp GetStateop(DSkillFrameStateOp obj, int j) { int o = __offset(122); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int StateopLength { get { int o = __offset(122); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillPropertyModify GetProperModify(int j) { return GetProperModify(new DSkillPropertyModify(), j); }
  public DSkillPropertyModify GetProperModify(DSkillPropertyModify obj, int j) { int o = __offset(124); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ProperModifyLength { get { int o = __offset(124); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillFrameEventSceneShock GetShocks(int j) { return GetShocks(new DSkillFrameEventSceneShock(), j); }
  public DSkillFrameEventSceneShock GetShocks(DSkillFrameEventSceneShock obj, int j) { int o = __offset(126); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ShocksLength { get { int o = __offset(126); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillSfx GetSfx(int j) { return GetSfx(new DSkillSfx(), j); }
  public DSkillSfx GetSfx(DSkillSfx obj, int j) { int o = __offset(128); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int SfxLength { get { int o = __offset(128); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillFrameEffect GetFrameEffects(int j) { return GetFrameEffects(new DSkillFrameEffect(), j); }
  public DSkillFrameEffect GetFrameEffects(DSkillFrameEffect obj, int j) { int o = __offset(130); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int FrameEffectsLength { get { int o = __offset(130); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillCameraMove GetCameraMoves(int j) { return GetCameraMoves(new DSkillCameraMove(), j); }
  public DSkillCameraMove GetCameraMoves(DSkillCameraMove obj, int j) { int o = __offset(132); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CameraMovesLength { get { int o = __offset(132); return o != 0 ? __vector_len(o) : 0; } }
  public DSkillWalkControl GetWalkControl(int j) { return GetWalkControl(new DSkillWalkControl(), j); }
  public DSkillWalkControl GetWalkControl(DSkillWalkControl obj, int j) { int o = __offset(134); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int WalkControlLength { get { int o = __offset(134); return o != 0 ? __vector_len(o) : 0; } }
  public DActionData GetActions(int j) { return GetActions(new DActionData(), j); }
  public DActionData GetActions(DActionData obj, int j) { int o = __offset(136); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int ActionsLength { get { int o = __offset(136); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<FBSkillData> CreateFBSkillData(FlatBufferBuilder builder,
      StringOffset _name = default(StringOffset),
      int skillID = 0,
      int skillPriority = 0,
      VectorOffset skillPhases = default(VectorOffset),
      int isLoop = 0,
      bool notLoopLastFrame = false,
      StringOffset hitEffect = default(StringOffset),
      StringOffset goHitEffectAsset = default(StringOffset),
      StringOffset goSFXAsset = default(StringOffset),
      int hitSFXID = 0,
      int hurtType = 0,
      float hurtTime = 0,
      int hurtPause = 0,
      float hurtPauseTime = 0,
      float forcex = 0,
      float forcey = 0,
      StringOffset dscription = default(StringOffset),
      StringOffset caracterAsset = default(StringOffset),
      int fps = 60,
      StringOffset animationName = default(StringOffset),
      StringOffset moveName = default(StringOffset),
      sbyte wapMode = 0,
      float iterpolationSpeed = 0,
      float animationSpeed = 1,
      int totalFrames = 15,
      int startUpFrames = 5,
      int activeFrames = 5,
      int rcoveryFrames = 5,
      bool ueSpellBar = false,
      float sellBarTime = 0,
      int cmboStartFrame = 0,
      int cmboSkillID = 0,
      float skilltime = 0,
      bool cameraRestore = false,
      float cameraRestoreTime = 0,
      float grabPosx = 0,
      float grabPosy = 0,
      float grabEndForceX = 0,
      float grabEndForceY = 0,
      float grabTime = 0,
      int grabEndEffectType = 0,
      int grabAction = 0,
      int grabNum = 0,
      float grabMoveSpeed = 0,
      bool grabSupportQuickPressDismis = false,
      bool isCharge = false,
      Offset<ChargeConfig> chargeConfig = default(Offset<ChargeConfig>),
      bool isSpeicalOperate = false,
      Offset<OperationConfig> operationConfig = default(Offset<OperationConfig>),
      Offset<SkillJoystickConfig> skillJoystickConfig = default(Offset<SkillJoystickConfig>),
      VectorOffset skillEvents = default(VectorOffset),
      sbyte triggerType = 0,
      VectorOffset HurtBlocks = default(VectorOffset),
      VectorOffset DefenceBlocks = default(VectorOffset),
      VectorOffset attachFrames = default(VectorOffset),
      VectorOffset effectFrames = default(VectorOffset),
      VectorOffset entityFrames = default(VectorOffset),
      VectorOffset frameTags = default(VectorOffset),
      VectorOffset frameGrap = default(VectorOffset),
      VectorOffset stateop = default(VectorOffset),
      VectorOffset properModify = default(VectorOffset),
      VectorOffset shocks = default(VectorOffset),
      VectorOffset sfx = default(VectorOffset),
      VectorOffset frameEffects = default(VectorOffset),
      VectorOffset cameraMoves = default(VectorOffset),
      VectorOffset walkControl = default(VectorOffset),
      VectorOffset actions = default(VectorOffset)) {
    builder.StartObject(67);
    FBSkillData.AddActions(builder, actions);
    FBSkillData.AddWalkControl(builder, walkControl);
    FBSkillData.AddCameraMoves(builder, cameraMoves);
    FBSkillData.AddFrameEffects(builder, frameEffects);
    FBSkillData.AddSfx(builder, sfx);
    FBSkillData.AddShocks(builder, shocks);
    FBSkillData.AddProperModify(builder, properModify);
    FBSkillData.AddStateop(builder, stateop);
    FBSkillData.AddFrameGrap(builder, frameGrap);
    FBSkillData.AddFrameTags(builder, frameTags);
    FBSkillData.AddEntityFrames(builder, entityFrames);
    FBSkillData.AddEffectFrames(builder, effectFrames);
    FBSkillData.AddAttachFrames(builder, attachFrames);
    FBSkillData.AddDefenceBlocks(builder, DefenceBlocks);
    FBSkillData.AddHurtBlocks(builder, HurtBlocks);
    FBSkillData.AddSkillEvents(builder, skillEvents);
    FBSkillData.AddSkillJoystickConfig(builder, skillJoystickConfig);
    FBSkillData.AddOperationConfig(builder, operationConfig);
    FBSkillData.AddChargeConfig(builder, chargeConfig);
    FBSkillData.AddGrabMoveSpeed(builder, grabMoveSpeed);
    FBSkillData.AddGrabNum(builder, grabNum);
    FBSkillData.AddGrabAction(builder, grabAction);
    FBSkillData.AddGrabEndEffectType(builder, grabEndEffectType);
    FBSkillData.AddGrabTime(builder, grabTime);
    FBSkillData.AddGrabEndForceY(builder, grabEndForceY);
    FBSkillData.AddGrabEndForceX(builder, grabEndForceX);
    FBSkillData.AddGrabPosy(builder, grabPosy);
    FBSkillData.AddGrabPosx(builder, grabPosx);
    FBSkillData.AddCameraRestoreTime(builder, cameraRestoreTime);
    FBSkillData.AddSkilltime(builder, skilltime);
    FBSkillData.AddCmboSkillID(builder, cmboSkillID);
    FBSkillData.AddCmboStartFrame(builder, cmboStartFrame);
    FBSkillData.AddSellBarTime(builder, sellBarTime);
    FBSkillData.AddRcoveryFrames(builder, rcoveryFrames);
    FBSkillData.AddActiveFrames(builder, activeFrames);
    FBSkillData.AddStartUpFrames(builder, startUpFrames);
    FBSkillData.AddTotalFrames(builder, totalFrames);
    FBSkillData.AddAnimationSpeed(builder, animationSpeed);
    FBSkillData.AddIterpolationSpeed(builder, iterpolationSpeed);
    FBSkillData.AddMoveName(builder, moveName);
    FBSkillData.AddAnimationName(builder, animationName);
    FBSkillData.AddFps(builder, fps);
    FBSkillData.AddCaracterAsset(builder, caracterAsset);
    FBSkillData.AddDscription(builder, dscription);
    FBSkillData.AddForcey(builder, forcey);
    FBSkillData.AddForcex(builder, forcex);
    FBSkillData.AddHurtPauseTime(builder, hurtPauseTime);
    FBSkillData.AddHurtPause(builder, hurtPause);
    FBSkillData.AddHurtTime(builder, hurtTime);
    FBSkillData.AddHurtType(builder, hurtType);
    FBSkillData.AddHitSFXID(builder, hitSFXID);
    FBSkillData.AddGoSFXAsset(builder, goSFXAsset);
    FBSkillData.AddGoHitEffectAsset(builder, goHitEffectAsset);
    FBSkillData.AddHitEffect(builder, hitEffect);
    FBSkillData.AddIsLoop(builder, isLoop);
    FBSkillData.AddSkillPhases(builder, skillPhases);
    FBSkillData.AddSkillPriority(builder, skillPriority);
    FBSkillData.AddSkillID(builder, skillID);
    FBSkillData.Add_name(builder, _name);
    FBSkillData.AddTriggerType(builder, triggerType);
    FBSkillData.AddIsSpeicalOperate(builder, isSpeicalOperate);
    FBSkillData.AddIsCharge(builder, isCharge);
    FBSkillData.AddGrabSupportQuickPressDismis(builder, grabSupportQuickPressDismis);
    FBSkillData.AddCameraRestore(builder, cameraRestore);
    FBSkillData.AddUeSpellBar(builder, ueSpellBar);
    FBSkillData.AddWapMode(builder, wapMode);
    FBSkillData.AddNotLoopLastFrame(builder, notLoopLastFrame);
    return FBSkillData.EndFBSkillData(builder);
  }

  public static void StartFBSkillData(FlatBufferBuilder builder) { builder.StartObject(67); }
  public static void Add_name(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(0, NameOffset.Value, 0); }
  public static void AddSkillID(FlatBufferBuilder builder, int skillID) { builder.AddInt(1, skillID, 0); }
  public static void AddSkillPriority(FlatBufferBuilder builder, int skillPriority) { builder.AddInt(2, skillPriority, 0); }
  public static void AddSkillPhases(FlatBufferBuilder builder, VectorOffset skillPhasesOffset) { builder.AddOffset(3, skillPhasesOffset.Value, 0); }
  public static VectorOffset CreateSkillPhasesVector(FlatBufferBuilder builder, int[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddInt(data[i]); return builder.EndVector(); }
  public static void StartSkillPhasesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddIsLoop(FlatBufferBuilder builder, int isLoop) { builder.AddInt(4, isLoop, 0); }
  public static void AddNotLoopLastFrame(FlatBufferBuilder builder, bool notLoopLastFrame) { builder.AddBool(5, notLoopLastFrame, false); }
  public static void AddHitEffect(FlatBufferBuilder builder, StringOffset hitEffectOffset) { builder.AddOffset(6, hitEffectOffset.Value, 0); }
  public static void AddGoHitEffectAsset(FlatBufferBuilder builder, StringOffset goHitEffectAssetOffset) { builder.AddOffset(7, goHitEffectAssetOffset.Value, 0); }
  public static void AddGoSFXAsset(FlatBufferBuilder builder, StringOffset goSFXAssetOffset) { builder.AddOffset(8, goSFXAssetOffset.Value, 0); }
  public static void AddHitSFXID(FlatBufferBuilder builder, int hitSFXID) { builder.AddInt(9, hitSFXID, 0); }
  public static void AddHurtType(FlatBufferBuilder builder, int hurtType) { builder.AddInt(10, hurtType, 0); }
  public static void AddHurtTime(FlatBufferBuilder builder, float hurtTime) { builder.AddFloat(11, hurtTime, 0); }
  public static void AddHurtPause(FlatBufferBuilder builder, int hurtPause) { builder.AddInt(12, hurtPause, 0); }
  public static void AddHurtPauseTime(FlatBufferBuilder builder, float hurtPauseTime) { builder.AddFloat(13, hurtPauseTime, 0); }
  public static void AddForcex(FlatBufferBuilder builder, float forcex) { builder.AddFloat(14, forcex, 0); }
  public static void AddForcey(FlatBufferBuilder builder, float forcey) { builder.AddFloat(15, forcey, 0); }
  public static void AddDscription(FlatBufferBuilder builder, StringOffset dscriptionOffset) { builder.AddOffset(16, dscriptionOffset.Value, 0); }
  public static void AddCaracterAsset(FlatBufferBuilder builder, StringOffset caracterAssetOffset) { builder.AddOffset(17, caracterAssetOffset.Value, 0); }
  public static void AddFps(FlatBufferBuilder builder, int fps) { builder.AddInt(18, fps, 60); }
  public static void AddAnimationName(FlatBufferBuilder builder, StringOffset animationNameOffset) { builder.AddOffset(19, animationNameOffset.Value, 0); }
  public static void AddMoveName(FlatBufferBuilder builder, StringOffset moveNameOffset) { builder.AddOffset(20, moveNameOffset.Value, 0); }
  public static void AddWapMode(FlatBufferBuilder builder, sbyte wapMode) { builder.AddSbyte(21, wapMode, 0); }
  public static void AddIterpolationSpeed(FlatBufferBuilder builder, float iterpolationSpeed) { builder.AddFloat(22, iterpolationSpeed, 0); }
  public static void AddAnimationSpeed(FlatBufferBuilder builder, float animationSpeed) { builder.AddFloat(23, animationSpeed, 1); }
  public static void AddTotalFrames(FlatBufferBuilder builder, int totalFrames) { builder.AddInt(24, totalFrames, 15); }
  public static void AddStartUpFrames(FlatBufferBuilder builder, int startUpFrames) { builder.AddInt(25, startUpFrames, 5); }
  public static void AddActiveFrames(FlatBufferBuilder builder, int activeFrames) { builder.AddInt(26, activeFrames, 5); }
  public static void AddRcoveryFrames(FlatBufferBuilder builder, int rcoveryFrames) { builder.AddInt(27, rcoveryFrames, 5); }
  public static void AddUeSpellBar(FlatBufferBuilder builder, bool ueSpellBar) { builder.AddBool(28, ueSpellBar, false); }
  public static void AddSellBarTime(FlatBufferBuilder builder, float sellBarTime) { builder.AddFloat(29, sellBarTime, 0); }
  public static void AddCmboStartFrame(FlatBufferBuilder builder, int cmboStartFrame) { builder.AddInt(30, cmboStartFrame, 0); }
  public static void AddCmboSkillID(FlatBufferBuilder builder, int cmboSkillID) { builder.AddInt(31, cmboSkillID, 0); }
  public static void AddSkilltime(FlatBufferBuilder builder, float skilltime) { builder.AddFloat(32, skilltime, 0); }
  public static void AddCameraRestore(FlatBufferBuilder builder, bool cameraRestore) { builder.AddBool(33, cameraRestore, false); }
  public static void AddCameraRestoreTime(FlatBufferBuilder builder, float cameraRestoreTime) { builder.AddFloat(34, cameraRestoreTime, 0); }
  public static void AddGrabPosx(FlatBufferBuilder builder, float grabPosx) { builder.AddFloat(35, grabPosx, 0); }
  public static void AddGrabPosy(FlatBufferBuilder builder, float grabPosy) { builder.AddFloat(36, grabPosy, 0); }
  public static void AddGrabEndForceX(FlatBufferBuilder builder, float grabEndForceX) { builder.AddFloat(37, grabEndForceX, 0); }
  public static void AddGrabEndForceY(FlatBufferBuilder builder, float grabEndForceY) { builder.AddFloat(38, grabEndForceY, 0); }
  public static void AddGrabTime(FlatBufferBuilder builder, float grabTime) { builder.AddFloat(39, grabTime, 0); }
  public static void AddGrabEndEffectType(FlatBufferBuilder builder, int grabEndEffectType) { builder.AddInt(40, grabEndEffectType, 0); }
  public static void AddGrabAction(FlatBufferBuilder builder, int grabAction) { builder.AddInt(41, grabAction, 0); }
  public static void AddGrabNum(FlatBufferBuilder builder, int grabNum) { builder.AddInt(42, grabNum, 0); }
  public static void AddGrabMoveSpeed(FlatBufferBuilder builder, float grabMoveSpeed) { builder.AddFloat(43, grabMoveSpeed, 0); }
  public static void AddGrabSupportQuickPressDismis(FlatBufferBuilder builder, bool grabSupportQuickPressDismis) { builder.AddBool(44, grabSupportQuickPressDismis, false); }
  public static void AddIsCharge(FlatBufferBuilder builder, bool isCharge) { builder.AddBool(45, isCharge, false); }
  public static void AddChargeConfig(FlatBufferBuilder builder, Offset<ChargeConfig> chargeConfigOffset) { builder.AddOffset(46, chargeConfigOffset.Value, 0); }
  public static void AddIsSpeicalOperate(FlatBufferBuilder builder, bool isSpeicalOperate) { builder.AddBool(47, isSpeicalOperate, false); }
  public static void AddOperationConfig(FlatBufferBuilder builder, Offset<OperationConfig> operationConfigOffset) { builder.AddOffset(48, operationConfigOffset.Value, 0); }
  public static void AddSkillJoystickConfig(FlatBufferBuilder builder, Offset<SkillJoystickConfig> skillJoystickConfigOffset) { builder.AddOffset(49, skillJoystickConfigOffset.Value, 0); }
  public static void AddSkillEvents(FlatBufferBuilder builder, VectorOffset skillEventsOffset) { builder.AddOffset(50, skillEventsOffset.Value, 0); }
  public static VectorOffset CreateSkillEventsVector(FlatBufferBuilder builder, Offset<SkillEvent>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSkillEventsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddTriggerType(FlatBufferBuilder builder, sbyte triggerType) { builder.AddSbyte(51, triggerType, 0); }
  public static void AddHurtBlocks(FlatBufferBuilder builder, VectorOffset HurtBlocksOffset) { builder.AddOffset(52, HurtBlocksOffset.Value, 0); }
  public static VectorOffset CreateHurtBlocksVector(FlatBufferBuilder builder, Offset<HurtDecisionBox>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartHurtBlocksVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddDefenceBlocks(FlatBufferBuilder builder, VectorOffset DefenceBlocksOffset) { builder.AddOffset(53, DefenceBlocksOffset.Value, 0); }
  public static VectorOffset CreateDefenceBlocksVector(FlatBufferBuilder builder, Offset<DefenceDecisionBox>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartDefenceBlocksVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddAttachFrames(FlatBufferBuilder builder, VectorOffset attachFramesOffset) { builder.AddOffset(54, attachFramesOffset.Value, 0); }
  public static VectorOffset CreateAttachFramesVector(FlatBufferBuilder builder, Offset<EntityAttachFrames>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartAttachFramesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEffectFrames(FlatBufferBuilder builder, VectorOffset effectFramesOffset) { builder.AddOffset(55, effectFramesOffset.Value, 0); }
  public static VectorOffset CreateEffectFramesVector(FlatBufferBuilder builder, Offset<EffectsFrames>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEffectFramesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddEntityFrames(FlatBufferBuilder builder, VectorOffset entityFramesOffset) { builder.AddOffset(56, entityFramesOffset.Value, 0); }
  public static VectorOffset CreateEntityFramesVector(FlatBufferBuilder builder, Offset<EntityFrames>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartEntityFramesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFrameTags(FlatBufferBuilder builder, VectorOffset frameTagsOffset) { builder.AddOffset(57, frameTagsOffset.Value, 0); }
  public static VectorOffset CreateFrameTagsVector(FlatBufferBuilder builder, Offset<DSkillFrameTag>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFrameTagsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFrameGrap(FlatBufferBuilder builder, VectorOffset frameGrapOffset) { builder.AddOffset(58, frameGrapOffset.Value, 0); }
  public static VectorOffset CreateFrameGrapVector(FlatBufferBuilder builder, Offset<DSkillFrameGrap>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFrameGrapVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddStateop(FlatBufferBuilder builder, VectorOffset stateopOffset) { builder.AddOffset(59, stateopOffset.Value, 0); }
  public static VectorOffset CreateStateopVector(FlatBufferBuilder builder, Offset<DSkillFrameStateOp>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartStateopVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddProperModify(FlatBufferBuilder builder, VectorOffset properModifyOffset) { builder.AddOffset(60, properModifyOffset.Value, 0); }
  public static VectorOffset CreateProperModifyVector(FlatBufferBuilder builder, Offset<DSkillPropertyModify>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartProperModifyVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddShocks(FlatBufferBuilder builder, VectorOffset shocksOffset) { builder.AddOffset(61, shocksOffset.Value, 0); }
  public static VectorOffset CreateShocksVector(FlatBufferBuilder builder, Offset<DSkillFrameEventSceneShock>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartShocksVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddSfx(FlatBufferBuilder builder, VectorOffset sfxOffset) { builder.AddOffset(62, sfxOffset.Value, 0); }
  public static VectorOffset CreateSfxVector(FlatBufferBuilder builder, Offset<DSkillSfx>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartSfxVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddFrameEffects(FlatBufferBuilder builder, VectorOffset frameEffectsOffset) { builder.AddOffset(63, frameEffectsOffset.Value, 0); }
  public static VectorOffset CreateFrameEffectsVector(FlatBufferBuilder builder, Offset<DSkillFrameEffect>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartFrameEffectsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddCameraMoves(FlatBufferBuilder builder, VectorOffset cameraMovesOffset) { builder.AddOffset(64, cameraMovesOffset.Value, 0); }
  public static VectorOffset CreateCameraMovesVector(FlatBufferBuilder builder, Offset<DSkillCameraMove>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCameraMovesVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddWalkControl(FlatBufferBuilder builder, VectorOffset walkControlOffset) { builder.AddOffset(65, walkControlOffset.Value, 0); }
  public static VectorOffset CreateWalkControlVector(FlatBufferBuilder builder, Offset<DSkillWalkControl>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartWalkControlVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddActions(FlatBufferBuilder builder, VectorOffset actionsOffset) { builder.AddOffset(66, actionsOffset.Value, 0); }
  public static VectorOffset CreateActionsVector(FlatBufferBuilder builder, Offset<DActionData>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartActionsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<FBSkillData> EndFBSkillData(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FBSkillData>(o);
  }
};

public sealed class FBSkillDataTable : Table {
  public static FBSkillDataTable GetRootAsFBSkillDataTable(ByteBuffer _bb) { return GetRootAsFBSkillDataTable(_bb, new FBSkillDataTable()); }
  public static FBSkillDataTable GetRootAsFBSkillDataTable(ByteBuffer _bb, FBSkillDataTable obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public FBSkillDataTable __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public string Path { get { int o = __offset(4); return o != 0 ? __string(o + bb_pos) : null; } }
  public string Type { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }
  public bool IsCommon { get { int o = __offset(8); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public FBSkillData Data { get { return GetData(new FBSkillData()); } }
  public FBSkillData GetData(FBSkillData obj) { int o = __offset(10); return o != 0 ? obj.__init(__indirect(o + bb_pos), bb) : null; }

  public static Offset<FBSkillDataTable> CreateFBSkillDataTable(FlatBufferBuilder builder,
      StringOffset path = default(StringOffset),
      StringOffset type = default(StringOffset),
      bool isCommon = false,
      Offset<FBSkillData> data = default(Offset<FBSkillData>)) {
    builder.StartObject(4);
    FBSkillDataTable.AddData(builder, data);
    FBSkillDataTable.AddType(builder, type);
    FBSkillDataTable.AddPath(builder, path);
    FBSkillDataTable.AddIsCommon(builder, isCommon);
    return FBSkillDataTable.EndFBSkillDataTable(builder);
  }

  public static void StartFBSkillDataTable(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddPath(FlatBufferBuilder builder, StringOffset pathOffset) { builder.AddOffset(0, pathOffset.Value, 0); }
  public static void AddType(FlatBufferBuilder builder, StringOffset typeOffset) { builder.AddOffset(1, typeOffset.Value, 0); }
  public static void AddIsCommon(FlatBufferBuilder builder, bool isCommon) { builder.AddBool(2, isCommon, false); }
  public static void AddData(FlatBufferBuilder builder, Offset<FBSkillData> dataOffset) { builder.AddOffset(3, dataOffset.Value, 0); }
  public static Offset<FBSkillDataTable> EndFBSkillDataTable(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FBSkillDataTable>(o);
  }
};

public sealed class FBSkillDataCollection : Table {
  public static FBSkillDataCollection GetRootAsFBSkillDataCollection(ByteBuffer _bb) { return GetRootAsFBSkillDataCollection(_bb, new FBSkillDataCollection()); }
  public static FBSkillDataCollection GetRootAsFBSkillDataCollection(ByteBuffer _bb, FBSkillDataCollection obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public static bool FBSkillDataCollectionBufferHasIdentifier(ByteBuffer _bb) { return __has_identifier(_bb, "SKIL"); }
  public FBSkillDataCollection __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public FBSkillDataTable GetCollection(int j) { return GetCollection(new FBSkillDataTable(), j); }
  public FBSkillDataTable GetCollection(FBSkillDataTable obj, int j) { int o = __offset(4); return o != 0 ? obj.__init(__indirect(__vector(o) + j * 4), bb) : null; }
  public int CollectionLength { get { int o = __offset(4); return o != 0 ? __vector_len(o) : 0; } }

  public static Offset<FBSkillDataCollection> CreateFBSkillDataCollection(FlatBufferBuilder builder,
      VectorOffset collection = default(VectorOffset)) {
    builder.StartObject(1);
    FBSkillDataCollection.AddCollection(builder, collection);
    return FBSkillDataCollection.EndFBSkillDataCollection(builder);
  }

  public static void StartFBSkillDataCollection(FlatBufferBuilder builder) { builder.StartObject(1); }
  public static void AddCollection(FlatBufferBuilder builder, VectorOffset collectionOffset) { builder.AddOffset(0, collectionOffset.Value, 0); }
  public static VectorOffset CreateCollectionVector(FlatBufferBuilder builder, Offset<FBSkillDataTable>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static void StartCollectionVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<FBSkillDataCollection> EndFBSkillDataCollection(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<FBSkillDataCollection>(o);
  }
  public static void FinishFBSkillDataCollectionBuffer(FlatBufferBuilder builder, Offset<FBSkillDataCollection> offset) { builder.Finish(offset.Value, "SKIL"); }
};


}
