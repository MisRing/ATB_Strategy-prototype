using UnityEngine;
using UnityEngine.UI;

public class FogOfWarRenderer : MonoBehaviour
{
    private int _sizeX = 128;
    private int _sizeZ = 128;

    private Texture2D _fogTexture;

    [Header("Visibility")]
    [SerializeField] private float _noVision = 0f;
    [SerializeField] private float _explored = 0.1f;
    [SerializeField] private float _visible = 1f;
    
    public void Initialize()
    { 
        _sizeX = GridParameters.LevelGrid.SizeX;
        _sizeZ = GridParameters.LevelGrid.SizeZ;
        _fogTexture = new Texture2D(_sizeX, _sizeZ, TextureFormat.RGBA32, false);

        _fogTexture.filterMode = FilterMode.Bilinear;
        _fogTexture.wrapMode = TextureWrapMode.Clamp;

        ClearTexture();

        Shader.SetGlobalTexture("_Fog_Texture", _fogTexture);
        Shader.SetGlobalVector("_TileSize", new Vector4(GridParameters.TILE_SIZE,  GridParameters.LEVEL_HEIGHT, GridParameters.TILE_SIZE));
        Shader.SetGlobalVector("_GridOffset", GridParameters.LevelGrid.transform.position);

    }

    // =====================================================
    // UPDATE
    // =====================================================

    public void UpdateFog(FloorData floorData)
    {
        for (int x = 0; x < _sizeX; x++)
        {
            for (int z = 0; z < _sizeZ; z++)
            {
                if (x >= floorData.Length || z >= floorData[x].Length)
                {
                    _fogTexture.SetPixel(x, z, new Vector4(0f, 0f, 0f, _noVision));
                    continue;
                }
                GridTile tile = floorData[x][z];

                Color color = GetVisibilityColor(tile.Visibility);

                _fogTexture.SetPixel(x, z, color);
            }
        }

        _fogTexture.Apply();
    }

    // =====================================================
    // HELPERS
    // =====================================================

    private Color GetVisibilityColor(TileVisibility visibility)
    {
        return visibility switch
        {
            TileVisibility.Hidden => new Vector4(0f, 0f, 0f, _noVision),
            TileVisibility.Explored => new Vector4(0.5f, 0.5f, 0.5f, _explored),
            TileVisibility.Visible => new Vector4(1f, 1f, 1f, _visible),
            _ => Color.magenta
        };
    }

    private void ClearTexture()
    {
        Color[] pixels = new Color[_sizeX * _sizeZ];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Vector4(0f, 0f, 0f, _noVision);
        }

        _fogTexture.SetPixels(pixels);

        _fogTexture.Apply();
    }
}