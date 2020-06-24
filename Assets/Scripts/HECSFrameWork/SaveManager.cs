using GlobalCommander;
using HECS.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace HECS.Systems
{
    public class SaveManager : INeedGlobalStart, IDisposable
    {
        public static string SavedEntitiesPath => Application.dataPath + "/saveEnities.dat";

        //регистрируем тех кто хочет записывать своё состояние
        private List<ISaveble> savebles = new List<ISaveble>(100);

        //здесь записываются ентити
        public List<SaveEntityContainer> savedEntities = new List<SaveEntityContainer>(1000);

        public SaveManager()
        {
            Commander.RecieveRegisterObject(this, savebles);
            Commander.AddListener<SaveGlobalCommand>(this, SaveCommandReact);
            Commander.RegisterObjectByEvent<INeedGlobalStart>(this, true);

            LoadData();
        }

        public void SaveEntity(IEntity saveEntity)
        {
            if (saveEntity == null)
            {
                Debug.LogError("это не ентити");
                return;
            }

            if (saveEntity is ISaveble saveble)
            {
                //var saveComponents = saveEntity.GetAllComponents.ToList();
                //var save = JsonConvert.SerializeObject(saveComponents, Formatting.Indented, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
                //var thisEntity = savedEntities.FirstOrDefault(x => x.ID == saveble.UID);
                
                //if (thisEntity != null)
                //{
                //    thisEntity.ID = saveble.UID;
                //    thisEntity.GID = saveble.GID;
                //    thisEntity.Save = save;
                //}
                //else
                //    savedEntities.Add(new SaveEntityContainer { ID = saveble.UID, GID = saveble.GID, Save = save });
            }
            else
                Debug.LogError("пытаемся сохранить не сохраняемую ентити");
        }

        public string GetEntitySave(int UID)
        {
            return savedEntities.FirstOrDefault(x => x.ID == UID).Save;
        }

        private void LoadData()
        {
            //if (File.Exists(SavedEntitiesPath))
            //{
            //    var json = File.ReadAllText(SavedEntitiesPath);
            //    savedEntities = JsonConvert.DeserializeObject<List<SaveEntityContainer>>(json);
            //}
        }

        private void SaveCommandReact(SaveGlobalCommand obj)
        {
            //foreach (var s in savebles)
            //    s.Save(this);

            //var test = JsonConvert.SerializeObject(savedEntities, Formatting.Indented);
            //File.WriteAllText(SavedEntitiesPath, test);
        }

#if UNITY_EDITOR
        [MenuItem ("HECS Options/Clear Data")]
#endif
        public static void ClearSavedData()
        {
            File.Delete(SavedEntitiesPath);
        }


        public bool TryGetSavedEntity(int id, out string savedEntity)
        {
            var save = savedEntities.FirstOrDefault(x => x.ID == id);
            
            if (save == null)
            {
                savedEntity = string.Empty;
                return false;
            }

            savedEntity = save.Save;
            return true;
        }

        public void GlobalStart()
        {
            foreach (var s in savebles)
                s.Load(this);
        }

        public void Dispose()
        {
            Commander.RegisterObjectByEvent<INeedGlobalStart>(this, false);
        }
    }
}