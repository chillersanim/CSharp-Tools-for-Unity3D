using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity_Tools.Components;
using Unity_Tools.Core;
using UnityEngine;

namespace Unity_Tools.Examples.Code_Examples
{
    public static class Components
    {
// Schedules raycasts on the main thread.
public static void DoParallelRaycast(IList<Ray> rays, float maxDistance, LayerMask layermask, List<RaycastHit> output)
{
    // Create native arrays for commands and results
    NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(rays.Count, Allocator.Persistent);
    NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(rays.Count, Allocator.Persistent);

    // Create a raycast job on the main thread
    Task<JobHandle> task = MainThreadDispatch.Invoke(() =>
    {
        // Fill the commands array with the raycast commands
        for (int i = 0; i < rays.Count; i++)
        {
            Ray ray = rays[i];
            commands[i] = new RaycastCommand(ray.origin, ray.direction, maxDistance, layermask);
        }

        // Schedule the raycast
        JobHandle result = RaycastCommand.ScheduleBatch(commands, results, 1);

        // Return the handle
        return result;
    });

    // Wait for the raycast job to complete and populate the output list
    MainThreadDispatch.Await(
        () => task.IsCompleted,   // Determines whether the job has completed
        () => 
        {
            // Make sure the job has completed (Required by Unity)
            JobHandle jobHandle = task.Result;
            jobHandle.Complete();

            // Add the result to the output
            output.AddRange(results);

            // Dispose the native arrays
            results.Dispose();
            commands.Dispose();
        });
}
    }
}
