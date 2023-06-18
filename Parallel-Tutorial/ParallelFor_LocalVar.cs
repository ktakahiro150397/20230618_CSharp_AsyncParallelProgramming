using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Parallel_Tutorial
{
    internal class ParallelFor_LocalVar
    {

        const int dataLength = 6;
        static int[] data = Enumerable.Range(1, dataLength).ToArray();

        public ParallelFor_LocalVar()
        {

        }

        public int SumParallel()
        {
            var retSum = 0;

            Console.WriteLine("スレッドローカル変数を利用した総和の計算----------------");

            // Parallel.Forでデータを列挙し、スレッドローカル変数に総和を保存する/
            Parallel.For(0, dataLength,
                () => 0, // Func<TLocal> スレッドローカル変数の初期化
                (i, loopState, localSum) => // 実際の処理 3つめの引数で、初期化したスレッド変数が参照できる
                {
                    Console.WriteLine($"i={i} data[i]={data[i]} localSum={localSum} tId={Thread.CurrentThread.ManagedThreadId}");
                    localSum += data[i];
                    return localSum;
                },
                (localSum) => // スレッドごとに、最後に呼び出される処理
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
