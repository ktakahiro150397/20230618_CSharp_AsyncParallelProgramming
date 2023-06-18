using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parallel_Tutorial
{
    internal class ParallelFor_Cancel
    {
        const int dataLength = 1024;
        static int[] data = Enumerable.Range(1, dataLength).ToArray();
        
        public ParallelFor_Cancel()
        {
        }

        public int SumParallel_Cancellable(ParallelOptions option)
        {
            var retSum = 0;

            Parallel.For(0, data.Length,
                option,
                () => 0,
                (i, loopState, localSum) =>
                {
                    // キャンセルされている場合に、OperationCanceledExceptionを発生させる
                    option.CancellationToken.ThrowIfCancellationRequested();

                    Console.WriteLine($"i={i} data[i]={data[i]} localSum={localSum} tId={Thread.CurrentThread.ManagedThreadId}");
                    localSum += data[i];
                    return localSum;
                },
                (localSum) =>
                {
                    Console.WriteLine($"** Thread break! tId={Thread.CurrentThread.ManagedThreadId}");

                    // retSumにこのスレッドで求めた総和を足す
                    Console.WriteLine($"** retSum={retSum} localSum(before)={localSum} tId={Thread.CurrentThread.ManagedThreadId}");
                    Interlocked.Add(ref retSum, localSum);
                    Console.WriteLine($"** localSum(after)={localSum} tId={Thread.CurrentThread.ManagedThreadId}");
                }
            );

            Console.WriteLine($"retSum={retSum}");
            return retSum;
        }


        public int SumParallel_Cancellable_RangePartitioner_Index(ParallelOptions option)
        {
            var retSum = 0;

            // Partitionerを生成(インデックス指定)
            // 指定したインデックス間でいい感じに分割してくれる
            var rangePartitioner = Partitioner.Create(0,dataLength);

            Parallel.ForEach(rangePartitioner,
                option,
                () => 0,
                (item, loopState, localSum) =>
                {
                    Console.WriteLine($"range = ({item.Item1},{item.Item2 - 1})");
                    for (var i = item.Item1; i < item.Item2; i++)
                    {
                        localSum += data[i];
                    }
                    return localSum;
                },
                (localSum) =>
                {
                    Console.WriteLine($"** Thread break! tId={Thread.CurrentThread.ManagedThreadId}");

                    // retSumにこのスレッドで求めた総和を足す
                    Console.WriteLine($"** retSum={retSum} localSum(before)={localSum} tId={Thread.CurrentThread.ManagedThreadId}");
                    Interlocked.Add(ref retSum, localSum);
                    Console.WriteLine($"** localSum(after)={localSum} tId={Thread.CurrentThread.ManagedThreadId}");
                });

            Console.WriteLine($"retSum={retSum}");
            return retSum;
        }

    }
}
