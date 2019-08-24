using UnityEngine;

namespace Unity_Tools.Core
{
    public static class CameraUtil
    {
        /// <summary>
        /// Gets a point outside the camera frustum so that the given bounds are next to the frustum and the point has an approximate distance to the camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="size">The size of the object to place in world coordinates. For no no size, use <see cref="Vector3.zero"/>.</param>
        /// <param name="distance">The target distance between the camera and the resulting point.</param>
        /// <param name="horizontalPosition">The horizontal position in the camera space. Range: [0, 1]</param>
        /// <param name="verticalPosition">The vertical position in the camera space. Range: [0, 1]</param>
        public static Vector3 GetPointOutsideFrustum(this Camera camera, Vector3 size, float distance, float horizontalPosition, float verticalPosition)
        {
            horizontalPosition = Mathf.Clamp01(horizontalPosition);
            verticalPosition = Mathf.Clamp01(verticalPosition);

            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);
            var leftNormal = frustumPlanes[0].normal;
            var rightNormal = frustumPlanes[1].normal;
            var downNormal = frustumPlanes[2].normal;
            var upNormal = frustumPlanes[3].normal;
            var offsetDirection = -(leftNormal * (1 - horizontalPosition) + rightNormal * horizontalPosition +
                                    upNormal * (1 - verticalPosition) + downNormal * verticalPosition);

            var screenPos = new Vector3((camera.pixelWidth - 1) * horizontalPosition,
                (camera.pixelHeight - 1) * verticalPosition);
            var ray = camera.ScreenPointToRay(screenPos);
            var borderPoint = ray.GetPoint(distance);
            
            var halfDiameter = size.magnitude / 2f;
            return borderPoint + offsetDirection * halfDiameter;
        }
    }
}
