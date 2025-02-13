using System.Collections.Generic;
using UnityEngine;
using System.Text;
using DefineDatas;

namespace Managers
{
    public class PoolManager : SingletonTemplate<PoolManager>
    {
        public struct PoolObjectData
        {
            public PoolObjectName name;
            public GameObject obj;
            public int initObjectNum;
            public int addObjectNum;
            public List<PoolObject> objs;
            public List<int> objIDs;
        }

        public PoolManagerData managerData;

        private Dictionary<PoolObjectName, PoolObjectData> poolDatas
             = new Dictionary<PoolObjectName, PoolObjectData>();

        void Start()
        {
            InitPoolDatas();
            CreateObjects();           
        }

        void InitPoolDatas()
        {
            for (int i = 0; i < managerData.poolObjectDatas.Count; i++)
            {
                PoolObjectData temp = new PoolObjectData();

                temp.name = managerData.poolObjectDatas[i].name;
                temp.obj = managerData.poolObjectDatas[i].obj;
                temp.initObjectNum = managerData.poolObjectDatas[i].initObjectNum;
                temp.addObjectNum = 0;
                temp.objs = new List<PoolObject>();
                temp.objIDs = new List<int>();

                poolDatas[temp.name] = temp;
            }
        }

        void CreateObjects()
        {
            if(poolDatas.Count == 0)
            {
                Debug.LogWarning("소환할 PoolObject가 없습니다.");
                return;
            }

            for(int i = 0; i < poolDatas.Count; i++)
            {
                PoolObjectData data = poolDatas[(PoolObjectName)i];
                data.addObjectNum = data.initObjectNum;

                for (int j = 1; j <= data.initObjectNum; j++)
                {
                    data.objs.Add(CreateNewObject(data, j));
                }

                poolDatas[(PoolObjectName)i] = data;
            }
        }

        PoolObject CreateNewObject(PoolObjectData pObjData, int id)
        {
            GameObject gb = Instantiate(pObjData.obj, transform); //부모 아래에 소환
            gb.name = MakeObjectName(pObjData.obj.name, id);

            PoolObject poolObj = gb.GetComponent<PoolObject>();
            if (poolObj == null) poolObj = gb.AddComponent<PoolObject>();

            gb.SetActive(false);
            poolObj.SetPoolObjectData(id, pObjData.name, transform);
            poolObj.SetCallback(DisActiveObject);
            poolObj.InitPoolObject();
            poolObj.isUsed = false;
            pObjData.objIDs.Add(id);

            return poolObj;
        }

        string MakeObjectName(string objName ,int id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(objName).Append("_").Append(id);
            return sb.ToString();
        }

        public PoolObject GetObject(string userName, PoolObjectName name)
        {
            if(!poolDatas.ContainsKey(name))
            {
                Debug.LogError("해당 이름의 PoolObject를 찾을 수 없습니다.");
                return null;
            }

            PoolObjectData data = poolDatas[name];
            PoolObject ob;
            if (data.objIDs.Count > 0)
            {
                ob = data.objs.Find(item => item.ObjID == data.objIDs[0]);
            }

            else
            {
                ob = CreateNewObject(data, ++data.addObjectNum);
                data.objs.Add(ob);
            }

            data.objIDs.Remove(data.objIDs[0]);
            poolDatas[name] = data;

            ob.userName = userName;
            ob.isUsed = true;
            ob.gameObject.SetActive(true);
            return ob;
        }
        public PoolObject GetObject(string userName, PoolObjectName name, Vector3 pos, Quaternion rot)
        {
            if (!poolDatas.ContainsKey(name))
            {
                Debug.LogError("해당 이름의 PoolObject를 찾을 수 없습니다 : " + name);
                return null;
            }

            PoolObjectData data = poolDatas[name];
            PoolObject ob;
            if (data.objIDs.Count > 0)
            {
                ob = data.objs.Find(item => item.ObjID == data.objIDs[0]);
            }

            else
            {
                ob = CreateNewObject(data, ++data.addObjectNum);
                data.objs.Add(ob);
            }

            data.objIDs.Remove(data.objIDs[0]);
            poolDatas[name] = data;

            ob.transform.position = pos;
            ob.transform.rotation = rot;
            ob.userName = userName;
            ob.isUsed = true;
            ob.gameObject.SetActive(true);
            return ob;
        }

        public PoolObject SearchObject(PoolObjectName name, int id)
        {
            if (!poolDatas.ContainsKey(name))
            {
                Debug.LogError("해당 이름의 PoolObject를 찾을 수 없습니다 : " + name);
                return null;
            }

            PoolObjectData data = poolDatas[name];
            PoolObject ob = data.objs.Find(item => item.ObjID == id);
            if (ob == default)
            {
                Debug.LogError("해당 ID : " + id + " 의 PoolObject를 찾을 수 없습니다");
                return null;
            }

            return ob;
        }

        public void DisActiveObject(PoolObjectName name, int id)
        {
            if (!poolDatas.ContainsKey(name))
            {
                Debug.LogError("해당 이름의 PoolObject를 찾을 수 없습니다 : " + name);
                return;
            }

            PoolObject temp = poolDatas[name].objs.Find(item => item.ObjID == id);
            if(temp == default)
            {
                Debug.LogError("해당 ID : " + id + " 의 PoolObject를 찾을 수 없습니다");
                return;
            }

            if(temp.isUsed)
            {
                temp.isUsed = false;
                temp.userName = null;
                temp.gameObject.SetActive(false);
                poolDatas[name].objIDs.Add(id);
            }

            else
            {
                Debug.LogWarning("이미 비 활성화된 오브젝트 입니다. 이름 : " + name + "/ ID : " + id);
            }
        }

    }
}