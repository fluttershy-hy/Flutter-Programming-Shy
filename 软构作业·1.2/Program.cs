using System;

namespace PrimeNumberDemo
{
    class Program
    {
        // 判断一个数是否为素数
        static bool IsPrime(int n)
        {
            if (n <= 1)
                return false;
            if (n == 2)
                return true;
            if (n % 2 == 0)
                return false;

            for (int i = 3; i * i <= n; i += 2)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }

        static void Main(string[] args)
        {
            Console.Write("请输入下限：");
            int lower = int.Parse(Console.ReadLine());

            Console.Write("请输入上限：");
            int upper = int.Parse(Console.ReadLine());

            int count = 0; // 计数，控制每10个换行

            Console.WriteLine($"\n{lower}到{upper}之间的素数：");
            for (int i = lower; i <= upper; i++)
            {
                if (IsPrime(i))
                {
                    Console.Write(i + "\t");
                    count++;

                    // 每输出10个素数，换一行
                    if (count % 10 == 0)
                    {
                        Console.WriteLine();
                    }
                }
            }

         
            if (count % 10 != 0)
            {
                Console.WriteLine();
            }
        }
    }
}