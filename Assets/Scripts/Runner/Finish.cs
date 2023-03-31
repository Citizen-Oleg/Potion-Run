using System;
using Events;
using Runner.PlayerComponent;
using Tools.SimpleEventBus;
using UnityEngine;

namespace Runner
{
    public class Finish : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BoilerCollider boiler))
            {
                EventStreams.UserInterface.Publish(new EventFinish(boiler.Boiler));
            }
        }
    }
}