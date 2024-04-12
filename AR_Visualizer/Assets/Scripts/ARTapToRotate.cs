using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ARTapToRotate : MonoBehaviour
{
    public bool hasPhysics = false;

    Vector3 mPrevPOs = Vector3.zero;
    Vector3 mPOsDelta = Vector3.zero;
    public float rotationSpeed;
    public float scaleSpeed = 1f;
    public float speed = 3f;
    private Vector3 position;
    private float width;
    private float height;
    public Vector3 ScaleMin;

    public Vector3 ScaleMax = new Vector3(2f, 2f, 2f);

    Vector3 resetPosition;
    Quaternion resetRotation;
    Vector3 resetScale;

    float rotX, rotY;

    Touch touch, touchZero, touchOne, initTouch;


    private void Awake()
    {
        InitTransform();
    }

    // Start is called before the first frame update
    void Start()
    {
        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasPhysics)
        {
            if (Input.GetMouseButton(0) && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    var MouseX = Input.GetAxis("Mouse X") * speed;
                    var MouseY = Input.GetAxis("Mouse Y") * speed;
                    transform.Rotate(0, -MouseX, 0, Space.World);

                }
            }
        }
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * Time.deltaTime * (scaleSpeed / 10);

            Vector3 newScale = transform.localScale - new Vector3(deltaMagnitudeDiff, deltaMagnitudeDiff, deltaMagnitudeDiff);
            // transform.localScale = newScale;
            Scale(newScale);
        }
    }

    protected virtual void Scale(Vector3 scale)
    {
        scale.x = Mathf.Clamp(scale.x, ScaleMin.x, ScaleMax.x);
        scale.y = Mathf.Clamp(scale.y, ScaleMin.y, ScaleMax.y);
        scale.z = Mathf.Clamp(scale.z, ScaleMin.z, ScaleMax.z);

        transform.localScale = scale;
    }
    public Vector3 TouchPosition(Vector2 touchPos)
    {
        Vector2 pos = touchPos;
        pos.x = (pos.x - width) / width;
        pos.y = (pos.y - height) / height;
        return new Vector3(-pos.x, pos.y, 0.0f); ;
    }

    public void InitTransform()
    {
        this.resetPosition = Vector3.zero;
        this.resetRotation = Quaternion.identity;
        this.resetScale = Vector3.one;
        rotX = this.transform.eulerAngles.x;
        rotY = this.transform.eulerAngles.y;
        //ScaleMin = this.transform.localScale; 
    }

    public void ResetObject()
    {
        this.transform.position = resetPosition;
        this.transform.rotation = resetRotation;
        this.transform.localScale = resetScale;
    }
}
