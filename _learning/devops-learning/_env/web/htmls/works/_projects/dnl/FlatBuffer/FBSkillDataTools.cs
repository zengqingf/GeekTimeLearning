using System;
using FlatBuffers;
using FBSkillData;
using SkillData = FBSkillData.FBSkillData;
using System.Collections.Generic;

public class FBSkillDataTools
{

    private delegate void AddString(FlatBufferBuilder builder, StringOffset StringOffset);

    private void FlatBufferAddString(AddString method, FlatBufferBuilder builder, string value)
    {
        if (string.IsNullOrEmpty(value) == false && method != null)
        {
            method(builder, builder.CreateString(value));
        }

    }

    private StringOffset CreateString(string value)
    {
        if (string.IsNullOrEmpty(value) == false)
        {
            return currentBuilder.CreateString(value);
        }

        return default(StringOffset);
    }
    void CreateSValue(SUnion value, FlatBufferBuilder builder, ref int offset, ref int type)
    {

    }

    VectorOffset CreateSkillPhasesVector(int[] data)
    {
        if (data != null && data.Length > 0)
        {
            return SkillData.CreateSkillPhasesVector(currentBuilder, data);
        }

        return default(VectorOffset);
    }

    T Call<T>(Func<T> func)
    {
        return func();
    }

    FlatBufferBuilder currentBuilder;

    Offset<SkillData> CreateFBSkillData(
        FlatBufferBuilder builder,
        DSkillData editorData
    )
    {
        return SkillData.CreateFBSkillData(
            builder,
            CreateString(editorData._name),
            editorData.skillID,
            editorData.skillPriority,
            CreateSkillPhasesVector(editorData.skillPhases),
            editorData.isLoop,
            editorData.notLoopLastFrame,
            CreateString(editorData.hitEffect),
            CreateString(editorData.goHitEffectAsset.m_AssetPath),
            CreateString(editorData.goSFXAsset.m_AssetPath),
            editorData.hitSFXID,
            editorData.hurtType,
            editorData.hurtTime,
            editorData.hurtPause,
            editorData.hurtPauseTime,
            editorData.forcex,
            editorData.forcey,
            CreateString(editorData.description),
            CreateString(editorData.characterAsset.m_AssetPath),
            editorData.fps,
            CreateString(editorData.animationName),
            CreateString(editorData.moveName),
            (sbyte)editorData.wrapMode,
            editorData.interpolationSpeed,
            editorData.animationSpeed,
            editorData.totalFrames,
            editorData.startUpFrames,
            editorData.activeFrames,
            editorData.recoveryFrames,
            editorData.useSpellBar,
            editorData.spellBarTime,
            editorData.comboStartFrame,
            editorData.comboSkillID,
            editorData.skilltime,
            editorData.cameraRestore,
            editorData.cameraRestoreTime,
            editorData.grabPosx,
            editorData.grabPosy,
            editorData.grabEndForceX,
            editorData.grabEndForceY,
            editorData.grabTime,
            editorData.grabEndEffectType,
            editorData.grabAction,
            editorData.grabNum,
            editorData.grabMoveSpeed,
            editorData.grabSupportQuickPressDismis,
            editorData.isCharge,
            Call(() =>
            {
                var chargeconfig = editorData.chargeConfig;
                return FBSkillData.ChargeConfig.CreateChargeConfig(builder,
                chargeconfig.repeatPhase,
                chargeconfig.changePhase,
                chargeconfig.switchPhaseID,
                chargeconfig.chargeDuration,
                chargeconfig.chargeMinDuration,
                CreateString(chargeconfig.effect),
                CreateString(chargeconfig.locator)
                );
            }),
            editorData.isSpeicalOperate,
            Call(() =>
            {
                var operationcfg = editorData.operationConfig;
                return FBSkillData.OperationConfig.CreateOperationConfig(
                    builder, operationcfg.changePhase,
                    FBSkillData.OperationConfig.CreateChangeSkillIDsVector(builder, operationcfg.changeSkillIDs)
                );
            }),
            Call(() =>
            {
                var skilljoycfg = editorData.skillJoystickConfig;
                var namestring = CreateString(skilljoycfg.effectName);
                FBSkillData.SkillJoystickConfig.StartSkillJoystickConfig(builder);
                FBSkillData.SkillJoystickConfig.AddMode(builder, (sbyte)skilljoycfg.mode);
                FBSkillData.SkillJoystickConfig.AddEffectName(builder, namestring);

                FBSkillData.SkillJoystickConfig.AddEffectMoveSpeed(
                    builder, FBSkillData.Vector3.CreateVector3(builder, skilljoycfg.effectMoveSpeed.x,
                    skilljoycfg.effectMoveSpeed.y,
                    skilljoycfg.effectMoveSpeed.z
                    ));

                FBSkillData.SkillJoystickConfig.AddEffectMoveRange(
                    builder, FBSkillData.Vector3.CreateVector3(builder, skilljoycfg.effectMoveRange.x,
                    skilljoycfg.effectMoveRange.y,
                    skilljoycfg.effectMoveRange.z
                    ));


                return FBSkillData.SkillJoystickConfig.EndSkillJoystickConfig(builder);

            }),
            Call(() =>
            {
                if (editorData.skillEvents != null && editorData.skillEvents.Length > 0)
                {
                    List<Offset<FBSkillData.SkillEvent>> list = new List<Offset<FBSkillData.SkillEvent>>();
                    for (int i = 0; i < editorData.skillEvents.Length; ++i)
                    {
                        var cur = editorData.skillEvents[i];
                        list.Add(
                            FBSkillData.SkillEvent.CreateSkillEvent(builder, (sbyte)cur.eventType, (sbyte)cur.eventAction,
                            CreateString(cur.paramter))
                        );
                    }

                    return SkillData.CreateSkillEventsVector(builder, list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            (sbyte)editorData.triggerType,
            Call(() =>
            {
                if (editorData.HurtBlocks != null && editorData.HurtBlocks.Length > 0)
                {
                    List<Offset<FBSkillData.HurtDecisionBox>> list = new List<Offset<FBSkillData.HurtDecisionBox>>();
                    for (int i = 0; i < editorData.HurtBlocks.Length; ++i)
                    {
                        var cur = editorData.HurtBlocks[i];

                        VectorOffset boxs = default(VectorOffset);

                        if (cur.boxs != null && cur.boxs.Length > 0)
                        {
                            List<Offset<FBSkillData.ShapeBox>> list2 = new List<Offset<FBSkillData.ShapeBox>>();

                            for (int j = 0; j < cur.boxs.Length; ++j)
                            {
                                var t = cur.boxs[j];

                                FBSkillData.ShapeBox.StartShapeBox(builder);
                                FBSkillData.ShapeBox.AddCenter(builder,FBSkillData.Vector2.CreateVector2(builder,t.center.x, t.center.y));
                                FBSkillData.ShapeBox.AddSize(builder,FBSkillData.Vector2.CreateVector2(builder,t.size.x, t.size.y));
                                list2.Add(
                                    FBSkillData.ShapeBox.EndShapeBox(builder)
                                );

                            }
                            
                            boxs = FBSkillData.HurtDecisionBox.CreateBoxsVector(builder,list2.ToArray());
                        }
                        list.Add(
                            FBSkillData.HurtDecisionBox.CreateHurtDecisionBox(
                                builder, boxs, cur.hasHit, cur.blockToggle, cur.zDim, cur.damage, cur.hurtID
                            )
                        );
                    }

                    return SkillData.CreateHurtBlocksVector(builder, list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }

            }
            ),
            Call(() =>
            {
                if (editorData.DefenceBlocks != null && editorData.DefenceBlocks.Length > 0)
                {
                    List<Offset<FBSkillData.DefenceDecisionBox>> list = new List<Offset<FBSkillData.DefenceDecisionBox>>();
                    for (int i = 0; i < editorData.DefenceBlocks.Length; ++i)
                    {
                        var cur = editorData.DefenceBlocks[i];

                        VectorOffset boxs = default(VectorOffset);

                        if (cur.boxs != null && cur.boxs.Length > 0)
                        {
                            List<Offset<FBSkillData.ShapeBox>> list2 = new List<Offset<FBSkillData.ShapeBox>>();

                            for (int j = 0; j < cur.boxs.Length; ++j)
                            {
                                var t = cur.boxs[j];
                                FBSkillData.ShapeBox.StartShapeBox(builder);
                                FBSkillData.ShapeBox.AddCenter(builder,FBSkillData.Vector2.CreateVector2(builder,t.center.x, t.center.y));
                                FBSkillData.ShapeBox.AddSize(builder,FBSkillData.Vector2.CreateVector2(builder,t.size.x, t.size.y));
                                list2.Add(
                                    FBSkillData.ShapeBox.EndShapeBox(builder)
                                );
                            }

                             FBSkillData.DefenceDecisionBox.StartBoxsVector(builder, cur.boxs.Length);
                            for (int j = cur.boxs.Length - 1; j >= 0 ; j--)
                            {
                                builder.AddOffset(list2[j].Value);
                            }

                            boxs =  FBSkillData.DefenceDecisionBox.CreateBoxsVector(builder,list2.ToArray());
                        }
                        list.Add(
                            FBSkillData.DefenceDecisionBox.CreateDefenceDecisionBox(
                                builder, boxs, cur.hasHit, cur.blockToggle, cur.type
                            )
                        );
                    }

                    return
                   SkillData.CreateDefenceBlocksVector(builder, list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }

            }
            ),
            Call(() =>
            {
                if (editorData.attachFrames != null && editorData.attachFrames.Length > 0)
                {
                    List<Offset<FBSkillData.EntityAttachFrames>> list = new
                    List<Offset<FBSkillData.EntityAttachFrames>>();

                    for (int i = 0; i < editorData.attachFrames.Length; ++i)
                    {
                        var t = editorData.attachFrames[i];
                        var name = CreateString(t.name);
                        var attachname = CreateString(t.attachName);
                        var entityPath = CreateString(t.entityAsset.m_AssetPath);
                        var tran = default(Offset<FBSkillData.TransformParam>);
                        if(t.trans != null)
                        tran = FBSkillData.TransformParam.CreateTransformParam(
                            builder, t.trans.localPosition.x, t.trans.localPosition.y, t.trans.localPosition.z, t.trans.localRotation.x, t.trans.localRotation.y,
                            t.trans.localRotation.z, t.trans.localRotation.w, t.trans.localScale.x, t.trans.localScale.y, t.trans.localScale.z);

                        var aframe = default(VectorOffset);

                        if (t.animations != null && t.animations.Length > 0)
                        {
                            List<Offset<FBSkillData.AnimationFrames>> list2 =
                                new List<Offset<FBSkillData.AnimationFrames>>();

                            for (int j = 0; j < t.animations.Length; ++j)
                            {
                                var cur = t.animations[j];
                                list2.Add(FBSkillData.AnimationFrames.CreateAnimationFrames(
                                    builder, cur.start,
                                    builder.CreateString(cur.anim), cur.blend, (sbyte)cur.mode, cur.speed
                                ));
                            }
                            aframe = FBSkillData.EntityAttachFrames.CreateAnimationsVector(builder, list2.ToArray());
                        }

                        FBSkillData.EntityAttachFrames.StartEntityAttachFrames(builder);

                        FBSkillData.EntityAttachFrames.AddName(builder, name);
                        FBSkillData.EntityAttachFrames.AddResID(builder, t.resID);
                        FBSkillData.EntityAttachFrames.AddStart(builder, t.start);
                        FBSkillData.EntityAttachFrames.AddEnd(builder, t.end);
                        FBSkillData.EntityAttachFrames.AddAttachName(builder, attachname);
                        FBSkillData.EntityAttachFrames.AddEntityAsset(builder, entityPath);
                        FBSkillData.EntityAttachFrames.AddTrans(builder, tran);
                        FBSkillData.EntityAttachFrames.AddAnimations(builder, aframe);

                        list.Add(FBSkillData.EntityAttachFrames.EndEntityAttachFrames(builder));
                    }

                    return SkillData.CreateAttachFramesVector(builder, list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.effectFrames != null && editorData.effectFrames.Length > 0)
                {
                    //FBSkillData.EffectsFrames
                    List<Offset<FBSkillData.EffectsFrames>> list = new
                   List<Offset<FBSkillData.EffectsFrames>>();

                    for (int i = 0; i < editorData.effectFrames.Length; ++i)
                    {
                        var cur = editorData.effectFrames[i];
                        var name = CreateString(cur.name);
                        var attachname = CreateString(cur.attachname);
                        var effectpath = CreateString(cur.effectAsset.m_AssetPath);

                        FBSkillData.EffectsFrames.StartEffectsFrames(builder);
                        FBSkillData.EffectsFrames.AddName(builder,name);
                        FBSkillData.EffectsFrames.AddEffectResID(builder, cur.effectResID);
                        FBSkillData.EffectsFrames.AddStartFrames(builder, cur.startFrames);
                        FBSkillData.EffectsFrames.AddEndFrames(builder, cur.endFrames);
                        FBSkillData.EffectsFrames.AddAttachname(builder, attachname);
                        FBSkillData.EffectsFrames.AddPlaytype(builder, (sbyte)cur.playtype);
                        FBSkillData.EffectsFrames.AddTimetype(builder, (sbyte)cur.timetype);
                        FBSkillData.EffectsFrames.AddTime(builder, cur.time);
                        FBSkillData.EffectsFrames.AddEffectAsset(builder,effectpath);
                        FBSkillData.EffectsFrames.AddAttachPoint(builder, (sbyte)cur.attachPoint);
                        FBSkillData.EffectsFrames.AddLocalPosition(builder,
                        FBSkillData.Vector3.CreateVector3(builder, cur.localPosition.x, cur.localPosition.y, cur.localPosition.z));
                        FBSkillData.EffectsFrames.AddLocalRotation(builder,
                        FBSkillData.Quaternion.CreateQuaternion(builder, cur.localRotation.x, cur.localRotation.y, cur.localRotation.z, cur.localRotation.w));
                        FBSkillData.EffectsFrames.AddLocalScale(builder,
                        FBSkillData.Vector3.CreateVector3(builder, cur.localScale.x, cur.localScale.y, cur.localScale.z));
                        FBSkillData.EffectsFrames.AddEffecttype(builder, cur.effecttype);
                        FBSkillData.EffectsFrames.AddLoop(builder, cur.loop);
                        FBSkillData.EffectsFrames.AddLoopLoop(builder, cur.loopLoop);
                        list.Add(FBSkillData.EffectsFrames.EndEffectsFrames(builder));
                    }
                    return SkillData.CreateEffectFramesVector(builder, list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.entityFrames != null && editorData.entityFrames.Length > 0)
                {
                    List<Offset<FBSkillData.EntityFrames>> list = new
                            List<Offset<FBSkillData.EntityFrames>>();

                    for (int i = 0; i < editorData.entityFrames.Length; ++i)
                    {
                        var cur = editorData.entityFrames[i];
                        var name = CreateString(cur.name);
                        var entitypath = CreateString(cur.entityAsset.m_AssetPath);
                    
                        FBSkillData.EntityFrames.StartEntityFrames(builder);
                        FBSkillData.EntityFrames.AddName(builder,name);
                        FBSkillData.EntityFrames.AddResID(builder, cur.resID);
                        FBSkillData.EntityFrames.AddType(builder, (sbyte)cur.type);
                        FBSkillData.EntityFrames.AddStartFrames(builder, cur.startFrames);
                        FBSkillData.EntityFrames.AddEntityAsset(builder, entitypath);
                        FBSkillData.EntityFrames.AddGravity(builder,
                        FBSkillData.Vector2.CreateVector2(builder, cur.gravity.x, cur.gravity.y));
                        FBSkillData.EntityFrames.AddSpeed(builder, cur.speed);
                        FBSkillData.EntityFrames.AddAngle(builder, cur.angle);
                        FBSkillData.EntityFrames.AddIsAngleWithEffect(builder, cur.isAngleWithEffect);
                        FBSkillData.EntityFrames.AddEmitposition(builder,
                        FBSkillData.Vector2.CreateVector2(builder, cur.emitposition.x, cur.emitposition.y));
                        FBSkillData.EntityFrames.AddEmitPositionZ(builder, cur.emitPositionZ);
                        FBSkillData.EntityFrames.AddAxisType(builder, (sbyte)cur.axisType);
                        FBSkillData.EntityFrames.AddShockTime(builder, cur.shockTime);
                        FBSkillData.EntityFrames.AddShockSpeed(builder, cur.shockSpeed);
                        FBSkillData.EntityFrames.AddShockRangeX(builder, cur.shockRangeX);
                        FBSkillData.EntityFrames.AddShockRangeY(builder, cur.shockRangeY);
                        FBSkillData.EntityFrames.AddIsRotation(builder, cur.isRotation);
                        FBSkillData.EntityFrames.AddRotateSpeed(builder, cur.rotateSpeed);
                        FBSkillData.EntityFrames.AddMoveSpeed(builder, cur.moveSpeed);
                        FBSkillData.EntityFrames.AddSceneShock(builder,
                        FBSkillData.ShockInfo.CreateShockInfo(builder, cur.sceneShock.shockTime, cur.sceneShock.shockSpeed,
                        cur.sceneShock.shockRangeX, cur.sceneShock.shockRangeY));
                        FBSkillData.EntityFrames.AddHitFallUP(builder, cur.hitFallUP);
                        FBSkillData.EntityFrames.AddForceY(builder, cur.forceY);
                        FBSkillData.EntityFrames.AddHurtID(builder, cur.hurtID);
                        FBSkillData.EntityFrames.AddLifeTime(builder, cur.lifeTime);
                        FBSkillData.EntityFrames.AddHitThrough(builder, cur.hitThrough);
                        FBSkillData.EntityFrames.AddHitCount(builder, cur.hitCount);
                        FBSkillData.EntityFrames.AddDistance(builder, cur.distance);
                        FBSkillData.EntityFrames.AddHitGroundClick(builder, cur.hitGroundClick);
                        FBSkillData.EntityFrames.AddDelayDead(builder, cur.delayDead);
                        FBSkillData.EntityFrames.AddOffsetType(builder, (sbyte)cur.offsetType);
                        FBSkillData.EntityFrames.AddTargetChooseType(builder, (sbyte)cur.targetChooseType);
                        FBSkillData.EntityFrames.AddRange(builder, FBSkillData.Vector2.CreateVector2(builder, cur.range.x, cur.range.y));
                        FBSkillData.EntityFrames.AddParaSpeed(builder, cur.paraSpeed);
                        FBSkillData.EntityFrames.AddParaGravity(builder, cur.paraGravity);
                        FBSkillData.EntityFrames.AddUseRandomLaunch(builder, cur.useRandomLaunch);
                        FBSkillData.EntityFrames.AddRandomLaunchInfo(builder,
                        FBSkillData.RandomLaunchInfo.CreateRandomLaunchInfo(
                            builder, cur.randomLaunchInfo.num, cur.randomLaunchInfo.isNumRand,
                            cur.randomLaunchInfo.numRandRange.x, cur.randomLaunchInfo.numRandRange.y, cur.randomLaunchInfo.interval, cur.randomLaunchInfo.rangeRadius
                        ));
                        list.Add(FBSkillData.EntityFrames.EndEntityFrames(builder));
                    }
                    return SkillData.CreateEntityFramesVector(builder, list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.frameTags != null && editorData.frameTags.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillFrameTag>> list = new
                    List<Offset<FBSkillData.DSkillFrameTag>>();

                    for (int i = 0; i < editorData.frameTags.Length; ++i)
                    {
                        var cur = editorData.frameTags[i];

                        list.Add(FBSkillData.DSkillFrameTag.CreateDSkillFrameTag(
                            builder, default(StringOffset), cur.startframe, cur.length, (sbyte)cur.tag)
                        );
                    }

                    return SkillData.CreateFrameTagsVector(builder, list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.frameGrap != null && editorData.frameGrap.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillFrameGrap>> list = new
                    List<Offset<FBSkillData.DSkillFrameGrap>>();

                    for (int i = 0; i < editorData.frameGrap.Length; ++i)
                    {
                        var cur = editorData.frameGrap[i];

                        list.Add(FBSkillData.DSkillFrameGrap.CreateDSkillFrameGrap(
                            builder, default(StringOffset), cur.startframe, cur.length, (sbyte)cur.op));
                    }

                    return
                   SkillData.CreateFrameGrapVector(builder,
                   list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.stateop != null && editorData.stateop.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillFrameStateOp>> list = new
                    List<Offset<FBSkillData.DSkillFrameStateOp>>();


                    for (int i = 0; i < editorData.stateop.Length; ++i)
                    {
                        var cur = editorData.stateop[i];

                        list.Add(FBSkillData.DSkillFrameStateOp.CreateDSkillFrameStateOp(
                            builder, default(StringOffset), cur.startframe, cur.length, (sbyte)cur.op, (sbyte)cur.state, cur.idata1, cur.idata2, cur.fdata1, cur.fdata2, (sbyte)cur.statetag)
                            );
                    }

                    return
                    SkillData.CreateStateopVector(builder,
                    list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.properModify != null && editorData.properModify.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillPropertyModify>> list = new
                    List<Offset<FBSkillData.DSkillPropertyModify>>();


                    for (int i = 0; i < editorData.properModify.Length; ++i)
                    {
                        var cur = editorData.properModify[i];

                        list.Add(FBSkillData.DSkillPropertyModify.CreateDSkillPropertyModify(
                            builder, default(StringOffset), cur.startframe, cur.length, (sbyte)cur.modifyfliter, cur.value, cur.movedValue, 0, 0, cur.jumpToTargetPos)
                            );
                    }

                    return SkillData.CreateProperModifyVector(builder,
                    list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.shocks != null && editorData.shocks.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillFrameEventSceneShock>> list = new
                    List<Offset<FBSkillData.DSkillFrameEventSceneShock>>();


                    for (int i = 0; i < editorData.shocks.Length; ++i)
                    {
                        var cur = editorData.shocks[i];

                        list.Add(FBSkillData.DSkillFrameEventSceneShock.CreateDSkillFrameEventSceneShock(
                            builder, default(StringOffset),
                            cur.startframe, cur.length, cur.time, cur.speed, cur.xrange, cur.yrange)
                            );
                    }

                    return
                    SkillData.CreateShocksVector(builder,
                    list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.sfx != null && editorData.sfx.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillSfx>> list = new
                    List<Offset<FBSkillData.DSkillSfx>>();


                    for (int i = 0; i < editorData.sfx.Length; ++i)
                    {
                        var cur = editorData.sfx[i];

                        list.Add(FBSkillData.DSkillSfx.CreateDSkillSfx(
                            builder, default(StringOffset),
                            cur.startframe, cur.length,
                            builder.CreateString(cur.soundClipAsset.m_AssetPath),
                            cur.loop, cur.soundID)
                            );
                    }

                    return
                    SkillData.CreateSfxVector(builder,
                    list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.frameEffects != null && editorData.frameEffects.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillFrameEffect>> list = new
                    List<Offset<FBSkillData.DSkillFrameEffect>>();


                    for (int i = 0; i < editorData.frameEffects.Length; ++i)
                    {
                        var cur = editorData.frameEffects[i];

                        list.Add(FBSkillData.DSkillFrameEffect.CreateDSkillFrameEffect(
                            builder, default(StringOffset), cur.startframe,
                            cur.length, cur.effectID, cur.buffTime,
                            cur.phaseDelete, cur.finishDelete,
                            cur.useBuffAni, cur.usePause, cur.pauseTime
                            )
                            );
                    }

                    return
                    SkillData.CreateFrameEffectsVector(builder,
                    list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ),
            Call(() =>
            {
                if (editorData.cameraMoves != null && editorData.cameraMoves.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillCameraMove>> list = new
                    List<Offset<FBSkillData.DSkillCameraMove>>();


                    for (int i = 0; i < editorData.cameraMoves.Length; ++i)
                    {
                        var cur = editorData.cameraMoves[i];

                        list.Add(FBSkillData.DSkillCameraMove.CreateDSkillCameraMove(
                            builder, default(StringOffset),
                            cur.startframe, cur.length, cur.offset, cur.duraction
                            )
                            );
                    }

                    return
                     SkillData.CreateCameraMovesVector(builder,
                     list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }

            }
            ),
            Call(() =>
            {
                if (editorData.walkControl != null && editorData.walkControl.Length > 0)
                {
                    List<Offset<FBSkillData.DSkillWalkControl>> list = new
                    List<Offset<FBSkillData.DSkillWalkControl>>();


                    for (int i = 0; i < editorData.walkControl.Length; ++i)
                    {
                        var cur = editorData.walkControl[i];

                        list.Add(FBSkillData.DSkillWalkControl.CreateDSkillWalkControl(
                            builder, default(StringOffset),
                            cur.startframe, cur.length, (sbyte)cur.walkMode, cur.walkSpeedPercent
                            )
                            );
                    }

                    return
                    SkillData.CreateWalkControlVector(builder,
                    list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }

            }
            ),
            Call(() =>
            {
                if (editorData.actions != null && editorData.actions.Length > 0)
                {
                    List<Offset<FBSkillData.DActionData>> list = new
                    List<Offset<FBSkillData.DActionData>>();


                    for (int i = 0; i < editorData.actions.Length; ++i)
                    {
                        var cur = editorData.actions[i];
                        FBSkillData.DActionData.StartDActionData(builder);
                        FBSkillData.DActionData.AddStartframe(builder, cur.startframe);
                        FBSkillData.DActionData.AddLength(builder, cur.length);
                        FBSkillData.DActionData.AddActionType(builder, (sbyte)cur.actionType);
                        FBSkillData.DActionData.AddDuration(builder, cur.duration);
                        FBSkillData.DActionData.AddDeltaScale(builder, cur.deltaScale);
                        FBSkillData.DActionData.AddDeltaPos(builder,
                        FBSkillData.Vector3.CreateVector3(builder, cur.deltaPos.x, cur.deltaPos.y, cur.deltaPos.z));
                        list.Add(FBSkillData.DActionData.EndDActionData(builder));

                    }

                    return
                    SkillData.CreateActionsVector(builder,
                    list.ToArray());
                }
                else
                {
                    return default(VectorOffset);
                }
            }
            ));
    }

    Offset<FBSkillDataTable> CreateFBSkillDataTable(FlatBufferBuilder builder, DSkillDataItem item)
    {
        var     filename    = item.filepath;
        var     path        = CreateString(filename);
        bool    isCommon    = filename.Contains("common",System.StringComparison.OrdinalIgnoreCase);
		var     tokens      = filename.Split('/');
		var     type        = CreateString(tokens[0]);
        var     data        = CreateFBSkillData(builder, item.skillData);

       
        return FBSkillData.FBSkillDataTable.CreateFBSkillDataTable(builder,
             path,type,isCommon,data
         );
    }

 
    public struct DSkillDataItem
    {
        public DSkillDataItem(string path, DSkillData data)
        {
            _filepath = path;
            _skillData = data;
        }

        string _filepath;
        DSkillData _skillData;

        public string filepath
        {
            get { return _filepath; }
        }
        public DSkillData skillData
        {
            get { return _skillData; }
        }
    }
    public bool CreateFBSkillDataCollection(
        FlatBufferBuilder builder, List<DSkillDataItem> collection
    )
    {
        try
        {
            currentBuilder = builder;
            List<Offset<FBSkillDataTable>> tables = new List<Offset<FBSkillDataTable>>();

            for (int i = 0; i < collection.Count; ++i)
            {
                var cur = collection[i];
                tables.Add(
                    CreateFBSkillDataTable(builder, cur)
                );
            }

            var collectionvector = FBSkillDataCollection.CreateCollectionVector(builder, tables.ToArray());
            FBSkillDataCollection.StartFBSkillDataCollection(builder);
            FBSkillDataCollection.AddCollection(
                builder,
                collectionvector
            );
            var offset = FBSkillDataCollection.EndFBSkillDataCollection(builder);
            FBSkillDataCollection.FinishFBSkillDataCollectionBuffer(builder, offset);

        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("FBSKillDataCollection Error!!" + e.Message +  e.StackTrace);
            return false;
        }
        return true;
    }
}
