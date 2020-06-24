using GlobalCommander;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HECS.Systems
{
    [DefaultExecutionOrder(-6000)]
    public class GlobalUpdateSystem : MonoBehaviour
    {
        private List<IUpdatable> Updatables = new List<IUpdatable>(300);
        private List<INeedGlobalStart> globalStartups = new List<INeedGlobalStart>(300);
        private List<IFixedUpdateLocal> fixedUpdatables = new List<IFixedUpdateLocal>(300);
        private List<DelayedAction> delayedActions = new List<DelayedAction>(16);
        private Queue<Action> queueFromAsync = new Queue<Action>();
        private HashSet<Func<bool>> UpdateFuncs = new HashSet<Func<bool>>();
        private bool funcResult;
        private int iterate = 0;
        private bool IsStarted;

        public void Awake()
        {
            Commander.RecieveRegisterObject(this, Updatables);
            Commander.RecieveRegisterObject(this, fixedUpdatables);
            Commander.AddListener<Register<INeedGlobalStart>> (this, RegisterGlobalStarters);
            Commander.AddListener<AddUpdateWithPredicate>(this, AddUpdateWithPredicateReact);
            Commander.AddListener<DispatchGlobalCommand>(this, DispatchGlobalCommandReact);
            Commander.AddListener<DelayedAction>(this, DelayedActionReact);
        }

        private void DelayedActionReact(DelayedAction obj)
        {
            delayedActions.Add(obj);
        }

        private void RegisterGlobalStarters(Register<INeedGlobalStart> obj)
        {
            if (IsStarted)
                obj.RegisterObject.GlobalStart();
            else
                globalStartups.AddOrRemoveElement(obj.RegisterObject, obj.Add);
        }

        private void Start()
        {
            IsStarted = true;

            foreach (var s in globalStartups)
                s.GlobalStart();
        }

        private void DispatchGlobalCommandReact(DispatchGlobalCommand obj)
        {
            queueFromAsync.Enqueue(obj.Action);
        }

        public void DispatchGlobalCommandReact(Action obj)
        {
            queueFromAsync.Enqueue(obj);
        }

        private void AddUpdateWithPredicateReact(AddUpdateWithPredicate obj)
        {
            UpdateFuncs.Add(obj.Func);
        }

        public void AddToDispatch(Action action)
        {
            queueFromAsync.Enqueue(action);
        }

        private void OneWayQueue()
        {
            if (queueFromAsync.Count == 0)
                return;

            while (queueFromAsync.Count > 0)
            {
                var act = queueFromAsync.Dequeue();
                act.Invoke();
            }
        }

        private void UpdateGlobalFunc()
        {
            if (UpdateFuncs.Count == 0)
                return;

            foreach (var f in UpdateFuncs)
            {
                funcResult = f();

                if (funcResult)
                    queueFromAsync.Enqueue(() => UpdateFuncs.Remove(f));
            }
        }

        private void Update()
        {
            UpdateDelayedActions();
            OneWayQueue();
            UpdateGlobalFunc();

            for (iterate = 0; iterate < Updatables.Count; ++iterate)
                Updatables[iterate].UpdateLocal();
        }

        private void UpdateDelayedActions()
        {
            if (delayedActions.Count == 0)
                return;

            foreach (var d in delayedActions)
            {
                if (d.TimeToRun <= Time.time)
                {
                    d.Action?.Invoke();
                    queueFromAsync.Enqueue(() => delayedActions.Remove(d));
                }
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < fixedUpdatables.Count; i++)
                fixedUpdatables[i].FixedUpdateLocal();
        }
    }

    public struct DispatchGlobalCommand
    {
        public Action Action;
    }

    public struct AddUpdateWithPredicate
    {
        public Func<bool> Func;
    }

    public struct DelayedAction
    {
        public float TimeToRun;
        public Action Action { get; set; }
    }

    public interface INeedGlobalStart 
    {
        void GlobalStart();
    }

    public interface ILateGlobalStart
    {
        void LateStart();
    }

    public interface IUpdatable
    {
        void UpdateLocal();
    }

    public interface IFixedUpdateLocal 
    {
        void FixedUpdateLocal();
    }
}

