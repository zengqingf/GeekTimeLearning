using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using FDungeonData = FBDungeonData.DDungeonData;
using FSceneDataConnect = FBDungeonData.DSceneDataConnect;

using FSceneData = FBSceneData.DSceneData;

public class DungeonDataTest {

	[Test]
	public void DungeonDataTestSimplePasses() {
		// Use the Assert class to test conditions.
        
        string[] allIds = AssetDatabase.FindAssets("t:DDungeonData", new string[] {"Assets/Resources/Data"});

        for (int i = 0; i < allIds.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allIds[i]);
            string newPath = path + ".bytes";

            DDungeonData data = AssetDatabase.LoadAssetAtPath<DDungeonData>(path);

            byte[] newPathData = System.IO.File.ReadAllBytes(newPath);
        
            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(newPathData);
            FDungeonData fbdata = FDungeonData.GetRootAsDDungeonData(buffer);

            
            if (!_isSame(fbdata, data))
            {
                UnityEngine.Debug.LogErrorFormat("不亦说乎 {0}", path);
            }
        }
	}



    private bool _isSame(FDungeonData fbdata, DDungeonData unitydata)
    {
		if (fbdata.Name != unitydata.name)
        {
            UnityEngine.Debug.LogErrorFormat("name");
            return false;
        }

		if (fbdata.Height != unitydata.height)
        {
            UnityEngine.Debug.LogErrorFormat("height");
            return false;
        }


		if (fbdata.Weidth != unitydata.weidth)
        {
            UnityEngine.Debug.LogErrorFormat("weidth");
            return false;
        }

		if (fbdata.AreaconnectlistLength != unitydata.areaconnectlist.Length)
        {
            UnityEngine.Debug.LogErrorFormat("AreaconnectlistLength");
            return false;
        }

		for (int i = 0; i < fbdata.AreaconnectlistLength; ++i) 
        {
			if (!_isSameAreaConnect(fbdata.GetAreaconnectlist(i), unitydata.areaconnectlist[i]))
            {
                UnityEngine.Debug.LogErrorFormat("Areaconnectlist[i] {0}", i);
                return false;
            }
		}

        return true;
    }

    private bool _isSameAreaConnect(FSceneDataConnect con, DSceneDataConnect ucon)
    {
        if (con.Id != ucon.id)
        {
            UnityEngine.Debug.Log("id");
            return false;
        }

        if (con.Areaindex != ucon.areaindex)
        {
            UnityEngine.Debug.Log("areaid");
            return false;
        }

        if (con.Isboss != ucon.isboss)
        {
            UnityEngine.Debug.Log("isbooss");
            return false;
        }

        if (con.Ishell != ucon.ishell)
        {
            UnityEngine.Debug.Log("ishell");
            return false;
        }

        if (con.Isnothell != ucon.isnothell)
        {
            UnityEngine.Debug.Log("isnothell");
            return false;
        }

        if (con.Isstart != ucon.isstart)
        {
            UnityEngine.Debug.Log("isstart");
            return false;
        }

        if (con.Sceneareapath != ucon.sceneareapath)
        {
            UnityEngine.Debug.Log("sceneareapath");
            return false;
        }

        if (con.Positionx != ucon.positionx)
        {
            UnityEngine.Debug.Log("posx");
            return false;
        }

        if (con.Positiony != ucon.positiony)
        {
            UnityEngine.Debug.Log("posy");
            return false;
        }

        if (con.IsconnectLength != ucon.isconnect.Length)
        {
            UnityEngine.Debug.Log("isconnectd.len");
            return false;
        }

        for (int i = 0; i < ucon.isconnect.Length; ++i)
        {
            if (con.GetIsconnect(i) != ucon.isconnect[i])
            {
                UnityEngine.Debug.LogFormat("isconnectd[i] {0}", i);
                return false;
            }
        }

        return true;
    }


	[Test]
	public void TestDSceneData()
	{
		string[] allIds = AssetDatabase.FindAssets("t:DSceneData", new string[] {"Assets/Resources/Data"});

		for (int i = 0; i < allIds.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(allIds[i]);
			string newPath = path + ".bytes";

			DSceneData data = AssetDatabase.LoadAssetAtPath<DSceneData>(path);

			byte[] newPathData = System.IO.File.ReadAllBytes(newPath);

			FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(newPathData);
			FSceneData fbdata = FSceneData.GetRootAsDSceneData(buffer);


			if (!_isSameDSceneData(fbdata, data))
			{
				UnityEngine.Debug.LogErrorFormat("不亦说乎 {0}", path);
			}
            break;
		}
	}

	private bool _isSameDSceneData(FSceneData fbscene, DSceneData unityscene)
	{
		if (fbscene.Name != unityscene.name) 
        {
			UnityEngine.Debug.Log("name");
			return false;
		}

		if (fbscene.Prefabpath != unityscene._prefabpath) 
        {
			UnityEngine.Debug.Log("Prefabpath");
			return false;
		}

        if (fbscene.CameraLookHeight != unityscene._CameraLookHeight) 
        {
			UnityEngine.Debug.Log("CameraLookHeight");
			return false;
		}

        if (fbscene.CameraDistance != unityscene._CameraDistance) 
        {
			UnityEngine.Debug.Log("CameraDistance");
			return false;
		}
    
        if (fbscene.CameraAngle != unityscene._CameraAngle) 
        {
			UnityEngine.Debug.Log("CameraAngle");
			return false;
		}

        if (fbscene.CameraNearClip != unityscene._CameraNearClip) 
        {
			UnityEngine.Debug.Log("CameraNearClip");
			return false;
		}
    
        if (fbscene.CameraFarClip != unityscene._CameraFarClip) 
        {
			UnityEngine.Debug.Log("_CameraFarClip");
			return false;
		}

        if (fbscene.CameraSize != unityscene._CameraSize) 
        {
			UnityEngine.Debug.Log("_CameraSize");
			return false;
		}

        if (fbscene.CameraSize != unityscene._CameraSize) 
        {
			UnityEngine.Debug.Log("_CameraSize");
            return false;
        }
    
        if (fbscene.CameraSize != unityscene._CameraSize) 
        {
			UnityEngine.Debug.Log("_CameraSize");
            return false;
        }

        if (!_isSameVec2(fbscene.CameraZRange, unityscene._CameraZRange))
        {
			UnityEngine.Debug.Log("CameraZRange");
            return false;
        }

        if (!_isSameVec2(fbscene.CameraXRange, unityscene._CameraXRange))
        {
			UnityEngine.Debug.Log("_CameraXRange");
            return false;
        }


        if (fbscene.CameraPersp != unityscene._CameraPersp) 
        {
			UnityEngine.Debug.Log("_CameraPersp");
            return false;
        }

        if (!_isSameVec3(fbscene.CenterPostionNew, unityscene._CenterPostionNew))
        {
			UnityEngine.Debug.Log("CenterPostionNew");
            return false;
        }

        if (!_isSameVec3(fbscene.ScenePostion, unityscene._ScenePostion))
        {
			UnityEngine.Debug.Log("_ScenePostion");
            return false;
        }

        if (fbscene.SceneUScale != unityscene._SceneUScale)
        {
			UnityEngine.Debug.Log("_SceneUScale");
            return false;
        }

        if (!_isSameVec2(fbscene.GridSize, unityscene._GridSize))
        {
			UnityEngine.Debug.Log("GirdSize");
            return false;
        }
   
        if (!_isSameVec2(fbscene.LogicXSize, unityscene._LogicXSize))
        {
			UnityEngine.Debug.Log("LogicXSize");
            return false;
        }

        if (!_isSameVec2(fbscene.LogicZSize, unityscene._LogicZSize))
        {
			UnityEngine.Debug.Log("LogicZSize");
            return false;
        }
   
        if (!_isSameColor(fbscene.ObjectDyeColor, unityscene._ObjectDyeColor))
        {
			UnityEngine.Debug.Log("_ObjectDyeColor");
            return false;
        }

        if (!_isSameVec3(fbscene.LogicPos, unityscene._LogicPos))
        {
            UnityEngine.Debug.Log("_LogicPos");
            return false;
        }

        if (fbscene.TipsID != unityscene._TipsID)
        {
            UnityEngine.Debug.Log("TipsID");
            return false;
        }

        if (fbscene.LightmapsettingsPath != unityscene._LightmapsettingsPath)
        {
            UnityEngine.Debug.Log("LightmapsettingsPath");
            return false;
        }

        if (fbscene.LogicXmin != unityscene._LogicXmin)
        {
            UnityEngine.Debug.Log("LightmapsettingsPath");
            return false;
        }

        if (fbscene.LogicXmax != unityscene._LogicXmax)
        {
            UnityEngine.Debug.Log("_LogicXmax");
            return false;
        }

        if (fbscene.LogicZmin != unityscene._LogicZmin)
        {
            UnityEngine.Debug.Log("_LogicZmin");
            return false;
        }

        if (fbscene.LogicZmax != unityscene._LogicZmax)
        {
            UnityEngine.Debug.Log("_LogicZmax");
            return false;
        }

        if (fbscene.EntityinfoLength != unityscene._entityinfo.Length)
        {
            UnityEngine.Debug.Log("EntityinfoLength");
            return false;
        }

        for (int i = 0; i < fbscene.EntityinfoLength; ++i)
        {
            if (!_isSameEntity(fbscene.GetEntityinfo(i), unityscene._entityinfo[i]))
            {
                UnityEngine.Debug.LogFormat("GetEntityinfo {0}", i);
                return false;
            }
        }

        if (fbscene.BlocklayerLength != unityscene._blocklayer.Length )
        {
            UnityEngine.Debug.LogFormat("_blocklayer.len");
            return false;
        }

        for (int i = 0; i < fbscene.BlocklayerLength; ++i)
        {
            if (fbscene.GetBlocklayer(i) != unityscene._blocklayer[i])
            {
                return false;
            }
        }

        if (fbscene.NpcinfoLength != unityscene._npcinfo.Length)
        {
            UnityEngine.Debug.Log("NpcinfoLength");
            return false;
        }

        for (int i = 0; i < fbscene.NpcinfoLength; ++i)
        {

            if (!_isSameEntity(fbscene.GetNpcinfo(i).Super, unityscene._npcinfo[i]))
            {
                UnityEngine.Debug.Log("Npcinfo.Super");
                return false;
            }

            if (!_isSameVec2(fbscene.GetNpcinfo(i).MaxFindRange, unityscene._npcinfo[i].maxFindRange))
            {
                UnityEngine.Debug.Log("Npcinfo.MaxFindRange");
                return false;
            }

            if (!_isSameVec2(fbscene.GetNpcinfo(i).MinFindRange, unityscene._npcinfo[i].minFindRange))
            {
                UnityEngine.Debug.Log("Npcinfo.MinFindRange");
                return false;
            }

            if (!_isSameQ(fbscene.GetNpcinfo(i).Rotation, unityscene._npcinfo[i].rotation))
            {
                UnityEngine.Debug.Log("Npcinfo.Rotation");
                return false;
            }
        }

        if (fbscene.MonsterinfoLength != unityscene._monsterinfo.Length)
        {
            UnityEngine.Debug.Log("MonsterinfoLength");
            return false;
        }

        for (int i = 0; i < fbscene.MonsterinfoLength; ++i)
        {
            if (!_isSameEntity(fbscene.GetMonsterinfo(i).Super, unityscene._monsterinfo[i]))
            {
                UnityEngine.Debug.Log("Monsterinfo.Super");
                return false;
            }

            if (fbscene.GetMonsterinfo(i).SwapType != (FBSceneData.MonsterSwapType)unityscene._monsterinfo[i].swapType)
            {
                UnityEngine.Debug.Log("Monsterinfo.SwapType");
                return false;
            }

            if (fbscene.GetMonsterinfo(i).FaceType != (FBSceneData.FaceType)unityscene._monsterinfo[i].faceType)
            {
                UnityEngine.Debug.Log("Monsterinfo.FaceType");
                return false;
            }

            if (fbscene.GetMonsterinfo(i).SwapNum != unityscene._monsterinfo[i].swapNum)
            {
                UnityEngine.Debug.Log("Monsterinfo.SwapNum");
                return false;
            }

            if (fbscene.GetMonsterinfo(i).SwapDelay != unityscene._monsterinfo[i].swapDelay)
            {
                UnityEngine.Debug.Log("Monsterinfo.SwapDelay");
                return false;
            }

            if (fbscene.GetMonsterinfo(i).FlushGroupID != unityscene._monsterinfo[i].flushGroupID)
            {
                UnityEngine.Debug.Log("Monsterinfo.FlushGroupID");
                return false;
            }

            if (fbscene.GetMonsterinfo(i).FlowRegionType != (FBSceneData.FlowRegionType)unityscene._monsterinfo[i].flowRegionType)
            {
                UnityEngine.Debug.Log("Monsterinfo.FlowRegionType");
                return false;
            }

            if (!_isSameRegionInfo(fbscene.GetMonsterinfo(i).RegionInfo, unityscene._monsterinfo[i].regionInfo))
            {
                UnityEngine.Debug.Log("Monsterinfo.RegionInfo");
                return false;
            }

            if (!_isSameDestrucInfo(fbscene.GetMonsterinfo(i).DestructInfo, unityscene._monsterinfo[i].destructInfo))
            {
                UnityEngine.Debug.Log("Monsterinfo.DestructInfo");
                return false;
            }
        }

        if (fbscene.DecoratorinfoLength != unityscene._decoratorinfo.Length)
        {
            UnityEngine.Debug.Log("DecoratorinfoLength");
            return false;
        }

        for (int i = 0; i < fbscene.DecoratorinfoLength; ++i)
        {
            if (!_isSameEntity(fbscene.GetDecoratorinfo(i).Super, unityscene._decoratorinfo[i]))
            {
                UnityEngine.Debug.Log("Decoratorinfo.Super");
                return false;
            }

            if (!_isSameVec3(fbscene.GetDecoratorinfo(i).LocalScale, unityscene._decoratorinfo[i].localScale))
            {
                UnityEngine.Debug.Log("Decoratorinfo.LocalScale");
                return false;
            }

            if (!_isSameQ(fbscene.GetDecoratorinfo(i).Rotation, unityscene._decoratorinfo[i].rotation))
            {
                UnityEngine.Debug.Log("Decoratorinfo.Rotation");
                return false;
            }
        }


        if (fbscene.DesructibleinfoLength != unityscene._desructibleinfo.Length)
        {
            UnityEngine.Debug.Log("DesructibleinfoLength");
            return false;
        }

        for (int i = 0; i < fbscene.DesructibleinfoLength; ++i)
        {

            if (!_isSameDestrucInfo(fbscene.GetDesructibleinfo(i), unityscene._desructibleinfo[i]))
            {
                UnityEngine.Debug.Log("Desructibleinfo.Super");
                return false;
            }

        }

        if (fbscene.RegioninfoLength != unityscene._regioninfo.Length)
        {
            UnityEngine.Debug.Log("RegioninfoLength");
            return false;
        }

        for (int i = 0; i < fbscene.RegioninfoLength; ++i)
        {
            if (!_isSameRegionInfo(fbscene.GetRegioninfo(i), unityscene._regioninfo[i]))
            {
                UnityEngine.Debug.Log("Regioninfo");
                return false;
            }
        }

        if (fbscene.TransportdoorLength != unityscene._transportdoor.Length)
        {
            UnityEngine.Debug.Log("TransportdoorLength");
            return false;
        }

        for (int i = 0; i < fbscene.TransportdoorLength; ++i)
        {
            if (!_isSameRegionInfo(fbscene.GetTransportdoor(i).Super, unityscene._transportdoor[i]))
            {
                UnityEngine.Debug.Log("Transportdoor.Super");
                return false;
            }

            if (fbscene.GetTransportdoor(i).Doortype != (FBSceneData.TransportDoorType)unityscene._transportdoor[i].doortype)
            {
                UnityEngine.Debug.Log("Transportdoor.DoorType");
                return false;
            }

            if (fbscene.GetTransportdoor(i).Nextdoortype != (FBSceneData.TransportDoorType)unityscene._transportdoor[i].nextdoortype)
            {
                UnityEngine.Debug.Log("Transportdoor.NextDoortype");
                return false;
            }

            if (fbscene.GetTransportdoor(i).Nextsceneid != unityscene._transportdoor[i].nextsceneid)
            {
                UnityEngine.Debug.Log("Transportdoor.Nextsceneid");
                return false;
            }

            if (!_isSameVec3(fbscene.GetTransportdoor(i).Birthposition, unityscene._transportdoor[i].birthposition))
            {
                UnityEngine.Debug.Log("Transportdoor.Birthposition");
                return false;
            }
        }

        if (!_isSameEntity(fbscene.Birthposition, unityscene._birthposition))
        {
            UnityEngine.Debug.Log("Birthposition");
            return false;
        }

        if (!_isSameEntity(fbscene.Hellbirthposition, unityscene._hellbirthposition))
        {
            UnityEngine.Debug.Log("Hellbirthposition");
            return false;
        }

        if (fbscene.TownDoorLength != unityscene._townDoor.Length)
        {
            UnityEngine.Debug.Log("TownDoorLength");
            return false;
        }

        for (int i = 0; i < fbscene.TownDoorLength; ++i)
        {
            if (!_isSameRegionInfo(fbscene.GetTownDoor(i).Super, unityscene._townDoor[i]))
            {
                UnityEngine.Debug.Log("TownDoor.Super");
                return false;
            }

            if (fbscene.GetTownDoor(i).SceneID != unityscene._townDoor[i].SceneID)
            {
                UnityEngine.Debug.Log("TownDoor.SceneID");
                return false;
            }

            if (fbscene.GetTownDoor(i).DoorID != unityscene._townDoor[i].DoorID)
            {
                UnityEngine.Debug.Log("TownDoor.DoorID");
                return false;
            }

            if (_isSameVec3(fbscene.GetTownDoor(i).BirthPos, unityscene._townDoor[i].BirthPos))
            {
                UnityEngine.Debug.Log("TownDoor.BirthPos");
                return false;
            }

            if (fbscene.GetTownDoor(i).TargetSceneID != unityscene._townDoor[i].TargetSceneID)
            {
                UnityEngine.Debug.Log("TownDoor.TargetSceneID");
                return false;
            }

            if (fbscene.GetTownDoor(i).TargetDoorID != unityscene._townDoor[i].TargetDoorID)
            {
                UnityEngine.Debug.Log("TownDoor.TargetDoorID");
                return false;
            }
        }

        if (fbscene.FunctionPrefabLength != unityscene._FunctionPrefab.Length)
        {
            UnityEngine.Debug.Log("FunctionPrefabLength");
            return false;
        }

        for (int i = 0; i < fbscene.FunctionPrefabLength; ++i)
        {
            if (!_isSameEntity(fbscene.GetFunctionPrefab(i).Super, unityscene._FunctionPrefab[i]))
            {
                UnityEngine.Debug.Log("FunctionPrefab.Super");
                return false;
            }

            if (fbscene.GetFunctionPrefab(i).EFunctionType  != (FBSceneData.FunctionType)unityscene._FunctionPrefab[i].eFunctionType)
            {
                UnityEngine.Debug.Log("FunctionPrefab.EFunctionType");
                return false;
            }
        }

        return true;
	}

    private bool _isSameDestrucInfo(FBSceneData.DDestructibleInfo info, DDestructibleInfo oinfo)
    {
        if (null == info && null == oinfo)
        {
            return true;
        }

        if (!_isSameEntity(info.Super, oinfo))
        {
            return false;
        }

        if (info.Level != oinfo.level)
        {
            return false;
        }

        if (info.FlushGroupID != oinfo.flushGroupID)
        {
            return false;
        }

        if (!_isSameQ(info.Rotation, oinfo.rotation))
        {
            return false;
        }

        return false;
    }


    private bool _isSameRegionInfo(FBSceneData.DRegionInfo info, DRegionInfo oinfo)
    {
        if (!_isSameEntity(info.Super, oinfo))
        {
            return false;
        }

        if (info.Regiontype != (FBSceneData.RegionType)oinfo.regiontype)
        {
            return false;
        }

        if (!_isSameVec2(info.Rect, oinfo.rect))
        {
            return false;
        }

        if (info.Radius != oinfo.radius)
        {
            return false;
        }

        if (!_isSameQ(info.Rotation, oinfo.rotation))
        {
            return false;
        }

        return true;
    }

    private bool _isSameEntity(FBSceneData.DEntityInfo info, DEntityInfo oinfo)
    {
        if (info.Globalid != oinfo.globalid)
        {
            UnityEngine.Debug.Log("Entity.globalid");
            return false;
        }

        if (info.Resid != oinfo.resid)
        {
            UnityEngine.Debug.Log("Entity.resid");
            return false;
        }

        if (info.Name != oinfo.name)
        {
            UnityEngine.Debug.Log("Entity.name");
            return false;
        }

        if (info.Path != oinfo.path)
        {
            UnityEngine.Debug.Log("Entity.path");
            return false;
        }

        if (info.Description != oinfo.description)
        {
            UnityEngine.Debug.Log("Entity.desct");
            return false;
        }

        if ((DEntityType)info.Type != oinfo.type)
        {
            UnityEngine.Debug.Log("Entity.type");
            return false;
        }

        if (info.Typename != oinfo.typename)
        {
            UnityEngine.Debug.Log("Entity.name");
            return false;
        }

        if (!_isSameVec3(info.Position, oinfo.position))
        {
            UnityEngine.Debug.Log("Entity.postion");
            return false;
        }

        if (info.Scale != oinfo.scale)
        {
            UnityEngine.Debug.Log("Entity.scale");
            return false;
        }

        if (!_isSameColor(info.Color, oinfo.color))
        {
            UnityEngine.Debug.Log("Entity.color");
            return false;
        }

        return true;
    }

    private bool _isSameQ(FBSceneData.Quaternion q, Quaternion oq)
    {
        if (q.X != oq.x || q.Y != oq.y || q.Z != oq.z || q.W != oq.w)
            return false;

        return true;
    }


    private bool _isSameColor(FBSceneData.Color c, Color oc)
    {
        if (c.R != oc.r || c.G != oc.g || c.B != oc.b || c.A != oc.a)
            return false;

        return true;
    }

    private bool _isSameVec3(FBSceneData.Vector3 v3, Vector3 ov3)
    {
        if (v3.X != ov3.x || v3.Y != ov3.y || v3.Z != ov3.z)
        {
            return false;
        }

        return true;
    }

    private bool _isSameVec2(FBSceneData.Vector2 v2, Vector2 ov2)
    {
        if (v2.X != ov2.x || v2.Y != ov2.y)
        {
            return false;
        }

        return true;
    }

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator DungeonDataTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
