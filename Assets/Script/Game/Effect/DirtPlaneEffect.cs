using UnityEngine;

// 外部依存
using Brush = Es.InkPainter.Brush;
using InkCanvas = Es.InkPainter.InkCanvas;

public class DirtPlaneEffect : System.IDisposable
{
  private Camera _renderCam;
  private float _distanceToCam = 10f;
  private GameObject _dirtPlane;
  private Mesh _dirtPlaneMesh;
  private MeshRenderer _planeRenderer;
  private InkCanvas _inkCanvas; 
  private Brush _dirtBrush;
  private Vector3 _dirtDefaultScale;
  private bool disposedValue;
  public float DirtScale
  {
    get => _dirtBrush.Scale;
    set => _dirtBrush.Scale = value;
  }
  public Color DirtBrushColor
  {
    get => _dirtBrush.Color;
    set => _dirtBrush.Color = value;
  }

  public DirtPlaneEffect(Camera renderCam)
  {
    Debug.Assert(renderCam != null);

    _renderCam = renderCam;

    _dirtBrush = new Brush(null, 0, Color.clear);
    
    var prefab = Resources.Load<GameObject>("Prefabs/DirtPlane");
    _dirtPlane = Object.Instantiate(prefab);

    SetupMesh();

    _planeRenderer = _dirtPlane.GetComponent<MeshRenderer>();
    _planeRenderer.sortingLayerID = SortingLayer.NameToID("Dirt");
    _inkCanvas = _dirtPlane.GetComponent<InkCanvas>();

    if (_inkCanvas == null)
    {
      throw new System.ArgumentNullException("ink canvas");
    }

    if (_planeRenderer == null)
    {
      throw new System.ArgumentNullException("mesh renderer");
    }

    _dirtPlane.SetActive(false);

    AdjustPlanePosAndRotToFillScreen();

    _dirtDefaultScale = _dirtPlane.transform.localScale;
    _dirtPlane.transform.SetParent(_renderCam.transform);
    
  }

  public DirtPlaneEffect(Camera renderCam, Texture brushTex, float brushScale, Color brushColor)
    :this(renderCam)
  {
    _dirtBrush.BrushTexture = brushTex;
    _dirtBrush.Scale = brushScale;
    _dirtBrush.Color = brushColor;
  }

  public void PaintUV(Vector2[] paintPos)
  {
    if (paintPos == null)
    {
      return;
    }

    _dirtPlane.SetActive(true);

    foreach (var pos in paintPos)
    {
      _inkCanvas.PaintUVDirect(_dirtBrush, pos);
    }
  }

  public void ResetDirt()
  {
    _dirtPlane.SetActive(false);
    _inkCanvas.ResetPaint();
  }

  public void Update()
  {
    AdjustPlaneScaleToFillScreen();
  }

  private void AdjustPlanePosAndRotToFillScreen()
  { 
    if (_renderCam == null)
    {
      Debug.LogWarning("camera is invalid");
      return;
    }

    if (!_renderCam.orthographic)
    {
      var dirtPlaneHeight = _dirtPlaneMesh.bounds.size.y;
      _distanceToCam = dirtPlaneHeight * 0.5f / Mathf.Tan(_renderCam.fieldOfView / (2f * Mathf.Rad2Deg));
      _dirtPlane.transform.position = _renderCam.transform.position + _renderCam.transform.forward * _distanceToCam;
      _dirtPlane.transform.up = _renderCam.transform.up;
    }
    else
    {
      throw new System.NotImplementedException();
    }
  }

  private void SetupMesh()
  {
    _dirtPlaneMesh = new Mesh();
    _dirtPlaneMesh.name = "PlaneMesh";

    float halfHeight = 1f;
    float halfWidth = halfHeight * _renderCam.aspect;

    System.Span<Vector3> vertices = stackalloc Vector3[4];
    System.Span<Vector2> uv = stackalloc Vector2[4];
    System.Span<int> triangles = stackalloc int[6];

    vertices[0].x = -halfWidth;
    vertices[0].y = -halfHeight;
    vertices[0].z = 0f;
    uv[0].x = 0f;
    uv[0].y = 0f;

    vertices[1].x = -halfWidth;
    vertices[1].y = halfHeight;
    vertices[1].z = 0f;
    uv[1].x = 0f;
    uv[1].y = 1f;

    vertices[2].x = halfWidth;
    vertices[2].y = halfHeight;
    vertices[2].z = 0f;    
    uv[2].x = 1f;
    uv[2].y = 1f;

    vertices[3].x = halfWidth;
    vertices[3].y = -halfHeight;
    vertices[3].z = 0f;
    uv[3].x = 1f;
    uv[3].y = 0f;

    triangles[0] = 0;
    triangles[1] = 1;
    triangles[2] = 3;
    triangles[3] = 1;
    triangles[4] = 2;
    triangles[5] = 3;

    _dirtPlaneMesh.SetVertices(vertices.ToArray());
    _dirtPlaneMesh.SetUVs(0, uv.ToArray());
    _dirtPlaneMesh.SetTriangles(triangles.ToArray(), 0);

    _dirtPlaneMesh.RecalculateBounds();
    _dirtPlaneMesh.RecalculateNormals();
    
    _dirtPlane.GetComponent<MeshFilter>().sharedMesh = _dirtPlaneMesh;
    _dirtPlane.GetComponent<MeshCollider>().sharedMesh = _dirtPlaneMesh;

  }

  private void AdjustPlaneScaleToFillScreen()
  {
    if (_renderCam == null)
    {
      Debug.LogWarning("camera is invalid");
      return;
    }

    var camScale = _renderCam.transform.localScale;
    var newPlaneScale = Vector3.one;

    newPlaneScale.x = Mathf.Approximately(0f, camScale.x) ? 0f : _dirtDefaultScale.x / camScale.x;
    newPlaneScale.y = Mathf.Approximately(0f, camScale.y) ? 0f : _dirtDefaultScale.y / camScale.y;
    newPlaneScale.z = Mathf.Approximately(0f, camScale.z) ? 0f : _dirtDefaultScale.z / camScale.z;

    _dirtPlane.transform.localScale = newPlaneScale;
  }
  protected virtual void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      if (disposing)
      {
        Object.Destroy(_dirtPlaneMesh);
        Object.Destroy(_dirtPlane);
      }

      disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    System.GC.SuppressFinalize(this);
  }
}