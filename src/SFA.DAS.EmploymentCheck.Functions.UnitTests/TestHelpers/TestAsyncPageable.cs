using System.Collections.Generic;
using Microsoft.DurableTask;

namespace SFA.DAS.EmploymentCheck.Functions.UnitTests.TestHelpers
{
    internal sealed class TestAsyncPageable<T> : AsyncPageable<T>
    {
        private readonly IReadOnlyList<T> _values;

        public TestAsyncPageable(IAsyncEnumerable<T> source)
        {
            var list = new List<T>();
            var e = source.GetAsyncEnumerator();
            try
            {
                while (e.MoveNextAsync().AsTask().GetAwaiter().GetResult())
                {
                    list.Add(e.Current);
                }
            }
            finally
            {
                e.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
            _values = list;
        }

        public override async IAsyncEnumerable<Page<T>> AsPages(string continuationToken = null, int? pageSizeHint = null)
        {
            yield return new Page<T>(_values, null);
            await System.Threading.Tasks.Task.CompletedTask;
        }
    }
}