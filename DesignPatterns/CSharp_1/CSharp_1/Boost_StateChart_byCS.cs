using System;
using System.Collections;
using System.Collections.Generic;

namespace StateChart
{
    public enum EResult
    { 
        None,
        Forward,
        Resume,
        Defered,
    }

    public enum EHistory
    {
        Shallow,
        Deep,
    }    public abstract class IEvent { public Type type { get { return GetType(); } } }
    
    public delegate void Reaction<T>(T fsm);
    public delegate EResult Reaction<U, T>(U fsm, T evt);
    interface IReaction {
        EResult Execute<FSM, EVENT>(FSM fsm_, EVENT evt);
    }
    public class CReaction<FSM, EVENT> : IReaction {
        Reaction<FSM, EVENT> reaction;
        public CReaction(Reaction<FSM, EVENT> reaction_) { reaction = reaction_; }
        public EResult Execute<F, E>(F fsm_, E evt)
        { return reaction((FSM)(object)fsm_, (EVENT)(object)evt); }
    }

    public abstract class IState<FSM> where FSM : IStateMachine<FSM>
    {
        public Type type { get { return GetType(); } }
        public EHistory History { get; set; }
        public Reaction<FSM> Entry { get; set; }
        public Reaction<FSM> Exit { get; set; }

        //could calc on runtime, but we need more fast spped this time.
        public int Depth { get; set; }
        public IState<FSM> OuterState { get; set; }
        public IState<FSM> ActiveState { get; set; }

        Dictionary<Type, IReaction> reactions = new Dictionary<Type, IReaction>();
        Dictionary<Type, Type> transitions = new Dictionary<Type, Type>();
        List<IState<FSM>> subStates = new List<IState<FSM>>();

        IState<FSM> initState = null;
        public IState<FSM> InitState
        { 
            get  
            {
                if (initState == null)
                    if (subStates.Count > 0) 
                        initState = subStates[0];
                return initState;
            } 
            set { initState = value; }
        }

        public IState(IState<FSM> ostate)
        {
            History = EHistory.Shallow;
            OuterState = ostate;
            if (OuterState != null) OuterState.AddSubState(this);
        }

        public IState(IState<FSM> ostate, EHistory history_)
        {
            OuterState = ostate;
            History = history_;
            if (OuterState != null) OuterState.AddSubState(this);
        }

        public void DoEntry(FSM fsm_)
        {
            //UnityEngine.Debug.Log("Entry: " + type.ToString());
            Console.WriteLine("Entry: " + type.ToString());
            if (Entry != null) Entry(fsm_);
            else OnEntry(fsm_);
        }
        public void DoExit(FSM fsm_)
        {
            //UnityEngine.Debug.Log("Exit : " + type.ToString());
            Console.WriteLine("Exit : " + type.ToString());
            if (Exit != null) Exit(fsm_);
            else OnExit(fsm_);
        }

        protected virtual void OnEntry(FSM fsm_) { }
        protected virtual void OnExit(FSM fsm_) { }

        public EResult Process<EVENT>(FSM fsm_, EVENT evt) where EVENT : IEvent 
        {
            IReaction reaction = null;
            bool hasit = reactions.TryGetValue(evt.type, out reaction);
            if (!hasit) return EResult.Forward;
            return reaction.Execute<FSM, EVENT>(fsm_, evt);
        }

        public void Bind<EVENT>(Reaction<FSM, EVENT> reaction) where EVENT : IEvent
        {
            if (transitions.ContainsKey(typeof(EVENT)))
                throw new System.InvalidOperationException();
            IReaction ireaction = new CReaction<FSM, EVENT>(reaction);
            reactions.Add(typeof(EVENT), ireaction); 
        }

        public void Bind<EVENT, TSTATE>()
            where EVENT : IEvent
            where TSTATE : IState<FSM>
        {
            if (reactions.ContainsKey(typeof(EVENT)))
                throw new System.InvalidOperationException();
            transitions.Add(typeof(EVENT), typeof(TSTATE));
        }

        public void AddSubState(IState<FSM> sstate)
        {
            IState<FSM> state = subStates.Find((x) => x.type == sstate.type);
            if (state != null) return;
            subStates.Add(sstate);
        }

        public IEnumerable<IState<FSM>> IterateSubState()
        {
            foreach (IState<FSM> state in subStates) 
                yield return state;
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;

namespace StateChart
{

    public abstract class IStateMachine<HOST>  where HOST : IStateMachine<HOST>
    {
        Dictionary<Type, IState<HOST>> typeStates = new Dictionary<Type, IState<HOST>>();
        List<IState<HOST>> activeStates = new List<IState<HOST>>();
        Queue<IEvent> eventQueue = new Queue<IEvent>();
        IState<HOST> outestState = null;
        bool bSuspend = false;

        public IStateMachine() { }

        public void Init(IState<HOST> state) 
        {
            IState<HOST> pstate = state;

            //add outer states
            while (pstate.OuterState != null) {
                pstate.OuterState.ActiveState = pstate;
                activeStates.Add(pstate);
                pstate = pstate.OuterState;
            } 
            activeStates.Add(pstate);
            outestState = pstate;
            
            //build global type-to-state table
            BuildStateTable(outestState, 0);

            //add init sub states
            pstate = state;
            while (pstate.InitState != null) {
                pstate.ActiveState = pstate.InitState;
                pstate = state.InitState;
                if(pstate != null) activeStates.Add(pstate);
            }

            activeStates.Sort((x, y) => x.Depth - y.Depth);
            foreach (IState<HOST> astate in activeStates) {
                astate.DoEntry((HOST)this);
            }
        }

        void BuildStateTable(IState<HOST> state, int depth_) 
        {
            if (state == null) return;
            state.Depth = depth_;
            typeStates.Add(state.type, state);
            foreach (IState<HOST> sstate in state.IterateSubState()) { 
                BuildStateTable(sstate, depth_ + 1); 
            }
        }

        EResult Transit(IState<HOST> state)
        {
            IState<HOST> lstate = null;

            lstate = outestState;
            while (lstate.ActiveState != null) {  // we could save it if state tree is too high.
                lstate = lstate.ActiveState;
            }

            IState<HOST> rstate = state;
            if (state.History == EHistory.Shallow)
                while (rstate.InitState != null)
                    rstate = state.InitState;
            else
                while (rstate.ActiveState != null)
                    rstate = rstate.ActiveState;


            IState<HOST> ltail = lstate;  //save tail of active states
            IState<HOST> rtail = rstate;    //save tail of init states

            int dis = lstate.Depth - rstate.Depth;
            if (dis > 0)
                { IState<HOST> tstate = lstate; lstate = rstate; rstate = tstate; } //rstate will be deepest state

            dis = Math.Abs(dis);
            for (int i = 0; i < dis; i++)  {
                rstate = rstate.OuterState;
            }
            if (rstate == lstate)  //is family
                return EResult.None;
            do
            { //find nearest outer state
                rstate = rstate.OuterState;
                lstate = lstate.OuterState;
            } while (lstate != rstate);

            do  // call exit chain 
            {
                ltail.DoExit((HOST)this);
                ltail = ltail.OuterState;
            } while (ltail != lstate);

            //add tail chain active states
            activeStates.RemoveRange(rstate.Depth + 1, activeStates.Count - rstate.Depth - 1);
            do
            {
                activeStates.Add(rtail);
                lstate = rtail;
                rtail = rtail.OuterState;
                rtail.ActiveState = lstate;
            } while (rtail != rstate);

            // do entry chain
            while (rstate.ActiveState != null)
            {
                rstate = rstate.ActiveState;
                rstate.DoEntry((HOST)this);
            }

            activeStates.Sort((x, y) => x.Depth - y.Depth);
            return EResult.None;
        }

        public EResult Transit(Type stateType)
        {
            IState<HOST> state = null;
            if (!typeStates.TryGetValue(stateType, out state))
                return EResult.None;
            return Transit(state);
        }

        public EResult Transit<TSTATE>()
        { return Transit(typeof(TSTATE)); }

        public void Process<EVENT>(EVENT evt) where EVENT : IEvent
        {
            if (bSuspend) return;

            eventQueue.Enqueue(evt);
            int eventCount = eventQueue.Count;
            while (eventCount > 0){
                eventCount--;            
                IEvent pevent = eventQueue.Dequeue();
                foreach (IState<HOST> state in activeStates)
                    if (bSuspend || state.Process((HOST)this, pevent) == EResult.None)
                        break;
            }
        }

        public void PostEvent<EVENT>(EVENT evt) where EVENT : IEvent
        {
            if (bSuspend) return;
            eventQueue.Enqueue(evt);
        }

        public void Suspend()
        { bSuspend = true; }
        public void Resume()
        { bSuspend = false; }
    }