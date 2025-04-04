using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JasperFx.CodeGeneration;
using JasperFx.Core.Reflection;
using JasperFx.Events;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Testing.Harness;

namespace EventSourcingTests.Aggregation;

public class AggregationContext : IntegrationContext
{
    protected SingleStreamProjection<MyAggregate, Guid> _projection;

    public AggregationContext(DefaultStoreFixture fixture) : base(fixture)
    {

    }

    protected override Task fixtureSetup()
    {
        return theStore.Advanced.Clean.DeleteDocumentsByTypeAsync(typeof(MyAggregate));
    }

    public void UsingDefinition<T>() where T : SingleStreamProjection<MyAggregate, Guid>, new()
    {
        _projection = new T();

        var rules = theStore.Options.CreateGenerationRules();
        rules.TypeLoadMode = TypeLoadMode.Dynamic;
    }

    public void UsingDefinition(Action<SingleStreamProjection<MyAggregate, Guid>> configure)
    {
        _projection = new SingleStreamProjection<MyAggregate, Guid>();
        configure(_projection);


        var rules = theStore.Options.CreateGenerationRules();
        rules.TypeLoadMode = TypeLoadMode.Dynamic;
    }


    public ValueTask<MyAggregate> LiveAggregation(Action<TestEventSlice> action)
    {
        var fragment = BuildStreamFragment(action);

        throw new NotImplementedException("Change this to the new model");
        //var aggregator = _projection.BuildAggregator(theStore.Options);
        //var events = (IReadOnlyList<IEvent>)fragment.Events();
        //return aggregator.BuildAsync(events, theSession, null, CancellationToken.None);
    }


    public static TestEventSlice BuildStreamFragment(Action<TestEventSlice> action)
    {
        var fragment = new TestEventSlice(Guid.NewGuid());
        action(fragment);
        return fragment;
    }

    public async Task InlineProject(Action<TestEventScenario> action)
    {
        var scenario = new TestEventScenario();
        action(scenario);

        var streams = scenario
            .Streams
            .ToDictionary()
            .Select(x => StreamAction.Append(x.Key, x.Value.Events().ToArray()))
            .ToArray();

        throw new NotImplementedException("Redo.");
        // var inline = _projection.BuildRuntime(theStore);
        //
        // await inline.ApplyAsync(theSession, streams, CancellationToken.None);
        // await theSession.SaveChangesAsync();
    }
}
