using System;
using Entile.Common;

namespace Entile.Worker
{
    public class InMemoryNotificationQueue : INotificationQueue
    {
        private PriorityQueue<DateTime, INotificationItem> _queue;

        public InMemoryNotificationQueue()
        {
            _queue = new PriorityQueue<DateTime, INotificationItem>();
        }

        public void EnqueueItem(INotificationItem notificationItem)
        {
            lock(_queue)
            {
                _queue.Enqueue(DateTime.Now, notificationItem);
            }
        }

        public void EnqueueItem(INotificationItem notificationItem, DateTime earliestSend)
        {
            lock (_queue)
            {
                _queue.Enqueue(earliestSend, notificationItem);
            }
        }

        public INotificationItem TakeItem()
        {
            lock (_queue)
            {
                if (_queue.IsEmpty)
                    return null;

                var topEntry = _queue.Peek();
                if (topEntry.Key >= DateTime.Now)
                    return null;

                return _queue.DequeueValue();
            }
        }
    }
}