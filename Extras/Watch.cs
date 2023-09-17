using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Extras
{
    public class Watch : IDisposable
    {
        readonly string _text;
        readonly Stopwatch _stopwatch;
        
        public Watch(string text)
        {
            _text = text;
            _stopwatch = Stopwatch.StartNew();
        }
        
        public void Dispose()
        {
            _stopwatch.Stop();
            Debug.Log($"{_text}: {_stopwatch.Elapsed:g}");
        }
    }
}