using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUtils
{
    public class DisposableTuple<T1, T2> : IDisposable
            where T1 : IDisposable
            where T2 : IDisposable
    {
        public T1 Item1;
        public T2 Item2;

        public DisposableTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public void Dispose()
        {
            Item1.Dispose();
            Item2.Dispose();
        }
    }
    public class DisposableTuple<T1, T2, T3> : IDisposable
        where T1 : IDisposable
        where T2 : IDisposable
        where T3 : IDisposable
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        public DisposableTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public void Dispose()
        {
            Item1.Dispose();
            Item2.Dispose();
            Item3.Dispose();
        }
    }
    public class DisposableTuple<T1, T2, T3, T4> : IDisposable
    where T1 : IDisposable
    where T2 : IDisposable
    where T3 : IDisposable
    where T4 : IDisposable
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        public DisposableTuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public void Dispose()
        {
            Item1.Dispose();
            Item2.Dispose();
            Item3.Dispose();
            Item4.Dispose();
        }
    }
    public class DisposableTuple<T1, T2, T3, T4, T5> : IDisposable
    where T1 : IDisposable
    where T2 : IDisposable
    where T3 : IDisposable
    where T4 : IDisposable
    where T5 : IDisposable
    {
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;
        public T5 Item5;

        public DisposableTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }

        public void Dispose()
        {
            Item1.Dispose();
            Item2.Dispose();
            Item3.Dispose();
            Item4.Dispose();
            Item5.Dispose();
        }
    }

    public static class AwaitUtils
    {
        public static async Task<DisposableTuple<T1,T2>> JoinedDisposableAwait<T1,T2>(Task<T1> task1, Task<T2> task2)
            where T1:IDisposable
            where T2 : IDisposable
        {
            await Task.WhenAll(task1, task2);

            // todo error handle logic

            return new DisposableTuple<T1, T2>(task1.Result, task2.Result);
        }

        public static async Task<DisposableTuple<T1, T2, T3>> JoinedDisposableAwait<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
            where T1 : IDisposable
            where T2 : IDisposable
            where T3 : IDisposable
        {
            await Task.WhenAll(task1, task2, task3);

            // todo error handle logic

            return new DisposableTuple<T1, T2, T3>(task1.Result, task2.Result, task3.Result);
        }

        public static async Task<DisposableTuple<T1, T2, T3, T4>> JoinedDisposableAwait<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
            where T1 : IDisposable
            where T2 : IDisposable
            where T3 : IDisposable
            where T4 : IDisposable
        {
            await Task.WhenAll(task1, task2, task3, task4);

            // todo error handle logic

            return new DisposableTuple<T1, T2, T3, T4>(task1.Result, task2.Result, task3.Result, task4.Result);
        }

        public static async Task<DisposableTuple<T1, T2, T3, T4, T5>> JoinedDisposableAwait<T1, T2, T3, T4, T5>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5)
            where T1 : IDisposable
            where T2 : IDisposable
            where T3 : IDisposable
            where T4 : IDisposable
            where T5 : IDisposable
        {
            await Task.WhenAll(task1, task2, task3, task4, task5);

            // todo error handle logic

            return new DisposableTuple<T1, T2, T3, T4, T5>(task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
        }

        public static async Task<Tuple<T1, T2>> JoinedAwait<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            await Task.WhenAll(task1, task2);

            // todo error handle logic

            return new Tuple<T1, T2>(task1.Result, task2.Result);
        }

        public static async Task<Tuple<T1, T2, T3>> JoinedAwait<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
        {
            await Task.WhenAll(task1, task2, task3);

            // todo error handle logic

            return new Tuple<T1, T2, T3>(task1.Result, task2.Result, task3.Result);
        }

        public static async Task<Tuple<T1, T2, T3, T4>> JoinedAwait<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
        {
            await Task.WhenAll(task1, task2, task3, task4);

            // todo error handle logic

            return new Tuple<T1, T2, T3, T4>(task1.Result, task2.Result, task3.Result, task4.Result);
        }

        public static async Task<Tuple<T1, T2, T3, T4, T5>> JoinedAwait<T1, T2, T3, T4, T5>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5)
        {
            await Task.WhenAll(task1, task2, task3, task4, task5);

            // todo error handle logic

            return new Tuple<T1, T2, T3, T4, T5>(task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
        }

    }
}
