using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
struct HomingJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<Vector3> transforms;
    public Vector3 targetPosition;
    public float speed;
    public float deltaTime;
    public NativeArray<Vector2> positions;
    public NativeArray<Vector2> directions;
    public NativeArray<Vector2> newPositions;

    public void Execute(int index)
    {
        Vector2 position = transforms[index];
        Vector2 direction = ((Vector2)targetPosition - position).normalized;

        positions[index] = position;
        directions[index] = direction;

        newPositions[index] = position + direction * speed * deltaTime;
    }
}