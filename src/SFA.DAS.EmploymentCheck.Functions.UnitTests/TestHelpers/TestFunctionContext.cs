using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers
{
    internal sealed class TestFunctionContext : FunctionContext
    {
        private readonly IServiceProvider _services;
        private readonly IDictionary<object, object> _items = new Dictionary<object, object>();

        public TestFunctionContext(ILogger logger)
        {
            var sc = new ServiceCollection();
            sc.AddLogging(b => b.AddProvider(new SingleLoggerProvider(logger)));
            _services = sc.BuildServiceProvider();
        }

        public override string InvocationId => Guid.NewGuid().ToString();
        public override string FunctionId => "test";
        public override TraceContext TraceContext => null;
        public override BindingContext BindingContext => null;
        public override IServiceProvider InstanceServices { get => _services; set { } }
        public override FunctionDefinition FunctionDefinition => new TestFunctionDefinition();
        public override IInvocationFeatures Features => new TestInvocationFeatures();
        public override IDictionary<object, object> Items { get => _items; set { } }
        public override RetryContext RetryContext => null;

        private sealed class SingleLoggerProvider : ILoggerProvider
        {
            private readonly ILogger _logger;
            public SingleLoggerProvider(ILogger logger) { _logger = logger; }
            public ILogger CreateLogger(string categoryName) => _logger;
            public void Dispose() { }
        }

        private sealed class TestInvocationFeatures : IInvocationFeatures
        {
            private readonly Dictionary<Type, object> _features = new Dictionary<Type, object>();
            T IInvocationFeatures.Get<T>() => _features.TryGetValue(typeof(T), out var v) ? (T)v : default;
            void IInvocationFeatures.Set<T>(T instance) => _features[typeof(T)] = instance;
            public IEnumerator<KeyValuePair<Type, object>> GetEnumerator() => _features.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _features.GetEnumerator();
        }

        private sealed class TestFunctionDefinition : FunctionDefinition
        {
            public override string PathToAssembly => string.Empty;
            public override string EntryPoint => string.Empty;
            public override string Id => "test";
            public override string Name => "test";
            public override IImmutableDictionary<string, BindingMetadata> InputBindings => ImmutableDictionary<string, BindingMetadata>.Empty;
            public override IImmutableDictionary<string, BindingMetadata> OutputBindings => ImmutableDictionary<string, BindingMetadata>.Empty;
            public override ImmutableArray<FunctionParameter> Parameters => ImmutableArray<FunctionParameter>.Empty;
        }
    }
}
