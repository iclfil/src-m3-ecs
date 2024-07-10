using Leopotam.EcsLite;
using MatchThree.ECS.Modules;
using MatchThree.Rack.ECS.Events;
using SevenBoldPencil.EasyEvents;
using UnityEngine;

namespace MatchThree.Rack.ECS.Systems
{
    //Только тогда, когда нужно проиграть всю модель самостоятельно. Чтобы она сама себя проиграла
    public class ModulesHandlerSystem : IEcsRunSystem
    {
        private readonly EventsBus bus;
        private readonly RackModuleNotify notify;

        public RackResult Result;

        public ModulesHandlerSystem(EventsBus bus, RackModuleNotify notify)
        {
            this.bus = bus;
            this.notify = notify;
        }

        public void Run(IEcsSystems systems)
        {

            if (bus.HasEventSingleton<StartStateEvent>())
            {
                var startingState = bus.GetEventBodySingleton<StartStateEvent>();
                EnableGroup(startingState.StartingState);
                bus.DestroyEventSingleton<StartStateEvent>();
                return;
            }

            if (bus.HasEventSingleton<StateFinishedEvent>())
            {
                var finishedEvent = bus.GetEventBodySingleton<StateFinishedEvent>();
                var name = finishedEvent.Name;
                var result = finishedEvent.Result;
                DisableGroup(name);
                bus.DestroyEventSingleton<StateFinishedEvent>();
                notify.Notify(name, result);
            }
        }

        private void EnableGroup(NameModule state)
        {
            bus.NewEventSingleton<EnableStateEvent>().RackState = state;
        }

        private void DisableGroup(NameModule state)
        {
            bus.NewEventSingleton<DisableStateEvent>().Name = state;
        }
    }
}

//if (StatesManager.HasStarting() == false)
//{
//    StatesManager.Starting();
//    _currentRackStateGame = StatesManager.Next();
//    Enable(_currentRackStateGame);
//    Result = RackResult.Ready;
//    return;
//}

//if (Bus.HasEventSingleton<StateFinishEvent>())
//{
//    StateFinishEvent data = Bus.GetEventBodySingleton<StateFinishEvent>();
//    RackState rackStateGame = data.RackState;
//    StatesManager.FinishState(rackStateGame, data.StateResult);
//    Disable(rackStateGame);
//    Bus.DestroyEventSingleton<StateFinishEvent>();

//    if (data.StateResult == StateResult.NoMatches)
//    {
//        if (Result != RackResult.HasMatches) //TODO косяк. Там идет в логике проход. И если он второй раз не нашел матчи, то возвращает NoMatches, и тогда у нас сессия неправильно отрабатывает
//            Result = RackResult.NoMatches;
//    }

//    if (data.StateResult == StateResult.HasMatches)
//        Result = RackResult.HasMatches;

//    if (rackStateGame == RackState.Finishing)
//        _finishPlaying?.Invoke(Result);
//}

//if (StatesManager.IsFinishStates() == false)
//{
//    _currentRackStateGame = StatesManager.Next();
//    Enable(_currentRackStateGame);
//}