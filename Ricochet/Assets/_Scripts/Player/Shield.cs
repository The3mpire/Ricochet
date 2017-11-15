using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D), typeof(LineRenderer))]
public class Shield : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private PlayerController playerController;

    [SerializeField] private int numberOfPoints;
    [SerializeField] private Vector2 startPoint;
    [SerializeField] private Vector2 endPoint;
    [SerializeField] private Vector2 startPointHandler;
    [SerializeField] private Vector2 endPointHandler;

    [SerializeField] private float lineRendererWidth;
    [SerializeField] private Gradient lineRendererColor;

    [SerializeField] private Material lineRendererMaterial;
    [SerializeField] private float colliderEdgeRadius;
    [SerializeField] private PhysicsMaterial2D colliderMaterial;
    #endregion

    #region Hidden Variables
    private EdgeCollider2D edgeCollider;
    private LineRenderer lineRenderer;
    #endregion

    #region Monobehaviour
    private void Awake()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }
    #endregion

    #region Public Functions
    public void BuildShield(int numOfPoints, Vector2 start, Vector2 end, Vector2 startHandler, Vector2 endHandler)
    {
        if (numOfPoints > 2)
        {
            numberOfPoints = numOfPoints;
        }
        startPoint = start;
        startPointHandler = startHandler;
        endPoint = end;
        endPointHandler = endHandler;
        RefreshShield();
    }

    public void RefreshShield()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        edgeCollider.edgeRadius = colliderEdgeRadius;
        edgeCollider.sharedMaterial = colliderMaterial;
        lineRenderer.sharedMaterial = lineRendererMaterial;
        lineRenderer.widthMultiplier = lineRendererWidth;
        lineRenderer.colorGradient = lineRendererColor;

        if (numberOfPoints < 2 || startPoint == endPoint)
        {
            numberOfPoints = 2;
            return;
        }

        edgeCollider.points = Calculate2DPoints();
        Vector3[] points = new Vector3[edgeCollider.points.Length];
        for (int i = 0; i < edgeCollider.points.Length; i++)
        {
            points[i] = new Vector3(edgeCollider.points[i].x, edgeCollider.points[i].y, 0);
        }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
    }
    #endregion

    #region Private Functions
    private Vector2 CalculateBezierPoint(float t, Vector2 startPoint, Vector2 endPoint, Vector2 startPointHandler, Vector2 endPointHandler)
    {
        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = t * tt;

        Vector2 p = uuu * startPoint;
        p += 3f * uu * t * startPointHandler;
        p += 3f * u * tt * endPointHandler;
        p += ttt * endPoint;

        return p;
    }

    private Vector2[] Calculate2DPoints()
    {
        Vector2[] points = new Vector2[numberOfPoints];

        points[0] = startPoint;
        for (int i = 1; i < numberOfPoints - 1; i++)
        {
            points[i] = CalculateBezierPoint((1f / (numberOfPoints - 1) * i), startPoint, endPoint, startPointHandler, endPointHandler);
        }
        points[numberOfPoints - 1] = endPoint;

        return points;
    }
    #endregion

    #region Setters
    public void SetColor(Color color)
    {
        Gradient g = new Gradient();
        GradientColorKey[] gck = new GradientColorKey[2];
        GradientAlphaKey[] gak = new GradientAlphaKey[2];

        gck[0].color = color;
        gck[0].time = 0.0F;
        gck[1].color = color;
        gck[1].time = 1.0F;

        gak[0].alpha = 1.0F;
        gak[0].time = 0.0F;
        gak[1].alpha = 1.0F;
        gak[1].time = 1.0F;

        lineRenderer.colorGradient.SetKeys(gck, gak);
    }

    public void SetMaterial(Material material)
    {
        lineRenderer.sharedMaterial = lineRendererMaterial;
    }

    public void SetColorAndMaterial(Gradient colorGradient, Material material)
    {
        lineRenderer.sharedMaterial = lineRendererMaterial;
        lineRenderer.colorGradient = lineRendererColor;
    }
    #endregion

    #region Getters
    public PlayerController GetPlayer()
    {
        return playerController;
    }
    public Vector2 GetStartPoint()
    {
        return startPoint;
    }
    public Vector2 GetEndPoint()
    {
        return endPoint;
    }
    public Vector2 GetStartPointHandler()
    {
        return startPointHandler;
    }
    public Vector2 GetEndPointHandler()
    {
        return endPointHandler;
    }
    #endregion
}
