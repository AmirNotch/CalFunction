using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

public class Suffix
{
    public int Index { get; set; }
    public int[] Rank { get; set; } = new int[2];
}

public class SuffixComparer : IComparer<Suffix>
{
    public int Compare(Suffix x, Suffix y)
    {
        if (x.Rank[0] != y.Rank[0])
            return x.Rank[0].CompareTo(y.Rank[0]);
        else
            return x.Rank[1].CompareTo(y.Rank[1]);
    }
}

public class Program
{
    public static int[] BuildSuffixArray(string txt, int n)
    {
        var suffixes = Enumerable.Range(0, n).Select(_ => new Suffix()).ToArray();
        for (int i = 0; i < n; i++)
        {
            suffixes[i].Index = i;
            suffixes[i].Rank[0] = txt[i] - 'a';
            suffixes[i].Rank[1] = (i + 1 < n) ? txt[i + 1] - 'a' : -1;
        }

        suffixes = suffixes.OrderBy(x => x, new SuffixComparer()).ToArray();

        int[] ind = new int[n];
        int k = 4;
        while (k < 2 * n)
        {
            int rank = 0;
            int prevRank = suffixes[0].Rank[0];
            suffixes[0].Rank[0] = rank;
            ind[suffixes[0].Index] = 0;
            for (int i = 1; i < n; i++)
            {
                if (suffixes[i].Rank[0] == prevRank && suffixes[i].Rank[1] == suffixes[i - 1].Rank[1])
                {
                    prevRank = suffixes[i].Rank[0];
                    suffixes[i].Rank[0] = rank;
                }
                else
                {
                    prevRank = suffixes[i].Rank[0];
                    rank++;
                    suffixes[i].Rank[0] = rank;
                }
                ind[suffixes[i].Index] = i;
            }
            for (int i = 0; i < n; i++)
            {
                int nextIndex = suffixes[i].Index + k / 2;
                suffixes[i].Rank[1] = (nextIndex < n) ? suffixes[ind[nextIndex]].Rank[0] : -1;
            }
            suffixes = suffixes.OrderBy(x => x, new SuffixComparer()).ToArray();
            k *= 2;
        }
        int[] suffixArr = new int[n];
        for (int i = 0; i < n; i++)
            suffixArr[i] = suffixes[i].Index;
        
        return suffixArr;
    }

    public static int[] LCP(string t, int[] suffixArr)
    {
        int n = suffixArr.Length;
        int[] lcpArr = new int[n];
        int[] invStuff = new int[n];
        for (int i = 0; i < n; i++)
            invStuff[suffixArr[i]] = i;
        
        int k = 0;
        for (int i = 0; i < n; i++)
        {
            if (invStuff[i] == n - 1)
            {
                k = 0;
                continue;
            }
            int j = suffixArr[invStuff[i] + 1];
            while (i + k < n && j + k < n && t[i + k] == t[j + k])
                k++;
            
            lcpArr[invStuff[i]] = k;
            if (k > 0)
                k--;
        }
        return lcpArr;
    }

    public static int MaxValue(string t)
    {
        int[] sArr = BuildSuffixArray(t, t.Length);
        int[] lcp = LCP(t, sArr);
        int bestResults = t.Length;
        for (int i = 1; i < t.Length; i++)
        {
            int cnt = 2;
            for (int j = i - 1; j >= 0; j--)
            {
                if (lcp[j] >= lcp[i])
                    cnt++;
                else
                    break;
            }
            for (int j = i + 1; j < t.Length; j++)
            {
                if (lcp[j] >= lcp[i])
                    cnt++;
                else
                    break;
            }
            bestResults = Math.Max(bestResults, cnt * lcp[i]);
        }
        return bestResults;
    }

    public static void Main(string[] args)
    {
        string t = Console.ReadLine();
        int result = MaxValue(t);
        Console.WriteLine(result);
    }
}
