using System;
using System.Collections.Generic;
using DefaultNamespace;
using Events;
using Tools.SimpleEventBus;
using UnityEngine;

namespace Runner.GeneratorSystem
{
    public class StripGenerator : IDisposable
    {
        public PartStrip Finish { get; private set; }

        private readonly Transform _startPoint;
        private readonly PartStrip _finish;
        private readonly PartStrip _start;

        private readonly StripPool _stripPool;

        public StripGenerator(Settings settings, StripPool stripPool)
        {
            _startPoint = settings.StartPoint;
            _finish = settings.Finish;
            _start = settings.Start;
            _stripPool = stripPool;
        }

        public void Generation(PartStrip partStripPrefab)
        {
            EventStreams.UserInterface.Publish(new EventGenerationRunner());
            var start = _stripPool.GetPartStripByID(_start.ID);
            start.transform.position = _startPoint.position;
            
            var pastPartStrip = start;
            
            var partStrip = _stripPool.GetPartStripByID(partStripPrefab.ID);
            partStrip.transform.position = pastPartStrip.ForwardPoint.position;
            pastPartStrip = partStrip;

            Finish = _stripPool.GetPartStripByID(_finish.ID);
            Finish.transform.position = pastPartStrip.ForwardPoint.position;
        }
        

        [Serializable]
        public class Settings
        {
            public Transform StartPoint;
            public PartStrip Finish;
            public PartStrip Start;
        }

        public void Dispose()
        {
        }
    }
}