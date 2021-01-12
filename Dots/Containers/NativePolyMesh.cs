using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityTools.Dots.Containers.Internal;

namespace UnityTools.Dots.Containers
{
    [BurstCompile]
    [NativeContainer]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativePolyMesh : IDisposable
    {
        #region Private Fields
        
        /// <summary>
        /// Pointer to the data location in memory.
        /// </summary>
        [NoAlias]
        [NativeDisableUnsafePtrRestriction]
        private NativePolyMeshData* data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;

        [NativeSetClassTypeToNullOnSchedule]
        internal DisposeSentinel m_DisposeSentinel;
#endif

        #endregion

        #region Constructors

        public NativePolyMesh(in Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            // Native allocation is only valid for Temp, Job and Persistent
            if (allocator != Allocator.Temp && allocator != Allocator.TempJob && allocator != Allocator.Persistent)
                throw new ArgumentException("Allocator must be Temp, TempJob or Persistent", nameof(allocator));
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out this.m_Safety, out this.m_DisposeSentinel, 0, allocator);
#endif
            
            this.data = (NativePolyMeshData*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<NativePolyMeshData>(),
                UnsafeUtility.AlignOf<NativePolyMeshData>(), allocator);
            this.data->Initialize(allocator);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (this.data == null)
                throw new ObjectDisposedException(
                    $"The {nameof(NativePolyMesh)} has already been disposed (or not yet been initialized).");
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref this.m_Safety, ref this.m_DisposeSentinel);
#endif

            UnsafeUtility.Free(this.data, this.data->allocator);
            this.data = null;
        }

        #endregion
    }
}