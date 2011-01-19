using System;
using Microsoft.Phone.Shell;

namespace Entile
{
    public interface IRemoteTileManager
    {
        void Start(string tileUri, UpdateInterval updateInterval);
        void Stop();
    }

    public class RemoteTileManager : IRemoteTileManager
    {
        private string _tileUri;
        private UpdateInterval _updateInterval;

        private ShellTileSchedule _schedule;

        public void Start(string tileUri, UpdateInterval updateInterval)
        {
            _tileUri = tileUri;
            _updateInterval = updateInterval;
            _schedule = new ShellTileSchedule()
                               {
                                   Interval = _updateInterval,
                                   RemoteImageUri = new Uri(_tileUri),
                                   Recurrence = UpdateRecurrence.Interval
                               };
            _schedule.StartTime = DateTime.Now;
            _schedule.Start();
        }

        public void Stop()
        {
            if (_schedule == null)
            {
                // This looks crazy, I know. But unfortunately, you cannot stop a schedule that hasn't been started...
                _schedule = new ShellTileSchedule();
                _schedule.RemoteImageUri = new Uri("http://localhost/");
                _schedule.Start();
            }
            _schedule.Stop();
            _schedule = null;
        }
    }
}