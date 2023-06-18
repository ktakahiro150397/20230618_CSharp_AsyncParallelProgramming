using Parallel_Tutorial;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

//// データ処理 同期版
//Console.WriteLine("同期処理版-----------------");
//const int Count = 8;
//int[] a = Enumerable.Range(16, Count).ToArray();
//var c = new int[a.Length];

//for (int i = 0; i < a.Length; i++)
//{
//    c[i] = a[i] * 2;
//}
//Util.WriteArray(c);


//Console.WriteLine("");
//Console.WriteLine("非同期処理 Parallel.For版-----------------");

//int[] c_parallel = new int[a.Length];

//// Parallel.Forで指定したインデックス間を並列で処理
//// Console.WriteLineで出力されるインデックスの順序は不定
//Parallel.For(0, a.Length, (i) =>
//{
//    Console.WriteLine($"処理中：{i}");
//    c_parallel[i] = a[i] * 2;
//});
//Util.WriteArray(c_parallel);

//int[] c_parallel_foreach = new int[a.Length];

//// Parallel.ForEachで指定したオブジェクトを列挙
//Parallel.ForEach(a, (i) =>
//{
//    Console.WriteLine($"処理中：{i}");
//});


//Console.WriteLine("同期処理　ループからの脱出-----------------");

//// 逐次処理 ループからの脱出
//for (int i = 0; i < 128; i++)
//{
//    Console.WriteLine($"i={i}");

//    if (i == 10)
//    {
//        break;
//    }
//}

//Console.WriteLine("非同期処理　ループからの脱出-----------------");

//const int parallel_for_iter_count = 16;

// stateを受けるオーバーロードから、処理の中断が可能
// Stop()呼び出し前に他スレッドで処理が進んでいる場合、それらは止まらない
//Parallel.For(0, 128, (i, state) =>
//{
//    Console.WriteLine($"i={i} tId={Thread.CurrentThread.ManagedThreadId}");

//    if (i == 10)
//    {
//        Console.WriteLine($"break! tId={Thread.CurrentThread.ManagedThreadId}");
//        // i=10がイテレートされている場合に、処理を中断する
//        state.Stop();
//    }

//});
//Console.WriteLine("");

// 処理中に、イテレーションが中断されているかどうかをstateから確認可能


//スレッドローカル変数
var parallelFor_local = new ParallelFor_LocalVar();
var result = parallelFor_local.SumParallel();

// キャンセルが可能な並列処理
var parallelFor_Cancellable = new ParallelFor_Cancel();
var cts = new CancellationTokenSource();
var option = new ParallelOptions()
{
    CancellationToken = cts.Token,
    MaxDegreeOfParallelism = 2
};

// タスクを指定秒数後にキャンセルする
_ = Task.Run(() =>
{
    Thread.Sleep(1000);
    cts.Cancel();
});

// 並列処理呼び出し
try
{
    parallelFor_Cancellable.SumParallel_Cancellable(option);
}
catch (OperationCanceledException cancelEx)
{
    // キャンセルされた時に例外をキャッチ
    Console.WriteLine(cancelEx.ToString());
}


// Partitionerによるデリゲート呼び出し回数軽減
var cts2 = new CancellationTokenSource();
var option2 = new ParallelOptions()
{
    CancellationToken = cts2.Token,
    MaxDegreeOfParallelism = 2
};

// タスクを指定秒数後にキャンセルする
_ = Task.Run(() =>
{
    Thread.Sleep(1000);
    cts2.Cancel();
});

// 並列処理呼び出し
try
{
    parallelFor_Cancellable.SumParallel_Cancellable_RangePartitioner_Index(option2);
}
catch (OperationCanceledException cancelEx)
{
    // キャンセルされた時に例外をキャッチ
    Console.WriteLine(cancelEx.ToString());
}



class Util
{
    public static void WriteArray(int[] a)
    {
        foreach (int it in a)
            Console.Write("{0,5:d}", it);

        Console.WriteLine("");
    }

}