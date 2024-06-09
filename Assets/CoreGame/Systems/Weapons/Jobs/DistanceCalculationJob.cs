using ProjTha;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
struct DistanceCalculationJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<TransformData> Transforms;
    public NativeArray<float> Distances;
    public Vector3 TargetPosition;

    public void Execute(int index)
    {
        //TODO Replace with DistanceSqr instead of Distance
        Distances[index] = Vector3.Distance(Transforms[index].Position, TargetPosition);
    }
}