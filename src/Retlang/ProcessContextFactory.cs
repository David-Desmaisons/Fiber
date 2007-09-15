using System;
using System.Collections.Generic;
using System.Text;

namespace Retlang
{
    public interface IProcessContextFactory: IThreadController
    {
        IProcessContext CreateAndStart();
        IProcessContext Create();
    }
    public class ProcessContextFactory: IProcessContextFactory
    {
        private int _maxQueueDepth = -1;
        private int _maxEnqueueWaitTime = -1;
        private readonly MessageBus _bus;
        private readonly CommandQueue _queue;
        private readonly ProcessThread _thread;
        private ITransferEnvelopeFactory _envelopeFactory = new BinaryTransferEnvelopeFactory();

        public ProcessContextFactory()
        {
            _queue = new CommandQueue();
            _thread = new ProcessThread(_queue);
            _bus = new MessageBus(_queue, _thread);
        }

        public void Start()
        {
            _queue.MaxEnqueueWaitTime = _maxEnqueueWaitTime;
            _queue.MaxDepth = _maxQueueDepth;
            _bus.Start();
        }

        public void Stop()
        {
            _bus.Stop();
        }

        public void Join()
        {
            _bus.Join();
        }

        public int MaxQueueDepth
        {
            get { return _maxQueueDepth; }
            set { _maxQueueDepth = value; }
        }

        public int MaxEnqueueWaitTime
        {
            get { return _maxEnqueueWaitTime; }
            set { _maxEnqueueWaitTime = value; }
        }

        public IMessageBus MessageBus
        {
            get { return _bus; }
        }

        public ITransferEnvelopeFactory TransferEnvelopeFactory
        {
            get { return _envelopeFactory; }
            set { _envelopeFactory = value; }
        }

        public IProcessContext CreateAndStart()
        {
            IProcessContext context = Create();
            context.Start();
            return context;
        }

        public IProcessContext Create()
        {
            CommandQueue queue = new CommandQueue();
            queue.MaxDepth = _maxQueueDepth;
            queue.MaxEnqueueWaitTime = _maxEnqueueWaitTime;
            return new ProcessContext(_bus, new ProcessThread(queue), _envelopeFactory);
        }
    }
}
