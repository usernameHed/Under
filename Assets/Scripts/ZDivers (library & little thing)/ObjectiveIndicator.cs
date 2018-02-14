using UnityEngine;
using UnityEngine.UI;

public class ObjectiveIndicator : MonoBehaviour 
{
    private static Camera usedCamera;
    public static Camera UsedCamera
    {
        get { return ObjectiveIndicator.usedCamera; }
        set { ObjectiveIndicator.usedCamera = value; }
    }

    private static Canvas usedCanvas;
    public static Canvas UsedCanvas
    {
        get { return ObjectiveIndicator.usedCanvas; }
        set { ObjectiveIndicator.usedCanvas = value; }
    }

    public int canvasSortOrder = 0;

    private Transform _trans;
    
    public enum DrawMode { OnGUI, UICanvas };
    private enum FadeMode { None, FadeIn, FadeOut };
    public enum Position { Top = 0, TopRight = 45, Right = 90, BottomRight = 135, Bottom = 180, BottomLeft = 225, Left = 270, TopLeft = 315, NotClamped = 360 };
    public enum AnimMode { NoAnimation, AnimateOnScreen, AnimateOutOfScreen, AlwaysAnimate};

    private bool isActive = false;
    private bool isVisible = false;
    private FadeMode currentFadeMode = FadeMode.None;
    private float currentAlpha = 0;
    private float fadeStartTime;

    private bool isOnScreen = true;
    private Vector3 viewportPosition;

    private Image linkedImage;
    private RectTransform linkedImageTrans;

    [Header("Base parameters")]
    public bool startVisible = true;                            //démare visible !
    public bool hideWhenKinematic = true;                       //cache le pointer quand il est kinematic !
    public DrawMode usedDrawMode = DrawMode.OnGUI;
    private CameraController cc;
    [Range(0, 0.1f)]    public float timeOpti = 0.1f;
    private float timeToGo;
    public bool isInPlayer = false;
    public bool isMulti = false;                                //en multi, add up !
    public float addUp = 83f;
    public bool isOpPowerPlayer = false;
    [Range(0, 1f)]    public float opacityOpOnScreen = 0.6f;
    [Range(0, 1f)]    public float opacityOpOutScreen = 1.0f;
    private CanvasGroup cg;

    private GameManager gameManager;

    [Header("On screen parameters")]
    public bool hideOnScreen = false;
    public Sprite onScreenSprite;
    public Color onScreenTextureColor = Color.white;
    public float onScreenTextureSize = 0.1f;
    public float onScreenTextureRotation = 0;

    [Header("Out of screen parameters")]
    public bool hideOutofScreen = false;
    public Sprite outofScreenSprite;
    public Sprite [] outofScreenSpriteArray;
    private int currentPlayer = -1;
    public Color outofScreenTextureColor = Color.white;
    public float outofScreenTextureScreenPercentSize = 0.1f;
    public Vector2 outofScreenBorderOffset;
    public float outofScreenTextureRotation = 0;
    public bool outofScreenRotateTexture = true;
    public Position outofScreenBaseOrientation = Position.Top;

    [Header("Fade parameters")]
    public float fadeInDuration = 2;
    public float fadeOutDuration = 4;

    [Header("Scale parameters")]
    public float minScale = 0.25f;
    public float maxScale = 1;
    public float minDistance = 1;
    public float maxDistance = 20;
    public float scaleOnScreen = 0.6f;
    private float currentScale;

    [Header("Animation parameters")]
    public AnimMode animationMode;
    public float scaleAnimAmplitude = 0.5f;
    private bool scaleAnimUp = true;
    private float currentAnimScalePercent = 1;
    private float animVelocity;
    public float scaleAnimSmooth = 0.4f;
    private bool animateScale = false;

    void OnDestroy()
    {
        hideOutofScreen = true;
    }

    // Orientation qui dépend de la position d'un autre gameobject
    // Rotation de l'image par rapport au bord de l'écran

    void Awake()
    {
        //hideOutofScreenTmp = hideOutofScreen;
        //hideOnScreenTmp = hideOnScreen;
        //if (isOpPowerPlayer)
          //  linkedImage.gameObject.transform.Rotate(0, 0, 180);

        _trans = transform;

        if (usedCamera == null)
            usedCamera = Camera.main;
        GameObject gameController = GameObject.FindGameObjectWithTag("GameController");
        if (gameController)
            gameManager = gameController.GetComponent<GameManager>();
    }

    void Start()
    {
        if (!Camera.main)
            return;
        cc = Camera.main.transform.gameObject.GetComponent<CameraController>();
        timeToGo = Time.fixedTime + timeOpti;

        if (usedDrawMode == DrawMode.UICanvas && usedCanvas == null)
        {
            GameObject newCanvas = new GameObject("Canvas_Objective", typeof(Canvas));
            newCanvas.layer = LayerMask.NameToLayer("UI");
            newCanvas.AddComponent<CanvasScaler>();
            usedCanvas = newCanvas.GetComponent<Canvas>();
            usedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            usedCanvas.sortingOrder = canvasSortOrder;

            RectTransform canvasTrans = newCanvas.GetComponent<RectTransform>();
            canvasTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width);
            canvasTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
        }

        if (usedDrawMode == DrawMode.UICanvas)
        {
            // Generate the image
            GameObject newImage = new GameObject(name + "_Image", typeof(Image));
            newImage.transform.SetParent(usedCanvas.transform, false);
            linkedImage = newImage.GetComponent<Image>();
            linkedImageTrans = newImage.GetComponent<RectTransform>();
            linkedImageTrans.gameObject.layer = LayerMask.NameToLayer("UI");
            linkedImageTrans.anchoredPosition = new Vector2(-Screen.width, -Screen.height);
            cg = newImage.AddComponent<CanvasGroup>() as CanvasGroup;
        }

        DeactivateEndFadeOut();

        if (startVisible)
        {
            Activate();
        }
    }

    /// <summary>
    /// Show the indicator with a fade in animation
    /// </summary>
    public void Activate()
    {
        isActive = true;
        currentFadeMode = FadeMode.FadeIn;
        fadeStartTime = Time.time;
        SetVisible();
    }

    /// <summary>
    /// Hide the indicator with a fade out animation
    /// </summary>
    public void Deactivate()
    {
        currentFadeMode = FadeMode.FadeOut;
        fadeStartTime = Time.time;
    }

    /// <summary>
    /// Called when the fade out of the deactivation end to hide the objective
    /// </summary>
    private void DeactivateEndFadeOut()
    {
        isActive = false;
        SetInvisible();
    }

    /// <summary>
    /// Called when the objective have to be activated
    /// </summary>
    public void SetVisible()
    {
        isVisible = true;
        if (linkedImage != null)
            linkedImage.enabled = true;
    }

    /// <summary>
    /// Called when the objective have to be hidden
    /// </summary>
    public void SetInvisible()
    {
        isVisible = false;
        if (linkedImage != null)
            linkedImage.enabled = false;
    }

    /// <summary>
    /// Start to animate the scale
    /// </summary>
    public void StartScaleAnimation()
    {
        animateScale = true;
    }

    /// <summary>
    /// Stop scale animation
    /// </summary>
    public void StopScaleAnimation()
    {
        animateScale = false;
    }

    void Update()
    {
        if (isActive && Time.fixedTime >= timeToGo)
        {
            UpdateVisibility();

            if (isVisible)
            {
                UpdateFade();
                UpdateScale();
                UpdateScaleAnimation();
                UpdateUICanvas();
            }
            timeToGo = Time.fixedTime + timeOpti;
        }
    }

    /// <summary>
    /// Update element visibility
    /// </summary>
    private void UpdateVisibility()
    {
        viewportPosition = usedCamera.WorldToViewportPoint(_trans.position);

        if (viewportPosition.x > 1 - outofScreenBorderOffset.x || viewportPosition.x < outofScreenBorderOffset.x 
            || viewportPosition.y > 1 - outofScreenBorderOffset.y || viewportPosition.y < outofScreenBorderOffset.y 
            || viewportPosition.z <= 0)
        {
            isOnScreen = false;
            if (hideOutofScreen)
                SetInvisible();
            else
                SetVisible();
        }
        else
        {
            isOnScreen = true;
            if (hideOnScreen)
                SetInvisible();
            else
                SetVisible();
        }
    }

    /// <summary>
    /// Update the current alpha depending of the current fade mode and fade speed
    /// </summary>
    private void UpdateFade()
    {
        if (currentFadeMode == FadeMode.FadeIn)
        {
            float fadeProgress = (Time.time - fadeStartTime) / fadeInDuration;
            currentAlpha = Mathf.Lerp(currentAlpha, 1, fadeProgress);
            if (currentAlpha == 1)
                currentFadeMode = FadeMode.None;
        }
        else if (currentFadeMode == FadeMode.FadeOut)
        {
            float fadeProgress = (Time.time - fadeStartTime) / fadeOutDuration;
            currentAlpha = Mathf.Lerp(currentAlpha, 0, fadeProgress);
            if (currentAlpha == 0)
            {
                currentFadeMode = FadeMode.None;
                DeactivateEndFadeOut();
            }
        }
    }

    /// <summary>
    /// Update the current scale according to the distance
    /// </summary>
    private void UpdateScale()
    {
        float distance = Vector3.Distance(_trans.position, usedCamera.transform.position);
        distance -= minDistance;

        currentScale = maxScale - distance * (maxScale - minScale) / maxDistance;
        currentScale = Mathf.Clamp(currentScale, minScale, maxScale);
        if (isOnScreen)
            currentScale = scaleOnScreen;
    }

    /// <summary>
    /// Scale animation update
    /// </summary>
    private void UpdateScaleAnimation()
    {
        if (animateScale || (animationMode == AnimMode.AnimateOnScreen && isOnScreen) || (animationMode == AnimMode.AnimateOutOfScreen && !isOnScreen) || animationMode == AnimMode.AlwaysAnimate)
        {
            if (scaleAnimUp)
            {
                currentAnimScalePercent = Mathf.SmoothDamp(currentAnimScalePercent, 1 + scaleAnimAmplitude, ref animVelocity, scaleAnimSmooth);
                if (currentAnimScalePercent >= 1 + scaleAnimAmplitude * 0.95f)
                    scaleAnimUp = false;
            }
            else
            {
                currentAnimScalePercent = Mathf.SmoothDamp(currentAnimScalePercent, 1 - scaleAnimAmplitude, ref animVelocity, scaleAnimSmooth);
                if (currentAnimScalePercent <= 1 - scaleAnimAmplitude * 0.95f)
                    scaleAnimUp = true;
            }
        }
        else
        {
            if(currentAnimScalePercent != 1)
                currentAnimScalePercent = Mathf.SmoothDamp(currentAnimScalePercent, 1, ref animVelocity, scaleAnimSmooth);
        }
    }

    /// <summary>
    /// Get the element rotation based on the base image position and the position required
    /// </summary>
    /// <param name="_baseImagePosition">Base image position</param>
    /// <param name="_currentPosition">Current position</param>
    /// <returns></returns>
    private float GetImageAngle(Position _baseImagePosition, Position _currentPosition)
    {
        return _baseImagePosition - _currentPosition;
    }

    /// <summary>
    /// Get the shortest screen measure (width or height)
    /// </summary>
    /// <returns></returns>
    private float GetScreenMeasure()
    {
        if (Screen.width > Screen.height)
            return Screen.height;
        else
            return Screen.width;
    }

    /// <summary>
    /// Get the viewport of the item with usedCamera
    /// </summary>
    /// <returns></returns>
    private Vector3 GetUsedViewport()
    {
        //Prepare viewport lerp between base viewport and calculated viewport
        const float beginThreshold = 0.3f;
        const float endThreshold = 1.0f;
        const float thresholdRange = endThreshold - beginThreshold;
        Vector3 viewportCalculated = GetViewportPosFromAngles();
        Vector3 usedViewport = viewportPosition;

        // Element behind camera
        if (viewportPosition.z < 0.0f)
        {
            usedViewport = viewportCalculated;
        }
        else if (viewportPosition.x < -beginThreshold)
        {
            if (viewportPosition.x <= -endThreshold)
                usedViewport = viewportCalculated;
            else
                usedViewport = Vector3.Lerp(viewportPosition, viewportCalculated, 1.0f - ((endThreshold - Mathf.Abs(viewportPosition.x)) / thresholdRange));
        }
        else if (viewportPosition.x > 1.0f + beginThreshold)
        {
            if (viewportPosition.x >= 1.0f + endThreshold)
                usedViewport = viewportCalculated;
            else
                usedViewport = Vector3.Lerp(viewportPosition, viewportCalculated, 1.0f - ((endThreshold - (viewportPosition.x - 1.0f)) / thresholdRange));
        }
        else if (viewportPosition.y < -beginThreshold)
        {
            if (viewportPosition.y <= -endThreshold)
                usedViewport = viewportCalculated;
            else
                usedViewport = Vector3.Lerp(viewportPosition, viewportCalculated, 1.0f - ((endThreshold - Mathf.Abs(viewportPosition.y)) / thresholdRange));
        }
        else if (viewportPosition.y > 1.0f + beginThreshold)
        {
            if (viewportPosition.y >= 1.0f + endThreshold)
                usedViewport = viewportCalculated;
            else
                usedViewport = Vector3.Lerp(viewportPosition, viewportCalculated, 1.0f - ((endThreshold - (viewportPosition.y - 1.0f)) / thresholdRange));
        }

        return usedViewport;
    }

    /// <summary>
    /// Give the Position of the texture depending of the screen position
    /// </summary>
    /// <param name="_screenPos"></param>
    /// <returns></returns>
    private Position GetOrientationFromScreenPos(Vector3 _screenPos, Vector2 _offset)
    {
        // Horizontal pos
        Position horizontal = Position.NotClamped;
        if (_screenPos.x <= usedCamera.pixelWidth * _offset.x)
            horizontal = Position.Left;
        else if (_screenPos.x >= usedCamera.pixelWidth - usedCamera.pixelWidth * _offset.x)
            horizontal = Position.Right;

        // Vertical pos
        Position vertical = Position.NotClamped;
        if (_screenPos.y <= usedCamera.pixelHeight * _offset.y)
            vertical = Position.Bottom;
        else if (_screenPos.y >= usedCamera.pixelHeight - usedCamera.pixelHeight * _offset.y)
            vertical = Position.Top;

        // Final pos
        if (horizontal == Position.Left && vertical == Position.Bottom)
            return Position.BottomLeft;
        else if (horizontal == Position.Left && vertical == Position.Top)
            return Position.TopLeft;
        else if (horizontal == Position.Right && vertical == Position.Bottom)
            return Position.BottomRight;
        else if (horizontal == Position.Right && vertical == Position.Top)
            return Position.TopRight;
        else if (horizontal == Position.NotClamped && vertical != Position.NotClamped)
            return vertical;
        else if (horizontal != Position.NotClamped && vertical == Position.NotClamped)
            return horizontal;
        else
            return Position.NotClamped;
    }

    /// <summary>
    /// Get a viewport pos computed with position of the element and the position of the camera without using the camera viewport
    /// </summary>
    /// <returns></returns>
    private Vector3 GetViewportPosFromAngles()
    {
        //Get camera => objective element angle
        Vector3 dirFromCamera = (_trans.position - usedCamera.transform.position).normalized;
        Quaternion dirQuat = Quaternion.LookRotation(dirFromCamera);
        float horizAngle = Mathf.DeltaAngle(usedCamera.transform.eulerAngles.y, dirQuat.eulerAngles.y);
        float vertAngle = -Mathf.DeltaAngle(usedCamera.transform.eulerAngles.x, dirQuat.eulerAngles.x);

        //Get the element position on screen from angle based on camera fov
        float horizFov = usedCamera.fieldOfView * usedCamera.aspect;
        float horizPerc = 0.0f;
        if (horizAngle < -(horizFov * 0.5f))
            horizPerc = 0.0f;
        else if (horizAngle > (horizFov * 0.5f))
            horizPerc = 1.0f;
        else
            horizPerc = (horizAngle + horizFov * 0.5f) / horizFov;

        float vertFov = usedCamera.fieldOfView;
        float vertPerc = 0.0f;
        if (vertAngle < -(vertFov * 0.5f))
            vertPerc = 0.0f;
        else if (vertAngle > (vertFov * 0.5f))
            vertPerc = 1.0f;
        else
            vertPerc = (vertAngle + vertFov * 0.5f) / vertFov;

        return new Vector3(horizPerc, vertPerc, -1);
    }

    /// <summary>
    /// Get the modified screen position depending of the texture and texture offset
    /// </summary>
    /// <param name="_screenPos">Current screen position</param>
    /// <param name="_width">Texture width</param>
    /// <param name="_height">Texture height</param>
    /// <returns></returns>
    private Vector3 GetModifiedScreenPos(Vector3 _screenPos, float _width, float _height, Vector2 _offset)
    {
        Vector3 screenPos = _screenPos;
        if (screenPos.x <= usedCamera.pixelWidth * _offset.x)
            screenPos.x = _width / 2f + usedCamera.pixelWidth * _offset.x;
        else if (screenPos.x >= usedCamera.pixelWidth - usedCamera.pixelWidth * _offset.x)
            screenPos.x = usedCamera.pixelWidth - (_width / 2f + usedCamera.pixelWidth * _offset.x);
        if (screenPos.y <= usedCamera.pixelHeight * _offset.y)
            screenPos.y = _height / 2f + usedCamera.pixelHeight * _offset.y;
        if (screenPos.y >= usedCamera.pixelHeight - usedCamera.pixelHeight * _offset.y)
            screenPos.y = usedCamera.pixelHeight - (_height / 2f + usedCamera.pixelHeight * _offset.y);

        if ((isInPlayer || isMulti) && isOnScreen)
            screenPos.y += addUp;
        return screenPos;
    }

    /// <summary>
    /// Update the image position on the canvas if the UICanvas mode was selected
    /// </summary>
    private void UpdateUICanvas()
    {
        if (usedDrawMode == DrawMode.UICanvas)
        {
            // Set UI width and height
            float elementWidth = 0;
            float elementHeight = 0;
            float angle = 0;
            Vector2 textureOffset = Vector2.zero;
            Color newColor = Color.white;

            if (isOnScreen && !hideOnScreen)
            {
                if (onScreenSprite != null)
                {
                    // Color
                    newColor = onScreenTextureColor;
                    newColor.a = currentAlpha;
                    linkedImage.color = newColor;

                    // Angle
                    angle = onScreenTextureRotation;

                    // Size
                    elementWidth = GetScreenMeasure() * onScreenTextureSize * currentScale * currentAnimScalePercent;
                    elementHeight = elementWidth * onScreenSprite.texture.height / onScreenSprite.texture.width;
                    
                    // Set UI size
                    linkedImageTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, elementWidth);
                    linkedImageTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, elementHeight);
                    linkedImage.sprite = onScreenSprite;
                }
                else
                    Debug.LogError("No onScreenSprite affected to " + name);
            }
            else if (!isOnScreen && !hideOutofScreen)
            {
                if (outofScreenSprite != null)
                {
                    // Color
                    newColor = outofScreenTextureColor;
                    newColor.a = currentAlpha;
                    linkedImage.color = newColor;

                    // Angle
                    angle = outofScreenTextureRotation;

                    // Offset
                    textureOffset = outofScreenBorderOffset;

                    // Size
                    elementWidth = GetScreenMeasure() * outofScreenTextureScreenPercentSize * currentScale * currentAnimScalePercent;
                    elementHeight = elementWidth * outofScreenSprite.texture.height / outofScreenSprite.texture.width;

                    // Set UI size
                    if (!gameObject || !linkedImageTrans)
                    {
                        Debug.Log("ici avant d'être détruit");
                        Destroy(gameObject);
                        return;
                    }
                    linkedImageTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, elementWidth);
                    linkedImageTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, elementHeight);
                    linkedImage.sprite = outofScreenSprite;
                }
                else
                    Debug.LogError("No outofScreenSprite affected to " + name);
            }

            Vector3 usedViewport = GetUsedViewport();
            Vector3 screenPos = new Vector3(Mathf.Lerp(0.0f, usedCamera.pixelWidth, Mathf.Clamp(usedViewport.x, 0.0f, 1.0f)), Mathf.Lerp(0.0f, usedCamera.pixelHeight, 
                Mathf.Clamp(usedViewport.y, 0.0f, 1.0f)), usedViewport.z);

            Position currentOrientation = GetOrientationFromScreenPos(screenPos, textureOffset);

            screenPos = GetModifiedScreenPos(screenPos, elementWidth, elementHeight, textureOffset);

            linkedImageTrans.anchoredPosition = new Vector2(screenPos.x - (Screen.width / 2), screenPos.y - (Screen.height / 2));

            // Orientation
            if (!isOnScreen)
            {
                if (outofScreenRotateTexture)
                    angle += GetImageAngle(outofScreenBaseOrientation, currentOrientation);

                angle = RotateTo(angle);
                linkedImageTrans.rotation = Quaternion.Euler(0, 0, angle);
                cg.alpha = opacityOpOutScreen;
            }
            else
            {
                linkedImageTrans.rotation = Quaternion.Euler(0, 0, 0);
                cg.alpha = opacityOpOnScreen;
            }
                
            testIfChangeSprite();
        }
    }

    private void testIfChangeSprite()
    {
        if (gameObject.GetComponent<EggsController>() && currentPlayer != (int)gameObject.GetComponent<EggsController>().SOE)
        {
            currentPlayer = (int)gameObject.GetComponent<EggsController>().SOE;
            if (currentPlayer == -1) //si bug -1, set to 0 blanc !
                currentPlayer = 0;

            if (currentPlayer == 5 && hideWhenKinematic) //si li est kinétic
            {
                hideOutofScreen = true;
                outofScreenSprite = outofScreenSpriteArray[0];
            }
            else
            {
                outofScreenSprite = outofScreenSpriteArray[currentPlayer];
            }
        }
        if (gameObject.GetComponent<PlayerController>() && gameManager.multi)   //test si l'OP est sur un joueur, et qu'on est en multi
        {
            outofScreenSprite = outofScreenSpriteArray[gameObject.GetComponent<PlayerController>().nb_team];    //change l'OP en la couleur de la team (si on est en multi)
        }
    }

    private float RotateTo(float angle)
    {
        Vector3 tmpDesiredPos = cc.m_DesiredPosition;
        tmpDesiredPos.z = 0.0f;
        Vector3 v3Pos;
        v3Pos = Camera.main.WorldToScreenPoint(tmpDesiredPos);
        v3Pos = linkedImageTrans.transform.position - v3Pos;
        angle = Mathf.Atan2(v3Pos.y, v3Pos.x) * Mathf.Rad2Deg;
        angle += 90.0f;
        if (angle < 0.0f)
            angle += 360.0f;
        return (angle);
    }

    public void hideAfterDeath()
    {
        hideOutofScreen = true;
        //onScreenSprite = null;
    }

    void OnGUI()
    {
        if (usedDrawMode == DrawMode.OnGUI && isVisible && ((!isOnScreen && !hideOutofScreen) || (isOnScreen && !hideOnScreen)))
        {
            float elementWidth = 0;
            float elementHeight = 0;
            float angle = 0;
            Vector2 textureOffset = Vector2.zero;
            Texture imageToDraw = null;
            Color newColor = Color.white;

            if (isOnScreen && !hideOnScreen)
            {
                if (onScreenSprite != null)
                {
                    // Color
                    newColor = onScreenTextureColor;
                    newColor.a = currentAlpha;

                    // Angle
                    angle = onScreenTextureRotation;

                    // Image
                    imageToDraw = onScreenSprite.texture;

                    // Size
                    elementWidth = GetScreenMeasure() * onScreenTextureSize * currentScale * currentAnimScalePercent;
                    elementHeight = elementWidth * onScreenSprite.texture.height / onScreenSprite.texture.width;
                }
                else
                    Debug.LogError("No onScreenSprite affected to " + name);
            }
            else if (!isOnScreen && !hideOutofScreen)
            {
                if (outofScreenSprite != null)
                {
                    // Color
                    newColor = outofScreenTextureColor;
                    newColor.a = currentAlpha;

                    // Angle
                    angle = outofScreenTextureRotation;

                    // Offset
                    textureOffset = outofScreenBorderOffset;

                    // texture
                    imageToDraw = outofScreenSprite.texture;

                    // Size
                    elementWidth = GetScreenMeasure() * outofScreenTextureScreenPercentSize * currentScale * currentAnimScalePercent;
                    elementHeight = elementWidth * outofScreenSprite.texture.height / outofScreenSprite.texture.width;
                }
                else
                    Debug.LogError("No outofScreenTextureScreenPercentSize affected to " + name);
            }

            Vector3 usedViewport = GetUsedViewport();
            Vector3 screenPos = new Vector3(Mathf.Lerp(0.0f, usedCamera.pixelWidth, Mathf.Clamp(usedViewport.x, 0.0f, 1.0f)), Mathf.Lerp(0.0f, usedCamera.pixelHeight,
                Mathf.Clamp(usedViewport.y, 0.0f, 1.0f)), usedViewport.z);

            Position currentOrientation = GetOrientationFromScreenPos(screenPos, textureOffset);

            screenPos = GetModifiedScreenPos(screenPos, elementWidth, elementHeight, textureOffset);

            // Orientation
            if (outofScreenRotateTexture)
                angle += GetImageAngle(outofScreenBaseOrientation, currentOrientation);

            GUIUtility.RotateAroundPivot(-angle, new Vector2(screenPos.x, Screen.height - screenPos.y));

            // Color
            GUI.color = newColor;

            // Draw out of screen texture
            if(imageToDraw != null)
                GUI.DrawTexture(new Rect(screenPos.x - (elementWidth / 2), Screen.height - (screenPos.y + elementHeight / 2), elementWidth, elementHeight), imageToDraw);
        }
    }
    
}
