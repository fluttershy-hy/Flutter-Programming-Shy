using System;
using System.Threading;

// 闹钟类
public class AlarmClock
{
    // 定义 Tick 事件（走时事件）
    public event EventHandler Tick;
    // 定义 Alarm 事件（响铃事件）
    public event EventHandler Alarm;

    // 闹钟响铃的时间（秒）
    public int AlarmTime { get; set; }

    public AlarmClock(int alarmTime)
    {
        AlarmTime = alarmTime;
    }

    // 启动闹钟
    public void Start()
    {
        Console.WriteLine($"闹钟已启动，将在 {AlarmTime} 秒后响铃...");
        int elapsedSeconds = 0;

        while (elapsedSeconds < AlarmTime)
        {
            // 每一秒触发一次 Tick 事件
            OnTick(EventArgs.Empty);
            Thread.Sleep(1000); // 等待1秒
            elapsedSeconds++;
        }

        // 时间到，触发 Alarm 事件
        OnAlarm(EventArgs.Empty);
    }

    // 触发 Tick 事件的保护方法
    protected virtual void OnTick(EventArgs e)
    {
        Tick?.Invoke(this, e);
    }

    // 触发 Alarm 事件的保护方法
    protected virtual void OnAlarm(EventArgs e)
    {
        Alarm?.Invoke(this, e);
    }
}

class Program
{
    static void Main(string[] args)
    {
        // 创建闹钟，设置 5 秒后响铃
        AlarmClock clock = new AlarmClock(5);

        // 订阅 Tick 事件（走时提示）
        clock.Tick += (sender, e) =>
        {
            Console.WriteLine($"嘀嗒... 当前时间：{DateTime.Now:HH:mm:ss}");
        };

        // 订阅 Alarm 事件（响铃提示）
        clock.Alarm += (sender, e) =>
        {
            Console.WriteLine("\n🔔 叮铃铃！闹钟响了！时间到啦！");
        };

        // 启动闹钟
        clock.Start();

        Console.WriteLine("\n按任意键退出...");
        Console.ReadKey();
    }
}