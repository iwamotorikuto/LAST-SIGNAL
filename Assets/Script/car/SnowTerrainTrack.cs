using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SnowTerrainTrack : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Terrain targetTerrain;
    [SerializeField] private Transform rightTrackPoint;
    [SerializeField] private Transform leftTrackPoint;

    [Header("タイヤの跡")]
    [SerializeField] private float trackWidth = 0.6f;
    [SerializeField] private float stampSpacing = 0.18f;

    [Header("土を塗る")]
    [SerializeField] private int dirtLayerIndex = 1;    
    [Range(0.0f, 1.0f)]
    [SerializeField] private float dirtStrength = 0.8f; //土の濃さ

    [Header("Raycast")]
    [SerializeField] private float rayStartHeight = 1.5f;
    [SerializeField] private float rayDistance = 4.0f;

    private TerrainData terrainData;
    private TerrainCollider terrainCollider;

    private Vector3 lastRightPosition;
    private Vector3 lastLeftPosition;

    //各マスの元の高さと今のへこみ具合(0～1で)
    private readonly Dictionary<int, float> originalHeights = new Dictionary<int, float>();
    private readonly Dictionary<int, float> dentWeights = new Dictionary<int, float>();

    private void Start()
    {
        if (!CheckReference()) return;

        terrainData = targetTerrain.terrainData;
        terrainCollider = targetTerrain.GetComponent<TerrainCollider>();

        if(terrainCollider == null)
        {
            enabled = false;
            return;
        }

        //最初の接地点の記録
        if(!TryGetGround(leftTrackPoint, out lastLeftPosition) || !TryGetGround(rightTrackPoint, out lastRightPosition))
        {
            enabled = false;
        }
    }

    private void Update()
    {
       UpdateWheel(leftTrackPoint, ref lastLeftPosition);
       UpdateWheel(rightTrackPoint, ref lastRightPosition);
    }

    private bool CheckReference()
    {
        if(targetTerrain != null && leftTrackPoint != null && rightTrackPoint != null)
        {
            return true;
        }

        enabled = false;
        return false;
    }

    //タイヤの真上からRayを飛ばして、地面の位置を確認する
    private bool TryGetGround(Transform point, out Vector3 groundPosition)
    {
        Ray ray = new Ray(point.position + Vector3.up * rayStartHeight, Vector3.down);

        if (terrainCollider.Raycast(ray, out RaycastHit hit, rayStartHeight + rayDistance))
        {
            groundPosition = hit.point;
            return true;
        }

        groundPosition = Vector3.zero;
        return false;
    }

    //前回の位置から一定距離進んでいた場合は、間を埋めるようにスタンプを押す
    private void UpdateWheel(Transform point, ref Vector3 lastPosition)
    {
        if (!TryGetGround(point, out Vector3 current)) return;

        Vector3 move = current - lastPosition;
        move.y = 0.0f;

        if (move.magnitude < stampSpacing) return;

        int steps = Mathf.FloorToInt(move.magnitude / stampSpacing);
        Vector3 direction = move.normalized;

        for (int i = 1; i <= steps; i++)
        {
            PaintDirt(lastPosition + direction * stampSpacing * i);
        }

        //押した分だけ前回の位置を進める
        lastPosition += direction * stampSpacing * steps;
    }

    //指定した位置を中心に、タイヤの跡となる円形のへこみを作る
    private void PaintDirt(Vector3 worldPosition)
    {
        if (!WorldToAlphamap(worldPosition, out int centerX, out int centerZ)) return;

        int resolution = terrainData.alphamapResolution;
        float cellX = terrainData.size.x / resolution;
        float cellZ = terrainData.size.z / resolution;
        float radius = trackWidth * 0.5f;

        //変更する範囲
        int radiusX = Mathf.Max(1, Mathf.CeilToInt(radius / cellX));
        int radiusZ = Mathf.Max(1, Mathf.CeilToInt(radius / cellZ));

        int startX = Mathf.Max(0, centerX - radiusX);
        int startZ = Mathf.Max(0, centerZ - radiusZ);
        int endX = Mathf.Min(resolution - 1, centerX + radiusX);
        int endZ = Mathf.Min(resolution - 1, centerZ + radiusZ);

        int width = endX - startX + 1;
        int height = endZ - startZ + 1;

        float[,,] maps = terrainData.GetAlphamaps(startX, startZ, width, height);
        int layerCount = terrainData.alphamapLayers; // 0～1の高さに変換

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (startX + x - centerX) * cellX;
                float dz = (startZ + z - centerZ) * cellZ;
                float distance = Mathf.Sqrt(dx * dx + dz * dz);

                if (distance >= radius) continue;

                //　中心ほど深く、縁は滑らかになり浅くなる
                float weight = 1.0f - Mathf.SmoothStep(0.55f, 1.0f, distance / radius);
                float target = weight * dirtStrength;

                //土は今よりも薄くはならない
                float oldDirt = maps[z, x, dirtLayerIndex];
                float newDirt = Mathf.Max(oldDirt, target);

                //変化が無いなら何もしない
                if (newDirt <= oldDirt) continue;

                //土以外のレイヤーを縮めて、合計を1に保つようにする　
                float otherSum = 1.0f - oldDirt;
                if (otherSum <= 0.0001f) continue;  //0で割らないように防ぐ

                float scale = (1.0f - newDirt) / otherSum;

                for(int layer = 0; layer < layerCount; layer++)
                {
                    if (layer == dirtLayerIndex) continue;
                    maps[z, x, layer] *= scale;
                }

                maps[z, x, dirtLayerIndex] = newDirt;
            }
        }

        terrainData.SetAlphamaps(startX, startZ, maps);
    }

    //ワールド座標　→ Heightmapのマス番号に変換を加える
    private bool WorldToAlphamap(Vector3 worldPosition, out int mapX, out int mapZ)
    {
        Vector3 local = targetTerrain.transform.InverseTransformPoint(worldPosition);
        float nx = local.x / terrainData.size.x;
        float nz = local.z / terrainData.size.z;

        mapX = 0;
        mapZ = 0;

        //Terrainの範囲外の場合は何もしない
        if (nx < 0.0f || nx > 1.0f || nz < 0.0f || nz > 1.0f) return false;

        int resolution = terrainData.alphamapResolution;
        mapX = Mathf.Min(resolution - 1, Mathf.FloorToInt(nx * resolution));
        mapZ = Mathf.Min(resolution - 1, Mathf.FloorToInt(nz * resolution));
        return true;
    }


}
