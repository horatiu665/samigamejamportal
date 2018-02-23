using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PortalFlipper : MonoBehaviour
{
    public Transform player;

    public Transform[] flipRoom;

    public Transform flipPivot;

    bool active = true;

    [SerializeField]
    private Collider _trig;
    public Collider trig
    {
        get
        {
            if (_trig == null)
            {
                _trig = GetComponent<Collider>();
            }
            return _trig;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        active = false;
        print("AC)");
    }

    private void OnTriggerExit(Collider other)
    {
        active = true;
        print("FAFAFA");
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        var toPlayer = player.position - flipPivot.position;
        var angleToPlayer = Vector3.Angle(toPlayer, flipPivot.forward);

        if (angleToPlayer > 90)
        {
            if (!active)
            {
                flipPivot.Rotate(0, 180, 0);
                return;
            }
            else
            {
                Flip();
            }
        }
    }

    public void Flip()
    {
        for (int i = 0; i < flipRoom.Length; i++)
        {
            var t = flipRoom[i];
            var parent = t.parent;
            t.SetParent(flipPivot);
            flipPivot.Rotate(0, 180, 0);
            t.SetParent(parent);
            flipPivot.Rotate(0, 180, 0);
        }

        flipPivot.Rotate(0, 180, 0);

    }

}