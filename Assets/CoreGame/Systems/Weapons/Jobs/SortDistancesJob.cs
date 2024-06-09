using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
struct SortDistancesJob : IJob
{
    [ReadOnly] public NativeArray<float> Distances;
    public NativeArray<int> Indices;

    public void Execute()
    {
        QuickSort(0, Indices.Length - 1);
    }

    void QuickSort(int left, int right)
    {
        int i = left, j = right;
        float pivot = Distances[Indices[(left + right) / 2]];

        while (i <= j)
        {
            while (Distances[Indices[i]] < pivot) i++;
            while (Distances[Indices[j]] > pivot) j--;

            if (i <= j)
            {
                int tmp = Indices[i];
                Indices[i] = Indices[j];
                Indices[j] = tmp;
                i++;
                j--;
            }
        }

        if (left < j) QuickSort(left, j);
        if (i < right) QuickSort(i, right);
    }
}
