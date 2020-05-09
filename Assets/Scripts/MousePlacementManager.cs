using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePlacementManager : MonoBehaviour
{
    // 登録されたオブジェクト
    [SerializeField] GameObject[] targetPrefabs;

    // ゲームカメラ
    Camera gameCamera;

    // カーソル選択中のオブジェクト
    GameObject cursorObject;

    // 選択中かどうか
    bool isPlacing = false;
    public bool IsPlacing { get { return isPlacing; } }

    int selectId = -1;

    private void Awake()
    {
        gameCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlacing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject targetCreate = Instantiate(targetPrefabs[selectId], cursorObject.transform.position, Quaternion.identity);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnUnselectTarget();
            }
            else
            {
                Vector3 touchScreenPosition = Input.mousePosition;

                touchScreenPosition.x = Mathf.Clamp(touchScreenPosition.x, 0.0f, Screen.width);
                touchScreenPosition.y = Mathf.Clamp(touchScreenPosition.y, 0.0f, Screen.height);

                Ray touchPointToRay = gameCamera.ScreenPointToRay(touchScreenPosition);
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(touchPointToRay, out hitInfo))
                {
                    if (hitInfo.collider.gameObject.tag == "Floor")
                    {
                        CheckAndPlaceObject(hitInfo);
                    }
                }
            }
        }
    }

    public void OnClickSelectBox(int boxId)
    {
        OnUnselectTarget();

        if (boxId < targetPrefabs.Length)
        {
            selectId = boxId;
            isPlacing = true;

            cursorObject = Instantiate(targetPrefabs[selectId]);
            cursorObject.layer = 2;
        }
    }

    public void OnUnselectTarget()
    {
        if (cursorObject)
        {
            Destroy(cursorObject);
        }
        selectId = -1;
        isPlacing = false;
    }

    void CheckAndPlaceObject(RaycastHit hitInfo)
    {
        GameObject hitTarget = hitInfo.collider.gameObject;
        GameObject cursorObjectReal = cursorObject.transform.Find("Box").gameObject;

        Vector3 localPosition = GetObjectRealLocation(hitInfo.point, cursorObjectReal.transform.localScale, hitTarget.transform);
        cursorObject.transform.position = localPosition;
    }

    Vector3 GetObjectRealLocation(Vector3 objPoint, Vector3 objScale, Transform target)
    {
        float maxObjX = objPoint.x + (objScale.x / 2);
        float minObjX = objPoint.x - (objScale.x / 2);
        float maxHitTargetX = target.position.x + (target.localScale.x / 2) - (objScale.x / 2);
        float minHitTargetX = target.position.x - (target.localScale.x / 2) + (objScale.x / 2);

        float maxObjZ = objPoint.z + (objScale.z / 2);
        float minObjZ = objPoint.z - (objScale.z / 2);
        float maxHitTargetZ = target.position.z + (target.localScale.z / 2) - (objScale.z / 2);
        float minHitTargetZ = target.position.z - (target.localScale.z / 2) + (objScale.z / 2);
        
        float roundHitX = Mathf.Round(objPoint.x * 2f) * 0.5f;
        float roundHitZ = Mathf.Round(objPoint.z * 2f) * 0.5f;
        float roundHitY = target.position.y + target.localScale.y;

        if (maxObjX > maxHitTargetX) roundHitX = maxHitTargetX;
        if (minObjX < minHitTargetX) roundHitX = minHitTargetX;
        if (maxObjZ > maxHitTargetZ) roundHitZ = maxHitTargetZ;
        if (minObjZ < minHitTargetZ) roundHitZ = minHitTargetZ;

        return new Vector3(roundHitX, roundHitY, roundHitZ);
    }
}
