using UnityEngine;

public class FogOfWarRenderer : MonoBehaviour
{
    private static readonly int FOG_TEXTURE_ARRAY = Shader.PropertyToID("_FogTextureArray");
    private static readonly int TILE_SIZE = Shader.PropertyToID("_TileSize");
    private static readonly int GRID_SIZE = Shader.PropertyToID("_GridSize");
    private static readonly int GRID_OFFSET = Shader.PropertyToID("_GridOffset");
    
    [Header("Main Settings")]
    [SerializeField] private float _updateSpeed = 3f;

    [Header("Visibility")]
    [SerializeField] private float _noVision = 0f;
    [SerializeField] private float _explored = 0.1f;
    [SerializeField] private float _visible = 1f;
    
    private int _sizeX;
    private int _sizeZ;
    private int _floors;

    private Texture2DArray _fogTextureArray;

    private Color[][] _buffers;
    private Color[][] _finalBuffers;

    public void Initialize()
    {
        _sizeX = GridParameters.LevelGrid.SizeX;
        _sizeZ = GridParameters.LevelGrid.SizeZ;
        _floors = GridParameters.LevelGrid.Floors;

        _fogTextureArray = new Texture2DArray(
            _sizeX,
            _sizeZ,
            _floors,
            TextureFormat.RGBA32,
            false,
            true);

        _fogTextureArray.filterMode = FilterMode.Bilinear;
        _fogTextureArray.wrapMode = TextureWrapMode.Clamp;
        
        _buffers = new Color[_floors][];
        _finalBuffers = new Color[_floors][];

        for (int f = 0; f < _floors; f++)
        {
            _buffers[f] = new Color[_sizeX * _sizeZ];
            _finalBuffers[f] = new Color[_sizeX * _sizeZ];
        }

        ClearTexture();

        Shader.SetGlobalTexture(FOG_TEXTURE_ARRAY, _fogTextureArray);

        Shader.SetGlobalVector(
            TILE_SIZE,
            new Vector4(
                GridParameters.TILE_SIZE,
                GridParameters.LEVEL_HEIGHT,
                GridParameters.TILE_SIZE
                )
            );

        Shader.SetGlobalVector(GRID_SIZE, new Vector3(_sizeX, _floors, _sizeZ));

        Shader.SetGlobalVector(
            GRID_OFFSET,
            GridParameters.LevelGrid.transform.position - Vector3.forward * 0.5f - Vector3.right * 0.5f
            );
    }
    
    public void UpdateFog(FloorData[] gridData)
    {
        for (int f = 0; f < _floors; f++)
        {
            Color[] buffer = _buffers[f];

            for (int x = 0; x < _sizeX; x++)
            {
                for (int z = 0; z < _sizeZ; z++)
                {
                    int index = x + z * _sizeX;

                    if (f >= gridData.Length ||
                        x >= gridData[f].Length ||
                        z >= gridData[f][x].Length)
                    {
                        buffer[index] = HiddenColor();
                        continue;
                    }

                    GridTile tile = gridData[f][x][z];

                    buffer[index] = GetVisibilityColor(tile.Visibility);
                }
            }
        }
    }

    private float _accum;

    private void Update()
    {
        FogOfWarUtility.CheckForced();
        
        _accum += Time.deltaTime;

        if (_accum < 0.033f)
            return;

        _accum = 0f;

        for (int f = 0; f < _floors; f++)
        {
            for (int k = 0; k < _finalBuffers[f].Length; k++)
            {
                _finalBuffers[f][k] = Color.Lerp(_finalBuffers[f][k], _buffers[f][k], _updateSpeed * Time.deltaTime);
            }

            _fogTextureArray.SetPixels(_finalBuffers[f], f);
        }

        _fogTextureArray.Apply(false, false);
    }

    private void ClearTexture()
    {
        Color hidden = HiddenColor();

        for (int f = 0; f < _floors; f++)
        {
            Color[] buffer = _buffers[f];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = hidden;
            }

            _fogTextureArray.SetPixels(buffer, f);
        }

        _fogTextureArray.Apply(false, false);
    }

    private Color GetVisibilityColor(TileVisibility visibility)
    {
        return visibility switch
        {
            TileVisibility.Hidden =>
                HiddenColor(),

            TileVisibility.Explored =>
                new Color(0.5f, 0.5f, 0.5f, _explored),

            TileVisibility.Visible =>
                new Color(1f, 1f, 1f, _visible),

            _ => Color.magenta
        };
    }

    private Color HiddenColor()
    {
        return new Color(0f, 0f, 0f, _noVision);
    }
}