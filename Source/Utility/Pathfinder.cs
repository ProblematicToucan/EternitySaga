using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.Pathfinding;
using Nez.Tiled;

namespace EternitySaga.Utils;
/// <summary>
/// Pathfinder utility <see cref="RenderableComponent"/> / 
/// <seealso cref="Component"/> using <see cref="AstarGridGraph"/> algorithm.
/// <inheritdoc/>
/// </summary>
public class Pathfinder : RenderableComponent
{
    // make sure we arent culled when debug
    public override float Width => 20;
    public override float Height => 20;
    private TmxMap _tilemap;
    private AstarGridGraph _astarGraph;
    private List<Point> _astarSearchPath;
    private List<Point> _lastAstarSearchPath;
    private Point GetWorldPosition(Vector2 vector2) => _tilemap.WorldToTilePosition(vector2);

    /// <inheritdoc cref="Pathfinder" path="/summary"/>
    public Pathfinder() { }

    public override void OnAddedToEntity()
    {
        base.OnAddedToEntity();
        LocalOffset = new(Width / -2, Height / -2);
        _tilemap = Entity.Scene.FindComponentOfType<TiledMapRenderer>().TiledMap;
        var layer = _tilemap.GetLayer<TmxLayer>("path");
        _astarGraph = new(layer);
        _astarSearchPath = new();
        _lastAstarSearchPath = _astarSearchPath;
    }

    /// <summary>Find path from
    /// <see cref="Vector2"/>start point to
    /// <see cref="Vector2"/>end point
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    public List<Vector2> SearchPath(Vector2 start, Vector2 end)
    {
        var startNode = GetWorldPosition(start);
        var endNode = GetWorldPosition(end);
        _astarSearchPath = _astarGraph.Search(startNode, endNode);
        if (_astarSearchPath == null) return null;
        _lastAstarSearchPath = _astarSearchPath;
        return ScaledAstarSearchPath(_astarSearchPath);
    }

    public override void DebugRender(Batcher batcher)
    {
        if (!DebugRenderEnabled) return;
        base.DebugRender(batcher);
        foreach (var node in ScaledAstarSearchPath(_lastAstarSearchPath))
        {
            batcher.DrawPixel(node.X, node.Y, Color.Orange, 4);
        }
    }

    private List<Vector2> ScaledAstarSearchPath(List<Point> astarSearchPath)
    {
        var scaledAstarSearchPath = astarSearchPath.Select(point =>
            new Vector2(
                point.X * _tilemap.TileWidth + _tilemap.TileWidth * 0.5f,
                point.Y * _tilemap.TileHeight + _tilemap.TileHeight * 0.5f))
                .ToList();
        return scaledAstarSearchPath;
    }

    public override void Render(Batcher batcher, Camera camera) { }
}