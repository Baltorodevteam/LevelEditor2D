using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class EnemyMarker : MonoBehaviour
{
    [SerializeField]
    GameObject dirArrow;

    [SerializeField]
    Text text;

    private void OnEnable()
    {
    }

    public void SetNr(int i)
    {
        text.text = "" + i;
    }

    public void ShowDirArrow(bool b, float fAngle)
    {
        dirArrow.SetActive(b);

        if(b)
        {
            Vector3 rotatedVector = Quaternion.AngleAxis(fAngle, Vector3.forward) * Vector3.up;
            rotatedVector *= 12.0f;

            dirArrow.transform.localPosition = rotatedVector;

            dirArrow.transform.rotation = Quaternion.Euler(0.0f, 0.0f, fAngle);
        }
    }
}
