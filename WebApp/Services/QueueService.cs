﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Services.Interfaces;
using System.Collections.Concurrent;
using WebApp.Extencions;

namespace WebApp.Services
{
    public class QueueService : IQueueChecker
    {
        ConcurrentQueue<Guid> queue = new ConcurrentQueue<Guid>();

        public void PutInQueue(Guid solutionId)
        {
            queue.Enqueue(solutionId);
        }

        public List<Guid> GetFromQueue(int count)
        {
            return queue.Take(count);
        }
    }
}