using System;
using System.Collections.Generic;
using SIO.Infrastructure.Domain;
using SIO.Infrastructure.Events;

namespace SIO.Infrastructure.Testing.Abstractions
{
    public abstract class AggregateSpecification<TAggregate, TAggregateState>
        where TAggregateState : IAggregateState, new()
        where TAggregate : Aggregate<TAggregateState>
    {
        private ExceptionMode _exceptionMode;

        protected TAggregate Aggregate { get; }
        protected Exception Exception { get; }

        protected abstract IEnumerable<IEvent> Given();
        protected abstract void When();
        protected void RecordExceptions()
        {
            _exceptionMode = ExceptionMode.Record;
        }


        protected AggregateSpecification()
        {
            Aggregate = (TAggregate)Activator.CreateInstance(typeof(TAggregate), new object[] { new TAggregateState() });

            var events = Given();

            try
            {
                Aggregate.FromHistory(events);
                When();
            }
            catch (Exception ex) when (_exceptionMode == ExceptionMode.Record)
            {
                Exception = ex;
            }
        }
    }
}
